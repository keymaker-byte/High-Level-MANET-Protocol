package hlmp.NetLayer;

import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.MulticastSocket;
import java.net.DatagramPacket;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.SocketTimeoutException;
import java.net.SocketException;
import java.util.concurrent.atomic.AtomicInteger;

import hlmp.NetLayer.Constants.NetHandlerState;
import hlmp.NetLayer.Interfaces.ResetIpHandler;
import hlmp.NetLayer.Interfaces.WifiInformationHandler;
import hlmp.Tools.BitConverter;


public class NetHandler implements WifiInformationHandler, ResetIpHandler{

	private ServerSocket tcpListener;
	private NetHandler myself;
	
	/**
	 * thread de escucha para clientes TCP
	 */
	private Thread tcpListenerThread;
	/**
	 * Lista de servidores TCP
	 */
	private RemoteMachineList tcpServerList;
	/**
	 * Lista de servidores TCP que se deben cerrar al final
	 */
	private RemoteMachineList oldServerList;
	/**
	 * IpAddress de TCP
	 */
	//private InetAddress tcpAddress;
	/**
	 * thread de escucha de mensajes UDP
	 */
	private Thread udpClientThread;
	/**
	 * Socket para enviar mensajes UDP
	 */
	private DatagramSocket udpClient;
	/**
	 * Socket para recivir mensajes UDP
	 */
	private MulticastSocket udpServer;
	/**
	 * IpAddress UDP
	 */
	private InetAddress udpMulticastAdress;
	/**
	 * Cola de mensajes UDP Multicast leidos
	 */
	private NetMessageQueue udpMessageQueue;
	/**
	 * Cola de mensajes TCP leidos
	 */
	private NetMessageQueue tcpMessageQueue;
	/**
	 * Configuración de red
	 */
	private NetData netData;
	/**
	 * Handler de eventos de comunicación
	 */
	private CommHandlerI commHandler;
	/**
	 * Lock para conectar y desconectar
	 */
	private Object connectLock;
	/**
	 * estado de este objeto (un valor de NetHandlerState)
	 */
	private int netHandlerState;
	/**
	 * Manejador de la conexion de red inalambrica
	 */
	private WifiHandler wifiHandler;
	/**
	 * Thread de partida
	 */
	private Thread startThread;
	/**
	 * Control de stop
	 */
	private AtomicInteger stopPoint;
	/**
	 * Control de duplicación de Ip
	 */
	private IpHandler ipHandler;
	/**
	 * Variable de control para el cambio de IP
	 */
	private AtomicInteger iphandlerPoint;
	/**
	 * Thread de reset
	 */
	private Thread resetThread;
	/**
	 * lock para reset
	 */
	private Object resetLock;
	
	/**
	 * Constructor
	 * @param netData Los parámetros de configuración
	 * @param commHandler El comunicador que maneja los eventos generados en la red 
	 */
	public NetHandler(NetData netData, CommHandlerI commHandler)
	{
		this.netData = netData;
		this.commHandler = commHandler;
		this.connectLock = new Object();
		this.resetLock = new Object();
		this.iphandlerPoint = new AtomicInteger(0);
		netData.pickNewIp();
		init();
		this.myself = this;
	}

	/**
	 * Inicializa las propiedades de la clase
	 */
	private void init()
	{
		//inicializa las listas
		udpMessageQueue = new NetMessageQueue();
		tcpMessageQueue = new NetMessageQueue();
		tcpServerList = new RemoteMachineList();
		oldServerList = new RemoteMachineList();
		//inicializa los objetos para TCP
		//tcpAddress = netData.getIpTcpListener();
		//tcpListenerThread = new TcpListenerThread(this, tcpAddress, netData.getTcpPort());
		tcpListenerThread = getListenTcpClientsThread();
		tcpListenerThread.setName("TCP NetHandler Main Thread");
		//inicializa los objetos UDP
//		udpMulticastAdress = InetAddress.getByName(netData.getIpUdpMulticast());
		udpClientThread = getListenUDPMessagesThread();
		udpClientThread.setName("UDP NetHandler Main Thread");
		//estado
		netHandlerState = NetHandlerState.INITIATED;
		wifiHandler = new WifiHandler(netData, this);
		startThread = getStartThread();
		startThread.setName("NetHandler Start Thread");
		stopPoint = new AtomicInteger(0);
		ipHandler = new IpHandler(netData, this);
		resetThread = getResetThread();
		resetThread.setName("NetHandler Reset Thread");
	}

