package hlmp.NetLayer;

import hlmp.NetLayer.Constants.NetHandlerState;
import hlmp.NetLayer.Interfaces.ResetIpHandler;
import hlmp.NetLayer.Interfaces.WifiInformationHandler;
import hlmp.Tools.BitConverter;

import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.MulticastSocket;
import java.net.DatagramPacket;
import java.net.ServerSocket;
import java.net.Socket;
import java.net.SocketTimeoutException;
import java.util.concurrent.atomic.AtomicInteger;

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
		stopPoint = new AtomicInteger(0);
		ipHandler = new IpHandler(netData, this);
		resetThread = getResetThread();
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
					commHandler.informationNetworkingHandler("NETHANDLER: disconnect aborting start " + e.getMessage());
				}
				try
				{
					resetThread.interrupt();
					resetThread.join();
				}
				catch (Exception e)
				{
					commHandler.informationNetworkingHandler("NETHANDLER: disconnect aborting reset " + e.getMessage());
				}
				stop(false);
				stopPoint.set(0); 
			}
		} 
	}

	/**
	 * Resetea la red
	 * @return un thread que resetea la red
	 */
	private Thread getResetThread()
	{
		return new Thread(){
			public void run(){
				commHandler.informationNetworkingHandler("NETHANDLER: reset IP...");
				synchronized(resetLock)
				{
					commHandler.resetNetworkingHandler();
					netData.pickNewIp();

					try
					{
						startThread.interrupt();
						startThread.join();
					}
					//            catch (ThreadAbortException e)
					//            {
						//                throw e;
					//            }
					catch (Exception e)
					{
					}

					myself.stop(true);
					connect();
				}
				commHandler.informationNetworkingHandler("NETHANDLER: reset IP... ok!");
			}
		};
		
	}
	
	/**
	 * Se gatilla para levantar los servicios
	 * Si ocurre un error se arroja
	 * @return un thread que levanta los servicios
	 */
    private Thread getStartThread(){
    	//TODO:
    	return new Thread(){

			@Override
			public void run() {
				try
	            {
	                commHandler.informationNetworkingHandler("NETHANDLER: start netHandler...");
	                netHandlerState = NetHandlerState.STARTING;
	                ////intenta poner la ip en modo estatico
	                try
	                {
	                    commHandler.informationNetworkingHandler("NETHANDLER: disable adapter...");
	                    SystemHandler.disableIpAdapter(netData.getNetworkAdapter());
	                    commHandler.informationNetworkingHandler("NETHANDLER: disable adapter... ok!");
	                }
//	                catch (ThreadAbortException e)
//	                {
//	                    throw e;
//	                }
	                catch (Exception e)
	                {
	                    commHandler.informationNetworkingHandler("NETHANDLER: disable adapter... failed! " + e.getMessage());
	                }
	                try
	                {
	                    commHandler.informationNetworkingHandler("NETHANDLER: set IP... " + netData.getIpTcpListener().getHostAddress());
	                    SystemHandler.setStaticIP(netData.getNetworkAdapter(), netData.getIpTcpListener().getHostAddress(), netData.getSubnetMask());
	                    commHandler.informationNetworkingHandler("NETHANDLER: set IP... ok!");
	                }
//	                catch (ThreadAbortException e)
//	                {
//	                    throw e;
//	                }
	                catch (Exception e)
	                {
	                    commHandler.informationNetworkingHandler("NETHANDLER: set IP... failed! " + e.getMessage());
	                }
	                try
	                {
	                    commHandler.informationNetworkingHandler("NETHANDLER: enable adapter...");
	                    SystemHandler.enableIpAdapter(netData.getNetworkAdapter());
	                    commHandler.informationNetworkingHandler("NETHANDLER: enable adapter... ok!");
	                }
//	                catch (ThreadAbortException e)
//	                {
//	                    throw e;
//	                }
	                catch (Exception e)
	                {
	                    commHandler.informationNetworkingHandler("NETHANDLER: enable adapter... failed! " + e.getMessage());
	                }

	                //hecha a andar wifiHandler
	                commHandler.informationNetworkingHandler("NETHANDLER: start wifi...");
	                wifiHandler.connect();
	                //espera por primera conexión
	                while (wifiHandler.getConnectionState() == hlmp.NetLayer.Constants.WifiConnectionState.DISCONNECTED)
	                {
	                    commHandler.informationNetworkingHandler("NETHANDLER: waiting for other devices");
	                    Thread.sleep(netData.getWaitTimeStart());
	                }
	                commHandler.informationNetworkingHandler("NETHANDLER: start wifi... ok!");
	                //Setea la IP en el sistema operativo
	                Boolean ipChange = false;
	                int timeOutIpChange = 0;
	                while (!ipChange)
	                {
	                    try
	                    {
	                        commHandler.informationNetworkingHandler("NETHANDLER: set IP... " + netData.getIpTcpListener().getHostAddress());
	                        SystemHandler.setStaticIP(netData.getNetworkAdapter(), netData.getIpTcpListener().getHostAddress(), netData.getSubnetMask());
	                        ipChange = true;
	                        commHandler.informationNetworkingHandler("NETHANDLER: set IP... ok!");
	                    }
//	                    catch (ThreadAbortException e)
//	                    {
//	                        throw e;
//	                    }
	                    catch (Exception e)
	                    {
	                        commHandler.informationNetworkingHandler("NETHANDLER: set IP... failed! " + e.getMessage());
	                        timeOutIpChange++;
	                        if (timeOutIpChange > netData.getWaitForStart())
	                        {
	                            throw new Exception("timeout, para configurar IP");
	                        }
	                        Thread.sleep(netData.getWaitTimeStart());
	                    }
	                }
	                //chequeo de ip
	                commHandler.informationNetworkingHandler("NETHANDLER: start strong DAD");
	                ipHandler.startStrongDAD();
	                //Servicios TCP
	                commHandler.informationNetworkingHandler("NETHANDLER: start TCP...");
	                //tcpListener.setReuseAddress(true);
	                Boolean tcpChange = false;
	                int timeOutTcpChange = 0;
	                while (!tcpChange)
	                {
	                    try
	                    {
	                        commHandler.informationNetworkingHandler("NETHANDLER: start TCP listener... " + netData.getIpTcpListener().getHostAddress() + ":" + netData.getTcpPort());
	                        //tcpListener.start();
	                        tcpListener = new ServerSocket(netData.getTcpPort());
	                        //tcpListener.bind(new InetSocketAddress(netData.getIpTcpListener(),netData.getTcpPort()));
	                        tcpChange = true;
	                        commHandler.informationNetworkingHandler("NETHANDLER: start TCP listener... ok!");
	                    }
//	                    catch (ThreadAbortException e)
//	                    {
//	                        throw e;
//	                    }
	                    catch (Exception e)
	                    {
	                    	e.printStackTrace();
	                    	commHandler.informationNetworkingHandler("NETHANDLER: start TCP listener... failed! " + e.getMessage());
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
	                commHandler.informationNetworkingHandler("NETHANDLER: start TCP... ok!");
	                //conecta a UDP
	                commHandler.informationNetworkingHandler("NETHANDLER: start UDP... " + netData.getIpUdpMulticast() + ":" + netData.getUdpPort());
	                
	                udpServer = new MulticastSocket(netData.getUdpPort());
	                udpClient = new DatagramSocket();
	                //udpClient.setReuseAddress(true);
	                
	                udpMulticastAdress = InetAddress.getByName(netData.getIpUdpMulticast());
	                udpServer.joinGroup(udpMulticastAdress);

	                udpClientThread.start();
	                commHandler.informationNetworkingHandler("NETHANDLER: start UDP... ok!");
	                //cambio a cheque de ip debil
	                commHandler.informationNetworkingHandler("NETHANDLER: start weak DAD");
	                ipHandler.chageToWeakDAD();
	                //Avisa que empiece la comunicacion
	                commHandler.startNetworkingHandler();
	                netHandlerState = NetHandlerState.STARTED;
	                commHandler.informationNetworkingHandler("NETHANDLER: start netHandler... ok!");
	                commHandler.informationNetworkingHandler("NETHANDLER: we are so connected!!! welcome to HLMP");
	            }
	            catch (InterruptedException e)
	            {
	                commHandler.informationNetworkingHandler("NETHANDLER: start netHandler... failed! " + e.getMessage());
	            }
	            catch (Exception e)
	            {
	                //disconnect();
	                commHandler.informationNetworkingHandler("NETHANDLER: start netHandler... failed! " + e.getMessage());
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
    	commHandler.informationNetworkingHandler("NETHANDLER: stop netHandler...");
        netHandlerState = NetHandlerState.STOPPING;
        try
        {
            commHandler.informationNetworkingHandler("NETHANDLER: stop DAD...");
            ipHandler.stop();
            commHandler.informationNetworkingHandler("NETHANDLER: stop DAD... ok!");
        }
        catch (Exception e)
        {
            commHandler.informationNetworkingHandler("NETHANDLER: stop DAD... failed! " + e.getMessage());
        }
        try
        {
            commHandler.informationNetworkingHandler("NETHANDLER: stop communication...");
            commHandler.stopNetworkingHandler();
            commHandler.informationNetworkingHandler("NETHANDLER: stop communication... ok!");
        }
        catch (Exception e)
        {
            commHandler.informationNetworkingHandler("NETHANDLER: stop communication... failed! " + e.getMessage());
        }
        //Cerrado de threads y canales de red UDP  
        try
        {
            commHandler.informationNetworkingHandler("NETHANDLER: drop multicast suscription...");
            udpServer.leaveGroup(udpMulticastAdress);
            commHandler.informationNetworkingHandler("NETHANDLER: drop multicast suscription... ok!");
        }
        catch (Exception e)
        {
            commHandler.informationNetworkingHandler("NETHANDLER: drop multicast suscription... failed! " + e.getMessage());
        }
        try
        {
            commHandler.informationNetworkingHandler("NETHANDLER: shutdown UDP client...");
            udpClient.close();
            //udpClient.Client.Shutdown(SocketShutdown.Both);
            commHandler.informationNetworkingHandler("NETHANDLER: shutdown UDP client... ok!");
        }
        catch (Exception e)
        {
            commHandler.informationNetworkingHandler(e.getMessage());
            commHandler.informationNetworkingHandler("NETHANDLER: shutdown UDP client... failed! " + e.getMessage());
        }
        try
        {
            commHandler.informationNetworkingHandler("NETHANDLER: close UDP socket...");
            udpServer.close();
            commHandler.informationNetworkingHandler("NETHANDLER: close UDP socket... ok!");
        }
        catch (Exception e)
        {
            commHandler.informationNetworkingHandler("NETHANDLER: close UDP socket... failed! " + e.getMessage());
        }
//        try
//        {
//            commHandler.informationNetworkingHandler("NETHANDLER: close UDP client...");
//            udpClient.Close();
//            commHandler.informationNetworkingHandler("NETHANDLER: close UDP client... ok!");
//        }
//        catch (Exception e)
//        {
//            commHandler.informationNetworkingHandler("NETHANDLER: close UDP client... failed! " + e.Message);
//        }
        try
        {
            commHandler.informationNetworkingHandler("NETHANDLER: stop UDP thread...");
            udpClientThread.interrupt();
            udpClientThread.join();
            commHandler.informationNetworkingHandler("NETHANDLER: stop UDP thread... ok!");
        }
        catch (Exception e)
        {
            commHandler.informationNetworkingHandler("NETHANDLER: stop UDP thread... failed! " + e.getMessage());
        }
        //Cerrado de threads y canales de red TCP
        try
        {
            commHandler.informationNetworkingHandler("NETHANDLER: stop TCP listener...");
            tcpListener.close();
            commHandler.informationNetworkingHandler("NETHANDLER: stop TCP listener... ok!");
        }
        catch (Exception e)
        {
            commHandler.informationNetworkingHandler("NETHANDLER: stop TCP listener... failed! " + e.getMessage());
        }
        try
        {
            commHandler.informationNetworkingHandler("NETHANDLER: stop TCP thread...");
            tcpListenerThread.interrupt();
            tcpListenerThread.join();
            commHandler.informationNetworkingHandler("NETHANDLER: stop TCP thread... ok!");
        }
        catch (Exception e)
        {
            commHandler.informationNetworkingHandler("NETHANDLER: stop TCP thread... failed! " + e.getMessage());
        }
      
        //se cierran las conexiones aun existentes TCP
        try
        {
            commHandler.informationNetworkingHandler("NETHANDLER: kill TCP links...");
            RemoteMachine[] serverRemoteMachines = tcpServerList.toObjectArray();
            for (int i = 0; i < serverRemoteMachines.length; i++)
            {
                try
                {
                    commHandler.informationNetworkingHandler("NETHANDLER: kill TCP link... " + serverRemoteMachines[i].getIp().getHostAddress());
                    killRemoteMachine(serverRemoteMachines[i]);
                    commHandler.informationNetworkingHandler("NETHANDLER: kill TCP link... ok!");
                }
                catch (Exception e)
                {
                    commHandler.informationNetworkingHandler("NETHANDLER: kill TCP link... failed! " + e.getMessage());
                }
            }
            commHandler.informationNetworkingHandler("NETHANDLER: kill TCP links... ok!");
        }
        catch (Exception e)
        {
            commHandler.informationNetworkingHandler("NETHANDLER: kill TCP links... failed! " + e.getMessage());
        }
        
        //detiene wifiHandler
        try
        {
            commHandler.informationNetworkingHandler("NETHANDLER: stop wifi...");
            wifiHandler.disconnect();
            commHandler.informationNetworkingHandler("NETHANDLER: stop wifi... ok!");
        }
        catch (Exception e)
        {
            commHandler.informationNetworkingHandler("NETHANDLER: stop wifi... failed! " + e.getMessage());
        }
        
        if(!reset)
        {
        	//se deja la IP en el sistema operativo como por defecto (DHCP)
        	try
        	{
        		commHandler.informationNetworkingHandler("NETHANDLER: dhcp on...");
        		SystemHandler.setDinamicIP(netData.getNetworkAdapter());
        		commHandler.informationNetworkingHandler("NETHANDLER: dhcp on... ok!");
        	}
        	catch (Exception e)
        	{
        		commHandler.informationNetworkingHandler("NETHANDLER: dhcp on... failed!" + e.getMessage());
        	}
        }
        
        
        //se cierran las viejas conexiones TCP
        try
        {
            commHandler.informationNetworkingHandler("NETHANDLER: kill TCP links..." + "(TCP is hard to kill)");
            RemoteMachine[] serverRemoteMachines = oldServerList.toObjectArray();
            for (int i = 0; i < serverRemoteMachines.length; i++)
            {
                try
                {
                    commHandler.informationNetworkingHandler("NETHANDLER: kill TCP link... " + serverRemoteMachines[i].getIp().getHostAddress());
                    killRemoteMachine(serverRemoteMachines[i]);
                    commHandler.informationNetworkingHandler("NETHANDLER: kill TCP link... ok!");
                }
                catch (Exception e)
                {
                    commHandler.informationNetworkingHandler("NETHANDLER: kill TCP link... failed! " + e.getMessage());
                }
            }
            commHandler.informationNetworkingHandler("NETHANDLER: kill TCP links... ok!");
        }
        catch (Exception e)
        {
            commHandler.informationNetworkingHandler("NETHANDLER: kill TCP links... failed! " + e.getMessage());
        }
        
        //reinicializacion de todos los objetos
        try
        {
            commHandler.informationNetworkingHandler("NETHANDLER: initialation of objects...");
            init();
            commHandler.informationNetworkingHandler("NETHANDLER: initialation of objects... ok!");
        }
        catch (Exception e)
        {
            commHandler.informationNetworkingHandler("NETHANDLER: initialation of objects... failed! " + e.getMessage());
        }
        netHandlerState = NetHandlerState.STOPPED;
        commHandler.informationNetworkingHandler("NETHANDLER: stop netHandler... ok!");
        commHandler.informationNetworkingHandler("NETHANDLER: bye bye!");
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
//				catch (IOException e)
//				{
//					throw e;
//				}
				catch (Exception e)
				{
					commHandler.informationNetworkingHandler("TCP WARINING: send failed " + e.getMessage());
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
				throw new Exception("there is no TCP link with that remote machine");
			}
		}
//		catch (InterruptedException e)
//		{
//			throw e;
//		}
		catch (Exception e)
		{
			commHandler.informationNetworkingHandler("TCP WARNING: send failed " + e.getMessage());
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
			commHandler.informationNetworkingHandler("UDP WARNING: send failed " + e.getMessage());
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
		//            commHandler.informationNetworkingHandler("UDP WARNING: send failed ErrorCode=" + e.ErrorCode);
		//            return false;
		//        }
		catch (Exception e)
		{
			commHandler.informationNetworkingHandler("UDP WARNING: send failed " + e.getMessage());
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
			commHandler.informationNetworkingHandler("TCP: connection...");
			InetAddress serverIp = null;
			if (o.getClass().equals(InetAddress.class)){
				serverIp = (InetAddress) o;
			}else{
				serverIp = InetAddress.getByName((String) o);
			}

			//IPAddress serverIp = (IPAddress)o;
			Socket tcpClient = new Socket(/*tcpAddress, 0*/);
			tcpClient.setSoLinger(false, 0);

			//tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, true);


			//Conexion asincrona con time out
			try{
				tcpClient.connect(new InetSocketAddress(serverIp, netData.getTcpPort()), netData.getTcpConnectTimeOut());
			}
			catch(SocketTimeoutException x){
				commHandler.informationNetworkingHandler("TCP: connection... time out!");
				return;
			}


			//IAsyncResult result = tcpClient.BeginConnect(serverIp, netData.TcpPort, null, null);
			//bool success = result.AsyncWaitHandle.WaitOne(netData.TcpConnectTimeOut, true);

			//tcpClient.EndConnect(result);
			ListenTCPMessagesThread clientThread = new ListenTCPMessagesThread(this);

			RemoteMachine remoteMachine = new RemoteMachine(serverIp, tcpClient, clientThread);
			clientThread.setRemoteMachine(remoteMachine);
			clientThread.start();
			RemoteMachine oldRemoteMachine = tcpServerList.getRemoteMachine(serverIp);
			if (oldRemoteMachine != null)
			{
				oldRemoteMachine.close();
				tcpServerList.remove(oldRemoteMachine);
			}
			tcpServerList.add(serverIp, remoteMachine);
			commHandler.informationNetworkingHandler("TCP: connection... ok!");

		}
		//		catch (InterruptedException e)
		//		{
		//			throw e;
		//		}
		catch (Exception e)
		{
			commHandler.informationNetworkingHandler("TCP: connection... failed! " + e.getMessage());
			commHandler.informationNetworkingHandler(e.getStackTrace().toString());
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
	private void disconnectFromAsync(Object o)
	{
		try
		{
			commHandler.informationNetworkingHandler("TCP: disconnection...");
			if(o.getClass().equals(InetAddress.class))
			{
				InetAddress machineIp = (InetAddress)o;
				RemoteMachine machine = tcpServerList.getRemoteMachine(machineIp);
				commHandler.informationNetworkingHandler("TCP: old list queue");
				tcpServerList.remove(machine);
				oldServerList.add(machine.getIp(), machine);
			}
			else if (o.getClass().equals(RemoteMachine.class))
			{
				RemoteMachine machine = (RemoteMachine)o;
				commHandler.informationNetworkingHandler("TCP: old list queue");
				tcpServerList.remove(machine);
				oldServerList.add(machine.getIp(), machine);
			}
			commHandler.informationNetworkingHandler("TCP: disconnection... ok!");
		}
//		catch (InterruptedException e)
//		{
//			throw e;
//		}
		catch (Exception e)
		{
			commHandler.informationNetworkingHandler("TCP: disconnection... failed! " + e.getMessage());
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
			commHandler.informationNetworkingHandler("TCP: kill...");
			if(o.getClass().equals(InetAddress.class))
			{
				InetAddress machineIp = (InetAddress)o;
				RemoteMachine machine = tcpServerList.getRemoteMachine(machineIp);
				commHandler.informationNetworkingHandler("TCP: close machine");
				machine.close();
				commHandler.informationNetworkingHandler("TCP: drop from queue");
				tcpServerList.remove(machine);
			}
			else if (o.getClass().equals(RemoteMachine.class))
			{
				RemoteMachine machine = (RemoteMachine)o;
				commHandler.informationNetworkingHandler("TCP: close machine");
				machine.close();
				commHandler.informationNetworkingHandler("TCP: drop from queue");
				tcpServerList.remove(machine);
			}
			commHandler.informationNetworkingHandler("TCP: kill... ok!");
		}
//		catch (InterruptedException e)
//		{
//			throw e;
//		}
		catch (Exception e)
		{
			commHandler.informationNetworkingHandler("TCP: kill... failed! " + e.getMessage());
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
				commHandler.informationNetworkingHandler("TCP WARNING: TCP message dropped");
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

	@Override
	public void wifiInformation(String message) {
		commHandler.informationNetworkingHandler("WIFI: " + message);

	}

	@Override
	public void resetIp() {
		if(iphandlerPoint.compareAndSet(0, 1))
		{
			try
			{
				resetThread.start();
			}
//			catch (ThreadAbortException e)
//			{
//				throw e;
//			}
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
		            	commHandler.informationNetworkingHandler("TCP: accepting client ...");
		                Socket tcpClient = tcpListener.accept();
		                commHandler.informationNetworkingHandler("TCP: new client detected");
		                tcpClient.setSoLinger(false, 0);
		                
		                //tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, true);
		                InetAddress ip = tcpClient.getInetAddress();
		                ListenTCPMessagesThread clientThread = new ListenTCPMessagesThread(myself);
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
		                commHandler.informationNetworkingHandler("TCP: new client connected");
		            }
		        }
//		        catch (InterruptedException e)
//		        {
//		            throw e;
//		        }
		        catch (Exception e)
		        {
		            commHandler.informationNetworkingHandler("TCP WARNING: TCP listener has stopped!! " + e.getMessage());
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
		                    	commHandler.informationNetworkingHandler("UDP WARNING: upd receive wrong message, bad length");
	                        }
	                    }else{
	                    	// agregado por NM
	                    	commHandler.informationNetworkingHandler("UDP WARNING: upd receive wrong message, lenght less than 4");
	                    }
	                }
	            }
//	            catch (InterruptedException e)
//	            {
//	                throw e;
//	            }
	            catch (Exception e)
	            {
	                commHandler.informationNetworkingHandler("UDP WARNING: udp client has stopped!!! " + e.getMessage());
	            }
			}
	
		};
	}

	public void informationNetworkingHandler(String message){
		commHandler.informationNetworkingHandler("WIFI: " + message);
	}
}
