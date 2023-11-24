namespace Kiyote.Files.Disk;

public interface IDiskFileSystemFactory {

	IReadWriteFileSystem CreateReadWriteFileSystem(
		string fileSystemId,
		string rootFolder
	);

	IReadOnlyFileSystem CreateReadOnlyFileSystem(
		string fileSystemId,
		string rootFolder
	);

}
