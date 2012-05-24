package hlmp.NetLayer;

import java.net.InetAddress;
import java.util.ArrayList;
import java.util.List;

public class SystemHandler {
	
	
	/**
	 * Cambia la IP del sistema operativo al estádo dinámico
	 * Si ocurre un error se arroja la excepción
	 * @param networkAdapter
	 */
	public static void setDinamicIP(NetworkAdapter networkAdapter){
		//TODO:
	}
	
	/**
	 * Cambia la Ip del sistema operativo a la configuración de red adhoc
	 * Si ocurre un error se arroja la excepción
	 * @param networkAdapter
	 * @param ip
	 * @param subnetMask
	 */
	public static void setStaticIP(NetworkAdapter networkAdapter, String ip, String subnetMask){
		//TODO:
	}
	
	/**
	 * Cambia el estado del adaptador de red a encendido
	 * Si ocurre un error se arroja la excepción
	 * @param networkAdapter
	 */
	public static void enableIpAdapter(NetworkAdapter networkAdapter){
		
	}
	
	/**
	 * Cambia el estado del adaptador de red a apagado
	 * Si ocurre un error se arroja la excepción
	 * @param networkAdapter
	 */
	public static void disableIpAdapter(NetworkAdapter networkAdapter){
		
	}
	
	/**
	 * Obtiene el estado DAD de la Ip asignada a TCP
	 * @param networkAdapter
	 * @param ipAdress
	 * @return un estado de la clase IpState
	 */
	public static int getIpState(NetworkAdapter networkAdapter, InetAddress ipAdress){
		//TODO:
		return hlmp.NetLayer.Constants.IpState.VALID;
	}
	
	/**
	 * Obtiene el numero de bytes enviados por el adaptador
	 * @param networkAdapter el adaptador de red
	 * @return el numero de bytes enviados
	 */
//	private static int getAliveValue(NetworkAdapter networkAdapter){
//		//TODO: 
//		return 0;
//	}
	
//	private static boolean isOperative(NetworkAdapter networkAdapter){
//		//TODO:
//		return true;
//	}
	
	/**
	 * Busca el adaptador de red WIFI
	 * @return El adaptador de red Wifi
	 */
	public static NetworkAdapter getWifiAdapter(){
		return null;
	}
	
	public static List<NetworkAdapter> getNetworkAdapters(){
		return new ArrayList<NetworkAdapter>();
	}
	
	/**
	 * Obtiene el texto de un error de sistema de windows a partir del código de error
	 * Si ocurre un error se arroja una Excepción
	 * @param index el código de error
	 * @return El texto con la descripción del error
	 */
//	private static String getManagementMessage(int index){
//		//TODO:
//		return "Management Message";
//	}
	
}
