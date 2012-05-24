package hlmp.CommLayer.Messages;

import java.util.UUID;

import hlmp.CommLayer.NetUser;

public abstract class Message {

	/**
	 * El autor de este mensaje
	 */
    protected NetUser senderNetUser;

    /**
     * El id unico de este mensaje
     */
    protected UUID id;

    /**
     * Los saltos de host que ha dado este mensaje
     */
    protected int jumps;

    /**
     * El motivo de fallo en la entrega (un valor de MessageFailReason)
     */
    protected int failReason;

    /**
     * El tipo de dato abstracto de este mensaje
     */
    protected int metaType;

    /**
     * El tipo de este mensaje
     */
    protected int type;

    /**
     * El tipo de protocolo de este mensaje
     */
    protected int protocolType;

	public NetUser getSenderNetUser() {
		return senderNetUser;
	}

	public void setSenderNetUser(NetUser senderNetUser) {
		this.senderNetUser = senderNetUser;
	}

	public UUID getId() {
		return id;
	}

	public void setId(UUID id) {
		this.id = id;
	}

	public int getJumps() {
		return jumps;
	}

	public void setJumps(int jumps) {
		this.jumps = jumps;
	}
	
	public void jumpsAdd1(){
		this.jumps++;
	}

	public int getFailReason() {
		return failReason;
	}

	public void setFailReason(int failReason) {
		this.failReason = failReason;
	}

	public int getMetaType() {
		return metaType;
	}

	public void setMetaType(int metaType) {
		this.metaType = metaType;
	}

	public int getType() {
		return type;
	}

	public void setType(int type) {
		this.type = type;
	}

	public int getProtocolType() {
		return protocolType;
	}

	public void setProtocolType(int protocolType) {
		this.protocolType = protocolType;
	}
    
	/**
	 * Convierte a este mensaje en un paquete de bytes
	 * @return un array de bytes con todos los datos del mensaje
	 */
    public abstract byte[] toByteArray();

    /**
     * Convierte un paquete de bytes en las propiedades del mensaje
     * @param messageData un array de bytes con todos los datos del mensaje
     */
    public abstract void byteArrayToProperties(byte[] messageData);

}
