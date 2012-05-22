package hlmp.NetLayer;

import java.net.InetAddress;

import hlmp.NetLayer.Constants.OpSystemType;


/**
 * Clase para los datos de configuraciòn necesarios que involucran una RED
 * @author nicolas
 *
 */
public class NetData {

	//private String ipTcpListener;
	private InetAddress ipTcpListener;
    private String ipUdpMulticast;
    private int tcpPort;
    private int udpPort;
    private String subnetMask;
    private String adhocNetworkName;
    private int opSystem;
    private int waitTimeWiFi;
    private int waitForStart;
    private int waitForWifi;
    private int waitTimeStart;
    private int timeOutWriteTCP;
    private int timeIntervalTimer;
    private int waitForTimerClose;
    private int waitForTCPConnection;
    private int waitForAck;
    private int maxMessagesProcess;
    private int qualityRiseNetUser;
    private int qualityMaxNetUser;
    private int qualityNormalNetUser;
    private int qualityLowNetUser;

    private int lolinessTimeOut;
    private int sendFailsToDisconnect;
    private int tcpConnectTimeOut;
    private int stateCritical;
    private int stateOverloaded;
    private int statePathNN;
    private int statePathNL;
    private int statePathNC;
    private int statePathON;
    private int statePathOL;
    private int statePathOC;
    private int statePathCN;
    private int statePathCL;
    private int statePathCC;
    private int statePathNotFound;
    
    public NetData()
    {
//        pickNewIp();
        ipUdpMulticast = "224.0.0.2";
        tcpPort = 30001;
        udpPort = 30002;
        subnetMask = "255.255.0.0";
        adhocNetworkName = "HLMP-MANET";
        opSystem = OpSystemType.UBUNTU1104;
        waitTimeWiFi = 15000;
        waitTimeStart = 4000; 
        waitForStart = 10;
        waitForWifi = 3;
        timeOutWriteTCP = 2000;
        timeIntervalTimer = 1000;
        waitForTimerClose = 20;
        waitForTCPConnection = 20;
        waitForAck = 10;
        maxMessagesProcess = 2500;
        qualityRiseNetUser = 5;
        qualityMaxNetUser = 25;
        qualityNormalNetUser = 15;
        qualityLowNetUser = 5;
        lolinessTimeOut = 15;
        sendFailsToDisconnect = 5;
        tcpConnectTimeOut = 2000;
        stateCritical = 20;
        stateOverloaded = 10;
        statePathNN = 1;
        statePathNL = 2;
        statePathNC = 4;
        statePathON = 10;
        statePathOL = 20;
        statePathOC = 40;
        statePathCN = 100;
        statePathCL = 200;
        statePathCC = 400;
        statePathNotFound = 1000000;
        
    }
    
    /**
     * Determina una nueva IP de manera aleatoria, en el rango de mascara de red 255.255.0.0
     */
//    public void pickNewIp(){
    	//Random r = new Random();
    	//String ip = "170.160" + "." + (r.nextInt(255)+1) + "." + (r.nextInt(255)+1);
//    	String ip = "6.6.6.6";
//    	try {
//			ipTcpListener = InetAddress.getByName(ip);
//    		//ipTcpListener = InetAddress.
//		} catch (UnknownHostException e) {
//			System.out.println("La ip "+ip+" no fue reconocida.");
//			e.printStackTrace();
//		}
    	
//    	TODO: fvalverd Cambiar esto cuando se automatize el crear la red adhoc	
//		try {
//	        for (Enumeration<NetworkInterface> en = NetworkInterface.getNetworkInterfaces(); en.hasMoreElements();) {
//	            NetworkInterface intf = en.nextElement();
//	            for (Enumeration<InetAddress> enumIpAddr = intf.getInetAddresses(); enumIpAddr.hasMoreElements();) {
//	                InetAddress inetAddress = enumIpAddr.nextElement();
//	                if (!inetAddress.isLoopbackAddress()) {
//	                	ipTcpListener = inetAddress;
//	                }
//	            }
//	        }
//	    } catch (SocketException e) {
//	    	e.printStackTrace();
//	    }
//    }

	/**
	 * @return Ip delegada para realizar conexiones TCP
	 */
	public InetAddress getIpTcpListener() {
		return ipTcpListener;
	}

