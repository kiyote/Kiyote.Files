namespace Kiyote.Files.Disk;

public interface IDiskFilesReaderFactory {

	DiskFilesReader Create(
		IFileSystem fileSystem,
		DiskFileSystemConfiguration config
	);

}
