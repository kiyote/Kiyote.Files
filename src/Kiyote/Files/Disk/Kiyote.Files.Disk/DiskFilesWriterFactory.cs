namespace Kiyote.Files.Disk;

internal sealed class DiskFilesWriterFactory : IDiskFilesWriterFactory {
	DiskFilesWriter IDiskFilesWriterFactory.Create(
		IFileSystem fileSystem,
		ConfiguredDiskFileSystem config,
		IFilesReader reader
	) {
		return new DiskFilesWriter(
			fileSystem,
			config,
			reader
		);
	}
}
