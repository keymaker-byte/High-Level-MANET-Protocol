package hlmp.SubProtocol.Chat.ControlI;

import hlmp.CommLayer.NetUser;

/**
 * Interfaz para eventos de mensajes de texto globales
 */
public interface ChatHandlerI {

	/**
     * Se gatilla cuando se ha recibido un nuevo mensaje de texto de un usuario
     * @param netUser El autor del mensaje
     * @param message El mensaje de texto
     */
    void chatMessageReceived(NetUser netUser, String message);

    /**
     * Se gatilla cuando se ha recibido un nuevo mensaje de texto a nivel grupal
     * @param netUser El autor del mensaje
     * @param message El mensaje de texto
     */
    void groupChatMessageReceived(NetUser netUser, String message);

    /**
     * Se gatilla cuando se recibe un mensaje de warning del protocolo
     * @param text el texto de información
     */
    void chatWarninglInformation(String text);
}
