package hlmp.CommLayer.Constants;

/// <summary>
/// Clase que enumera los estados posibles de la comunicación
/// </summary>
public final class CommunicationState
{
  /// <summary>
  /// Constante para el estado conectando
  /// </summary>
  public static final int STARTING = 1;

  /// <summary>
  /// Constante para el estado conectado
  /// </summary>
  public static final int STARTED = 2;

  /// <summary>
  /// Constante para el estado desconectando
  /// </summary>
  public static final int STOPPING = 3;

  /// <summary>
  /// Constante para el estado desconectado
  /// </summary>
  public static final int STOPPED = 4;

  /// <summary>
  /// Constante para eln estado iniciado
  /// </summary>
  public static final int INITIATED = 5;
}