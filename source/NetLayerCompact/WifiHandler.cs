using System;
using System.Collections.Generic;
using System.Text;
using SystemInteropCompact;
using System.Threading;
using OpenNETCF.Net.NetworkInformation;
using OpenNETCF.Net;

namespace NetLayerCompact
{

    /// <summary>
    /// Clase que enumera los estados posibles de la conexión inalambrica
    /// </summary>
    internal static class WifiConnectionState
    {
        public const Int32 CONNECTED = 1;
        public const Int32 DISCONNECTED = 2;
        public const Int32 WAINTING = 4;
        public const Int32 STOP = 5;
    }

    /// <summary>
    /// Clase que maneja la conexión Wifi
    /// </summary>
    internal class WifiHandler
    {
        /// <summary>
        /// Datos de configuración
        /// </summary>
        private NetData netData;
        /// <summary>
        /// Thread principal
        /// </summary>
        private Thread wakeUpDaemonThread;
        /// <summary>
        /// Guarda el estado de la conexión
        /// </summary>
        private Int32 connectionState;
        /// <summary>
        /// delegado para enviar información de la wifi
        /// </summary>
        public delegate void WifiInformationDelegate(String message);
        /// <summary>
        /// función que envia información de la red inalambrica
        /// </summary>
        private WifiInformationDelegate wifiInformation;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="netData">los datos de red</param>
        /// <param name="wifiInformation">el manejador de eventos</param>
        public WifiHandler(NetData netData, WifiInformationDelegate wifiInformation)
        {
            this.netData = netData;
            this.wifiInformation = wifiInformation;
            connectionState = WifiConnectionState.DISCONNECTED;
        }

        /// <summary>
        /// Guarda el estado de la conexión
        /// </summary>
        public Int32 ConnectionState
        {
            get { return connectionState; }
        }

        /// <summary>
        /// Conecta a la red adhoc
        /// </summary>
        public void connect()
        {
            wakeUpDaemonThread = new Thread(new ThreadStart(wakeUpDaemon));
            wakeUpDaemonThread.Start();
        }

        /// <summary>
        /// Desconecta de la red adhoc
        /// </summary>
        public void disconnect()
        {
            connectionState = WifiConnectionState.STOP;
            wakeUpDaemonThread.Abort();
            wakeUpDaemonThread.Join();
        }

        /// <summary>
        /// Conecta a la red adhoc
        /// Si ocurre algun error se informa en informationNetworkingHandler
        /// </summary>
        internal void openWLanConnection()
        {
            try
            {
                //Revisa si hay un adaptador seleccionado en la configuración
                if (netData.NetworkAdapter == null)
                {
                    throw new Exception("no hay un adaptador de red seleccionado");
                }

                INetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (INetworkInterface networkInterface in networkInterfaces)
                {
                    if (netData.NetworkAdapter.Name.Equals(networkInterface.Name))
                    {
                        WirelessZeroConfigNetworkInterface adapter = (WirelessZeroConfigNetworkInterface)networkInterface;
                        adapter.AddPreferredNetwork(netData.AdhocNetworkName, false, "", 1, AuthenticationMode.Open, WEPStatus.WEPDisabled, null);
                        adapter.ConnectToPreferredNetwork(netData.AdhocNetworkName);
                        connectionState = WifiConnectionState.WAINTING;
                        break;
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                wifiInformation("error " + e.Message);
            }
        }

        /// <summary>
        /// Verifica la conexión de la red ad-hoc
        /// </summary>
        internal void checkWLanConnection()
        {
            try
            {
                //Revisa si hay un adaptador seleccionado en la configuración
                if (netData.NetworkAdapter == null)
                {
                    throw new Exception("no hay un adaptador de red seleccionado");
                }

                INetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (INetworkInterface networkInterface in networkInterfaces)
                {
                    if (netData.NetworkAdapter.Name.Equals(networkInterface.Name))
                    {
                        WirelessZeroConfigNetworkInterface adapter = (WirelessZeroConfigNetworkInterface)networkInterface;
                        String ssid = adapter.AssociatedAccessPoint;
                        if (ssid == null)
                        {
                            connectionState = WifiConnectionState.DISCONNECTED;
                            wifiInformation("disconnected");
                        }
                        else if (netData.AdhocNetworkName.Equals(ssid) && adapter.OperationalStatus == OperationalStatus.Up && SystemHandler.getIpState(netData.NetworkAdapter, netData.IpTcpListener) != IpState.NOTFOUND)
                        {
                            connectionState = WifiConnectionState.CONNECTED;
                            wifiInformation("connected");
                        }
                        else if (netData.AdhocNetworkName.Equals(ssid) && adapter.OperationalStatus != OperationalStatus.Up)
                        {

                            connectionState = WifiConnectionState.WAINTING;
                            wifiInformation("waiting");
                        }
                        else 
                        {
                            connectionState = WifiConnectionState.DISCONNECTED;
                            wifiInformation("disconnected");
                        }
                        break;
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                wifiInformation("error " + e.Message);
            }
        }


        /// <summary>
        /// Es ejecutado por un thread para verificar el estado de la conexión con la red inalambrica
        /// </summary>
        private void wakeUpDaemon()
        {
            while (true)
            {
                //chequear
                checkWLanConnection();
                switch (connectionState)
                {
                    case WifiConnectionState.CONNECTED:
                        {
                            break;
                        }
                    case WifiConnectionState.DISCONNECTED:
                        {
                            openWLanConnection();
                            break;
                        }
                    case WifiConnectionState.WAINTING:
                        {
                            break;
                        }
                }
                if (connectionState == WifiConnectionState.STOP)
                {
                    return;
                }
                else
                {
                    Thread.Sleep(netData.WaitTimeWiFi);
                }
            }
        }
    }
}
