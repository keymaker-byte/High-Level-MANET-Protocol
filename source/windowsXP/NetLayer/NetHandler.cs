using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.IO;


namespace NetLayer
{
    /// <summary>
    /// Clase que enumera los estados posibles del NetHandler
    /// </summary>
    internal static class NetHandlerState
    {
        public const Int32 STARTING = 1;
        public const Int32 STARTED = 2;
        public const Int32 STOPPING = 3;
        public const Int32 STOPPED = 4;
        public const Int32 INITIATED = 5;
        public const Int32 STOPFORCED = 6;
    }

    /// <summary>
    /// Clase de bajo nivel que se encarga de construir y administrar la red adhoc
    /// </summary>
    public class NetHandler
    {
        
        /// <summary>
        /// thread de escucha para clientes TCP
        /// </summary>
        private Thread tcpListenerThread;
        /// <summary>
        /// Servidor de mensajes TCP
        /// </summary>
        private TcpListener tcpListener;
        /// <summary>
        /// Lista de servidores TCP
        /// </summary>
        private RemoteMachineList tcpServerList;
        /// <summary>
        /// IpAddress de TCP
        /// </summary>
        private IPAddress tcpAddress;
        /// <summary>
        /// thread de escucha de mensajes UDP
        /// </summary>
        private Thread udpClientThread;
        /// <summary>
        /// Serividor de mensajes UDP
        /// </summary>
        private UdpClient udpClient;
        /// <summary>
        /// EndPoint UDP
        /// </summary>
        private IPEndPoint udpIpEndPoint;
        /// <summary>
        /// IpAddress UDP
        /// </summary>
        private IPAddress udpMulticastAdress;
        /// <summary>
        /// Cola de mensajes UDP Multicast leidos
        /// </summary>
        private NetMessageQueue udpMessageQueue;
        /// <summary>
        /// Cola de mensajes TCP leidos
        /// </summary>
        private NetMessageQueue tcpMessageQueue;
        /// <summary>
        /// Configuración de red
        /// </summary>
        private NetData netData;
        /// <summary>
        /// Handler de eventos de comunicación
        /// </summary>
        private CommHandlerI commHandler;
        /// <summary>
        /// Lock para conectar y desconectar
        /// </summary>
        private Object connectLock;
        /// <summary>
        /// estado de este objeto (un valor de NetHandlerState)
        /// </summary>
        private Int32 netHandlerState;
        /// <summary>
        /// Manejador de la conexion de red inalambrica
        /// </summary>
        private WifiHandler wifiHandler;
        /// <summary>
        /// Thread de partida
        /// </summary>
        private Thread startThread;
        /// <summary>
        /// Control de stop
        /// </summary>
        private Int32 stopPoint;
        /// <summary>
        /// Control de duplicación de Ip
        /// </summary>
        private IpHandler ipHandler;
        /// <summary>
        /// Variable de control para el cambio de IP
        /// </summary>
        private Int32 iphandlerPoint;
        /// <summary>
        /// Thread de reset
        /// </summary>
        private Thread resetThread;
        /// <summary>
        /// lock para reset
        /// </summary>
        private Object resetLock;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="netData">Los parámetros de configuración</param>
        /// <param name="commHandler">El comunicador que maneja los eventos generados en la red</param>
        public NetHandler(NetData netData, CommHandlerI commHandler)
        {
            this.netData = netData;
            this.commHandler = commHandler;
            this.connectLock = new Object();
            this.resetLock = new Object();
            this.iphandlerPoint = 0;
            netData.pickNewIp();
            init();
        }

        /// <summary>
        /// Lista de maquinas de la red adhoc que son directamente visibles para esta máquina.
        /// Se posee una conexión TCP directa con cada una de ellas.
        /// </summary>
        public RemoteMachineList TcpServerList
        {
            get { return tcpServerList; }
        }

        /// <summary>
        /// Cola de mensajes UDP que ha recibido esta máquina
        /// </summary>
        public NetMessageQueue UdpMessageQueue
        {
            get { return udpMessageQueue; }
        }

        /// <summary>
        /// Cola de mensajes TCP que ha recibido esta máquina
        /// </summary>
        public NetMessageQueue TcpMessageQueue
        {
            get { return tcpMessageQueue; }
        }

