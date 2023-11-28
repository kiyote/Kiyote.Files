namespace Kiyote.Files.Disk;

internal sealed class DiskFilesWriterFactory : IDiskFilesWriterFactory {
	DiskFilesWriter IDiskFilesWriterFactory.Create(
		IFileSystem fileSystem,
		DiskFileSystemConfiguration config,
		IFilesReader reader
	) {
		return new DiskFilesWriter(
			fileSystem,
			config,
			reader
		);
	}
}
