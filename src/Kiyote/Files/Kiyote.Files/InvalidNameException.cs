using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Files;

[ExcludeFromCodeCoverage]
public sealed class InvalidNameException: Exception {
	public InvalidNameException(
		string message
	) : base( message ) {
	}

	public InvalidNameException(
		string message,
		Exception innerException
	) : base( message, innerException ) {
	}

	public InvalidNameException() {
	}
}