	/**
	 * Levanta los servicios, Esta función es No bloqueante, levanta un thread
	 * Esta funcion siempre gatilla startNetworkingHandler o gatilla una excepcion
	 */
	public void connect()
	{
		synchronized(connectLock)
		{
			synchronized(resetLock)
			{
				if(stopPoint.compareAndSet(0, 0))
				{
					if (netHandlerState == NetHandlerState.INITIATED || netHandlerState == NetHandlerState.STOPPED)
					{
						startThread.start();
					}
				}  
			}
		}
	}

	/**
	 * Termina los servicios, Esta función es Bloqueante hasta que termine
	 * esta funcion siempre gatilla stopNetworkingHandler
	 */
	public void disconnect()
	{
//		TODO: parametrizar los valores 0 y 1 por CONECTADO y DESCONECTADO
		debug("NETHANDLER: disconnect...");
		if(stopPoint.compareAndSet(0, 1))
		{
			synchronized(connectLock)
			{
				try
				{
					startThread.interrupt();
					startThread.join();
				}
				catch (Exception e)
				{
					debug("NETHANDLER: disconnect aborting start " + e.getMessage());
				}
				stop(false);
				stopPoint.set(0); 
			}
		} 
		debug("NETHANDLER: disconnect... OK");
	}

	private void debug(String text) {
//		commHandler.informationNetworkingHandler(text);
		System.out.println(text);
	}
	

	/**
	 * Resetea la red
	 * @return un thread que resetea la red
	 */
	private Thread getResetThread()
	{
		return new Thread(){
			public void run(){
				debug("NETHANDLER: reset IP...");
				debug("NETHANDLER: ANTES de synchronized(resetLock)");
				synchronized(resetLock)
				{
					commHandler.resetNetworkingHandler();
					netData.pickNewIp();

					try
					{
						startThread.interrupt();
						startThread.join();
					}
					catch (InterruptedException e)
					{
					}
					catch (Exception e)
					{
						debug("NETHANDLER: error on  restart startThread");
					}

					myself.stop(true);
					connect();
				}
				debug("NETHANDLER: DESPUES de synchronized(resetLock)");
				debug("NETHANDLER: reset IP... OK");
			}
		};
		
	}
	
