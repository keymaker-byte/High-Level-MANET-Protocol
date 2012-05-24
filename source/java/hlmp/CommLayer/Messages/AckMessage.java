package hlmp.CommLayer.Messages;

import hlmp.CommLayer.NetUser;
import hlmp.CommLayer.Constants.*;
import hlmp.Tools.BitConverter;

import java.util.UUID;


public class AckMessage extends UnicastMessage {
	
	/**
	 * El id del mensaje a confirmar
	 */
    private UUID messageId;
    
    

	public AckMessage() {
		super();
		this.type = MessageType.ACK;
		this.protocolType = MessageProtocolType.HLMP;
	}

	public AckMessage(NetUser targetNetUser, UUID messageId) {
		this();
		this.targetNetUser = targetNetUser;
		this.messageId = messageId;
	}
	
	public UUID getMessageId() {
		return messageId;
	}

	public void setMessageId(UUID messageId) {
		this.messageId = messageId;
	}

	@Override
	public byte[] makePack() {
		byte[] messageId = BitConverter.UUIDtoBytes(this.messageId); //16 (0 - 15)
        return messageId;
	}

	@Override
	public void unPack(byte[] messagePack) {
		this.messageId = BitConverter.bytesToUUID(messagePack);
	}
	
	/**
	 * Sobreescribe el metodo toString
	 * @return El string que representa este objeto
	 */
	@Override
    public String toString(){
        return super.toString() + "AckMessage: MessageId=" + this.messageId.toString();
    }

}