	/**
	 * @param ipTcpListener Ip delegada para realizar conexiones TCP
	 */
	public void setIpTcpListener(InetAddress ipTcpListener) {
		this.ipTcpListener = ipTcpListener;
	}

	/**
	 * @return Ip delegada para enviar y recibir mensajes multicast UDP
	 */
	public String getIpUdpMulticast() {
		return ipUdpMulticast;
	}

	/**
	 * @param ipUdpMulticast Ip delegada para enviar y recibir mensajes multicast UDP
	 */
	public void setIpUdpMulticast(String ipUdpMulticast) {
		this.ipUdpMulticast = ipUdpMulticast;
	}

	/**
	 * @return Puerto delegado para levantar servidor TCP
	 */
	public int getTcpPort() {
		return tcpPort;
	}

	/**
	 * @param tcpPort Puerto delegado para levantar servidor TCP
	 */
	public void setTcpPort(int tcpPort) {
		this.tcpPort = tcpPort;
	}

	/**
	 * @return Puerto delegado para escuchar y enviar mensajes multicast UDP
	 */
	public int getUdpPort() {
		return udpPort;
	}

	/**
	 * @param udpPort Puerto delegado para escuchar y enviar mensajes multicast UDP
	 */
	public void setUdpPort(int udpPort) {
		this.udpPort = udpPort;
	}

	/**
	 * @return Mascara de Sub Red delegada para realizar conexiones TCP
	 */
	public String getSubnetMask() {
		return subnetMask;
	}

	/**
	 * @param subnetMask Mascara de Sub Red delegada para realizar conexiones TCP
	 */
	public void setSubnetMask(String subnetMask) {
		this.subnetMask = subnetMask;
	}

	/**
	 * @return El nombre de la red adhoc que se debe crear/unir
	 */
	public String getAdhocNetworkName() {
		return adhocNetworkName;
	}

	/**
	 * @param adhocNetworkName El nombre de la red adhoc que se debe crear/unir
	 */
	public void setAdhocNetworkName(String adhocNetworkName) {
		this.adhocNetworkName = adhocNetworkName;
	}

	/**
	 * @return El sistema operativo 
	 */
	public int getOpSystem() {
		return opSystem;
	}

	/**
	 * @param opSystem El sistema operativo 
	 */
	public void setOpSystem(int opSystem) {
		this.opSystem = opSystem;
	}

	/**
	 * @return Tiempo que se espera luego de enviar una llamada de conexion a red inalambrica, para intentar enviar otra en caso de que no haya habido un evento de conexión
	 */
	public int getWaitTimeWiFi() {
		return waitTimeWiFi;
	}

	/**
	 * @param waitTimeWiFi Tiempo que se espera luego de enviar una llamada de conexion a red inalambrica, para intentar enviar otra en caso de que no haya habido un evento de conexión
	 */
	public void setWaitTimeWiFi(int waitTimeWiFi) {
		this.waitTimeWiFi = waitTimeWiFi;
	}

	/**
	 * @return Veces que se intenta configurar la Ip o levantar los servicios TCP, en caso de intentos fallidos
	 */
	public int getWaitForStart() {
		return waitForStart;
	}

	/**
	 * @param waitForStart Veces que se intenta configurar la Ip o levantar los servicios TCP, en caso de intentos fallidos
	 */
	public void setWaitForStart(int waitForStart) {
		this.waitForStart = waitForStart;
	}

	/**
	 * @return Veces que se intenta conectar a Wifi Sin Exito
	 */
	public int getWaitForWifi() {
		return waitForWifi;
	}

	/**
	 * @param waitForWifi Veces que se intenta conectar a Wifi Sin Exito
	 */
	public void setWaitForWifi(int waitForWifi) {
		this.waitForWifi = waitForWifi;
	}

	/**
	 * @return Tiempo que se espera para intentar configurar la IP o levantar los servicios TCP en caso de intentos fallidos
	 */
	public int getWaitTimeStart() {
		return waitTimeStart;
	}

	/**
	 * @param waitTimeStart Tiempo que se espera para intentar configurar la IP o levantar los servicios TCP en caso de intentos fallidos
	 */
	public void setWaitTimeStart(int waitTimeStart) {
		this.waitTimeStart = waitTimeStart;
	}

