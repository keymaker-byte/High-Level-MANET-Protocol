package hlmp.NetLayer;

import java.net.InetAddress;
//import java.net.UnknownHostException;
//import java.util.Random;
import hlmp.NetLayer.Constants.*;

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
    private NetworkAdapter networkAdapter;
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
        pickNewIp();
      //ipUdpMulticast = "224.0.0.2"; // original
        ipUdpMulticast = "224.2.2.4";
        tcpPort = 30001;
        udpPort = 30002;
        subnetMask = "255.255.0.0";
        networkAdapter = SystemHandler.getWifiAdapter();
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
    
    public void pickNewIp(){
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
//    	//TODO ipTcpListener = ip;
    }

	public InetAddress getIpTcpListener() {
		return ipTcpListener;
	}

	public void setIpTcpListener(InetAddress ipTcpListener) {
		this.ipTcpListener = ipTcpListener;
	}

	public String getIpUdpMulticast() {
		return ipUdpMulticast;
	}

	public void setIpUdpMulticast(String ipUdpMulticast) {
		this.ipUdpMulticast = ipUdpMulticast;
	}

	public int getTcpPort() {
		return tcpPort;
	}

	public void setTcpPort(int tcpPort) {
		this.tcpPort = tcpPort;
	}

	public int getUdpPort() {
		return udpPort;
	}

	public void setUdpPort(int udpPort) {
		this.udpPort = udpPort;
	}

	public String getSubnetMask() {
		return subnetMask;
	}

	public void setSubnetMask(String subnetMask) {
		this.subnetMask = subnetMask;
	}

	public NetworkAdapter getNetworkAdapter() {
		return networkAdapter;
	}

	public void setNetworkAdapter(NetworkAdapter networkAdapter) {
		this.networkAdapter = networkAdapter;
	}

	public String getAdhocNetworkName() {
		return adhocNetworkName;
	}

	public void setAdhocNetworkName(String adhocNetworkName) {
		this.adhocNetworkName = adhocNetworkName;
	}

	public int getOpSystem() {
		return opSystem;
	}

	public void setOpSystem(int opSystem) {
		this.opSystem = opSystem;
	}

	public int getWaitTimeWiFi() {
		return waitTimeWiFi;
	}

	public void setWaitTimeWiFi(int waitTimeWiFi) {
		this.waitTimeWiFi = waitTimeWiFi;
	}

	public int getWaitForStart() {
		return waitForStart;
	}

	public void setWaitForStart(int waitForStart) {
		this.waitForStart = waitForStart;
	}

	public int getWaitForWifi() {
		return waitForWifi;
	}

	public void setWaitForWifi(int waitForWifi) {
		this.waitForWifi = waitForWifi;
	}

	public int getWaitTimeStart() {
		return waitTimeStart;
	}

	public void setWaitTimeStart(int waitTimeStart) {
		this.waitTimeStart = waitTimeStart;
	}

	public int getTimeOutWriteTCP() {
		return timeOutWriteTCP;
	}

	public void setTimeOutWriteTCP(int timeOutWriteTCP) {
		this.timeOutWriteTCP = timeOutWriteTCP;
	}

	public int getTimeIntervalTimer() {
		return timeIntervalTimer;
	}

	public void setTimeIntervalTimer(int timeIntervalTimer) {
		this.timeIntervalTimer = timeIntervalTimer;
	}

	public int getWaitForTimerClose() {
		return waitForTimerClose;
	}

	public void setWaitForTimerClose(int waitForTimerClose) {
		this.waitForTimerClose = waitForTimerClose;
	}

	public int getWaitForTCPConnection() {
		return waitForTCPConnection;
	}

	public void setWaitForTCPConnection(int waitForTCPConnection) {
		this.waitForTCPConnection = waitForTCPConnection;
	}

	public int getWaitForAck() {
		return waitForAck;
	}

	public void setWaitForAck(int waitForAck) {
		this.waitForAck = waitForAck;
	}

	public int getMaxMessagesProcess() {
		return maxMessagesProcess;
	}

	public void setMaxMessagesProcess(int maxMessagesProcess) {
		this.maxMessagesProcess = maxMessagesProcess;
	}

	public int getQualityRiseNetUser() {
		return qualityRiseNetUser;
	}

	public void setQualityRiseNetUser(int qualityRiseNetUser) {
		this.qualityRiseNetUser = qualityRiseNetUser;
	}

	public int getQualityMaxNetUser() {
		return qualityMaxNetUser;
	}

	public void setQualityMaxNetUser(int qualityMaxNetUser) {
		this.qualityMaxNetUser = qualityMaxNetUser;
	}

	public int getQualityNormalNetUser() {
		return qualityNormalNetUser;
	}

	public void setQualityNormalNetUser(int qualityNormalNetUser) {
		this.qualityNormalNetUser = qualityNormalNetUser;
	}

	public int getQualityLowNetUser() {
		return qualityLowNetUser;
	}

	public void setQualityLowNetUser(int qualityLowNetUser) {
		this.qualityLowNetUser = qualityLowNetUser;
	}

	public int getLolinessTimeOut() {
		return lolinessTimeOut;
	}

	public void setLolinessTimeOut(int lolinessTimeOut) {
		this.lolinessTimeOut = lolinessTimeOut;
	}

	public int getSendFailsToDisconnect() {
		return sendFailsToDisconnect;
	}

	public void setSendFailsToDisconnect(int sendFailsToDisconnect) {
		this.sendFailsToDisconnect = sendFailsToDisconnect;
	}

	public int getTcpConnectTimeOut() {
		return tcpConnectTimeOut;
	}

	public void setTcpConnectTimeOut(int tcpConnectTimeOut) {
		this.tcpConnectTimeOut = tcpConnectTimeOut;
	}

	public int getStateCritical() {
		return stateCritical;
	}

	public void setStateCritical(int stateCritical) {
		this.stateCritical = stateCritical;
	}

	public int getStateOverloaded() {
		return stateOverloaded;
	}

	public void setStateOverloaded(int stateOverloaded) {
		this.stateOverloaded = stateOverloaded;
	}

	public int getStatePathNN() {
		return statePathNN;
	}

	public void setStatePathNN(int statePathNN) {
		this.statePathNN = statePathNN;
	}

	public int getStatePathNL() {
		return statePathNL;
	}

	public void setStatePathNL(int statePathNL) {
		this.statePathNL = statePathNL;
	}

	public int getStatePathNC() {
		return statePathNC;
	}

	public void setStatePathNC(int statePathNC) {
		this.statePathNC = statePathNC;
	}

	public int getStatePathON() {
		return statePathON;
	}

	public void setStatePathON(int statePathON) {
		this.statePathON = statePathON;
	}

	public int getStatePathOL() {
		return statePathOL;
	}

	public void setStatePathOL(int statePathOL) {
		this.statePathOL = statePathOL;
	}

	public int getStatePathOC() {
		return statePathOC;
	}

	public void setStatePathOC(int statePathOC) {
		this.statePathOC = statePathOC;
	}

	public int getStatePathCN() {
		return statePathCN;
	}

	public void setStatePathCN(int statePathCN) {
		this.statePathCN = statePathCN;
	}

	public int getStatePathCL() {
		return statePathCL;
	}

	public void setStatePathCL(int statePathCL) {
		this.statePathCL = statePathCL;
	}

	public int getStatePathCC() {
		return statePathCC;
	}

	public void setStatePathCC(int statePathCC) {
		this.statePathCC = statePathCC;
	}

	public int getStatePathNotFound() {
		return statePathNotFound;
	}

	public void setStatePathNotFound(int statePathNotFound) {
		this.statePathNotFound = statePathNotFound;
	}

}
