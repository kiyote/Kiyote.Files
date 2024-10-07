namespace Kiyote.Files.Virtual;

internal sealed class FileSystemBuilder<T> : IFileSystemBuilder<T> where T : IFileSystemIdentifier {

	private readonly FileSystemId _fileSystemId;
	private readonly IVirtualPathHandler _virtualPathHandler;

	private readonly Dictionary<FolderId, MappedFileSystem<IFileSystem>> _readWrite;
	private readonly Dictionary<FolderId, MappedFileSystem<IReadOnlyFileSystem>> _readOnly;

	public FileSystemBuilder(
		IVirtualPathHandler virtualPathHandler,
		FileSystemId fileSystemId
	) {
		_virtualPathHandler = virtualPathHandler;
		_fileSystemId = fileSystemId;
		_readWrite = [];
		_readOnly = [];
	}

	public IFileSystem<T> Build() {
		return new FileSystemAdapter<T>(
			new VirtualFileSystem(
				_virtualPathHandler,
				_fileSystemId,
				_readOnly,
				_readWrite
			)
		);
	}

	void IFileSystemBuilder<T>.AddReadWrite(
		FolderId virtualRoot,
		IFileSystem fileSystem,
		IVirtualPathMapper pathMapper
	) {
		Add( _readWrite, virtualRoot, fileSystem, pathMapper );
	}

	void IFileSystemBuilder<T>.AddReadOnly(
		FolderId virtualRoot,
		IReadOnlyFileSystem fileSystem,
		IVirtualPathMapper pathMapper
	) {
		Add( _readOnly, virtualRoot, fileSystem, pathMapper );
	}

	private static void Add<TItem>(
		Dictionary<FolderId, MappedFileSystem<TItem>> cache,
		FolderId folderId,
		TItem fileSystem,
		IVirtualPathMapper pathMapper
	) {
		if( !cache.TryGetValue( folderId, out MappedFileSystem<TItem>? _ ) ) {
			cache[ folderId ] = new MappedFileSystem<TItem>( folderId, pathMapper, fileSystem );
			return;
		}

		throw new InvalidOperationException();
	}
}
