package hlmp.commlayer;
import java.util.concurrent.ConcurrentLinkedQueue;

import hlmp.commlayer.messages.Message;

public class MessageMonitoredQueue {
	
	/// <summary>
    /// Cola de prioridad FIFO
    /// </summary>
	private ConcurrentLinkedQueue<Message> queue;
	
	/// <summary>
    /// Cantidad en la cola
    /// </summary>
    private int itemCount;
    
    /// <summary>
    /// Default Constructor
    /// </summary>
    public MessageMonitoredQueue()
    {
        queue = new ConcurrentLinkedQueue<Message>();
        itemCount = 0;
    }
    
    /// <summary>
    /// Obtiene el primer mensaje en la cola, null si esta vacía
    /// </summary>
    /// <returns>el primer mensaje de la cola o null si está vacía</returns>
    public synchronized Message draw(){
    	while (itemCount == 0){
    		try {
				wait();
			} catch (InterruptedException e) {
			}
    	}

    	Message message = queue.poll();
    	this.itemCount--;
    	return message;
    }
    

    /// <summary>
    /// Coloca un mensaje en la cola
    /// </summary>
    /// <param name="m">el mensaje a colocar en la cola</param>
    public synchronized void put(Message m){
    	this.queue.add(m);
    	this.itemCount++;
    	notify();
    }

    /// <summary>
    /// Retorna el tamaño de la cola
    /// </summary>
    /// <returns>el tamaño de la cola</returns>
    public int size(){
        return this.itemCount;
    }

    /// <summary>
    /// Desbloquea forzosamente el bloquedo de draw
    /// </summary>
    public synchronized void unblok(){
    	notify();
    }
}
