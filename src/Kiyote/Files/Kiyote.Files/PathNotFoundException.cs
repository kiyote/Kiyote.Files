using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Files;

[ExcludeFromCodeCoverage]
public sealed class PathNotFoundException : Exception {
	public PathNotFoundException(
		string message
	) : base( message ) {
	}

	public PathNotFoundException(
		string message,
		Exception innerException
	) : base( message, innerException ) {
	}

	public PathNotFoundException() {
	}
}
