package hlmp.CommLayer.Exceptions;

public class ArgumentNullException extends Exception {

	private static final long serialVersionUID = 1L;

	public ArgumentNullException() {
	}

	public ArgumentNullException(String detailMessage) {
		super(detailMessage);
	}

	public ArgumentNullException(Throwable throwable) {
		super(throwable);
	}

	public ArgumentNullException(String detailMessage, Throwable throwable) {
		super(detailMessage, throwable);
	}

}