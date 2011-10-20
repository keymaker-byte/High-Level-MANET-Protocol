package hlmp.SubProtocol.Ping.ControlI;

import hlmp.CommLayer.NetUser;

/**
 * Interfaz para eventos de mensajes de ping
 * @author nicolas
 *
 */
public interface PingHandlerI {

	/**
	 * Se gatilla cuando se ha recibido la confirmacion de un ping
	 * @param netUser El autor de la respuesta de ping
	 * @param milliseconds Milisegundos totales
	 */
	public void pingResponseMessageReceived(NetUser netUser, long milliseconds);
}
