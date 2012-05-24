package hlmp.SubProtocol.Chat;

import hlmp.CommLayer.MessageTypeList;
import hlmp.CommLayer.NetUser;
import hlmp.CommLayer.SubProtocol;
import hlmp.CommLayer.Messages.Message;
import hlmp.SubProtocol.Chat.ControlI.ChatHandlerI;
import hlmp.SubProtocol.Chat.Messages.ChatMessage;
import hlmp.SubProtocol.Chat.Messages.GroupChatMessage;

/**
 * Protocolo de envio y recepcion de mensajes de texto
 */
public class ChatProtocol extends SubProtocol {
	
	/**
     * Handler de eventos de mensajes de texto
     */
    private ChatHandlerI controlChatHandler;

    /**
     * Constructor
     * @param controlChatHandler Un manejador de eventos de chat
     */
    public ChatProtocol(ChatHandlerI controlChatHandler)
    {
        this.controlChatHandler = controlChatHandler;
    }

	@Override
	protected void proccessMessage(Message message) {
		switch (message.getType())
        {
            case Types.CHATMESSAGE:
                {
                    ChatMessage textMessage = (ChatMessage)message;
                    controlChatHandler.chatMessageReceived(textMessage.getSenderNetUser(), textMessage.getText());
                    break;
                }
            case Types.GROUPCHATMESSAGE:
                {
                    GroupChatMessage textMessage = (GroupChatMessage)message;
                    controlChatHandler.groupChatMessageReceived(textMessage.getSenderNetUser(), textMessage.getText());
                    break;
                }
        }

	}

	@Override
	protected void errorMessage(Message message) {
		switch (message.getType())
        {
            case Types.CHATMESSAGE:
                {
                    ChatMessage textMessage = (ChatMessage)message;
                    controlChatHandler.chatWarninglInformation("No se ha podido entregar el mensage: " + textMessage.getText() + ". Al usuario: " + textMessage.getTargetNetUser().getName());
                    break;
                }
            case Types.GROUPCHATMESSAGE:
                {
                    GroupChatMessage textMessage = (GroupChatMessage)message;
                    controlChatHandler.chatWarninglInformation("No se ha podido enviar el mensage: " + textMessage.getText());
                    break;
                }
        }

	}
	
	@Override
	public MessageTypeList getMessageTypes() {
		MessageTypeList typeCollection = new MessageTypeList();
        typeCollection.add(hlmp.SubProtocol.Chat.Types.CHATMESSAGE, new ChatMessage());
        typeCollection.add(hlmp.SubProtocol.Chat.Types.GROUPCHATMESSAGE, new GroupChatMessage());
        return typeCollection;
	}

	
	/**
     * Envía un mensaje de texto a la red
     * @param netUser El usuario destino del mensaje, null si es para todos
     * @param text El texto del mensaje, si es de largo 0 no se envía
     */
    public void sendTextMessage(NetUser netUser, String text)
    {
        if (text.length() > 0)
        {
            if (netUser != null)
            {
                ChatMessage textMessage = new ChatMessage(netUser, text);
                sendMessageEvent(textMessage);
            }
            else
            {
                GroupChatMessage textMessage = new GroupChatMessage(text);
                sendMessageEvent(textMessage);
            }
        }
    }
	
}