        /// <summary>
        /// Los datos de red
        /// </summary>
        public NetData NetData
        {
            get { return netData; }
        }


        /// <summary>
        /// Inicializa las propiedades de la clase
        /// </summary>
        private void init()
        {
            //inicializa las listas
            udpMessageQueue = new NetMessageQueue();
            tcpMessageQueue = new NetMessageQueue();
            tcpServerList = new RemoteMachineList();
            //inicializa los objetos para TCP
            tcpAddress = netData.IpTcpListener;
            tcpListener = new TcpListener(tcpAddress, netData.TcpPort);
            tcpListenerThread = new Thread(new ThreadStart(listenTCPClients));
            tcpListenerThread.Name = "TCP NetHandler Main Thread";
            //inicializa los objetos UDP
            udpClient = new UdpClient();
            udpMulticastAdress = IPAddress.Parse(netData.IpUdpMulticast);
            udpClientThread = new Thread(new ThreadStart(listenUDPMessages));
            udpClientThread.Name = "UDP NetHandler Main Thread";
            //estado
            netHandlerState = NetHandlerState.INITIATED;
            wifiHandler = new WifiHandler(netData, wifiInformation);
            startThread = new Thread(new ThreadStart(start));
            stopPoint = 0;
            ipHandler = new IpHandler(netData, resetIp);
            resetThread = new Thread(new ThreadStart(reset));
        }

        /// <summary>
        /// Levanta los servicios, Esta función es No bloqueante, levanta un thread
        /// Esta funcion siempre gatilla startNetworkingHandler o gatilla una excepcion
        /// </summary>
        public void connect()
        {
            lock (connectLock)
            {
                lock (resetLock)
                {
                    if (Interlocked.CompareExchange(ref stopPoint, 0, 0) == 0)
                    {
                        if (netHandlerState == NetHandlerState.INITIATED || netHandlerState == NetHandlerState.STOPPED)
                        {
                            startThread.Start();
                        }
                    }  
                }
            }
        }

        /// <summary>
        /// Termina los servicios, Esta función es Bloqueante hasta que termine
        /// esta funcion siempre gatilla stopNetworkingHandler
        /// </summary>
        public void disconnect()
        {
            if (Interlocked.CompareExchange(ref stopPoint, 1, 0) == 0)
            {
                lock (connectLock)
                {
                    try
                    {
                        startThread.Abort();
                        startThread.Join();
                    }
                    catch (Exception e)
                    {
                        commHandler.informationNetworkingHandler("NETHANDLER: disconnetc aborting start " + e.Message);
                    }
                    try
                    {
                        resetThread.Abort();
                        resetThread.Join();
                    }
                    catch (Exception e)
                    {
                        commHandler.informationNetworkingHandler("NETHANDLER: disconnetc aborting reset " + e.Message);
                    }
                    stop();
                    stopPoint = 0; 
                }
            } 
        }

        /// <summary>
        /// Funcion para que gatilla IpHandler solamente
        /// </summary>
        private void resetIp()
        {
            if (Interlocked.CompareExchange(ref iphandlerPoint, 1, 0) == 0)
            {
                try
                {
                    resetThread.Start();
                }
                catch (ThreadAbortException e)
                {
                    throw e;
                }
                catch (Exception)
                {
                }
                iphandlerPoint = 0;
            }
        }

        /// <summary>
        /// Resetea la red
        /// </summary>
        private void reset()
        {
            commHandler.informationNetworkingHandler("NETHANDLER: resetIP...");
            lock (resetLock)
            {
                commHandler.resetNetworkingHandler();
                netData.pickNewIp();

                try
                {
                    startThread.Abort();
                    startThread.Join();
                }
                catch (ThreadAbortException e)
                {
                    throw e;
                }
                catch (Exception)
                {
                }

                stop();
                connect();
            }
            commHandler.informationNetworkingHandler("NETHANDLER: resetIP... ok!");
        }

