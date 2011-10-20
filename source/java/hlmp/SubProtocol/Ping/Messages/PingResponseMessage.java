package hlmp.SubProtocol.Ping.Messages;

import hlmp.CommLayer.NetUser;
import hlmp.CommLayer.Messages.FastUnicastMessage;
import hlmp.SubProtocol.Ping.*;
import hlmp.Tools.BitConverter;
/**
 * Envía una respuesta de mensaje de Ping a la maquina remota destino para medir tiempo de envio + respuesta
 * @author nicolas
 *
 */
public class PingResponseMessage extends FastUnicastMessage {

	private long timestamp;

    private byte[] data;

    /**
     * Constructor
     */
    public PingResponseMessage()
    {
    	super();
        this.type = Types.PINGRESPONSEMESSAGE;
        this.protocolType = Types.PINGPROTOCOL;
        data = new byte[16];
    }

    /**
     * Constructor
     * @param timeStamp
     * @param targetNetUser
     */
    public PingResponseMessage(long timeStamp, NetUser targetNetUser)
    {
    	this();
        this.timestamp = timeStamp;
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
