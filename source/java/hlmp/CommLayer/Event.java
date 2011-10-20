package hlmp.CommLayer;

/**
 * Contenedor para ordenar las llamadas a eventos
 * @author nicolas
 *
 */
public class Event {

	/**
	 * Tipo de evento, un entero de CommunicationEvent
	 */
	private int eventType;
	
	/**
	 * Parametro para notificar al Observer
	 */
	private Object param;
	
	
	public Event(int eventType, Object param) {
		super();
		this.eventType = eventType;
		this.param = param;
	}
	
	public int getEventType() {
		return eventType;
	}
	public Object getParam() {
		return param;
	}
	
}
