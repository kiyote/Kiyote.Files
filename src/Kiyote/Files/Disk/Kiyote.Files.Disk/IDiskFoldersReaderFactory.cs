namespace Kiyote.Files.Disk;

public interface IDiskFoldersReaderFactory {

	DiskFoldersReader Create(
		IFileSystem fileSystem,
		DiskFileSystemConfiguration config
	);

}
