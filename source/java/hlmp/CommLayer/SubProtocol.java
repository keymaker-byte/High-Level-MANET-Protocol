package hlmp.CommLayer;

import hlmp.CommLayer.Messages.Message;

/**
 * Clase abstracta que representa las funcionalidades de un sub protocolo
 * @author nicolas
 *
 */
public abstract class SubProtocol {

	/**
	 * Clase Communication asociada para enviar mensajes
	 */
	private Communication communication;
	
	public void setComm(Communication comm){
		this.communication = comm;
	}
	
	/**
	 * se debe gatillar cuando el protocolo quiere enviar un mensaje a la red
	 * @param message mensaje a enviar
	 */
	public void sendMessageEvent(Message message){
		this.communication.send(message);
	}

	/**
	 * Se gatilla cuando se recibe un mensaje
	 * @param message El mensaje recibido
	 */
    protected abstract void proccessMessage(Message message);

    /**
     * Se gatilla cuando no es posible entregar un mensaje
     * @param message El mensaje no entregado
     */
    protected abstract void errorMessage(Message message);

    /**
     * Obtiene la lista de tipos de mensajes usados por este protocolo
     * @return Una tabla con valores y tipos de mensajes usados en el protocolo
     */
    public abstract MessageTypeList getMessageTypes();
	
}
