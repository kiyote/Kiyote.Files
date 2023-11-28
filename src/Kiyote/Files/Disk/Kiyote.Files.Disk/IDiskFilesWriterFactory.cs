namespace Kiyote.Files.Disk;

public interface IDiskFilesWriterFactory {

	DiskFilesWriter Create(
		IFileSystem fileSystem,
		ConfiguredDiskFileSystem config,
		IFilesReader reader
	);
}