	/**
	 * Se gatilla para levantar los servicios
	 * Si ocurre un error se arroja
	 * @return un thread que levanta los servicios
	 */
    private Thread getStartThread(){
    	return new Thread(){

			@Override
			public void run() {
				try
	            {
	                debug("NETHANDLER: start netHandler...");
	                netHandlerState = NetHandlerState.STARTING;
	                ////intenta poner la ip en modo estatico
	                try
	                {
	                    debug("NETHANDLER: disable adapter...");
	                    SystemHandler.disableIpAdapter(netData.getNetworkAdapter());
	                    debug("NETHANDLER: disable adapter... OK");
	                }
	                catch (Exception e)
	                {
	                    debug("NETHANDLER: disable adapter... failed! " + e.getMessage());
	                }
	                try
	                {
	                    debug("NETHANDLER: set IP... " + netData.getIpTcpListener().getHostAddress());
	                    SystemHandler.setStaticIP(netData.getNetworkAdapter(), netData.getIpTcpListener().getHostAddress(), netData.getSubnetMask());
	                    debug("NETHANDLER: set IP... OK");
	                }
//	                catch (ThreadAbortException e)
//	                {
//	                    throw e;
//	                }
	                catch (Exception e)
	                {
	                    debug("NETHANDLER: set IP... failed! " + e.getMessage());
	                }
	                try
	                {
	                    debug("NETHANDLER: enable adapter...");
	                    SystemHandler.enableIpAdapter(netData.getNetworkAdapter());
	                    debug("NETHANDLER: enable adapter... OK");
	                }
//	                catch (ThreadAbortException e)
//	                {
//	                    throw e;
//	                }
	                catch (Exception e)
	                {
	                    debug("NETHANDLER: enable adapter... failed! " + e.getMessage());
	                }

	                //hecha a andar wifiHandler
	                debug("NETHANDLER: start wifi...");
	                wifiHandler.connect();
	                //espera por primera conexión
	                while (wifiHandler.getConnectionState() == hlmp.NetLayer.Constants.WifiConnectionState.DISCONNECTED)
	                {
	                    debug("NETHANDLER: waiting for other devices");
	                    Thread.sleep(netData.getWaitTimeStart());
	                }
	                debug("NETHANDLER: start wifi... OK");
	                //Setea la IP en el sistema operativo
	                Boolean ipChange = false;
	                int timeOutIpChange = 0;
	                while (!ipChange)
	                {
	                    try
	                    {
	                        debug("NETHANDLER: set IP... " + netData.getIpTcpListener().getHostAddress());
	                        SystemHandler.setStaticIP(netData.getNetworkAdapter(), netData.getIpTcpListener().getHostAddress(), netData.getSubnetMask());
	                        ipChange = true;
	                        debug("NETHANDLER: set IP... OK");
	                    }
//	                    catch (ThreadAbortException e)
//	                    {
//	                        throw e;
//	                    }
	                    catch (Exception e)
	                    {
	                        debug("NETHANDLER: set IP... failed! " + e.getMessage());
	                        timeOutIpChange++;
	                        if (timeOutIpChange > netData.getWaitForStart())
	                        {
	                            throw new Exception("timeout, para configurar IP");
	                        }
	                        Thread.sleep(netData.getWaitTimeStart());
	                    }
	                }
	                // Check IP
	                debug("NETHANDLER: start strong DAD");
	                ipHandler.startStrongDAD();
	                
	                // Start TCP
	                debug("NETHANDLER: start TCP...");
	                //tcpListener.setReuseAddress(true);
	                Boolean tcpChange = false;
	                int timeOutTcpChange = 0;
	                while (!tcpChange)
	                {
	                    try
	                    {
	                        debug("NETHANDLER: start TCP listener... " + netData.getIpTcpListener().getHostAddress() + ":" + netData.getTcpPort());
	                        //tcpListener.start();
	                        tcpListener = new ServerSocket(netData.getTcpPort());
	                        //tcpListener.bind(new InetSocketAddress(netData.getIpTcpListener(),netData.getTcpPort()));
	                        tcpChange = true;
	                        debug("NETHANDLER: start TCP listener... OK");
	                    }
//	                    catch (ThreadAbortException e)
//	                    {
//	                        throw e;
//	                    }
	                    catch (Exception e)
	                    {
	                    	e.printStackTrace();
	                    	debug("NETHANDLER: start TCP listener... failed! " + e.getMessage());
	                        timeOutTcpChange++;
	                        if (timeOutTcpChange > netData.getWaitForStart())
	                        {
	                            throw new Exception("timeout, para levantar servicio TCP");
	                        }
	                        else
	                        {
	                            Thread.sleep(netData.getWaitTimeStart());
	                        }
	                    }
	                }
	                tcpListenerThread.start();
	                debug("NETHANDLER: start TCP... OK");
	                
	                // Start UDP
	                debug("NETHANDLER: start UDP...");
	                debug("NETHANDLER: start UDP... " + netData.getIpUdpMulticast() + ":" + netData.getUdpPort());
	                udpServer = new MulticastSocket(netData.getUdpPort());
	                udpClient = new DatagramSocket();
	                udpClient.setBroadcast(true);
	                udpClient.setReuseAddress(true);
	                udpMulticastAdress = InetAddress.getByName(netData.getIpUdpMulticast());
	                udpServer.joinGroup(udpMulticastAdress);
	                udpClientThread.start();
	                debug("NETHANDLER: start UDP... OK");
	                
	                // Week IP Check
	                debug("NETHANDLER: start weak DAD");
	                ipHandler.chageToWeakDAD();
	                
	                // Start Comminication
	                commHandler.startNetworkingHandler();
	                netHandlerState = NetHandlerState.STARTED;
	                debug("NETHANDLER: start netHandler... OK");
	                debug("NETHANDLER: We are so connected, welcome to HLMP !!!");
	            }
	            catch (InterruptedException e)
	            {
	            	System.out.println(e.getStackTrace());
	                debug("NETHANDLER: start netHandler... failed! " + e.getMessage());
	            }
	            catch (Exception e)
	            {
	                disconnect();
	            	debug("NETHANDLER: start netHandler... failed! " + e.getMessage());
	                commHandler.errorNetworkingHandler(e);
	            }
			}
    		
    	};
    }

