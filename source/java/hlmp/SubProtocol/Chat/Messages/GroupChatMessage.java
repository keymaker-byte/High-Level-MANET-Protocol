package hlmp.SubProtocol.Chat.Messages;

import hlmp.CommLayer.Messages.SafeMulticastMessage;
import hlmp.Tools.BitConverter;

public class GroupChatMessage extends SafeMulticastMessage{
	
	/**
     * El texto de este mensaje
     */
    private String text;

    /**
     * Constructor parametrizado
     */
    public GroupChatMessage()
    {
    	super();
        this.type = hlmp.SubProtocol.Chat.Types.GROUPCHATMESSAGE;
        this.protocolType = hlmp.SubProtocol.Chat.Types.CHATPROTOCOL;
    }

    /**
     * Constructor parametrizado
     * @param text El texto del mensaje
     */
    public GroupChatMessage(String text)
    {
    	this();
        this.text = text;
    }

    
	public String getText() {
		return text;
	}

	public void setText(String text) {
		this.text = text;
	}

	@Override
	public byte[] makePack() {
		byte[] text = BitConverter.stringToByte(this.text);
		return text;
	}

	@Override
	public void unPack(byte[] messagePack) {
		this.text = BitConverter.byteToString(messagePack);
		
	}
	
	/**
     * Sobreescribe el metodo toString
     * @return El string que representa este objeto
     */
	@Override
    public String toString()
    {
        return super.toString() + "GroupTextMessage: Text=" + this.text;
    }

}
