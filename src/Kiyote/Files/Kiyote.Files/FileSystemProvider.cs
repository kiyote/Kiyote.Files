using Microsoft.Extensions.DependencyInjection;

namespace Kiyote.Files;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in DI" )]
internal sealed class FileSystemProvider : IFileSystemProvider {

	private readonly IServiceProvider _services;

	public FileSystemProvider(
		IServiceProvider services
	) {
		_services = services;
	}

	IReadOnlyFileSystem? IFileSystemProvider.GetReadOnly(
		string fileSystemId
	) {
		return _services.GetKeyedService<IReadOnlyFileSystem>(
			fileSystemId + FileSystemIdentifier.ReadOnly
		);
	}

	IReadOnlyFileSystem? IFileSystemProvider.GetReadOnly(
		FileId fileId
	) {
		return ( this as IFileSystemProvider ).GetReadOnly( fileId.FileSystemId );
	}

	IReadWriteFileSystem? IFileSystemProvider.GetReadWrite(
		string fileSystemId
	) {
		return _services.GetKeyedService<IReadWriteFileSystem>(
			fileSystemId + FileSystemIdentifier.ReadWrite
		);
	}

	IReadWriteFileSystem? IFileSystemProvider.GetReadWrite(
		FileId fileId
	) {
		return ( this as IFileSystemProvider ).GetReadWrite( fileId.FileSystemId );
	}
}
