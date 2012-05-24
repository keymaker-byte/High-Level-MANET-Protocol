package hlmp.NetLayer;

import javax.xml.bind.annotation.XmlRootElement;

@XmlRootElement
/**
 * Clase que representa un mensaje recibido o enviado por la RED
 */
public class NetMessage {
	
	/**
	 * El cuerpo del mensaje
	 */
	private byte[] body;
	
	public NetMessage(){
		this.body = new byte[1];
		body[0]=0;
	}
	
	/**
	 * Constructor
	 * @param body El contenido del mensaje
	 */
	public NetMessage(byte[] body) {
		super();
		this.body = body;
	}

	public byte[] getBody() {
		return this.body;
	}

	public void setBody(byte[] body) {
		this.body = body;
	}


	/**
	 * El largo de este mensaje, en número de bytes
	 * @return
	 */
	public int getSize(){
		return this.body.length;
	}
}