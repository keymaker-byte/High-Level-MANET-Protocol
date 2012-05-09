package hlmp.CommLayer.Messages;

import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.UUID;

import hlmp.CommLayer.NetUser;
import hlmp.CommLayer.Constants.*;
import hlmp.NetLayer.NetHandler;
import hlmp.NetLayer.NetMessage;
import hlmp.Tools.BitConverter;

public abstract class UnicastMessage extends Message {

	/// <summary>
	/// El receptor de este mensaje
	/// </summary>
	protected NetUser targetNetUser;

	/// <summary>
	/// Default Constructor
	/// </summary>
	public UnicastMessage()
	{
		this.id = UUID.randomUUID();
		this.jumps = 0;
		this.failReason = MessageFailReason.NOTFAIL;
		this.metaType = MessageMetaType.UNICAST;
		this.type = MessageType.NOTYPE;
		this.protocolType = MessageProtocolType.NOTYPE;
	}

	public NetUser getTargetNetUser() {
		return targetNetUser;
	}
	public void setTargetNetUser(NetUser targetNetUser) {
		this.targetNetUser = targetNetUser;
	}

	/// <summary>
	/// Convierte a este mensaje en un paquete de bytes
	/// </summary>
	/// <returns>un array de bytes con todos los datos del mensaje</returns>
	@Override
	public byte[] toByteArray() {
		byte[] metaPack = makeMetaPack();
        byte[] pack = makePack();
        byte[] messageData = new byte[metaPack.length + pack.length];
        System.arraycopy(metaPack, 0, messageData, 0, metaPack.length);
        System.arraycopy(pack, 0, messageData, metaPack.length, pack.length);
        return messageData;
	}

	/// <summary>
	/// Convierte un paquete de bytes en las propiedades del mensaje
	/// </summary>
	/// <param name="messageData">un array de bytes con todos los datos del mensaje</param>
	@Override
	public void byteArrayToProperties(byte[] messageData) {
		byte[] metaPack = new byte[72];
        byte[] pack = new byte[messageData.length - 72];
        System.arraycopy(messageData, 0, metaPack, 0, metaPack.length);
        System.arraycopy(messageData, 72, pack, 0, pack.length);
        metaUnPack(metaPack);
        unPack(pack);

	}

	/**
	 * Envía el mensaje a la MANET
	 * @param netHandler El manejador de la red
	 * @param ip la ip de la maquina remota destino
	 * @return true si se envio correctamente, false si no
	 */
	public boolean send(NetHandler netHandler, InetAddress ip)
	{
		return netHandler.sendTcpMessage(new NetMessage(toByteArray()), ip);
	}

	/// <summary>
	/// Convierte la meta data de este mensaje en una estructura de bytes
	/// </summary>
	/// <returns>el array de bytes con la meta data</returns>
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

	/// <summary>
	/// Convierte una estructura de bytes en la meta data de este mensaje
	/// </summary>
	/// <param name="messageMetaPack">un array de bytes con la meta data</param>
	private void metaUnPack(byte[] messageMetaPack)
	{
		this.metaType = BitConverter.readInt(messageMetaPack, 0);
        type = BitConverter.readInt(messageMetaPack, 4);
        protocolType = BitConverter.readInt(messageMetaPack, 8);
        senderNetUser = new NetUser();
        byte[] userId = new byte[16];
        System.arraycopy(messageMetaPack, 12, userId, 0, 16);
        senderNetUser.setId(BitConverter.bytesToUUID(userId));
        byte[] userIP = new byte[4];
        System.arraycopy(messageMetaPack, 28, userIP, 0, 4);
        try {
			senderNetUser.setIp(InetAddress.getByAddress(userIP));
		} catch (UnknownHostException e) {
			e.printStackTrace();
		}
        byte[] messageId = new byte[16];
        System.arraycopy(messageMetaPack, 32, messageId, 0, 16);
        id = BitConverter.bytesToUUID(messageId);
        jumps = BitConverter.readInt(messageMetaPack, 48);
        targetNetUser = new NetUser();
        byte[] targetId = new byte[16];
        System.arraycopy(messageMetaPack, 52, targetId, 0, 16);
        targetNetUser.setId(BitConverter.bytesToUUID(targetId));
        byte[] targetIP = new byte[4];
        System.arraycopy(messageMetaPack, 68, targetIP, 0, 4);
        try {
			targetNetUser.setIp(InetAddress.getByAddress(targetIP));
		} catch (UnknownHostException e) {
			e.printStackTrace();
		}
	}

	/// <summary>
	/// Convierte las propiedades del mensaje en un paquete de bytes
	/// </summary>
	/// <returns>un paquete de bytes con las propiedades del mensaje</returns>
	public abstract byte[] makePack();

	/// <summary>
	/// Convierte un paquete de bytes en las propiedades del mensaje
	/// </summary>
	/// <param name="messagePack">El paquete de bytes</param>
	public abstract void unPack(byte[] messagePack);

	/// <summary>
	/// Sobreescribe el metodo toString
	/// </summary>
	/// <returns>El string que representa este objeto</returns>
	@Override
	public String toString(){
		return "UnicastMessage Id = " + this.id + " : ";
	}

}