        /// <summary>
        /// Se gatilla para levantar los servicios
        /// Si ocurre un error se arroja
        /// </summary>
        private void start()
        {
            try
            {
                commHandler.informationNetworkingHandler("NETHANDLER: start netHandler...");
                netHandlerState = NetHandlerState.STARTING;
                commHandler.informationNetworkingHandler("NETHANDLER: nerwork adapter is... " + netData.NetworkAdapter.Description);
                ////intenta poner la ip en modo estatico
                try
                {
                    commHandler.informationNetworkingHandler("NETHANDLER: disable adapter...");
                    SystemHandler.disableIpAdapter(netData.NetworkAdapter);
                    commHandler.informationNetworkingHandler("NETHANDLER: disable adapter... ok!");
                }
                catch (ThreadAbortException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    commHandler.informationNetworkingHandler("NETHANDLER: disable adapter... failed! " + e.Message);
                }
                try
                {
                    commHandler.informationNetworkingHandler("NETHANDLER: set IP... " + netData.IpTcpListener);
                    SystemHandler.setStaticIP(netData.NetworkAdapter, netData.IpTcpListener.ToString(), netData.SubnetMask);
                    commHandler.informationNetworkingHandler("NETHANDLER: set IP... ok!");
                }
                catch (ThreadAbortException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    commHandler.informationNetworkingHandler("NETHANDLER: set IP... failed! " + e.Message);
                }
                try
                {
                    commHandler.informationNetworkingHandler("NETHANDLER: enable adapter...");
                    SystemHandler.enableIpAdapter(netData.NetworkAdapter);
                    commHandler.informationNetworkingHandler("NETHANDLER: enable adapter... ok!");
                }
                catch (ThreadAbortException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    commHandler.informationNetworkingHandler("NETHANDLER: enable adapter... failed! " + e.Message);
                }

                //hecha a andar wifiHandler
                commHandler.informationNetworkingHandler("NETHANDLER: start wifi...");
                wifiHandler.connect();
                //espera por primera conexión
                while (wifiHandler.ConnectionState == WifiConnectionState.DISCONNECTED)
                {
                    commHandler.informationNetworkingHandler("NETHANDLER: waiting for other devices");
                    System.Threading.Thread.Sleep(netData.WaitTimeStart);
                }
                commHandler.informationNetworkingHandler("NETHANDLER: start wifi... ok!");
                //Setea la IP en el sistema operativo
                Boolean ipChange = false;
                Int32 timeOutIpChange = 0;
                while (!ipChange)
                {
                    try
                    {
                        commHandler.informationNetworkingHandler("NETHANDLER: set IP... " + netData.IpTcpListener);
                        SystemHandler.setStaticIP(netData.NetworkAdapter, netData.IpTcpListener.ToString(), netData.SubnetMask);
                        ipChange = true;
                        commHandler.informationNetworkingHandler("NETHANDLER: set IP... ok!");
                    }
                    catch (ThreadAbortException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        commHandler.informationNetworkingHandler("NETHANDLER: set IP... failed! " + e.Message);
                        timeOutIpChange++;
                        if (timeOutIpChange > netData.WaitForStart)
                        {
                            throw new Exception("timeout, para configurar IP");
                        }
                        System.Threading.Thread.Sleep(netData.WaitTimeStart);
                    }
                }
                //chequeo de ip
                commHandler.informationNetworkingHandler("NETHANDLER: start strong DAD");
                ipHandler.startStrongDAD();
                //Servicios TCP
                commHandler.informationNetworkingHandler("NETHANDLER: start TCP...");
                tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                Boolean tcpChange = false;
                Int32 timeOutTcpChange = 0;
                while (!tcpChange)
                {
                    try
                    {
                        commHandler.informationNetworkingHandler("NETHANDLER: start TCP listener... " + netData.IpTcpListener + ":" + netData.TcpPort);
                        tcpListener.Start();
                        tcpChange = true;
                        commHandler.informationNetworkingHandler("NETHANDLER: start TCP listener... ok!");
                    }
                    catch (ThreadAbortException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        commHandler.informationNetworkingHandler("NETHANDLER: start TCP listener... failed! " + e.Message);
                        timeOutTcpChange++;
                        if (timeOutTcpChange > netData.WaitForStart)
                        {
                            throw new Exception("timeout, para levantar servicio TCP");
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(netData.WaitTimeStart);
                        }
                    }
                }
                tcpListenerThread.Start();
                commHandler.informationNetworkingHandler("NETHANDLER: start TCP... ok!");
                //conecta a UDP
                commHandler.informationNetworkingHandler("NETHANDLER: start UDP... " + netData.IpUdpMulticast + ":" + netData.UdpPort);
                udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                //udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, netData.UdpPort));
                udpClient.Client.Bind(new IPEndPoint(tcpAddress, netData.UdpPort));
                udpClient.JoinMulticastGroup(udpMulticastAdress, tcpAddress);
                udpIpEndPoint = new IPEndPoint(udpMulticastAdress, netData.UdpPort);
                udpClientThread.Start();
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
            catch (ThreadAbortException e)
            {
                commHandler.informationNetworkingHandler("NETHANDLER: start netHandler... failed! " + e.Message);
            }
            catch (Exception e)
            {
                //disconnect();
                commHandler.informationNetworkingHandler("NETHANDLER: start netHandler... failed! " + e.Message);
                commHandler.errorNetworkingHandler(e);
            }
        }

        /// <summary>
        /// Se gatilla para terminar los servicios
        /// Si ocurre algun error se informa en informationNetworkingHandler, no se detiene ejecución
        /// </summary>
        private void stop()
        {
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
                commHandler.informationNetworkingHandler("NETHANDLER: stop DAD... failed! " + e.Message);
            }
            try
            {
                commHandler.informationNetworkingHandler("NETHANDLER: stop communication...");
                commHandler.stopNetworkingHandler();
                commHandler.informationNetworkingHandler("NETHANDLER: stop communication... ok!");
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("NETHANDLER: stop communication... failed! " + e.Message);
            }
            //Cerrado de threads y canales de red UDP  
            try
            {
                commHandler.informationNetworkingHandler("NETHANDLER: drop multicast suscription...");
                udpClient.DropMulticastGroup(udpMulticastAdress);
                commHandler.informationNetworkingHandler("NETHANDLER: drop multicast suscription... ok!");
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("NETHANDLER: drop multicast suscription... failed! " + e.Message);
            }
            try
            {
                commHandler.informationNetworkingHandler("NETHANDLER: shudown UDP client...");
                udpClient.Client.Shutdown(SocketShutdown.Both);
                commHandler.informationNetworkingHandler("NETHANDLER: shudown UDP client... ok!");
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler(e.Message);
                commHandler.informationNetworkingHandler("NETHANDLER: shudown UDP client... failed! " + e.Message);
            }
            try
            {
                commHandler.informationNetworkingHandler("NETHANDLER: close UDP socket...");
                udpClient.Client.Close();
                commHandler.informationNetworkingHandler("NETHANDLER: close UDP socket... ok!");
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("NETHANDLER: close UDP socket... failed! " + e.Message);
            }
            try
            {
                commHandler.informationNetworkingHandler("NETHANDLER: close UDP client...");
                udpClient.Close();
                commHandler.informationNetworkingHandler("NETHANDLER: close UDP client... ok!");
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("NETHANDLER: close UDP client... failed! " + e.Message);
            }
            try
            {
                commHandler.informationNetworkingHandler("NETHANDLER: stop UDP thread...");
                udpClientThread.Abort();
                udpClientThread.Join();
                commHandler.informationNetworkingHandler("NETHANDLER: stop UDP thread... ok!");
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("NETHANDLER: stop UDP thread... failed! " + e.Message);
            }
            //Cerrado de threads y canales de red TCP
            try
            {
                commHandler.informationNetworkingHandler("NETHANDLER: stop TCP listener...");
                tcpListener.Stop();
                commHandler.informationNetworkingHandler("NETHANDLER: stop TCP listener... ok!");
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("NETHANDLER: stop TCP listener... failed! " + e.Message);
            }
            try
            {
                commHandler.informationNetworkingHandler("NETHANDLER: stop TCP thread...");
                tcpListenerThread.Abort();
                tcpListenerThread.Join();
                commHandler.informationNetworkingHandler("NETHANDLER: stop TCP thread... ok!");
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("NETHANDLER: stop TCP thread... failed! " + e.Message);
            }
            //se deja la IP en el sistema operativo como por defecto (DHCP)
            try
            {
                commHandler.informationNetworkingHandler("NETHANDLER: dhcp on...");
                SystemHandler.setDinamicIP(netData.NetworkAdapter);
                commHandler.informationNetworkingHandler("NETHANDLER: dhcp on... ok!");
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("NETHANDLER: dhcp on... failed!" + e.Message);
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
                commHandler.informationNetworkingHandler("NETHANDLER: stop wifi... failed! " + e.Message);
            }
            //se cierran las conexiones aun existentes TCP
            try
            {
                commHandler.informationNetworkingHandler("NETHANDLER: kill TCP links...");
                RemoteMachine[] serverRemoteMachines = tcpServerList.toObjectArray();
                for (int i = 0; i < serverRemoteMachines.Length; i++)
                {
                    try
                    {
                        commHandler.informationNetworkingHandler("NETHANDLER: kill TCP link... " + serverRemoteMachines[i].Ip);
                        disconnectFrom(serverRemoteMachines[i]);
                        commHandler.informationNetworkingHandler("NETHANDLER: kill TCP link... ok!");
                    }
                    catch (Exception e)
                    {
                        commHandler.informationNetworkingHandler("NETHANDLER: kill TCP link... failed! " + e.Message);
                    }
                }
                commHandler.informationNetworkingHandler("NETHANDLER: kill TCP links... ok!");
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("NETHANDLER: kill TCP links... failed! " + e.Message);
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
                commHandler.informationNetworkingHandler("NETHANDLER: initialation of objects... failed! " + e.Message);
            }
            netHandlerState = NetHandlerState.STOPPED;
            commHandler.informationNetworkingHandler("NETHANDLER: stop netHandler... ok!");
            commHandler.informationNetworkingHandler("NETHANDLER: bye bye!");
        }

