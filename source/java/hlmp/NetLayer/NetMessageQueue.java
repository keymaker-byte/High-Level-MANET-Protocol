package hlmp.NetLayer;

import java.util.concurrent.ConcurrentLinkedQueue;

/**
 * Clase para la cola de mensajes que llegan por la RED
 */
public class NetMessageQueue {
	
	private ConcurrentLinkedQueue<NetMessage> queue;
	/**
	 * Cantidad en la cola
	 */
	private int itemCount;
	
	/**
	 * Constructor vacío
	 */
	public NetMessageQueue() {
		this.queue = new ConcurrentLinkedQueue<NetMessage>();
		this.itemCount = 0;
	}
	
	/**
	 * Obtiene el primer mensaje en la cola, null si esta vacía
	 * Si esta vacia espera a que haya un mensaje.
	 * @return el primer mensaje de la cola o null si está vacía
	 */
	public synchronized NetMessage draw(){
		while(itemCount == 0){
			//	continue;
			try {
				wait();
			} catch (InterruptedException e) {
			}
		}
		
		NetMessage netMessage = this.queue.poll();
		this.itemCount--;
		
		return netMessage;
	}
	
	/**
	 * Coloca un mensaje en la cola
	 * @param m el mensaje a colocar en la cola
	 */
	public synchronized void put(NetMessage m){
		this.queue.add(m);
		this.itemCount++;
		notify();
	}
	
	/**
	 * Retorna el tamaño de la cola
	 * @return el tamaño de la cola
	 */
	public int size(){
		return this.itemCount;
	}
	
	/**
	 * Desbloquea forzosamente el bloqueo de draw
	 */
	public synchronized void unblock(){
		notify();
	}

}