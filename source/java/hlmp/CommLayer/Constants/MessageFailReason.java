package hlmp.CommLayer.Constants;

/// <summary>
/// Clase que enumera las acciones y errores posibles al tratar de enviar un mensaje
/// </summary>
public final class MessageFailReason{
	/// <summary>
	/// El mensaje no ha fallado
	/// </summary>
	public static final int NOTFAIL = 1;

	/// <summary>
	/// El mensaje ha fallado en la entrega al vecino elegido
	/// </summary>
	public static final int TCPFAIL = 2;

	/// <summary>
	/// No existe una ruta al destinatario, pero el usuario si se encuentra en la MANET
	/// </summary>
	public static final int NOTROUTETOHOST = 3;

	/// <summary>
	/// El mensaje debe ser destruido
	/// </summary>
	public static final int DESTROY = 4;

	/// <summary>
	/// El usuario destino no existe en la red
	/// </summary>
	public static final int NOTROUTEBUTHOSTONNET = 5;
}