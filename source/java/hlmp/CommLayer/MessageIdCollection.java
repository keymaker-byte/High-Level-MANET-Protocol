package hlmp.CommLayer;

import java.util.HashMap;
import java.util.UUID;
import java.util.concurrent.ConcurrentLinkedQueue;

/**
 * Colección de id's de mensajes.
 * Los ids son unicos en la colección
 * Si se agrega un id cuando el tamaño es igual a MaxSize, se borra el primero en la cola antes de agregar
 */
public class MessageIdCollection {

	/**
	 * Tabla de hashing para busqueda de orden constante
	 */
	private HashMap<UUID, UUID> messageIdList;

	/**
	 * Cola de prioridad para entrada y salida en orden constante
	 */
	private ConcurrentLinkedQueue<UUID> messageIdqueue;

	/**
	 * El tamaño máximo de la colección
	 */
	private int maxSize;        

	/**
	 * Default Constructor
	 */
	public MessageIdCollection()
	{
		messageIdList = new HashMap<UUID, UUID>();
		messageIdqueue = new ConcurrentLinkedQueue<UUID>();
		this.maxSize = 100;
	}
	
	public int getMaxSize() {
		return maxSize;
	}

	public void setMaxSize(int maxSize) {
		this.maxSize = maxSize;
	}

	/**
	 * Coloca un id en la colección
	 * @param id El id a agregar a la colección
	 */
	public synchronized void add(UUID id)
	{
		if (!messageIdList.containsKey(id))
		{
			if (messageIdqueue.size() >= this.maxSize)
			{
				UUID deadId = messageIdqueue.poll();
				messageIdList.remove(deadId);
			}
			messageIdList.put(id, id);
			messageIdqueue.add(id);
		}
	}

	/**
	 * Retorna el tamaño de la cola
	 * @return El tamaño de la cola
	 */
	public synchronized int size()
	{
		return messageIdqueue.size();
	}

	/**
	 * Verifica si existe un id en la colección
	 * @param id El id a buscar en la colección
	 * @return true si el id existe en la colección, false si no
	 */
	public synchronized boolean contains(UUID id)
	{
		return messageIdList.containsKey(id);
	}
}
