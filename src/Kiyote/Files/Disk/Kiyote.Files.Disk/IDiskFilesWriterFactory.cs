namespace Kiyote.Files.Disk;

public interface IDiskFilesWriterFactory {

	DiskFilesWriter Create(
		IFileSystem fileSystem,
		DiskFileSystemConfiguration config,
		IFilesReader reader
	);
}
