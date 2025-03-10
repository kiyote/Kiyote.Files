﻿namespace Kiyote.Files.Virtual;

public sealed record MappedFileSystem<T>(
	FolderId MountAtFolder,
	IVirtualPathMapper PathMapper,
	T FileSystem
);
