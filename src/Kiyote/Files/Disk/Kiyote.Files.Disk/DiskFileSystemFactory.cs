namespace Kiyote.Files.Disk;

internal sealed class DiskFileSystemFactory : IDiskFileSystemFactory {

	private readonly IFileSystem _fileSystem;
	private readonly IDiskFilesReaderFactory _filesReaderFactory;
	private readonly IDiskFilesWriterFactory _filesWriterFactory;
	private readonly IDiskFoldersReaderFactory _foldersReaderFactory;

	public DiskFileSystemFactory(
		IFileSystem fileSystem,
		IDiskFilesReaderFactory diskFilesReaderFactory,
		IDiskFilesWriterFactory diskFilesWriterFactory,
		IDiskFoldersReaderFactory diskFoldersReaderFactory
	) {
		_fileSystem = fileSystem;
		_filesReaderFactory = diskFilesReaderFactory;
		_filesWriterFactory = diskFilesWriterFactory;
		_foldersReaderFactory = diskFoldersReaderFactory;
	}

	IReadOnlyFileSystem IDiskFileSystemFactory.CreateReadOnlyFileSystem(
		string fileSystemId,
		string rootFolder
	) {
		return ( this as IDiskFileSystemFactory ).CreateReadWriteFileSystem(
			fileSystemId,
			rootFolder
		);
	}

	IReadWriteFileSystem IDiskFileSystemFactory.CreateReadWriteFileSystem(
		string fileSystemId,
		string rootFolder
	) {
		ConfiguredDiskFileSystem config = new ConfiguredDiskFileSystem(
			fileSystemId,
			rootFolder
		);
		IFilesReader filesReader = _filesReaderFactory.Create( _fileSystem, config );
		IFilesWriter filesWriter = _filesWriterFactory.Create( _fileSystem, config, filesReader );
		IFoldersReader foldersReader = _foldersReaderFactory.Create( _fileSystem, config );
		return new DiskFileSystem(
			filesReader,
			filesWriter,
			foldersReader
		);
	}
}
