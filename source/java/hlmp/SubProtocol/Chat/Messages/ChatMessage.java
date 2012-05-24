package hlmp.SubProtocol.Chat.Messages;

import hlmp.CommLayer.NetUser;
import hlmp.CommLayer.Messages.SafeUnicastMessage;
import hlmp.Tools.BitConverter;

public class ChatMessage extends SafeUnicastMessage{
	
	/**
     * El texto de este mensaje
     */
    private String text;

    /**
     * Constructor parametrizado
     */
    public ChatMessage()
    {
    	super();
        this.type = hlmp.SubProtocol.Chat.Types.CHATMESSAGE;
        this.protocolType = hlmp.SubProtocol.Chat.Types.CHATPROTOCOL;
    }

    /**
     * Constructor parametrizado
     * @param targetNetUser El usuario destinatario de este mensaje
     * @param text El texto del mensaje
     */
    public ChatMessage(NetUser targetNetUser, String text)
    {
    	this();
        this.targetNetUser = targetNetUser;
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
        return super.toString() + "TextMessage: Text=" + this.text;
    }

}
