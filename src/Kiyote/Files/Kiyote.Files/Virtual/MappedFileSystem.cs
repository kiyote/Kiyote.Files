namespace Kiyote.Files.Virtual;

public sealed record MappedFileSystem<T>(
	FolderId VirtualRoot,
	IVirtualPathMapper PathMapper,
	T FileSystem
);
