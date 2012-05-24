package hlmp.CommLayer;

import hlmp.CommLayer.Messages.Message;

import java.util.concurrent.ConcurrentLinkedQueue;

public class MessageQueue {

	/// <summary>
	/// Cola de prioridad FIFO
	/// </summary>
	private ConcurrentLinkedQueue<Message> queue;

	/// <summary>
	/// Default Constructor
	/// </summary>
	public MessageQueue()
	{
		queue = new ConcurrentLinkedQueue<Message>();
	}

	/// <summary>
	/// Desencola el primer mensaje de la lista
	/// </summary>
	/// <returns>El primer mensaje de la cola, null si está vacía</returns>
	public synchronized Message draw()
	{
		if (queue.size() > 0)
		{
			return queue.poll();
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Coloca un mensaje en la cola
	/// </summary>
	/// <param name="m">El mensaje a colocar en la cola</param>
	public synchronized void put(Message m)
	{
		queue.add(m);
	}

	/// <summary>
	/// Calcula el tamaño de la cola
	/// </summary>
	/// <returns>El tamaño de la cola</returns>
	public int size()
	{
		return queue.size();
	}
}
