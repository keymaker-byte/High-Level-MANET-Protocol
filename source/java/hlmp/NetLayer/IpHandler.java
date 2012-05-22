package hlmp.NetLayer;

import java.util.concurrent.ConcurrentLinkedQueue;
import hlmp.NetLayer.Constants.*;
import hlmp.NetLayer.Interfaces.ResetIpHandler;
import hlmp.NetLayer.Interfaces.WifiHandler;

public class IpHandler{

	/**
	 * Handler para cambio de ip
	 */
	private ResetIpHandler resetIpHandler;

	/**
	 * Thread que verifica la IP
	 */
    private Thread checkIpThread;
	/**
	 * El estado de este objeto (un parametro de IpHandlerState)
	 */
	private int state;
	/**
	 * Objeto de lock
	 */
	private Object stopLock;
	/**
	 * Cola de Ip's de mensajes i'm alive recibidos
	 */
	private ConcurrentLinkedQueue<String> queue;
	/**
	 * Objeto de lock para la cola
	 */
	private Object queueLock;
	/**
	 * Los datos de red
	 */
	private NetData netData;

	/**
	 * Contador de loliness
	 */
	//private int lolinessTimeOut;
	/**
	 * valor que que cambia si el usuario esta correctamente recibiendo la multidifusión
	 */

	private WifiHandler wifiHandler;
	/**
	 * valor que cambia cuando el usuario esta correctamente difundiendo mensajes
	 */
	//private int lastAliveValue;


	public IpHandler(NetData netData , ResetIpHandler resetIpHandler, WifiHandler wifiHandler) {
		this.wifiHandler = wifiHandler;
		this.resetIpHandler = resetIpHandler;
		this.netData = netData;
		this.checkIpThread = getCheckIpThread();
		this.checkIpThread.setName("IpHandler Thread");
		this.state = IphandlerState.STOPPED;
		this.stopLock = new Object();
		this.queueLock = new Object();
		queue = new ConcurrentLinkedQueue<String>();
		//this.lolinessTimeOut = 0;
		//this.lastAliveValue = 0;

	}

	/**
	 * Comienza la verificación fuerte
	 */
	public void startStrongDAD(){
		this.state = IphandlerState.STARTEDSTRONG;
		this.checkIpThread.start();
	}

	/**
	 * Comienza la verificación débil
	 */
	public void chageToWeakDAD(){
		this.state = IphandlerState.STARTEDWEAK;
	}

	/**
	 * Detiene la verificación
	 */
	public void stop(){
		
		synchronized (stopLock){
			this.state = IphandlerState.STOPPED;
		}
		try {
			checkIpThread.interrupt();
			checkIpThread.join();
		} catch (InterruptedException e) {
			return;
		}
	}
	
	/**
	 * Verifica que no exista Ip duplicada en el sistema operativo, si gatilla resetIpDelegate, asegura que el Thread se detendrá
	 */
	private Thread getCheckIpThread(){
		return new Thread(){

			public void run(){
				// TODO: fvalverd corroborar que getCheckIpThread funciona
				while(true){
					try {
						sleep(netData.getWaitTimeStart());
					} catch (InterruptedException e) {
						return;
					}
					//Chequea Strong DAD
					if (state == IphandlerState.STARTEDSTRONG)
					{
						int ipState = wifiHandler.getIpState();
						switch (ipState)
						{
						case IpState.DUPLICATE:
						{
							resetIpHandler.resetIp();
							return;
						}
						case IpState.NOTFOUND:
						{
							resetIpHandler.resetIp();
							return;
						}
						}
					}
					else if (state == IphandlerState.STARTEDWEAK)
					{
						//Chequea Weak DAD
						synchronized (queueLock){
							while (queue.size() > 0){
								String outterIp = queue.poll();
								if (outterIp.equals(netData.getIpTcpListener())){
									resetIpHandler.resetIp();
									return;
								}
							}
						}
						//chequea Strong DAD
						switch (wifiHandler.getIpState()) {
							case IpState.DUPLICATE:
							{
								resetIpHandler.resetIp();
								return;
							}
							case IpState.INVALID:
							{
								resetIpHandler.resetIp();
								return;
							}
							case IpState.NOTFOUND:
							{
								resetIpHandler.resetIp();
								return;
							}
						}
						
						// Detiene o duerme segun corresponda
						synchronized (stopLock){
							if (state == IphandlerState.STOPPED){
								return;
							}
						}

					}
				}
			}
		};
	}
	
	/**
	 * Coloca una ip a la cola de ips para chequear
	 * @param outterIp la ip a encolar
	 */
	public void put(String outterIp)
	{
		synchronized (queueLock){
			this.queue.add(outterIp);
		}
	}
	
}