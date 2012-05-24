package hlmp.CommLayer.Constants;

/**
 * Clase que enumera los eventos que lanza
 */
public final class CommunicationEvent
{
  /**
   * Constante para el evento connect
   */
  public static final int CONNECT = 0;
  
  /**
   * Constante para el evento connecting
   */
  public static final int CONNECTING = 1;

  /**
   * Constante para el evento disconnecting
   */
  public static final int DISCONNECTING = 2;

  /**
   * Constante para el evento disconnecting
   */
  public static final int DISCONNECT = 12;
  
  /**
   * Constante para el evento reconnecting
   */
  public static final int RECONNECTING = 3;

  /**
   * Constante para el evento adduser
   */
  public static final int ADDUSER = 4;

  /**
   * Constante para el evento removeuser
   */
  public static final int REMOVEUSER = 5;

  /**
   * Constante para el evento refreshuser
   */
  public static final int REFRESHUSER = 6;

  /**
   * Constante para el evento refreshlocaluser
   */
  public static final int REFRESHLOCALUSER = 7;

  /**
   * Constante para el evento net information
   */
  public static final int NETINFORMATION = 8;

  /**
   * Constante para el evento exception
   */
  public static final int EXCEPTION = 9;

  /**
   * Constante para el evento process subProtocol
   */
  public static final int PROCESSMESSAGE = 10;

  /**
   * Constante para el evento error subProtocol
   */
  public static final int ERRORMESSAGE = 13;
  
  /**
   * Constante para el procesamiento de mensages de parte del subprocol
   */
  public static final int SUBPROTOCOLPROCESSMESSAGE = 14; 
  
  /**
   * Constante para el procesamiento de mensages de error de parte del subprocol
   */
  public static final int SUBPROTOCOLERRORMESSAGE = 15;
}