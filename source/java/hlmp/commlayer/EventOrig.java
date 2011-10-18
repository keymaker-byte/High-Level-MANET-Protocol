package hlmp.commlayer;

public class EventOrig {
	
	/**
     * El evento
     */
    private EventDelegate eventHandler;

    /**
     * El evento
     */
    public EventDelegate getEventHandler() {
		return eventHandler;
	}

	public void setEventHandler(EventDelegate eventHandler) {
		this.eventHandler = eventHandler;
	}

    /**
     * El parametro
     */
    private Object param;



    public Object getParam() {
		return param;
	}

	public void setParam(Object param) {
		this.param = param;
	}

	/**
     * Constructor
     * @param eventHandler 
     * @param param 
     */
    public EventOrig(EventDelegate eventHandler, Object param)
    {
        this.eventHandler = eventHandler;
        this.param = param;
    }

    /**
     * Ejecuta el evento
     */
    public void execute()
    {
        try
        {
            if (param != null)
            {
                eventHandler.execute(param);
            }
            else
            {
                eventHandler.execute();
            }
        }
        catch (Exception e)
        {

        }
    }
}
