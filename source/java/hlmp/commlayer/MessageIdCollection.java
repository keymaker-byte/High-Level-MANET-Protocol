package hlmp.commlayer;

import hlmp.commlayer.messages.Message;

import java.util.HashMap;
import java.util.UUID;
import java.util.concurrent.ConcurrentLinkedQueue;

/// <summary>
/// Colección de id's de mensajes.
/// Los ids son unicos en la colección
/// Si se agrega un id cuando el tamaño es igual a MaxSize, se borra el primero en la cola antes de agregar
/// </summary>
public class MessageIdCollection {

	/// <summary>
	/// Tabla de hashing para busqueda de orden constante
	/// </summary>
	private HashMap<UUID, UUID> messageIdList;

	/// <summary>
	/// Cola de prioridad para entrada y salida en orden constante
	/// </summary>
	private ConcurrentLinkedQueue<UUID> messageIdqueue;

	/// <summary>
	/// El tamaño máximo de la colección
	/// </summary>
	private int maxSize;        

	/// <summary>
	/// Default Constructor
	/// </summary>
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

	/// <summary>
	/// Coloca un id en la colección
	/// </summary>
	/// <param name="id">El id a agregar a la colección</param>
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

	/// <summary>
	/// Retorna el tamaño de la cola
	/// </summary>
	/// <returns>El tamaño de la cola</returns>
	public synchronized int size()
	{
		return messageIdqueue.size();
	}

	/// <summary>
	/// Verifica si existe un id en la colección
	/// </summary>
	/// <param name="id">El id a buscar en la colección</param>
	/// <returns>true si el id existe en la colección, false si no</returns>
	public synchronized boolean contains(UUID id)
	{
		return messageIdList.containsKey(id);
	}
}