    /**
     * Se gatilla para terminar los servicios
     * Si ocurre algun error se informa en informationNetworkingHandler, no se detiene ejecución
     */
    private void stop(boolean reset){
    	debug("NETHANDLER: stop netHandler...");
        netHandlerState = NetHandlerState.STOPPING;
        try
        {
            debug("NETHANDLER: stop DAD...");
        	ipHandler.stop();
            debug("NETHANDLER: stop DAD... OK");
        }
        catch (Exception e)
        {
        	debug("NETHANDLER: stop DAD... failed! " + e.getMessage());
        }
        try
        {
            debug("NETHANDLER: stop communication...");
        	commHandler.stopNetworkingHandler();
            debug("NETHANDLER: stop communication... OK");
        }
        catch (Exception e)
        {
        	debug("NETHANDLER: stop communication... failed! " + e.getMessage());
        }
        
        // Close UDP  
        try
        {
            debug("NETHANDLER: drop multicast suscription...");
            udpServer.leaveGroup(udpMulticastAdress);
            debug("NETHANDLER: drop multicast suscription... OK");
        }
        catch (Exception e)
        {
            debug("NETHANDLER: drop multicast suscription... failed! " + e.getMessage());
        }
        try
        {
        	debug("NETHANDLER: shutdown UDP client...");
            udpClient.close();
            debug("NETHANDLER: shutdown UDP client... OK");
        }
        catch (Exception e)
        {
        	debug("NETHANDLER: shutdown UDP client... failed! " + e.getMessage());
        }
        try
        {

        	debug("NETHANDLER: stop UDP thread...");
            udpClientThread.interrupt();
            try
        	{
            	debug("NETHANDLER: close UDP socket...");
				udpServer.close();
				debug("NETHANDLER: close UDP socket... OK");
	        }
	        catch (Exception e)
	        {
	        	debug("NETHANDLER: stop UDP thread... failed! " + e.getMessage());
	        }
            udpClientThread.join();
            debug("NETHANDLER: stop UDP thread... OK");
            
        }
        catch (Exception e)
        {
        	debug("NETHANDLER: stop UDP thread... failed! " + e.getMessage());
        }

        // Close TCP
        try
        {
        	debug("NETHANDLER: stop TCP listener...");
            tcpListener.close();
            debug("NETHANDLER: stop TCP listener... OK");
        }
        catch (Exception e)
        {
        	debug("NETHANDLER: stop TCP listener... failed! " + e.getMessage());
        }
        try
        {
        	debug("NETHANDLER: stop TCP thread...");
            tcpListenerThread.interrupt();
            tcpListenerThread.join();
            debug("NETHANDLER: stop TCP thread... OK");
        }
        catch (Exception e)
        {
        	debug("NETHANDLER: stop TCP thread... failed! " + e.getMessage());
        }
        try
        {
        	debug("NETHANDLER: kill TCP links...");
            RemoteMachine[] serverRemoteMachines = tcpServerList.toObjectArray();
            for (int i = 0; i < serverRemoteMachines.length; i++)
            {
                try
                {
                	debug("NETHANDLER: kill TCP link... " + serverRemoteMachines[i].getIp().getHostAddress());
                    killRemoteMachine(serverRemoteMachines[i]);
                    debug("NETHANDLER: kill TCP link... OK");
                }
                catch (Exception e)
                {
                	debug("NETHANDLER: kill TCP link... failed! " + e.getMessage());
                }
            }
            debug("NETHANDLER: kill TCP links... OK");
        }
        catch (Exception e)
        {
        	debug("NETHANDLER: kill TCP links... failed! " + e.getMessage());
        }
        
        // Stop wifiHandler
        try
        {
        	debug("NETHANDLER: stop wifi...");
            wifiHandler.disconnect();
            debug("NETHANDLER: stop wifi... OK");
        }
        catch (Exception e)
        {
        	debug("NETHANDLER: stop wifi... failed! " + e.getMessage());
        }
        
        if(!reset)
        {
        	// Deja la IP en el sistema operativo como por defecto (DHCP)
        	try
        	{
        		debug("NETHANDLER: dhcp on...");
        		SystemHandler.setDinamicIP(netData.getNetworkAdapter());
        		debug("NETHANDLER: dhcp on... OK");
        	}
        	catch (Exception e)
        	{
        		debug("NETHANDLER: dhcp on... failed!" + e.getMessage());
        	}
        }
        
        
        //se cierran las viejas conexiones TCP
        try
        {
        	debug("NETHANDLER: kill TCP links..." + "(TCP is hard to kill)");
            RemoteMachine[] serverRemoteMachines = oldServerList.toObjectArray();
            for (int i = 0; i < serverRemoteMachines.length; i++)
            {
                try
                {
                	debug("NETHANDLER: kill TCP link... " + serverRemoteMachines[i].getIp().getHostAddress());
                    killRemoteMachine(serverRemoteMachines[i]);
                    debug("NETHANDLER: kill TCP link... OK");
                }
                catch (Exception e)
                {
                	debug("NETHANDLER: kill TCP link... failed! " + e.getMessage());
                }
            }
            debug("NETHANDLER: kill TCP links... OK");
        }
        catch (Exception e)
        {
        	debug("NETHANDLER: kill TCP links... failed! " + e.getMessage());
        }
        
        // Restart Objects
        try
        {
        	debug("NETHANDLER: restart objects...");
            init();
            debug("NETHANDLER: restart... OK");
        }
        catch (Exception e)
        {
        	debug("NETHANDLER: initialation of objects... failed! " + e.getMessage());
        }
        netHandlerState = NetHandlerState.STOPPED;
        debug("NETHANDLER: stop netHandler... OK");
        debug("NETHANDLER: bye bye!");
    }

