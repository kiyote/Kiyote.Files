namespace Kiyote.Files.Disk;

internal sealed class DiskFileSystemFactory : IDiskFileSystemFactory {

	private readonly IFileSystem _fileSystem;

	public DiskFileSystemFactory(
		IFileSystem fileSystem
	) {
		_fileSystem = fileSystem;
	}

	IReadOnlyFileSystem IDiskFileSystemFactory.CreateReadOnlyFileSystem(
		string fileSystemId,
		string rootFolder
	) {
		return new DiskFileSystem( _fileSystem, fileSystemId, rootFolder );
	}

	IReadWriteFileSystem IDiskFileSystemFactory.CreateReadWriteFileSystem(
		string fileSystemId,
		string rootFolder
	) {
		return new DiskFileSystem( _fileSystem, fileSystemId,rootFolder );
	}

}
