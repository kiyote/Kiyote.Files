namespace Kiyote.Files.Disk;

internal sealed class DiskFileSystemFactory : IDiskFileSystemFactory {

	private readonly IFileSystem _fileSystem;

	public DiskFileSystemFactory(
		IFileSystem fileSystem
	) {
		_fileSystem = fileSystem;
	}

	IReadOnlyFileSystem IDiskFileSystemFactory.CreateReadOnlyFileSystem(
		string rootFolder
	) {
		return new DiskFileSystem( _fileSystem, rootFolder );
	}

	IReadWriteFileSystem IDiskFileSystemFactory.CreateReadWriteFileSystem(
		string rootFolder
	) {
		return new DiskFileSystem( _fileSystem, rootFolder );
	}

}
