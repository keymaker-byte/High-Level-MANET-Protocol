package hlmp.netlayer;

public class WifiHandler {
	/**
	 * Datos de configuración
	 */
//    private NetData netData;
    /**
     * Thread principal
     */
//    private Thread wakeUpDaemonThread;
    /// <summary>
    /// Cliente de WLan, necesario para controlar las redes y los dispositivos inalambricos
    /// </summary>
//    private WlanClient wLanClient;
    /// <summary>
    /// Evento de conexión (windows XPSp3)
    /// </summary>
//    private WlanClient.WlanInterface.WlanConnectionNotificationEventHandler wlanConnectionNotificationEventHandler;
    /// <summary>
    /// Eventos de red adhoc (windows vista)
    /// </summary>
//    private WlanClient.WlanInterface.WlanAdhocNetworkNotificationEventHandler wlanAdhocNetworkNotificationEventHandler;
    /**
     * Guarda el estado de la conexión
     */
    private int connectionState;
    /**
     * Punto de control para lock de eventos
     */
//    private Object syncPoint;
    /**
     * función que envia información de la red inalambrica
     */
//    private WifiInformationHandler wifiInformation;
    
    /**
     * Constructor
     * @param netData los datos de red
     * @param wifiInformation el manejador de eventos 
     */
    public WifiHandler(NetData netData, WifiInformationHandler wifiInformation)
    {
//        this.netData = netData;
//        this.wifiInformation = wifiInformation;
        //inicializa los objetos Wlan
//        wLanClient = new WlanClient();
//        wlanConnectionNotificationEventHandler = new WlanClient.WlanInterface.WlanConnectionNotificationEventHandler(WlanConnectionAction);
//        wlanAdhocNetworkNotificationEventHandler = new WlanClient.WlanInterface.WlanAdhocNetworkNotificationEventHandler(WlanAdhocNetworkAction);
        connectionState = WifiConnectionState.DISCONNECTED;
//        syncPoint = new Object();
    }
    
    public void connect(){
    	connectionState = WifiConnectionState.CONNECTED;
    }
    
    /**
     * Desconecta de la red adhoc
     */
    public void disconnect()
    {
        connectionState = WifiConnectionState.STOP;
//        wakeUpDaemonThread.Abort();
//        wakeUpDaemonThread.Join();
//        closeWLanConnection();
    }

	public int getConnectionState() {
		return connectionState;
	}

	public void setConnectionState(int connectionState) {
		this.connectionState = connectionState;
	}

    
}
