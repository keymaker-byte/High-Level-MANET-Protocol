package hlmp.commlayer;

import hlmp.commlayer.messages.Message;

/// <summary>
/// Interfaz que representa las funcionalidades de un sub protocolo
/// </summary>
public interface SubProtocolI {
	
	/// <summary>
    /// Evento que se debe gatillar cuando el protocolo quiere enviar un mensaje a la red
    /// </summary>
    
	//Event Communication.MessageEvent sendMessageEvent;

    /// <summary>
    /// Se gatilla cuando se recibe un mensaje
    /// </summary>
    /// <param name="message">El mensaje recibido</param>
    void proccessMessage(Message message);

    /// <summary>
    /// Se gatilla cuando no es posible entregar un mensaje
    /// </summary>
    /// <param name="message">El mensaje no entregado</param>
    void errorMessage(Message message);

    /// <summary>
    /// Obtiene la lista de tipos de mensajes usados por este protocolo
    /// </summary>
    /// <returns>Una tabla con valores y tipos de mensajes usados en el protocolo</returns>
    public MessageTypeList getMessageTypes();

}
