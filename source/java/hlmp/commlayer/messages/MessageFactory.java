package hlmp.commlayer.messages;

import hlmp.commlayer.NetUser;
import hlmp.netlayer.Tools;
import hlmp.commlayer.constants.*;

import java.net.InetAddress;
import java.util.HashMap;
import java.util.UUID;

/**
 * Fabricador de mensajes provenientes de la red
 */
public class MessageFactory {
	
	/**
	 * Tipos adicionalaes de mensajes
	 */
    private HashMap<Integer,Class<Message>> messageTypes;

    /**
     * Constructor
     * @param messageTypes Tabla de hash con valores y tipos de mensajes adicionales
     */
    public MessageFactory(HashMap<Integer, Class<Message>> messageTypes)
    {
        this.messageTypes = messageTypes;
    }

    /**
     * Fabrica un mensaje que ha provenido de la red
     * @param messageData Los datos del mensajes como un conjunto de bytes
     * @return Un mensaje de alto nivel
     */
    public Message makeMessage(byte[] messageData) 
    {
        Message message = null;
        try
        {
            int messageType = Tools.readInt(messageData, 4);
            if (messageType < 100)
            {
                switch (messageType)
                {
                    case MessageType.ACK:
                        {
                            message = new AckMessage();
                            message.byteArrayToProperties(messageData);
                            break;
                        }
                    case MessageType.IMALIVE:
                        {
                            message = new ImAliveMessage();
                            message.byteArrayToProperties(messageData);
                            break;
                        }
                }
            }
            else
            {
                Class<Message> classType = messageTypes.get(messageType);
                message = classType.newInstance();
                message.byteArrayToProperties(messageData);
            }
        }
//        catch (InterruptedException e)
//        {
//            throw e;
//        }
        catch (Exception e)
        {
            //String s = e.getMessage();
        }
        return message;
    }
    
  	/**
     * Obtiene el id del mensaje
     * @param messageData Los datos del mensajes como un conjunto de bytes
     * @return El id del mensage
     */
    public UUID getMessageId(byte[] messageData)
    {
        try
        {
            byte[] messageId = new byte[16];
            System.arraycopy(messageData, 32, messageId, 0, 16);
            UUID Id = Tools.bytesToUUID(messageId);
            return Id;
        }
        catch (Exception e)
        {
        }
        return UUID.randomUUID();
    }
    
    
  /// <summary>
    /// Obtiene el target user solo si es de tipo unicast
    /// </summary>
    /// <param name="messageData">Los datos del mensajes como un conjunto de bytes</param>
    /// <returns>El destinatario</returns>
    public NetUser getTargetNetUser(byte[] messageData)
    {
        try
        {
            NetUser targetNetUser = new NetUser();
            byte[] targetId = new byte[16];
            System.arraycopy(messageData, 52, targetId, 0, 16);
            targetNetUser.setId(Tools.bytesToUUID(targetId));
            byte[] targetIP = new byte[4];
            System.arraycopy(messageData, 68, targetIP, 0, 4);
            targetNetUser.setIp(InetAddress.getByAddress(targetIP));
            return targetNetUser;
        }
        catch (Exception e)
        {
        }
        return null;
    }
    
    
  /// <summary>
    /// Obtiene el sender user
    /// </summary>
    /// <param name="messageData">Los datos del mensajes como un conjunto de bytes</param>
    /// <returns>El sender</returns>
    public NetUser getSenderNetUser(byte[] messageData)
    {
        try
        {
            NetUser senderNetUser = new NetUser();
            byte[] userId = new byte[16];
            System.arraycopy(messageData, 12, userId, 0, 16);
            senderNetUser.setId(Tools.bytesToUUID(userId));
            byte[] userIP = new byte[4];
            System.arraycopy(messageData, 28, userIP, 0, 4);
            senderNetUser.setIp(InetAddress.getByAddress(userIP));
            return senderNetUser;
        }
        catch (Exception e)
        {
        }
        return null;
    }
    
  /// <summary>
    /// Obtiene el metatipo del mensaje
    /// </summary>
    /// <param name="messageData">Los datos del mensajes como un conjunto de bytes</param>
    /// <returns>El metatipo del mensage</returns>
    public int getMessageMetaType(byte[] messageData)
    {
        try
        {
            int MetaType = Tools.readInt(messageData, 0);
            return MetaType;
        }
        catch (Exception e)
        {
        }
        return 0;
    }
}
