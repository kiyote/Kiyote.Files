using Microsoft.Extensions.DependencyInjection;

namespace Kiyote.Files;

internal sealed class FileSystemProvider : IFileSystemProvider {

	public const string ReadOnly = "RO";
	public const string ReadWrite = "RW";

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
			fileSystemId + ReadOnly
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
			fileSystemId + ReadWrite
		);
	}

	IReadWriteFileSystem? IFileSystemProvider.GetReadWrite(
		FileId fileId
	) {
		return ( this as IFileSystemProvider ).GetReadWrite( fileId.FileSystemId );
	}
}
