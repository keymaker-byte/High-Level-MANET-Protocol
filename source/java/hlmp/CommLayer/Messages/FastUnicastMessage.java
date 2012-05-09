package hlmp.CommLayer.Messages;

import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.UUID;

import hlmp.CommLayer.NetUser;
import hlmp.CommLayer.Constants.*;
import hlmp.NetLayer.NetHandler;
import hlmp.NetLayer.NetMessage;
import hlmp.Tools.BitConverter;

/**
 * representa un Mensaje Unicast TCP con protocolo de seguridad en la entrega
 */
public abstract class FastUnicastMessage extends Message {

	/**
	 * El usuario receptor de este mensaje
	 */
    protected NetUser targetNetUser;

    /**
     * Default Constructor
     */
    public FastUnicastMessage()
    {
        this.id = UUID.randomUUID();
        this.jumps = 0;
        this.failReason = MessageFailReason.NOTFAIL;
        this.metaType = MessageMetaType.FASTUNICAST;
        this.type = MessageType.NOTYPE;
    }
	
	@Override
	public byte[] toByteArray() {
		byte[] metaPack = makeMetaPack();
        byte[] pack = makePack();
        byte[] messageData = new byte[metaPack.length + pack.length];
        System.arraycopy(metaPack, 0, messageData, 0, metaPack.length);
        System.arraycopy(pack, 0, messageData, metaPack.length, pack.length);
        return messageData;
	}

	@Override
	public void byteArrayToProperties(byte[] messageData) {
		 byte[] metaPack = new byte[72];
         byte[] pack = new byte[messageData.length - 72];
         System.arraycopy(messageData, 0, metaPack, 0, metaPack.length);
         System.arraycopy(messageData, 72, pack, 0, pack.length);
         metaUnPack(metaPack);
         unPack(pack);

	}

	public NetUser getTargetNetUser() {
		return targetNetUser;
	}

	public void setTargetNetUser(NetUser targetNetUser) {
		this.targetNetUser = targetNetUser;
	}
	
	/**
	 * Envía el mensaje a la MANET
	 * @param netHandler El manejador de la red
	 * @param ip la ip de la maquina remota destino
	 */
    public boolean send(NetHandler netHandler, InetAddress ip)
    {
        return netHandler.sendUdpMessage(new NetMessage(toByteArray()), ip);
    }

    /**
     * Convierte la meta data de este mensaje en una estructura de bytes
     * @return el array de bytes con la meta data
     */
    private byte[] makeMetaPack()
    {
        byte[] messageMetaType = BitConverter.intToByteArray(this.metaType); //4 (0 - 3)
        byte[] messageType = BitConverter.intToByteArray(this.type); //4 (4 - 7)
        byte[] messageProtocolType = BitConverter.intToByteArray(this.protocolType); //4 (8 - 11)
        byte[] userId = BitConverter.UUIDtoBytes(this.senderNetUser.getId()); //16 (12 - 27)
        byte[] userIp = this.senderNetUser.getIp().getAddress(); //4 (28-31)
        byte[] messageId = BitConverter.UUIDtoBytes(this.id); //16 (32 - 47)
        byte[] messageJumps = BitConverter.intToByteArray(this.jumps); //4 (48 - 51)
        byte[] targetId = BitConverter.UUIDtoBytes(this.targetNetUser.getId()); //16 (52 - 67)
        byte[] targetIp = this.targetNetUser.getIp().getAddress(); //4 (68-71)

        byte[] pack = new byte[72];
        System.arraycopy(messageMetaType, 0, pack, 0, 4);
        System.arraycopy(messageType, 0, pack, 4, 4);
        System.arraycopy(messageProtocolType, 0, pack, 8, 4);
        System.arraycopy(userId, 0, pack, 12, 16);
        System.arraycopy(userIp, 0, pack, 28, 4);
        System.arraycopy(messageId, 0, pack, 32, 16);
        System.arraycopy(messageJumps, 0, pack, 48, 4);
        System.arraycopy(targetId, 0, pack, 52, 16);
        System.arraycopy(targetIp, 0, pack, 68, 4);

        return pack;
    }

    /**
     * Convierte una estructura de bytes en la meta data de este mensaje
     * @param messageMetaPack un array de bytes con la meta data
     */
    private void metaUnPack(byte[] messageMetaPack)
    {
        this.metaType = BitConverter.readInt(messageMetaPack, 0);
        this.type = BitConverter.readInt(messageMetaPack, 4);
        this.protocolType = BitConverter.readInt(messageMetaPack, 8);
        this.senderNetUser = new NetUser();
        byte[] userId = new byte[16];
        System.arraycopy(messageMetaPack, 12, userId, 0, 16);
        this.senderNetUser.setId(BitConverter.bytesToUUID(userId));
        byte[] userIP = new byte[4];
        System.arraycopy(messageMetaPack, 28, userIP, 0, 4);
        try {
        	this.senderNetUser.setIp(InetAddress.getByAddress(userIP));
		} catch (UnknownHostException e) {
			e.printStackTrace();
		}
        byte[] messageId = new byte[16];
        System.arraycopy(messageMetaPack, 32, messageId, 0, 16);
        this.id = BitConverter.bytesToUUID(messageId);
        this.jumps = BitConverter.readInt(messageMetaPack, 48);
        this.targetNetUser = new NetUser();
        byte[] targetId = new byte[16];
        System.arraycopy(messageMetaPack, 52, targetId, 0, 16);
        this.targetNetUser.setId(BitConverter.bytesToUUID(targetId));
        byte[] targetIP = new byte[4];
        System.arraycopy(messageMetaPack, 68, targetIP, 0, 4);
        try {
        	this.targetNetUser.setIp(InetAddress.getByAddress(targetIP));
		} catch (UnknownHostException e) {
			e.printStackTrace();
		}
    }

    /**
     * Convierte las propiedades del mensaje en un paquete de bytes
     * @return un paquete de bytes con las propiedades del mensaje
     */
    public abstract byte[] makePack();

    /**
     * Convierte un paquete de bytes en las propiedades del mensaje
     * @param messagePack El paquete de bytes
     */
    public abstract void unPack(byte[] messagePack);

    /**
	 * Sobreescribe el metodo toString
	 * @return El string que representa este objeto
	 */
    @Override
    public String toString()
    {
        return "FastUnicastMessage Id = " + id + " : ";
    }
	
	

}