    /**
     * Envia el mensaje por TCP al usuario indicado
     * @param netMessage mensage a enviar
     * @param ip direccion del usuario
     * @return true si se envio correctamente, false si no
     */
	public boolean sendTcpMessage(NetMessage netMessage, InetAddress ip){
		try
		{
			RemoteMachine remoteMachine = tcpServerList.getRemoteMachine(ip); 
			if (remoteMachine != null)
			{
				try
				{
					remoteMachine.sendNetMessage(netMessage, netData.getTimeOutWriteTCP());
				}
				catch (Exception e)
				{
					debug("TCP WARNING 1: send failed " + e.getMessage());
					if (remoteMachine.getFails() >= netData.getSendFailsToDisconnect())
					{
						disconnectFrom(remoteMachine);
					}
					return false;
				}
				return true;
			}
			else
			{
				throw new Exception("There is no TCP link with that remote machine");
			}
		}
//		catch (InterruptedException e)
//		{
//			throw e;
//		}
		catch (Exception e)
		{
			debug("TCP WARNING 2: send failed " + e.getMessage());
			return false;
		}
	}

	/**
	 * Envía un mensaje TCP a todas las máquinas remotas visibles
	 * Si ocurre algun error se arroja
	 * @param message el mensaje a envíar
	 */
	public void sendTcpMessage(NetMessage message) throws InterruptedException{
		RemoteMachine[] serverRemoteMachines = tcpServerList.toObjectArray();
		for (int i = 0; i < serverRemoteMachines.length; i++)
		{
			sendTcpMessage(message, serverRemoteMachines[i].getIp());
		}
	}

