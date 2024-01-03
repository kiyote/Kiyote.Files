using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Files;

[ExcludeFromCodeCoverage]
public sealed class ContentUnavailableException: Exception {
	public ContentUnavailableException(
		string message
	) : base( message ) {
	}

	public ContentUnavailableException(
		string message,
		Exception innerException
	) : base( message, innerException ) {
	}

	public ContentUnavailableException() {
	}
}
