using System;
using System.Collections.Generic;
using System.Text;
using SystemInterop;
using System.Threading;

namespace NetLayer
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
        /// Cliente de WLan, necesario para controlar las redes y los dispositivos inalambricos
        /// </summary>
        private WlanClient wLanClient;
        /// <summary>
        /// Evento de conexión (windows XPSp3)
        /// </summary>
        private WlanClient.WlanInterface.WlanConnectionNotificationEventHandler wlanConnectionNotificationEventHandler;
        /// <summary>
        /// Eventos de red adhoc (windows vista)
        /// </summary>
        private WlanClient.WlanInterface.WlanAdhocNetworkNotificationEventHandler wlanAdhocNetworkNotificationEventHandler;
        /// <summary>
        /// Guarda el estado de la conexión
        /// </summary>
        private Int32 connectionState;
        /// <summary>
        /// Punto de control para lock de eventos
        /// </summary>
        private Object syncPoint;
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
            //inicializa los objetos Wlan
            wLanClient = new WlanClient();
            wlanConnectionNotificationEventHandler = new WlanClient.WlanInterface.WlanConnectionNotificationEventHandler(WlanConnectionAction);
            wlanAdhocNetworkNotificationEventHandler = new WlanClient.WlanInterface.WlanAdhocNetworkNotificationEventHandler(WlanAdhocNetworkAction);
            connectionState = WifiConnectionState.DISCONNECTED;
            syncPoint = new Object();
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
            closeWLanConnection();
        }

        /// <summary>
        /// Conecta a la red adhoc
        /// Si ocurre algun error se informa en informationNetworkingHandler
        /// </summary>
        private void openWLanConnection()
        {
            try
            {
                //Revisa si hay un adaptador seleccionado en la configuración
                if (netData.NetworkAdapter == null)
                {
                    throw new Exception("no hay un adaptador de red seleccionado");
                }
                //Conecta al SSID de la red ad-hoc
                foreach (WlanClient.WlanInterface wlanIface in wLanClient.Interfaces)
                {
                    if (wlanIface.NetworkInterface != null && wlanIface.NetworkInterface.Id == netData.NetworkAdapter.Id)
                    {
                        //Se crea el profile de red adhoc
                        String adhocProfile = String.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}-adhoc</name><SSIDConfig><SSID><name>{0}</name></SSID></SSIDConfig><connectionType>IBSS</connectionType><connectionMode>manual</connectionMode><MSM><security><authEncryption><authentication>open</authentication><encryption>none</encryption><useOneX>false</useOneX></authEncryption></security></MSM></WLANProfile>", netData.AdhocNetworkName);
                        try
                        {
                            wlanIface.SetProfile(Wlan.WlanProfileFlags.AllUser, adhocProfile, true);
                        }
                        catch (ThreadAbortException e)
                        {
                            throw e;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("error al configurar el perfil de red adhoc " + e.Message);
                        }
                        //Se registra la función de notificación de conexiones en la red adhoc
                        try
                        {
                            wlanIface.WlanConnectionNotification -= wlanConnectionNotificationEventHandler;
                            wlanIface.WlanAdhocNetworkNotification -= wlanAdhocNetworkNotificationEventHandler;
                            wlanIface.WlanConnectionNotification += wlanConnectionNotificationEventHandler;
                            wlanIface.WlanAdhocNetworkNotification += wlanAdhocNetworkNotificationEventHandler;
                        }
                        catch (ThreadAbortException e)
                        {
                            throw e;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("error al asignar funciones de notificación de red adhoc " + e.Message);
                        }
                        //Se envían las peticiones de conexión
                        try
                        {
                            wlanIface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Independent, netData.AdhocNetworkName + "-adhoc");
                            wifiInformation("connection request");
                        }
                        catch (ThreadAbortException e)
                        {
                            throw e;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("error al conectar al profile " + e.Message);
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
        /// Se desconecta de la red adhoc
        /// Si ocurre algun error se informa en informationNetworkingHandler
        /// </summary>
        private void closeWLanConnection()
        {
            try
            {
                //Revisa que haya un adaptador seleccionado en la configuración
                if (netData.NetworkAdapter == null)
                {
                    throw new Exception("no hay un adaptador de red seleccionado");
                }
                //Desconecta del SSID de red adhoc
                foreach (WlanClient.WlanInterface wlanIface in wLanClient.Interfaces)
                {
                    if (wlanIface.NetworkInterface != null && wlanIface.NetworkInterface.Id == netData.NetworkAdapter.Id)
                    {
                        //Elimina las funciones registradas para notificación
                        try
                        {
                            wlanIface.WlanConnectionNotification -= wlanConnectionNotificationEventHandler;
                            wlanIface.WlanAdhocNetworkNotification -= wlanAdhocNetworkNotificationEventHandler;
                        }
                        catch (ThreadAbortException e)
                        {
                            throw e;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("error al des-registrar el notificador de conexión inalambrica " + e.Message);
                        }
                        //Envía petición de desconexión
                        try
                        {
                            wlanIface.Disconnect();
                            wifiInformation("disconnection request");
                        }
                        catch (ThreadAbortException e)
                        {
                            throw e;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("error al desconectar " + e.Message);
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
        /// Se gatilla para recibir notificaciones de conexión a la red inalambrica (Windows XP, SP3)
        /// Si ocurre un error se informa en informationNetworkingHandler
        /// </summary>
        /// <param name="notifyData">Los datos de notificación</param>
        /// <param name="connNotifyData">Los datos de conección</param>
        private void WlanConnectionAction(Wlan.WlanNotificationData notifyData, Wlan.WlanConnectionNotificationData connNotifyData)
        {
            lock(syncPoint)
            {
                try
                {
                    if ((notifyData.notificationSource.Equals(Wlan.WlanNotificationSource.ACM) &&
                         notifyData.NotificationCode.Equals(Wlan.WlanNotificationCodeAcm.ConnectionComplete)) ||
                        (notifyData.notificationSource.Equals(Wlan.WlanNotificationSource.MSM) &&
                         notifyData.notificationCode.Equals(Wlan.WlanNotificationCodeMsm.Connected)))
                    {
                        if (connNotifyData.profileName == netData.AdhocNetworkName + "-adhoc")
                        {
                            if (netData.OpSystem.Equals(OpSystemType.WINXPSP3))
                            {
                                wifiInformation("connection");
                                switch (connectionState)
                                {
                                    case WifiConnectionState.CONNECTED:
                                        {
                                            break;
                                        }
                                    case WifiConnectionState.DISCONNECTED:
                                        {
                                            connectionState = WifiConnectionState.CONNECTED;
                                            break;
                                        }
                                }
                            }
                        }
                        else
                        {
                            wifiInformation("unknown connection (" + connNotifyData.profileName + ")");
                            connectionState = WifiConnectionState.DISCONNECTED;
                            closeWLanConnection();
                            openWLanConnection();
                        }
                    }
                    else if ((notifyData.notificationSource.Equals(Wlan.WlanNotificationSource.ACM) &&
                         notifyData.NotificationCode.Equals(Wlan.WlanNotificationCodeAcm.Disconnected)) ||
                        (notifyData.notificationSource.Equals(Wlan.WlanNotificationSource.MSM) &&
                         notifyData.notificationCode.Equals(Wlan.WlanNotificationCodeMsm.Disconnected)))
                    {
                        if (netData.OpSystem.Equals(OpSystemType.WINXPSP3))
                        {
                            wifiInformation("disconnection");
                            switch (connectionState)
                            {
                                case WifiConnectionState.CONNECTED:
                                    {
                                        connectionState = WifiConnectionState.DISCONNECTED;
                                        break;
                                    }
                                case WifiConnectionState.DISCONNECTED:
                                    {
                                        break;
                                    }
                            }
                        }
                        else if (netData.OpSystem.Equals(OpSystemType.WINVISTA))
                        {
                            wifiInformation("disconenction");
                            switch (connectionState)
                            {
                                case WifiConnectionState.CONNECTED:
                                    {
                                        connectionState = WifiConnectionState.DISCONNECTED;
                                        break;
                                    }
                                case WifiConnectionState.DISCONNECTED:
                                    {
                                        break;
                                    }
                                case WifiConnectionState.WAINTING:
                                    {
                                        connectionState = WifiConnectionState.DISCONNECTED;
                                        break;
                                    }
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
                    wifiInformation("error " + e.Message);
                }
            }
        }

        /// <summary>
        /// Se gatilla para recibir notificaciones en el cambio del estado de la red adhoc (Windows Vista)
        /// Si ocurre un error se informa en informationNetworkingHandler
        /// </summary>
        /// <param name="notifyData">Los datos de notificación</param>
        /// <param name="adhocNetworkState">El estado de la red adhoc que esta siendo notificado</param>
        private void WlanAdhocNetworkAction(Wlan.WlanNotificationData notifyData, Wlan.WlanAdhocNetworkState adhocNetworkState)
        {
            lock(syncPoint)
            {
                try
                {
                    if ((notifyData.notificationSource.Equals(Wlan.WlanNotificationSource.ACM) &&
                         notifyData.NotificationCode.Equals(Wlan.WlanNotificationCodeAcm.AdhocNetworkStateChange)))
                    {
                        if (adhocNetworkState.Equals(Wlan.WlanAdhocNetworkState.Connected))
                        {
                            if (netData.OpSystem.Equals(OpSystemType.WINVISTA))
                            {
                                wifiInformation("connection");
                                switch (connectionState)
                                {
                                    case WifiConnectionState.CONNECTED:
                                        {
                                            break;
                                        }
                                    case WifiConnectionState.DISCONNECTED:
                                        {
                                            connectionState = WifiConnectionState.CONNECTED;
                                            break;
                                        }
                                    case WifiConnectionState.WAINTING:
                                        {
                                            connectionState = WifiConnectionState.CONNECTED;
                                            break;
                                        }
                                }
                            }
                        }
                        else if (adhocNetworkState.Equals(Wlan.WlanAdhocNetworkState.Formed))
                        { 
                            if (netData.OpSystem.Equals(OpSystemType.WINVISTA))
                            {
                                wifiInformation("alone connection");
                                switch (connectionState)
                                {
                                    case WifiConnectionState.CONNECTED:
                                        {
                                            connectionState = WifiConnectionState.WAINTING;
                                            break;
                                        }
                                    case WifiConnectionState.DISCONNECTED:
                                        {
                                            connectionState = WifiConnectionState.WAINTING;
                                            break;
                                        }
                                    case WifiConnectionState.WAINTING:
                                        {
                                            break;
                                        }  
                                }
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
                    wifiInformation("error " + e.Message);
                }
            }
        }

        /// <summary>
        /// Es ejecutado por un thread para verificar el estado de la conexión con la red inalambrica
        /// </summary>
        private void wakeUpDaemon()
        {
            int sleepingPill = 0;
            while (true)
            {
                lock (syncPoint)
                {
                    switch (connectionState)
                    {
                        case WifiConnectionState.CONNECTED:
                            {
                                sleepingPill = 0;
                                break;
                            }
                        case WifiConnectionState.DISCONNECTED:
                            {
                                sleepingPill++;
                                if (sleepingPill > netData.WaitForWifi)
                                {
                                    sleepingPill = 0;
                                    try
                                    {
                                        wifiInformation("WIFI: disable adapter...");
                                        SystemHandler.disableIpAdapter(netData.NetworkAdapter);
                                        wifiInformation("WIFI: disable adapter... ok!");
                                    }
                                    catch (ThreadAbortException e)
                                    {
                                        throw e;
                                    }
                                    catch (Exception e)
                                    {
                                        wifiInformation("WIFI: disable adapter... failed! " + e.Message);
                                    }
                                    try
                                    {
                                        wifiInformation("WIFI: enable adapter...");
                                        SystemHandler.enableIpAdapter(netData.NetworkAdapter);
                                        wifiInformation("WIFI: enable adapter... ok!");
                                    }
                                    catch (ThreadAbortException e)
                                    {
                                        throw e;
                                    }
                                    catch (Exception e)
                                    {
                                        wifiInformation("WIFI: enable adapter... failed! " + e.Message);
                                    }
                                }
                                else
                                {
                                    openWLanConnection();
                                }
                                break;
                            }
                        case WifiConnectionState.WAINTING:
                            {
                                break;
                            }
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