	/**
	 * Envia un mensaje UDP a todas las maquinas remotas visibles
	 * Si ocurre un error se informa
	 * @param message El mensaje a envíar
	 */
	public boolean sendUdpMessage(NetMessage message){
		try
		{
			byte[] lenght =  BitConverter.intToByteArray(message.getSize());
			byte[] netByteMessage = new byte[4 + message.getSize()];
			System.arraycopy(lenght, 0, netByteMessage, 0, 4);
			System.arraycopy(message.getBody(), 0, netByteMessage, 4, message.getSize());
			DatagramPacket packet = new DatagramPacket(netByteMessage, netByteMessage.length, udpMulticastAdress, netData.getUdpPort());
			packet.setLength(netByteMessage.length);
			udpClient.send(packet);
			return true;
		}
		//        catch (ThreadAbortException e)
		//        {
		//            throw e;
		//        }
		catch (Exception e)
		{
			debug("UDP WARNING: send failed " + e.getMessage());
			return false;
		}
	}

	/**
	 * Envia un mensaje UDP a todas las maquinas remotas visibles
	 * Si ocurre un error se informa
	 * @param message El mensaje a envíar
	 * @param ip la direccion IP a la cual enviar el mensaje
	 */
	public boolean sendUdpMessage(NetMessage message, InetAddress ip){
		try
		{
			byte[] lenght = BitConverter.intToByteArray(message.getSize());
			byte[] netByteMessage = new byte[4 + message.getSize()];
			System.arraycopy(lenght, 0, netByteMessage, 0, 4);
			System.arraycopy(message.getBody(), 0, netByteMessage, 4, message.getSize());
			
			DatagramPacket packet = new DatagramPacket(netByteMessage, netByteMessage.length, ip, netData.getUdpPort());
			packet.setLength(netByteMessage.length);
			udpClient.send(packet);
			return true;
		}
		//        catch (ThreadAbortException e)
		//        {
		//            throw e;
		//        }
		//        catch (SocketException e)
		//        {
		//            debug("UDP WARNING: send failed ErrorCode=" + e.ErrorCode);
		//            return false;
		//        }
		catch (Exception e)
		{
			debug("UDP WARNING: send failed " + e.getMessage());
			return false;
		}
	}

	/**
	 * Se conecta a una máquina remota por TCP para enviarle mensajes posteriormente
	 * Si ocurre un error se notifica en informationNetworkingHandler
	 * @param serverIp La dirección IP de la máquina remota, debe ser un String
	 */
	public void connectTo(String serverIp)
	{
		connectToAsync(serverIp);
	}

	/**
	 * Se conecta a una máquina remota por TCP para enviarle mensajes posteriormente
	 * Si ocurre un error se notifica en informationNetworkingHandler
	 * @param o La dirección IP de la máquina remota en formato IPAddress
	 */
	private void connectToAsync(Object o)
	{
		try
		{
			debug("TCP: connection...");
			InetAddress serverIp = null;
			if (o.getClass().equals(InetAddress.class))
			{
				serverIp = (InetAddress) o;
			}
			else
			{
				serverIp = InetAddress.getByName((String) o);
			}

			Socket tcpClient = new Socket();
			tcpClient.setSoTimeout(0);

			try
			{
				tcpClient.connect(new InetSocketAddress(serverIp, netData.getTcpPort()), netData.getTcpConnectTimeOut());
			}
			catch(SocketTimeoutException x)
			{
				debug("TCP: connection... time out!");
				return;
			}

			ListenTCPMessagesThread clientThread = new ListenTCPMessagesThread(this);
			clientThread.setName("ListenTCPMessagesThread_" + serverIp.getHostAddress());
			RemoteMachine remoteMachine = new RemoteMachine(serverIp, tcpClient, clientThread);
			clientThread.setRemoteMachine(remoteMachine);
			clientThread.start();
			
			RemoteMachine oldRemoteMachine = tcpServerList.getRemoteMachine(serverIp);
			if (oldRemoteMachine != null)
			{
				tcpServerList.remove(oldRemoteMachine);
			}
			tcpServerList.add(serverIp, remoteMachine);
			debug("TCP: connection... OK");
		}
		catch (Exception e)
		{
			debug("TCP: connection... failed! " + e.getMessage());
			debug(e.getStackTrace().toString());
		}
	}

