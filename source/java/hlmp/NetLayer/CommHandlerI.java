package hlmp.NetLayer;

public interface CommHandlerI {
	
	/**
	 * Se gatilla cuando debe comenzar la comunicación
	 */
    abstract void startNetworkingHandler();
    
    /**
     * Se gatilla cuando se debe detener la comunicación
     */
    abstract void stopNetworkingHandler();
    
    /**
     * Se gatilla cuando se reseteará la coneción
     */
    abstract void resetNetworkingHandler();
    
    /**
     * Se gatilla cuando ocurre un error en la red
     * @param e La excepción generada
     */
    abstract void errorNetworkingHandler(Exception e);
    
    /**
     * Se gatilla para enviar información del estado de la red.
     * @param message El mensaje envíado
     */
    abstract void informationNetworkingHandler(String message);

}
