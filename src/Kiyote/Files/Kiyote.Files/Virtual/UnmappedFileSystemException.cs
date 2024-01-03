using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Files.Virtual;

[ExcludeFromCodeCoverage]
public sealed class UnmappedFileSystemException : Exception {
	public UnmappedFileSystemException(
		string message
	) : base( message ) {
	}

	public UnmappedFileSystemException(
		string message,
		Exception innerException
	) : base( message, innerException ) {
	}

	public UnmappedFileSystemException() {
	}
}