	/**
	 * Desconecta los servicios TCP asociados a una maquina remota
	 * @param machineIp la Ip de la maquina a desconectar debe ser un String
	 */
	public void disconnectFrom(InetAddress machineIp)
	{
		//disconnectFromAsync(machineIp);
		killRemoteMachine(machineIp);
	}

	/**
	 * Desconecta los servicios TCP asociados a una maquina remota
	 * @param machine la maquina a desconectar debe ser un String
	 */
	public void disconnectFrom(RemoteMachine machine)
	{
		//disconnectFromAsync(machine);
		killRemoteMachine(machine);
	}

	/**
	 * Desconecta los servicios TCP asociados a una maquina remota
	 * @param o La ip de la maquina remota a desconectar en formato de String o la maquina remota
	 */
	@SuppressWarnings("unused")
	private void disconnectFromAsync(Object o)
	{
		try
		{
			debug("TCP: disconnection...");
			if(o.getClass().equals(InetAddress.class))
			{
				InetAddress machineIp = (InetAddress)o;
				RemoteMachine machine = tcpServerList.getRemoteMachine(machineIp);
				debug("TCP: old list queue");
				tcpServerList.remove(machine);
				oldServerList.add(machine.getIp(), machine);
			}
			else if (o.getClass().equals(RemoteMachine.class))
			{
				RemoteMachine machine = (RemoteMachine)o;
				debug("TCP: old list queue");
				tcpServerList.remove(machine);
				oldServerList.add(machine.getIp(), machine);
			}
			debug("TCP: disconnection... OK");
		}
//		catch (InterruptedException e)
//		{
//			throw e;
//		}
		catch (Exception e)
		{
			debug("TCP: disconnection... failed! " + e.getMessage());
		}
	}
	
	/**
	 * Desconecta los servicios TCP asociados a una maquina remota
	 * @param o La ip de la maquina remota a desconectar en formato de String o la maquina remota
	 */
	private void killRemoteMachine(Object o)
	{
		try
		{
			debug("TCP: kill...");
			if(o.getClass().equals(InetAddress.class))
			{
				InetAddress machineIp = (InetAddress)o;
				RemoteMachine machine = tcpServerList.getRemoteMachine(machineIp);
				debug("TCP: close machine");
				machine.close();
				debug("TCP: drop from queue");
				tcpServerList.remove(machine);
			}
			else if (o.getClass().equals(RemoteMachine.class))
			{
				RemoteMachine machine = (RemoteMachine)o;
				debug("TCP: close machine");
				machine.close();
				debug("TCP: drop from queue");
				tcpServerList.remove(machine);
			}
			debug("TCP: kill... OK");
		}
//		catch (InterruptedException e)
//		{
//			throw e;
//		}
		catch (Exception e)
		{
			debug("TCP: kill... failed! " + e.getMessage());
		}
	}

	/**
	 * Agrega mensajes TCP recibidos a la cola
	 * @param message el mensaje recibido
	 */
	public void addTCPMessages(NetMessage message)
	{
//		try
//		{
			//TODO PARAMETRIZAR
			if (tcpMessageQueue.size() < 50)
			{
				tcpMessageQueue.put(message);
			}
			else
			{
				debug("TCP WARNING: TCP message dropped");
			}
//		}finally{
//			
//		}
//		catch (InterruptedException e)
//		{
//			throw e;
//		}
	} 

	/**
	 * Los datos de red
	 * @return
	 */
	public NetData getNetData()
	{
		return netData;
	}

