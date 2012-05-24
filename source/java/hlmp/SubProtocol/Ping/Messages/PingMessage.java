package hlmp.SubProtocol.Ping.Messages;

import hlmp.CommLayer.NetUser;
import hlmp.CommLayer.Messages.FastUnicastMessage;
import hlmp.SubProtocol.Ping.*;
import hlmp.Tools.BitConverter;
/**
 * Envía un mensaje de Ping a la maquina remota destino para medir tiempo de envio + respuesta
 * @author nicolas
 *
 */
public class PingMessage extends FastUnicastMessage {

	private long timestamp;
	
	private byte[] data;

	/**
	 * Constructor
	 */
    public PingMessage()
    {
    	super();
        this.type = Types.PINGMESSAGE;
        this.protocolType = Types.PINGPROTOCOL;
        data = new byte[16];
        this.timestamp = System.currentTimeMillis();
    }

    /**
     * Constuctor
     * @param targetNetUser 
     */
    public PingMessage(NetUser targetNetUser)
    {
    	this();
        this.targetNetUser = targetNetUser;
    }
    
	@Override
	public byte[] makePack() {
		byte[] timestamp = new byte[8];
		BitConverter.writeLong(this.timestamp, timestamp, 0);
		System.arraycopy(timestamp, 0, data, 0, 8);
        return data;
	}

	@Override
	public void unPack(byte[] messagePack) {
		this.timestamp = BitConverter.readLong(messagePack, 0);
	}

	public long getTimestamp() {
		return timestamp;
	}

	public void setTimestamp(long timestamp) {
		this.timestamp = timestamp;
	}
	

}
