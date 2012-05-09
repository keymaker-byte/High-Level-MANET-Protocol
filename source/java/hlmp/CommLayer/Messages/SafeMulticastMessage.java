package hlmp.CommLayer.Messages;

import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.UUID;

import hlmp.CommLayer.NetUser;
import hlmp.CommLayer.Constants.*;
import hlmp.NetLayer.NetHandler;
import hlmp.NetLayer.NetMessage;
import hlmp.Tools.BitConverter;

public abstract class SafeMulticastMessage extends Message {

	/**
     * Default Constructor
     */
    public SafeMulticastMessage()
    {
        this.id = UUID.randomUUID();
        this.jumps = 0;
        this.failReason = MessageFailReason.NOTFAIL;
        this.metaType = MessageMetaType.SAFEMULTICAST;
        this.type = MessageType.NOTYPE;
        this.protocolType = MessageProtocolType.NOTYPE;
    }

    /**
     * Convierte un paquete de bytes en las propiedades del mensaje
     * @param messageData un array de bytes con todos los datos del mensaje
     */
    @Override
	public void byteArrayToProperties(byte[] messageData){
        byte[] metaPack = new byte[52];
        byte[] pack = new byte[messageData.length - 52];
        System.arraycopy(messageData, 0, metaPack, 0, metaPack.length);
        System.arraycopy(messageData, 52, pack, 0, pack.length);
        metaUnPack(metaPack);
        unPack(pack);
    }

    /**
     * Convierte a este mensaje en un paquete de bytes
     * @return un array de bytes con todos los datos del mensaje
     */
    @Override
	public byte[] toByteArray(){
    	byte[] metaPack = makeMetaPack();
        byte[] pack = makePack();
        byte[] messageData = new byte[metaPack.length + pack.length];
        System.arraycopy(metaPack, 0, messageData, 0, metaPack.length);
        System.arraycopy(pack, 0, messageData, metaPack.length, pack.length);
        return messageData;
    }

    /**
	 * Envía el mensaje a la MANET
	 * @param netHandler El manejador de la red
	 */
    public void send(NetHandler netHandler)
    {
        try {
			netHandler.sendTcpMessage(new NetMessage(toByteArray()));
		} catch (InterruptedException e) {
			e.printStackTrace();
		}
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

        byte[] pack = new byte[52];
        System.arraycopy(messageMetaType, 0, pack, 0, 4);
        System.arraycopy(messageType, 0, pack, 4, 4);
        System.arraycopy(messageProtocolType, 0, pack, 8, 4);
        System.arraycopy(userId, 0, pack, 12, 16);
        System.arraycopy(userIp, 0, pack, 28, 4);
        System.arraycopy(messageId, 0, pack, 32, 16);
        System.arraycopy(messageJumps, 0, pack, 48, 4);
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
    public String toString(){
        return "SafeMulticastMessage Id = " + id + " : ";
    }

}
