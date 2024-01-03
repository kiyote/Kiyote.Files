using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Files;

[ExcludeFromCodeCoverage]
public sealed class FolderExistsException : Exception {
	public FolderExistsException(
		string message
	) : base( message ) {
	}

	public FolderExistsException(
		string message,
		Exception innerException
	) : base( message, innerException ) {
	}

	public FolderExistsException() {
	}
}
