namespace Kiyote.Files.Virtual;

public interface IFileSystemBuilder<T> {

	void AddReadWrite(
		FolderId virtualRoot,
		IFileSystem fileSystem,
		IVirtualPathMapper pathMapper
	);

	void AddReadOnly(
		FolderId virtualRoot,
		IReadOnlyFileSystem fileSystem,
		IVirtualPathMapper pathMapper
	);

}