	/**
	 * @return TimeOut para escribir en un Socket TCP
	 */
	public int getTimeOutWriteTCP() {
		return timeOutWriteTCP;
	}

	/**
	 * @param timeOutWriteTCP TimeOut para escribir en un Socket TCP
	 */
	public void setTimeOutWriteTCP(int timeOutWriteTCP) {
		this.timeOutWriteTCP = timeOutWriteTCP;
	}

	/**
	 * @return Intervalo de tiempo para el timer
	 */
	public int getTimeIntervalTimer() {
		return timeIntervalTimer;
	}

	/**
	 * @param timeIntervalTimer Intervalo de tiempo para el timer
	 */
	public void setTimeIntervalTimer(int timeIntervalTimer) {
		this.timeIntervalTimer = timeIntervalTimer;
	}

	/**
	 * @return Veces que se intentará para esperar al cierre del timer
	 */
	public int getWaitForTimerClose() {
		return waitForTimerClose;
	}

	/**
	 * @param waitForTimerClose Veces que se intentará para esperar al cierre del timer
	 */
	public void setWaitForTimerClose(int waitForTimerClose) {
		this.waitForTimerClose = waitForTimerClose;
	}

	/**
	 * @return Veces que se espera para intentar una conección TCP a un NetUser que cumple los requerimientos
	 */
	public int getWaitForTCPConnection() {
		return waitForTCPConnection;
	}

	/**
	 * @param waitForTCPConnection Veces que se espera para intentar una conección TCP a un NetUser que cumple los requerimientos
	 */
	public void setWaitForTCPConnection(int waitForTCPConnection) {
		this.waitForTCPConnection = waitForTCPConnection;
	}

	/**
	 * @return Veces que se espera para reenviar un mensaje safe no confirmado
	 */
	public int getWaitForAck() {
		return waitForAck;
	}

	/**
	 * @param waitForAck Veces que se espera para reenviar un mensaje safe no confirmado
	 */
	public void setWaitForAck(int waitForAck) {
		this.waitForAck = waitForAck;
	}

	/**
	 * @return El numero maximo de mensajes que se procesan por intervalo de tiempo
	 */
	public int getMaxMessagesProcess() {
		return maxMessagesProcess;
	}

	/**
	 * @param maxMessagesProcess El numero maximo de mensajes que se procesan por intervalo de tiempo
	 */
	public void setMaxMessagesProcess(int maxMessagesProcess) {
		this.maxMessagesProcess = maxMessagesProcess;
	}

	/**
	 * @return Inncremento a la calidad de señal que se le hace al netUSer al recibir un mensaje i'm alive
	 */
	public int getQualityRiseNetUser() {
		return qualityRiseNetUser;
	}

	/**
	 * @param qualityRiseNetUser Inncremento a la calidad de señal que se le hace al netUSer al recibir un mensaje i'm alive
	 */
	public void setQualityRiseNetUser(int qualityRiseNetUser) {
		this.qualityRiseNetUser = qualityRiseNetUser;
	}

	/**
	 * @return Calidad de señal maxima de un netuser
	 */
	public int getQualityMaxNetUser() {
		return qualityMaxNetUser;
	}

	/**
	 * @param qualityMaxNetUser Calidad de señal maxima de un netuser
	 */
	public void setQualityMaxNetUser(int qualityMaxNetUser) {
		this.qualityMaxNetUser = qualityMaxNetUser;
	}

	/**
	 * @return Calidad de señal normal de un netuser
	 */
	public int getQualityNormalNetUser() {
		return qualityNormalNetUser;
	}

	/**
	 * @param qualityNormalNetUser Calidad de señal normal de un netuser
	 */
	public void setQualityNormalNetUser(int qualityNormalNetUser) {
		this.qualityNormalNetUser = qualityNormalNetUser;
	}

	/**
	 * @return Calidad de señal baja de un netuser
	 */
	public int getQualityLowNetUser() {
		return qualityLowNetUser;
	}

	/**
	 * @param qualityLowNetUser Calidad de señal baja de un netuser
	 */
	public void setQualityLowNetUser(int qualityLowNetUser) {
		this.qualityLowNetUser = qualityLowNetUser;
	}

	/**
	 * @return Tiempo de espera para determinar caso de loliness
	 */
	public int getLolinessTimeOut() {
		return lolinessTimeOut;
	}