	/**
	 * Lista de maquinas de la red adhoc que son directamente visibles para esta máquina.
	 * Se posee una conexión TCP directa con cada una de ellas.
	 * @return
	 */
	public RemoteMachineList getTcpServerList() {
		return tcpServerList;
	}

	/**
	 * Cola de mensajes UDP que ha recibido esta máquina
	 * @return
	 */
	public NetMessageQueue getUdpMessageQueue() {
		return udpMessageQueue;
	}

	/**
	 * Cola de mensajes TCP que ha recibido esta máquina
	 * @return
	 */
	public NetMessageQueue getTcpMessageQueue() {
		return tcpMessageQueue;
	}

	/**
	 * Registra una ip externa para el chequeo de ip duplicada
	 * @param ip la ip a registrar
	 */
	public void registerIp(String ip)
	{
		ipHandler.put(ip);
	}

	public void wifiInformation(String message) {
		debug("WIFI: " + message);

	}

	public void resetIp() {
		if(iphandlerPoint.compareAndSet(0, 1))
		{
			try
			{
				resetThread.start();
			}
			catch (Exception e)
			{
			}
			iphandlerPoint.set(0);
		}

	}
	
	private Thread getListenTcpClientsThread(){
		return new Thread(){
			@Override
			public void run() {
				try
		        {
		            while (true)
		            {
		            	debug("TCP: accepting client ...");
		                Socket tcpClient = tcpListener.accept();
		                tcpClient.setSoTimeout(0);
		                debug("TCP: new client detected");
		                
		                InetAddress ip = tcpClient.getInetAddress();
		                
		                ListenTCPMessagesThread clientThread = new ListenTCPMessagesThread(myself);
		                clientThread.setName("ListenTCPMessages_" + ip.getHostAddress());
		                RemoteMachine remoteMachine = new RemoteMachine(ip, tcpClient, clientThread);
		                clientThread.setRemoteMachine(remoteMachine);
		                clientThread.start();
		                
		                RemoteMachine oldRemoteMachine = tcpServerList.getRemoteMachine(ip);
		                if (oldRemoteMachine != null)
		                {
		                	oldRemoteMachine.close();
		                    tcpServerList.remove(oldRemoteMachine);
		                }
		                tcpServerList.add(ip, remoteMachine);
		                debug("TCP: new client connected");
		            }
		        }
		        catch (SocketException e)
		        {
		            return;
		        }
		        catch (Exception e)
		        {
		        	debug("TCP WARNING: TCP listener has stopped!! " + e.getMessage());
		        }
				
			}
		};
	}
	
	private Thread getListenUDPMessagesThread(){
		return new Thread(){

			@Override
			public void run() {
				try
	            {
	                while (true)
	                {
	                	byte[] buf = new byte[1000];
	                	DatagramPacket packet = new DatagramPacket(buf, buf.length);
	                	udpServer.receive(packet);
	                    byte[] buffer = packet.getData();
	                    if (buffer.length > 4)
	                    {
	                    	byte[] arraySize = new byte[4];
	                    	System.arraycopy(buffer, 0, arraySize, 0, 4);
	                    	int size = BitConverter.byteArrayToInt(arraySize);
	                        if (buffer.length >= 4 + size)
	                        {
	                            byte[] body = new byte[size];
	                            System.arraycopy(buffer, 4, body, 0, size);
	                            NetMessage message = new NetMessage(body);
	                            udpMessageQueue.put(message);
	                        }else{
	                        	// agregado por NM
		                    	debug("UDP WARNING: upd receive wrong message, bad length");
	                        }
	                    }else{
	                    	// agregado por NM
	                    	debug("UDP WARNING: upd receive wrong message, lenght less than 4");
	                    }
	                }
	            }
	            catch (Exception e)
	            {
	            	if (!this.isInterrupted()) {
	            		debug("UDP WARNING: udp client has stopped!!! " + e.getMessage());
	            	}
	            }
			}
	
		};
	}

	public void informationNetworkingHandler(String message){
		debug("NETHANDLER: " + message);
	}
}
