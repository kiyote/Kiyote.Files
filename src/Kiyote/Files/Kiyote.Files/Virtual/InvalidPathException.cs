namespace Kiyote.Files.Virtual;

public sealed class InvalidPathException : Exception {
	public InvalidPathException(
		string message
	) : base( message ) {
	}

	public InvalidPathException(
		string message,
		Exception innerException
	) : base( message, innerException ) {
	}

	public InvalidPathException() {
	}
}
