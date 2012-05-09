package hlmp.CommLayer.Exceptions;

public class ArgumentOutOfRangeException extends Exception {

	private static final long serialVersionUID = 1L;

	public ArgumentOutOfRangeException() {
	}

	public ArgumentOutOfRangeException(String detailMessage) {
		super(detailMessage);
	}

	public ArgumentOutOfRangeException(Throwable throwable) {
		super(throwable);
	}

	public ArgumentOutOfRangeException(String detailMessage, Throwable throwable) {
		super(detailMessage, throwable);
	}

}