	/**
	 * @param lolinessTimeOut Tiempo de espera para determinar caso de loliness
	 */
	public void setLolinessTimeOut(int lolinessTimeOut) {
		this.lolinessTimeOut = lolinessTimeOut;
	}

	/**
	 * @return Fallos para solicitar una desconexión a la maquina remota destino
	 */
	public int getSendFailsToDisconnect() {
		return sendFailsToDisconnect;
	}

	/**
	 * @param sendFailsToDisconnect Fallos para solicitar una desconexión a la maquina remota destino
	 */
	public void setSendFailsToDisconnect(int sendFailsToDisconnect) {
		this.sendFailsToDisconnect = sendFailsToDisconnect;
	}

	/**
	 * @return Tiempo de espera para conexion TCP
	 */
	public int getTcpConnectTimeOut() {
		return tcpConnectTimeOut;
	}

	/**
	 * @param tcpConnectTimeOut Tiempo de espera para conexion TCP
	 */
	public void setTcpConnectTimeOut(int tcpConnectTimeOut) {
		this.tcpConnectTimeOut = tcpConnectTimeOut;
	}

	/**
	 * @return Valor para el estado critico
	 */
	public int getStateCritical() {
		return stateCritical;
	}

	/**
	 * @param stateCritical Valor para el estado critico
	 */
	public void setStateCritical(int stateCritical) {
		this.stateCritical = stateCritical;
	}

	/**
	 * @return Valor para el estado sobrecargado
	 */
	public int getStateOverloaded() {
		return stateOverloaded;
	}

	/**
	 * @param stateOverloaded Valor para el estado sobrecargado
	 */
	public void setStateOverloaded(int stateOverloaded) {
		this.stateOverloaded = stateOverloaded;
	}

	/**
	 * @return
	 */
	public int getStatePathNN() {
		return statePathNN;
	}

	/**
	 * @param statePathNN
	 */
	public void setStatePathNN(int statePathNN) {
		this.statePathNN = statePathNN;
	}

	/**
	 * @return
	 */
	public int getStatePathNL() {
		return statePathNL;
	}

	/**
	 * @param statePathNL
	 */
	public void setStatePathNL(int statePathNL) {
		this.statePathNL = statePathNL;
	}

	/**
	 * @return
	 */
	public int getStatePathNC() {
		return statePathNC;
	}

	/**
	 * @param statePathNC
	 */
	public void setStatePathNC(int statePathNC) {
		this.statePathNC = statePathNC;
	}

	/**
	 * @return
	 */
	public int getStatePathON() {
		return statePathON;
	}

	/**
	 * @param statePathON
	 */
	public void setStatePathON(int statePathON) {
		this.statePathON = statePathON;
	}

	/**
	 * @return
	 */
	public int getStatePathOL() {
		return statePathOL;
	}

	/**
	 * @param statePathOL
	 */
	public void setStatePathOL(int statePathOL) {
		this.statePathOL = statePathOL;
	}

	/**
	 * @return
	 */
	public int getStatePathOC() {
		return statePathOC;
	}

	/**
	 * @param statePathOC
	 */
	public void setStatePathOC(int statePathOC) {
		this.statePathOC = statePathOC;
	}

	/**
	 * @return
	 */
	public int getStatePathCN() {
		return statePathCN;
	}

	/**
	 * @param statePathCN
	 */
	public void setStatePathCN(int statePathCN) {
		this.statePathCN = statePathCN;
	}

	/**
	 * @return
	 */
	public int getStatePathCL() {
		return statePathCL;
	}

	/**
	 * @param statePathCL
	 */
	public void setStatePathCL(int statePathCL) {
		this.statePathCL = statePathCL;
	}

	/**
	 * @return
	 */
	public int getStatePathCC() {
		return statePathCC;
	}

	/**
	 * @param statePathCC
	 */
	public void setStatePathCC(int statePathCC) {
		this.statePathCC = statePathCC;
	}

	/**
	 * @return Valor de peso para cuando no se encuentra un camino
	 */
	public int getStatePathNotFound() {
		return statePathNotFound;
	}

	/**
	 * @param statePathNotFound Valor de peso para cuando no se encuentra un camino
	 */
	public void setStatePathNotFound(int statePathNotFound) {
		this.statePathNotFound = statePathNotFound;
	}

}
