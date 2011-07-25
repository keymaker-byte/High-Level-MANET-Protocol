using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.IO;


namespace NetLayerCompact
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
        /// Lista de servidores TCP que se deben cerrar al final
        /// </summary>
        private RemoteMachineList oldServerList;
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
            oldServerList = new RemoteMachineList();
            //inicializa los objetos para TCP
            tcpAddress = netData.IpTcpListener;
            tcpListener = new TcpListener(tcpAddress, netData.TcpPort);
            tcpListenerThread = new Thread(new ThreadStart(listenTCPClients));
            //inicializa los objetos UDP
            udpClient = new UdpClient();
            udpMulticastAdress = IPAddress.Parse(netData.IpUdpMulticast);
            udpClientThread = new Thread(new ThreadStart(listenUDPMessages));
            //estado
            netHandlerState = NetHandlerState.INITIATED;
            wifiHandler = new WifiHandler(netData, wifiInformation);
            startThread = new Thread(new ThreadStart(start));
            stopPoint = 0;
            ipHandler = new IpHandler(netData, resetIp);
            resetThread = new Thread(new ThreadStart(reset));
        }

        /// Levanta los servicios, Esta función es No bloqueante, levanta un thread
        /// Esta funcion siempre gatilla startNetworkingHandler o gatilla una excepcion
        /// </summary>
        /// <summary>
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
                        commHandler.informationNetworkingHandler("NETHANDLER: disconnect aborting start " + e.Message);
                    }
                    try
                    {
                        resetThread.Abort();
                        resetThread.Join();
                    }
                    catch (Exception e)
                    {
                        commHandler.informationNetworkingHandler("NETHANDLER: disconnect aborting reset " + e.Message);
                    }
                    stop(false);
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
            commHandler.informationNetworkingHandler("NETHANDLER: reset IP...");
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

                stop(true);
                connect();
            }
            commHandler.informationNetworkingHandler("NETHANDLER: reset IP... ok!");
        }

        /// <summary>
        /// Se gatilla para levantar los servicios
        /// Si ocurre un error se arroja
        /// </summary>
        private void start()
        {
            try
            {
                //comienza el proceso de conexion
                commHandler.informationNetworkingHandler("NETHANDLER: start netHandler...");
                netHandlerState = NetHandlerState.STARTING;
                commHandler.informationNetworkingHandler("NETHANDLER: nerwork adapter name ... " + netData.NetworkAdapter.Name);
                commHandler.informationNetworkingHandler("NETHANDLER: nerwork adapter description is... " + netData.NetworkAdapter.Description);

                switch (netData.OpSystem)
                {
                    case OpSystemType.WINMOBILE5POCKETPC:
                        {

                            //cambio de IP
                            commHandler.informationNetworkingHandler("NETHANDLER: setting IP...");
                            Boolean ipChange = false;
                            Int32 timeOutIpChange = 0;
                            while (!ipChange)
                            {
                                try
                                {
                                    commHandler.informationNetworkingHandler("NETHANDLER: setting IP... " + netData.IpTcpListener + "...");
                                    SystemHandler.setRegistryIP(netData);
                                    ipChange = SystemHandler.resetAdapter(netData);
                                    if (!ipChange)
                                    {
                                        throw new Exception("adapter not found");
                                    }
                                }
                                catch (ThreadAbortException e)
                                {
                                    throw e;
                                }
                                catch (Exception e)
                                {
                                    commHandler.informationNetworkingHandler("NETHANDLER: setting IP... failed! " + e.Message);
                                    timeOutIpChange++;
                                    if (timeOutIpChange > netData.WaitForStart)
                                    {
                                        throw new Exception("timeout, for IP config");
                                    }
                                    //reset el dispositivo de red inalambrico
                                    System.Threading.Thread.Sleep(netData.WaitTimeStart);
                                    netData.NetworkAdapter = SystemHandler.getWifiAdapter();
                                }
                            }
                            commHandler.informationNetworkingHandler("NETHANDLER: setting IP... ok!");

                            //hecha a andar wifiHandler
                            commHandler.informationNetworkingHandler("NETHANDLER: starting wifi...");
                            wifiHandler.connect();

                            //espera por primera conexión
                            Int32 timeOutWifiChange = 0;
                            while (wifiHandler.ConnectionState != WifiConnectionState.CONNECTED)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: waiting for other devices");
                                System.Threading.Thread.Sleep(netData.WaitTimeStart);
                                timeOutWifiChange++;
                                if (timeOutWifiChange > netData.WaitForWifi)
                                {
                                    try
                                    {
                                        commHandler.informationNetworkingHandler("NETHANDLER: reset adapter...");
                                        SystemHandler.resetAdapter(netData);
                                        commHandler.informationNetworkingHandler("NETHANDLER: reset adapter... ok!");
                                        timeOutWifiChange = 0;
                                    }
                                    catch (ThreadAbortException e)
                                    {
                                        throw e;
                                    }
                                    catch (Exception e)
                                    {
                                        commHandler.informationNetworkingHandler("NETHANDLER: reset Adapter... failed! " + e.Message);
                                    }
                                }
                            }
                            commHandler.informationNetworkingHandler("NETHANDLER: starting wifi... ok!");
                            break;
                        }
                    case OpSystemType.WINMOBILE5IPAQ:
                        {

                            //cambio de IP
                            commHandler.informationNetworkingHandler("NETHANDLER: setting IP...");
                            Boolean ipChange = false;
                            Int32 timeOutIpChange = 0;
                            while (!ipChange)
                            {
                                try
                                {
                                    commHandler.informationNetworkingHandler("NETHANDLER: setting IP... " + netData.IpTcpListener + "...");
                                    SystemHandler.setRegistryIP(netData);
                                    ipChange = SystemHandler.resetAdapter2(netData);
                                    if (!ipChange)
                                    {
                                        throw new Exception("adapter not found");
                                    }
                                }
                                catch (ThreadAbortException e)
                                {
                                    throw e;
                                }
                                catch (Exception e)
                                {
                                    commHandler.informationNetworkingHandler("NETHANDLER: setting IP... failed! " + e.Message);
                                    timeOutIpChange++;
                                    if (timeOutIpChange > netData.WaitForStart)
                                    {
                                        throw new Exception("timeout, for IP config");
                                    }
                                    //reset el dispositivo de red inalambrico
                                    System.Threading.Thread.Sleep(netData.WaitTimeStart);
                                    netData.NetworkAdapter = SystemHandler.getWifiAdapter();
                                }
                            }
                            commHandler.informationNetworkingHandler("NETHANDLER: setting IP... ok!");

                            //hecha a andar wifiHandler
                            commHandler.informationNetworkingHandler("NETHANDLER: starting wifi...");
                            wifiHandler.connect();

                            //espera por primera conexión
                            Int32 timeOutWifiChange = 0;
                            while (wifiHandler.ConnectionState != WifiConnectionState.CONNECTED)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: waiting for other devices");
                                System.Threading.Thread.Sleep(netData.WaitTimeStart);
                                timeOutWifiChange++;
                                if (timeOutWifiChange > netData.WaitForWifi)
                                {
                                    try
                                    {
                                        commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi...");
                                        SystemHandler.turnOnAdapter(netData, false);
                                        commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi... ok!");
                                        timeOutWifiChange = 0;
                                    }
                                    catch (ThreadAbortException e)
                                    {
                                        throw e;
                                    }
                                    catch (Exception e)
                                    {
                                        commHandler.informationNetworkingHandler("NETHANDLER: reset Adapter... failed! " + e.Message);
                                    }
                                }
                            }
                            commHandler.informationNetworkingHandler("NETHANDLER: starting wifi... ok!");
                            break;
                        }
                    case OpSystemType.WINMOBILE6SMARTPHONE:
                        {
                            //reset el dispositivo de red inalambrico 
                            try
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turning off wifi...");
                                SystemHandler.turnOffAdapter(netData);
                                commHandler.informationNetworkingHandler("NETHANDLER: turning off wifi... ok!");
                            }
                            catch (ThreadAbortException e)
                            {
                                throw e;
                            }
                            catch (Exception e)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turning off wifi... failed! " + e.Message);
                            }
                            try
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi...");
                                SystemHandler.turnOnAdapter(netData, true);
                                commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi... ok!");
                            }
                            catch (ThreadAbortException e)
                            {
                                throw e;
                            }
                            catch (Exception e)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi... failed!" + e.Message);
                            }

                            //cambia la IP del dispositivo
                            commHandler.informationNetworkingHandler("NETHANDLER: setting IP...");
                            Boolean ipChange = false;
                            Int32 timeOutIpChange = 0;
                            while (!ipChange)
                            {
                                try
                                {
                                    commHandler.informationNetworkingHandler("NETHANDLER: setting IP... " + netData.IpTcpListener);
                                    SystemHandler.setRegistryIP(netData);
                                    if (!(ipChange = SystemHandler.rebindAdapter(netData)))
                                    {
                                        throw new Exception("adapter not found");
                                    }
                                }
                                catch (ThreadAbortException e)
                                {
                                    throw e;
                                }
                                catch (Exception e)
                                {
                                    timeOutIpChange++;
                                    if (timeOutIpChange > netData.WaitForStart)
                                    {
                                        throw new Exception("timeout, for IP config");
                                    }
                                    commHandler.informationNetworkingHandler("NETHANDLER: setting IP... failed! " + e.Message);                                    
                                    System.Threading.Thread.Sleep(netData.WaitTimeStart);
                                }
                            }
                            commHandler.informationNetworkingHandler("NETHANDLER: setting IP... ok!");

                            //reset el dispositivo de red inalambrico
                            try
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi...");
                                SystemHandler.turnOnAdapter(netData, true);
                                commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi... ok!");
                            }
                            catch (ThreadAbortException e)
                            {
                                throw e;
                            }
                            catch (Exception e)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi... failed! " + e.Message);
                            }

                            //hecha a andar wifiHandler
                            commHandler.informationNetworkingHandler("NETHANDLER: starting wifi...");
                            wifiHandler.connect();

                            //espera por primera conexión del WIFI
                            Int32 timeOutWifiChange = 0;
                            while (wifiHandler.ConnectionState != WifiConnectionState.CONNECTED)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: waiting for other devices");
                                System.Threading.Thread.Sleep(netData.WaitTimeStart);
                                timeOutWifiChange++;
                                if (timeOutWifiChange > netData.WaitForWifi)
                                {
                                    //reset el dispositivo de red inalambrico
                                    try
                                    {
                                        commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi...");
                                        SystemHandler.turnOnAdapter(netData, false);
                                        commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi... ok!");
                                    }
                                    catch (ThreadAbortException e)
                                    {
                                        throw e;
                                    }
                                    catch (Exception e)
                                    {
                                        commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi... failed! " + e.Message);
                                    }
                                    timeOutWifiChange = 0;
                                }
                            }
                            commHandler.informationNetworkingHandler("NETHANDLER: starting wifi... ok!");
                            break;
                        }
                    case OpSystemType.SAMSUNGOMNIAII:
                        {
                            //reset el dispositivo de red inalambrico 
                            try
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turning off wifi...");
                                SystemHandler.turnOffAdapter(netData);
                                commHandler.informationNetworkingHandler("NETHANDLER: turning off wifi... ok!");
                            }
                            catch (ThreadAbortException e)
                            {
                                throw e;
                            }
                            catch (Exception e)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turning off wifi... failed! " + e.Message);
                            }
                            try
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi...");
                                SystemHandler.turnOnAdapter(netData, true);
                                commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi... ok!");
                            }
                            catch (ThreadAbortException e)
                            {
                                throw e;
                            }
                            catch (Exception e)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi... failed!" + e.Message);
                            }

                            //cambia la IP del dispositivo
                            commHandler.informationNetworkingHandler("NETHANDLER: setting IP...");
                            Boolean ipChange = false;
                            Int32 timeOutIpChange = 0;
                            while (!ipChange)
                            {
                                try
                                {
                                    commHandler.informationNetworkingHandler("NETHANDLER: setting IP... " + netData.IpTcpListener);
                                    SystemHandler.setRegistryIP(netData);
                                    if (!(ipChange = SystemHandler.rebindAdapter(netData)))
                                    {
                                        throw new Exception("adapter not found");
                                    }
                                }
                                catch (ThreadAbortException e)
                                {
                                    throw e;
                                }
                                catch (Exception e)
                                {
                                    timeOutIpChange++;
                                    if (timeOutIpChange > netData.WaitForStart)
                                    {
                                        throw new Exception("timeout, for IP config");
                                    }
                                    commHandler.informationNetworkingHandler("NETHANDLER: setting IP... failed! " + e.Message);
                                    System.Threading.Thread.Sleep(netData.WaitTimeStart);
                                }
                            }
                            commHandler.informationNetworkingHandler("NETHANDLER: setting IP... ok!");

                            //reset el dispositivo de red inalambrico
                            try
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi...");
                                SystemHandler.turnOnAdapter(netData, true);
                                commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi... ok!");
                            }
                            catch (ThreadAbortException e)
                            {
                                throw e;
                            }
                            catch (Exception e)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi... failed! " + e.Message);
                            }

                            //hecha a andar wifiHandler
                            commHandler.informationNetworkingHandler("NETHANDLER: starting wifi...");
                            wifiHandler.connect();

                            //espera por primera conexión del WIFI
                            Int32 timeOutWifiChange = 0;
                            while (wifiHandler.ConnectionState != WifiConnectionState.CONNECTED)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: waiting for other devices");
                                System.Threading.Thread.Sleep(netData.WaitTimeStart);
                                timeOutWifiChange++;
                                if (timeOutWifiChange > netData.WaitForWifi)
                                {
                                    //reset el dispositivo de red inalambrico
                                    try
                                    {
                                        commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi...");
                                        SystemHandler.turnOnAdapter(netData, false);
                                        commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi... ok!");
                                    }
                                    catch (ThreadAbortException e)
                                    {
                                        throw e;
                                    }
                                    catch (Exception e)
                                    {
                                        commHandler.informationNetworkingHandler("NETHANDLER: turning on wifi... failed! " + e.Message);
                                    }
                                    timeOutWifiChange = 0;
                                }
                            }
                            commHandler.informationNetworkingHandler("NETHANDLER: starting wifi... ok!");
                            break;
                        }
                }

                //chequeo de ip duplicada
                commHandler.informationNetworkingHandler("NETHANDLER: strong DAD on");
                ipHandler.startStrongDAD();

                //Servicios TCP
                commHandler.informationNetworkingHandler("NETHANDLER: starting TCP...");
                tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                Boolean tcpChange = false;
                Int32 timeOutTcpChange = 0;
                while (!tcpChange)
                {
                    try
                    {
                        if (timeOutTcpChange > 0)
                        {
                            wifiHandler.openWLanConnection();
                        }
                        commHandler.informationNetworkingHandler("NETHANDLER: starting TCP... on " + netData.IpTcpListener + ":" + netData.TcpPort);
                        tcpListener.Start();
                        tcpChange = true;
                    }
                    catch (ThreadAbortException e)
                    {
                        throw e;
                    }
                    catch (Exception e)
                    {
                        commHandler.informationNetworkingHandler("NETHANDLER: starting TCP... failed! " + e.Message);
                        timeOutTcpChange++;
                        if (timeOutTcpChange > netData.WaitForStart)
                        {
                            throw new Exception("timeout, for TCP service to start");
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(netData.WaitTimeStart);
                        }
                    }
                }
                tcpListenerThread.Start();
                commHandler.informationNetworkingHandler("NETHANDLER: starting TCP... ok!");

                //conecta a UDP
                commHandler.informationNetworkingHandler("NETHANDLER: starting UDP...");
                commHandler.informationNetworkingHandler("NETHANDLER: starting UDP... on " + netData.IpUdpMulticast + ":" + netData.UdpPort);
                udpClient.Client.Bind(new IPEndPoint(tcpAddress, netData.UdpPort));
                udpClient.JoinMulticastGroup(udpMulticastAdress);
                udpIpEndPoint = new IPEndPoint(udpMulticastAdress, netData.UdpPort);
                udpClientThread.Start();
                commHandler.informationNetworkingHandler("NETHANDLER: starting UDP... ok!");

                //cambio a chequeo de ip debil
                commHandler.informationNetworkingHandler("NETHANDLER: weak DAD on");
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
        private void stop(Boolean reset)
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
                        killRemoteMachine(serverRemoteMachines[i]);
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
            //detiene wifiHandler
            try
            {
                commHandler.informationNetworkingHandler("NETHANDLER: stop wifi...");
                wifiHandler.disconnect();
                commHandler.informationNetworkingHandler("NETHANDLER: stop wifi... ok!");
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("NETHANDLER: stop wifi... failed! "+ e.Message);
            }

            if (!reset)
            {
                //se deja la IP en el sistema operativo como por defecto (DHCP)
                try
                {
                    commHandler.informationNetworkingHandler("NETHANDLER: dhcp on...");
                    SystemHandler.setRegistryDHCP(netData);
                    commHandler.informationNetworkingHandler("NETHANDLER: dhcp on... ok!");
                }
                catch (Exception e)
                {
                    commHandler.informationNetworkingHandler("NETHANDLER: dhcp on... failed! " + e.Message);
                }
                switch (netData.OpSystem)
                {
                    case OpSystemType.WINMOBILE5POCKETPC:
                        {
                            try
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: reset adapter...");
                                SystemHandler.resetAdapter(netData);
                                commHandler.informationNetworkingHandler("NETHANDLER: reset adapter... ok!");
                            }
                            catch (Exception e)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: reset adapter... failed! " + e.Message);
                            }
                            try
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turn off adapter...");
                                SystemHandler.turnOffAdapter(netData);
                                commHandler.informationNetworkingHandler("NETHANDLER: turn off adapter... ok!");
                            }
                            catch (Exception e)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turn off adapter... failed! " + e.Message);
                            }
                            break;
                        }
                    case OpSystemType.WINMOBILE6SMARTPHONE:
                        {
                            try
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: rebind adapter...");
                                SystemHandler.rebindAdapter(netData);
                                commHandler.informationNetworkingHandler("NETHANDLER: rebind adapter... ok!");
                            }
                            catch (Exception e)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: rebind adapter... failed! " + e.Message);
                            }
                            try
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turn off adapter...");
                                SystemHandler.turnOffAdapter(netData);
                                commHandler.informationNetworkingHandler("NETHANDLER: turn off adapter... ok!");
                            }
                            catch (Exception e)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turn off adapter... failed! " + e.Message);
                            }

                            break;
                        }
                    case OpSystemType.SAMSUNGOMNIAII:
                        {
                            try
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: rebind adapter...");
                                SystemHandler.rebindAdapter(netData);
                                commHandler.informationNetworkingHandler("NETHANDLER: rebind adapter... ok!");
                            }
                            catch (Exception e)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: rebind adapter... failed! " + e.Message);
                            }
                            try
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turn on adapter...");
                                SystemHandler.turnOnAdapter(netData, true);
                                commHandler.informationNetworkingHandler("NETHANDLER: turn on adapter... ok!");
                            }
                            catch (Exception e)
                            {
                                commHandler.informationNetworkingHandler("NETHANDLER: turn on adapter... failed! " + e.Message);
                            }
                            break;
                        }
                }
            }

            //se cierran las viejas conexiones  TCP
            try
            {
                commHandler.informationNetworkingHandler("NETHANDLER: kill TCP links... " + "(TCP is hard to kill)");
                RemoteMachine[] serverRemoteMachines = oldServerList.toObjectArray();
                for (int i = 0; i < serverRemoteMachines.Length; i++)
                {
                    try
                    {
                        commHandler.informationNetworkingHandler("NETHANDLER: kill TCP link... " + serverRemoteMachines[i].Ip);
                        killRemoteMachine(serverRemoteMachines[i]);
                        commHandler.informationNetworkingHandler("NETHANDLER: kill TCP link... ok!");
                    }
                    catch (Exception e)
                    {
                        commHandler.informationNetworkingHandler("NETHANDLER: kill TCP link... failed!" + e.Message);
                    }
                }
                commHandler.informationNetworkingHandler("NETHANDLER: kill TCP links... ok! " + "(oh yea!)");
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("NETHANDLER: kill TCP links... failed!" + e.Message);
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
        /// <param name="o">La dirección IP de la máquina remota, debe ser un String</param>
        private void connectToAsync(Object o)
        {
            try
            {
                commHandler.informationNetworkingHandler("TCP: connection...");
                IPAddress serverIp = (IPAddress)o;
                TcpClient tcpClient = new TcpClient(new IPEndPoint(tcpAddress, 0));
                tcpClient.LingerState.LingerTime = 0;
                tcpClient.LingerState.Enabled = false;
                IAsyncResult result = tcpClient.Client.BeginConnect(new IPEndPoint(serverIp, netData.TcpPort), null, null);

                bool success = result.AsyncWaitHandle.WaitOne(netData.TcpConnectTimeOut, false);
                if (!success)
                {
                    commHandler.informationNetworkingHandler("TCP: connection... time out!");
                    return;
                }
                else
                {
                    tcpClient.Client.EndConnect(result);
                    
                    RemoteMachine remoteMachine = new RemoteMachine(serverIp, tcpClient);
                    remoteMachine.Buffer = new byte[4];
                    remoteMachine.TcpClient.GetStream().BeginRead(remoteMachine.Buffer, 0, remoteMachine.Buffer.Length, listenTCPMessages, remoteMachine);
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
            //disconnectFromAsync(machineIp);
            killRemoteMachine(machineIp);
        }

        /// <summary>
        /// Desconecta los servicios TCP asociados a una maquina remota
        /// </summary>
        /// <param name="machine">la maquina a desconectar debe ser un String</param>
        public void disconnectFrom(RemoteMachine machine)
        {
            //disconnectFromAsync(machine);
            killRemoteMachine(machine);
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
                    commHandler.informationNetworkingHandler("TCP: old list queue");
                    tcpServerList.remove(machine);
                    oldServerList.add(machine.Ip, machine);
                }
                else if (o.GetType().Equals(typeof(RemoteMachine)))
                {
                    RemoteMachine machine = (RemoteMachine)o;
                    commHandler.informationNetworkingHandler("TCP: old list queue");
                    tcpServerList.remove(machine);
                    oldServerList.add(machine.Ip, machine);
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
        /// Desconecta los servicios TCP asociados a una maquina remota
        /// </summary>
        /// <param name="o">La ip de la maquina remota a desconectar en formato de String o la maquina remota</param>
        private void killRemoteMachine(Object o)
        {
            try
            {
                commHandler.informationNetworkingHandler("TCP: kill");
                if (o.GetType().Equals(typeof(IPAddress)))
                {
                    IPAddress machineIp = (IPAddress)o;
                    RemoteMachine machine = tcpServerList.getRemoteMachine(machineIp);
                    commHandler.informationNetworkingHandler("TCP: close machine");
                    machine.close();
                    commHandler.informationNetworkingHandler("TCP: drop from queue");
                    tcpServerList.remove(machine);
                }
                else if (o.GetType().Equals(typeof(RemoteMachine)))
                {
                    RemoteMachine machine = (RemoteMachine)o;
                    commHandler.informationNetworkingHandler("TCP: close machine");
                    machine.close();
                    commHandler.informationNetworkingHandler("TCP: drop from queue");
                    tcpServerList.remove(machine);
                }
                commHandler.informationNetworkingHandler("TCP: kill... ok!");
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("TCP: kill... failed! " + e.Message);
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
                    tcpClient.LingerState.LingerTime = 0; 
                    tcpClient.LingerState.Enabled = false;
                    IPAddress ip = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
                    RemoteMachine remoteMachine = new RemoteMachine(ip, tcpClient);
                    remoteMachine.Buffer = new byte[4];
                    remoteMachine.TcpClient.GetStream().BeginRead(remoteMachine.Buffer, 0, remoteMachine.Buffer.Length, listenTCPMessages, remoteMachine);

                    RemoteMachine oldRemoteMachine = tcpServerList.getRemoteMachine(ip);
                    if (oldRemoteMachine != null)
                    {
                        oldRemoteMachine.close();
                        tcpServerList.remove(oldRemoteMachine);
                        //oldServerList.add(oldRemoteMachine.Ip, oldRemoteMachine);
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
                            ipHandler.putReceibedBytes(buffer.Length);
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
        /// Escucha mensajes TCP recibidos
        /// </summary>
        internal void listenTCPMessages(IAsyncResult ar)
        {
            RemoteMachine remoteMachine = (RemoteMachine)ar.AsyncState;
            try
            {
                NetworkStream nStream = remoteMachine.TcpClient.GetStream();
                int r = nStream.EndRead(ar);
                if (r != 4)
                {
                    throw new Exception("TCP WARNING: we need 4 bytes header, but we got " + r);
                }
                int length = BitConverter.ToInt32(remoteMachine.Buffer, 0);
                remoteMachine.Buffer = new byte[length];
                remoteMachine.BufferFilled = 0;
                remoteMachine.TcpClient.GetStream().BeginRead(remoteMachine.Buffer, 0, remoteMachine.Buffer.Length, listenTCPMessagesData, remoteMachine);
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
        /// Escucha mensajes TCP recibidos
        /// </summary>
        internal void listenTCPMessagesData(IAsyncResult ar)
        {
            RemoteMachine remoteMachine = (RemoteMachine)ar.AsyncState;
            try
            {
                NetworkStream nStream = remoteMachine.TcpClient.GetStream();
                remoteMachine.BufferFilled += nStream.EndRead(ar);
                if (remoteMachine.BufferFilled < remoteMachine.Buffer.Length)
                {
                    remoteMachine.TcpClient.GetStream().BeginRead(
                        remoteMachine.Buffer, 
                        remoteMachine.BufferFilled, 
                        remoteMachine.Buffer.Length - remoteMachine.BufferFilled, 
                        listenTCPMessagesData, 
                        remoteMachine
                        );
                }
                else
                {
                    //byte[] data = new byte[r];
                    //Buffer.BlockCopy(remoteMachine.Buffer, 0, data, 0, r);
                    NetMessage message = new NetMessage(remoteMachine.Buffer);
                    addTCPMessages(message);

                    remoteMachine.Buffer = new byte[4];
                    remoteMachine.TcpClient.GetStream().BeginRead(
                        remoteMachine.Buffer, 
                        0, 
                        remoteMachine.Buffer.Length, 
                        listenTCPMessages, 
                        remoteMachine
                        );
                }
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                commHandler.informationNetworkingHandler("TCP WARNING: reading failed " + e.Message);
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
                if (tcpMessageQueue.size() < 25)
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
