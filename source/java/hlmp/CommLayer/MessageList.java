package hlmp.CommLayer;

import hlmp.CommLayer.Messages.Message;

import java.util.HashMap;
import java.util.Iterator;
import java.util.UUID;



/// <summary>
/// Lista de mensajes que asegura busqueda en orden constante
/// </summary>
public class MessageList {

	/// <summary>
	/// La tabla de hashing de la colección
	/// </summary>
	private HashMap<UUID, Message> messageCollection;

	/// <summary>
	/// Default Constructor
	/// </summary>
	public MessageList()
	{
		messageCollection = new HashMap<UUID, Message>();
	}

	/// <summary>
	/// Agrega un mensaje a la lista
	/// </summary>
	/// <param name="message">El mensaje a agregar</param>
	public synchronized void add(Message message){

		//Si no tengo el ip, entonces agrego al usuario como alguien nuevo
		if (!messageCollection.containsKey(message.getId()))
		{
			messageCollection.put(message.getId(), message);
		}
		//Si ya la tengo, actualizo el objeto usuario
		else
		{
			messageCollection.remove(message.getId());
			messageCollection.put(message.getId(), message);
		} 
	}

	/// <summary>
	/// Remueve un mensaje de la lista
	/// </summary>
	/// <param name="id">El id del mensaje a remover</param>
	/// <returns>true si existia y fue removido, false si no</returns>
	public synchronized boolean remove(UUID id)
	{
		if (messageCollection.containsKey(id))
		{
			messageCollection.remove(id);
			return true;
		}
		else
		{
			return false;
		} 
	}

	/// <summary>
	/// Obtiene un mensaje de la lista con busqueda en orden constante
	/// </summary>
	/// <param name="id">el id del mensaje a buscar</param>
	/// <returns>El mensaje de la lista, null si no existía</returns>
	public synchronized Message getMessage(UUID id)
	{
		Message o = messageCollection.get(id);
		if (o != null)
		{
			return o;
		}
		else
		{
			return null;
		} 
	}

	/// <summary>
	/// fabrica un array con la lista de mensajes
	/// </summary>
	/// <returns>Un array de los mensajes listados</returns>
	public synchronized Message[] messageListToArray()
	{
		Message[] us = new Message[messageCollection.size()];
		Iterator<Message> iterator = messageCollection.values().iterator();
		int i = 0;
		while (iterator.hasNext())
		{
			us[i] = iterator.next();
			i++;
		}
		return us; 
	}

	/// <summary>
	/// Calcula el tamaño de la lista
	/// </summary>
	/// <returns>El tamaño de la lista</returns>
	public int size()
	{
		return messageCollection.size(); 
	}
}
