package hlmp.netlayer;


final class IphandlerState {
	public static final int STARTEDSTRONG = 1;
	public static final int STOPPED = 2;
	public static final int STARTEDWEAK = 3;
}

final class IpState {
	public static final int NOTFOUND = 0;
	public static final int INVALID = 1;
	public static final int DUPLICATE = 2;
	public static final int VALID = 3;
}

final class NetHandlerState {
	public static final int STARTING = 1;
	public static final int STARTED = 2;
	public static final int STOPPING = 3;
	public static final int STOPPED = 4;
	public static final int INITIATED = 5;
	public static final int STOPFORCED = 6;
}

final class OpSystemType {
	/**
	 * Windows XP Service Pack 3
	 */
	public static final int WINXPSP3 = 1;
	
	/**
	 * Windows Vista
	 */
	public static final int WINVISTA = 2;
	
	/**
	 * Android 2.1
	 */
	public static final int ANDROID21 = 3;
	
	/**
	 * Ubuntu 11.04
	 */
	public static final int UBUNTU1104 = 4;
}

final class WifiConnectionState {
	public static final int CONNECTED = 1;
    public static final int DISCONNECTED = 2;
    public static final int WAINTING = 4;
    public static final int STOP = 5;
}