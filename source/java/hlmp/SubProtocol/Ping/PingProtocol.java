package hlmp.SubProtocol.Ping;

import hlmp.CommLayer.MessageTypeList;
import hlmp.CommLayer.NetUser;
import hlmp.CommLayer.SubProtocol;
import hlmp.CommLayer.Messages.Message;
import hlmp.SubProtocol.Ping.ControlI.PingHandlerI;
import hlmp.SubProtocol.Ping.Messages.*;

/**
 * Protocolo de envio y recepcion de pings
 * @author nicolas
 *
 */
public class PingProtocol extends SubProtocol{

	/// <summary>
    /// Handler de eventos de ping
    /// </summary>
    private PingHandlerI controlPingHandler;


    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="controlPingHandler">Un manejador de eventos de ping</param>
    public PingProtocol(PingHandlerI controlPingHandler)
    {
        this.controlPingHandler = controlPingHandler;
    }
	
	@Override
	protected void proccessMessage(Message message) {
		switch (message.getType())
        {
            case Types.PINGMESSAGE:
                {
                    PingMessage pingMessage = (PingMessage)message;
                    PingResponseMessage pingResponseMessage = new PingResponseMessage(pingMessage.getTimestamp(), message.getSenderNetUser());
                    sendMessageEvent(pingResponseMessage);
                    break;
                }
            case Types.PINGRESPONSEMESSAGE:
                {
                    PingResponseMessage pingResponseMessage = (PingResponseMessage)message;
                    controlPingHandler.pingResponseMessageReceived(pingResponseMessage.getSenderNetUser(), System.currentTimeMillis()-pingResponseMessage.getTimestamp());
                    break;
                }
        }
		
	}

	@Override
	protected void errorMessage(Message message) {
				
	}

	@Override
	public MessageTypeList getMessageTypes() {
		MessageTypeList typeCollection = new MessageTypeList();
        typeCollection.add(Types.PINGMESSAGE, new PingMessage());
        typeCollection.add(Types.PINGRESPONSEMESSAGE, new PingResponseMessage());
        return typeCollection;
	}

	/**
	 * Envía un mensaje de ping a la red
	 * @param netUser El usuario destino del mensaje
	 */
    public void sendPingMessage(NetUser netUser)
    {
        PingMessage pingMessage = new PingMessage(netUser);
        sendMessageEvent(pingMessage);
    }
}