        /// <summary>
        /// Registra una ip externa para el chequeo de ip duplicada
        /// </summary>
        /// <param name="ip">la ip a registrar</param>
        public void registerIp(IPAddress ip)
        {
            ipHandler.put(ip);
        }

        /// <summary>
        /// Función que gatilla WifiHandler
        /// </summary>
        /// <param name="message">los mensages recibidos</param>
        private void wifiInformation(String message)
        {
            commHandler.informationNetworkingHandler("WIFI: " + message);
        }

        /// <summary>
        /// Envía un mensaje TCP a todas las máquinas remotas visibles
        /// Si ocurre algun error se arroja
        /// </summary>
        /// <param name="message">el mensaje a envíar</param>
        public void sendTcpMessage(NetMessage message)
        {
            RemoteMachine[] serverRemoteMachines = tcpServerList.toObjectArray();
            for (int i = 0; i < serverRemoteMachines.Length; i++)
            {
                sendTcpMessage(message, serverRemoteMachines[i].Ip);
            }
        }

        /// <summary>
        /// Envia un mensaje UDP a todas las maquinas remotas visibles
        /// Si ocurre un error se informa
        /// </summary>
        /// <param name="message">El mensaje a envíar</param>
        public bool sendUdpMessage(NetMessage message)
        {
            try
            {
                byte[] lenght = BitConverter.GetBytes(message.getSize());
                byte[] netByteMessage = new byte[4 + message.getSize()];
                lenght.CopyTo(netByteMessage, 0);
                message.Body.CopyTo(netByteMessage, 4);
                udpClient.Send(netByteMessage, netByteMessage.Length, udpIpEndPoint);
                return true;
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("UDP WARNING: send failed " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Envia un mensaje UDP a todas las maquinas remotas visibles
        /// Si ocurre un error se informa
        /// </summary>
        /// <param name="message">El mensaje a envíar</param>
        /// <param name="ip">la direccion IP a la cual enviar el mensaje</param>
        public bool sendUdpMessage(NetMessage message, IPAddress ip)
        {
            try
            {
                byte[] lenght = BitConverter.GetBytes(message.getSize());
                byte[] netByteMessage = new byte[4 + message.getSize()];
                lenght.CopyTo(netByteMessage, 0);
                message.Body.CopyTo(netByteMessage, 4);
                udpClient.Send(netByteMessage, netByteMessage.Length, ip.ToString(), netData.UdpPort);
                return true;
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (SocketException e)
            {
                commHandler.informationNetworkingHandler("UDP WARNING: send failed ErrorCode=" + e.ErrorCode);
                return false;
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("UDP WARNING: send failed " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Envía un mensaje TCP a la maquina cuya ip es pasada como parámetro
        /// Si ocurre un error se arroja
        /// </summary>
        /// <param name="netMessage">El mensaje a envíar</param>
        /// <param name="ip">la IP de la máquina destino</param>
        public bool sendTcpMessage(NetMessage netMessage, IPAddress ip)
        {
            try
            {
                RemoteMachine remoteMachine = tcpServerList.getRemoteMachine(ip); 
                if (remoteMachine != null)
                {
                    try
                    {
                        remoteMachine.sendNetMessage(netMessage, netData.TimeOutWriteTCP);
                    }
                    catch (ThreadAbortException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        commHandler.informationNetworkingHandler("TCP WARINING: send failed " + e.Message);
                        if (remoteMachine.Fails >= netData.SendFailsToDisconnect)
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
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("TCP WARNING: send failed " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Se conecta a una máquina remota por TCP para enviarle mensajes posteriormente
        /// Si ocurre un error se notifica en informationNetworkingHandler
        /// </summary>
        /// <param name="serverIp">La dirección IP de la máquina remota, debe ser un String</param>
        public void connectTo(IPAddress serverIp)
        {
            connectToAsync(serverIp);
        }

        /// <summary>
        /// Se conecta a una máquina remota por TCP para enviarle mensajes posteriormente
        /// Si ocurre un error se notifica en informationNetworkingHandler
        /// </summary>
        /// <param name="o">La dirección IP de la máquina remota en formato IPAddress</param>
        private void connectToAsync(Object o)
        {
            try
            {
                commHandler.informationNetworkingHandler("TCP: connection...");
                IPAddress serverIp = (IPAddress)o;
                TcpClient tcpClient = new TcpClient(new IPEndPoint(tcpAddress, 0));
                tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, true);
                tcpClient.LingerState.LingerTime = 0;
                tcpClient.LingerState.Enabled = false;
                //Conexion asincrona con time out
                IAsyncResult result = tcpClient.BeginConnect(serverIp, netData.TcpPort, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(netData.TcpConnectTimeOut, true);
                if (!success)
                {
                    commHandler.informationNetworkingHandler("TCP: connection... time out!");
                    return;
                }
                else
                {
                    tcpClient.EndConnect(result);
                    Thread clientThread = new Thread(new ParameterizedThreadStart(listenTCPMessages));
                    RemoteMachine remoteMachine = new RemoteMachine(serverIp, tcpClient, clientThread);
                    clientThread.Start(remoteMachine);
                    RemoteMachine oldRemoteMachine = tcpServerList.getRemoteMachine(serverIp);
                    if (oldRemoteMachine != null)
                    {
                        oldRemoteMachine.close();
                        tcpServerList.remove(oldRemoteMachine);
                    }
                    tcpServerList.add(serverIp, remoteMachine);
                    commHandler.informationNetworkingHandler("TCP: connection... ok!");
                }
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("TCP: connection... failed! " + e.Message);
                commHandler.informationNetworkingHandler(e.StackTrace);
            }
        }

        /// <summary>
        /// Desconecta los servicios TCP asociados a una maquina remota
        /// </summary>
        /// <param name="machineIp">la Ip de la maquina a desconectar debe ser un String</param>
        public void disconnectFrom(IPAddress machineIp)
        {
            disconnectFromAsync(machineIp);
        }

        /// <summary>
        /// Desconecta los servicios TCP asociados a una maquina remota
        /// </summary>
        /// <param name="machine">la maquina a desconectar debe ser un String</param>
        public void disconnectFrom(RemoteMachine machine)
        {
            disconnectFromAsync(machine);
        }

        /// <summary>
        /// Desconecta los servicios TCP asociados a una maquina remota
        /// </summary>
        /// <param name="o">La ip de la maquina remota a desconectar en formato de String o la maquina remota</param>
        private void disconnectFromAsync(Object o)
        {
            try
            {
                commHandler.informationNetworkingHandler("TCP: disconnection...");
                if (o.GetType().Equals(typeof(IPAddress)))
                {
                    IPAddress machineIp = (IPAddress)o;
                    RemoteMachine machine = tcpServerList.getRemoteMachine(machineIp);
                    commHandler.informationNetworkingHandler("TCP: close signal");
                    machine.close();
                    commHandler.informationNetworkingHandler("TCP: remove from queue");
                    tcpServerList.remove(machine);
                }
                else if (o.GetType().Equals(typeof(RemoteMachine)))
                {
                    RemoteMachine machine = (RemoteMachine)o;
                    commHandler.informationNetworkingHandler("TCP: close signal");
                    machine.close();
                    commHandler.informationNetworkingHandler("TCP: remove from queue");
                    tcpServerList.remove(machine);
                }
                commHandler.informationNetworkingHandler("TCP: disconnection... ok!");
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("TCP: disconnection... failed! " + e.Message);
            }
        }

        /// <summary>
        /// Escucha nuevos clientes TCP que quieran establecer comunicación con ésta máquina
        /// Si ocurre un error se notifica en errorNetworkingHandler
        /// </summary>
        private void listenTCPClients()
        {
            try
            {
                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    commHandler.informationNetworkingHandler("TCP: new client detected");
                    tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                    tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, true);
                    tcpClient.LingerState.LingerTime = 0; 
                    tcpClient.LingerState.Enabled = false;
                    IPAddress ip = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
                    Thread clientThread = new Thread(new ParameterizedThreadStart(listenTCPMessages));
                    RemoteMachine remoteMachine = new RemoteMachine(ip, tcpClient, clientThread);
                    clientThread.Start(remoteMachine);
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
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("TCP WARNING: TCP listener has stopped!! " + e.Message);
            }
        }

        /// <summary>
        /// Escucha mensajes UDP recibidos
        /// Si ocurre un error se notifica en errorNetworkingHandler
        /// </summary>
        private void listenUDPMessages()
        {
            try
            {
                while (true)
                {
                    IPEndPoint ipEndPoint = new IPEndPoint(tcpAddress, netData.UdpPort);
                    byte[] buffer = udpClient.Receive(ref ipEndPoint);
                    if (buffer.Length > 4)
                    {
                        Int32 size = BitConverter.ToInt32(buffer, 0);
                        if (buffer.Length == 4 + size)
                        {
                            byte[] body = new byte[size];
                            Array.Copy(buffer, 4, body, 0, size);
                            NetMessage message = new NetMessage(body);
                            udpMessageQueue.put(message);
                        }
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("UDP WARNING: udp client has stopped!!! " + e.Message);
            }
        }

        /// <summary>
        /// Escucha mensajes TCP recibidos.
        /// Este método es gatillado por los threads asignados a cada cliente ya conectado.
        /// Si ocurre un error se notifica en errorNetworkingHandler
        /// </summary>
        /// <param name="o">la máquina remota que envía el mensaje, debe ser un objeto RemoteMachine</param>
        private void listenTCPMessages(Object o)
        {
            RemoteMachine remoteMachine = (RemoteMachine)o;
            try
            {
                while (true)
                {
                    StringBuilder blockData = new StringBuilder();
                    NetworkStream nStream = remoteMachine.TcpClient.GetStream();
                    byte[] length = new byte[4];
                    int m = nStream.Read(length, 0, 4);
                    while (m < 4)
                    {
                        m += nStream.Read(length, m, 4 - m);
                    }
                    byte[] data = new byte[BitConverter.ToInt32(length, 0)];
                    int n = nStream.Read(data, 0, data.Length);
                    while (n < data.Length)
                    {
                        n += nStream.Read(data, n, data.Length - n);
                    }
                    NetMessage message = new NetMessage(data);
                    addTCPMessages(message);
                }
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("TCP WARNING: header reading failed " + e.Message);
                disconnectFrom(remoteMachine);
            }
        }

        /// <summary>
        /// Agrega mensajes TCP recibidos a la cola
        /// </summary>
        /// <param name="message">el mensaje recibido</param>
        private void addTCPMessages(NetMessage message)
        {
            try
            {
                //TODO PARAMETRIZAR
                if (tcpMessageQueue.size() < 50)
                {
                    tcpMessageQueue.put(message);
                }
                else
                {
                    commHandler.informationNetworkingHandler("TCP WARNING: TCP message dropped");
                }
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
        } 
        
    }
}