package hlmp.NetLayer.Interfaces;

import java.net.InetAddress;


public interface WifiHandler {

	/**
     * Conecta la red adhoc
     */
    public void connect();
    
    /**
     * Desconecta la red adhoc
     */
    public void disconnect();

    
	public int getConnectionState();

	public int getIpState();
   
	public InetAddress getInetAddress();
}