package hlmp.CommLayer;

import hlmp.CommLayer.Messages.Message;

import java.util.HashMap;
/**
 * Equivalente a un HashMap sincronizado
 * 
 * Creado para C# donde las Collection, al agregar un elemento, si la key ya estaba no la reemplaza
 * @author nicolas
 *
 */
public class MessageTypeList {

	private HashMap<Integer, Message> collection;
	
	/**
	 * Default Constructor
	 */
    public MessageTypeList()
    {
        this.collection = new HashMap<Integer, Message>();
    }

    /**
     * Agrega un tipo de mensaje a la lista
     * @param messageType El tipo del mensaje, los tipos entre 0 y 1000 esta reservados para el sistema
     * @param messageTypeObject El tipo del mensaje, como objeto tipo, puede ser obtenido mediante typeof
     */
    public synchronized void add(int messageType, Message messageTypeObject) 
    {
    	//Si no tengo el ip, entonces agrego al usuario como alguien nuevo
    	if (!collection.containsKey(messageType))
    	{
    		collection.put(messageType, messageTypeObject);
    	}
    	//Si ya la tengo, actualizo el objeto usuario
    	else
    	{
    		collection.remove(messageType);
    		collection.put(messageType, messageTypeObject);
    	}
    }

	public HashMap<Integer, Message> getCollection() {
		return collection;
	}
    
}
