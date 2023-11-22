namespace Kiyote.Files.Disk;

public interface IDiskFileSystemFactory {

	IReadWriteFileSystem CreateReadWriteFileSystem(
		string rootFolder
	);

	IReadOnlyFileSystem CreateReadOnlyFileSystem(
		string rootFolder
	);

}
