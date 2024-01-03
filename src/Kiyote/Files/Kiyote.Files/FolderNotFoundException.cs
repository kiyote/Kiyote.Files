using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Files;

[ExcludeFromCodeCoverage]
public sealed class FolderNotFoundException : Exception {
	public FolderNotFoundException(
		string message
	) : base( message ) {
	}

	public FolderNotFoundException(
		string message,
		Exception innerException
	) : base( message, innerException ) {
	}

	public FolderNotFoundException() {
	}
}
