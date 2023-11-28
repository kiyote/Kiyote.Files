namespace Kiyote.Files.Disk;

internal sealed class DiskFilesReaderFactory : IDiskFilesReaderFactory {
	DiskFilesReader IDiskFilesReaderFactory.Create(
		IFileSystem fileSystem,
		DiskFileSystemConfiguration config
	) {
		return new DiskFilesReader( fileSystem, config );
	}
}
