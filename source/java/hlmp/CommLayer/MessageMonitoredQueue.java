package hlmp.CommLayer;
import java.util.concurrent.ConcurrentLinkedQueue;

import hlmp.CommLayer.Messages.Message;

public class MessageMonitoredQueue {
	
	/**
     * Cola de prioridad FIFO
     */
	private ConcurrentLinkedQueue<Message> queue;
	
	/**
     * Cantidad en la cola
     */
    private int itemCount;
    
    /**
     * Default Constructor
     */
    public MessageMonitoredQueue()
    {
        queue = new ConcurrentLinkedQueue<Message>();
        itemCount = 0;
    }
    
    /**
     * Obtiene el primer mensaje en la cola, o espera uno si esta vacia
     * @return el primer mensaje de la cola
     * @throws InterruptedException 
     */
    public synchronized Message draw() throws InterruptedException{
    	while (itemCount == 0){
    		wait();
    	}

    	Message message = queue.poll();
    	this.itemCount--;
    	return message;
    }
    

    /**
     * Coloca un mensaje en la cola
     * @param m el mensaje a colocar en la cola
     */
    public synchronized void put(Message m){
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
     * Desbloquea forzosamente el bloquedo de draw
     */
    public synchronized void unblock(){
    	notify();
    }
}
