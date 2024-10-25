namespace Kiyote.Files.Virtual;

internal sealed class VirtualFileSystem : IFileSystem {

	private readonly FileSystemId _fileSystemId;
	private readonly FileSystemTree _tree;
	private readonly Dictionary<Node, List<MappedFileSystem<IFileSystem>>> _readWrite;
	private readonly Dictionary<Node, List<MappedFileSystem<IReadOnlyFileSystem>>> _readOnly;
	private readonly char[] _invalidPathChars;
	private readonly char[] _invalidFileNameChars;
	private readonly string[] _invalidNames;
	private readonly FolderIdentifier _root;
	private readonly IVirtualPathHandler _virtualPathHandler;
	private readonly List<MappedFileSystem<IReadOnlyFileSystem>> _unmappedRO;
	private readonly List<MappedFileSystem<IFileSystem>> _unmappedRW;

	public VirtualFileSystem(
		IVirtualPathHandler virtualPathHandler,
		FileSystemId fileSystemId,
		Dictionary<FolderId, MappedFileSystem<IReadOnlyFileSystem>> readOnly,
		Dictionary<FolderId, MappedFileSystem<IFileSystem>> readWrite
	) {
		_virtualPathHandler = virtualPathHandler;
		_fileSystemId = fileSystemId;

		_invalidPathChars = readWrite
			.SelectMany( rw => rw.Value.FileSystem.GetInvalidPathChars() )
			.Distinct()
			.ToArray();

		_invalidFileNameChars = readWrite
			.SelectMany( rw => rw.Value.FileSystem.GetInvalidFileNameChars() )
			.Distinct()
			.ToArray();

		_invalidNames = readWrite
			.SelectMany( rw => rw.Value.FileSystem.GetInvalidNames() )
			.Distinct()
			.ToArray();

		_tree = new FileSystemTree( _virtualPathHandler.Separator );
		_readWrite = [];
		_readOnly = [];
		foreach( KeyValuePair<FolderId, MappedFileSystem<IFileSystem>> pair in readWrite ) {
			FolderId path = pair.Key;
			Node node = _tree.Insert( path.AsSpan() );

			if( !_readWrite.TryGetValue( node, out List<MappedFileSystem<IFileSystem>>? rwfs ) ) {
				_readWrite[ node ] = [];
			}
			_readWrite[ node ].Add( pair.Value );

			if( !_readOnly.TryGetValue( node, out List<MappedFileSystem<IReadOnlyFileSystem>>? rofs ) ) {
				_readOnly[ node ] = [];
			}
			_readOnly[ node ].Add( new MappedFileSystem<IReadOnlyFileSystem>(
				pair.Value.MountAtFolder,
				pair.Value.PathMapper,
				pair.Value.FileSystem
			) );
		}

		foreach( KeyValuePair<FolderId, MappedFileSystem<IReadOnlyFileSystem>> pair in readOnly ) {
			FolderId path = pair.Key;
			Node node = _tree.Insert( path.AsSpan() );

			if( !_readOnly.TryGetValue( node, out List<MappedFileSystem<IReadOnlyFileSystem>>? rofs ) ) {
				_readOnly[ node ] = [];
			}
			_readOnly[ node ].Add( pair.Value );
		}
		_root = new FolderIdentifier( _fileSystemId, _virtualPathHandler.Separator.ToString() );
		_unmappedRW = [];
		_unmappedRO = [];
	}

	Task<FileIdentifier> IFileSystem.CreateFileAsync(
		string fileName,
		Func<Stream, CancellationToken, Task> contentWriter,
		CancellationToken cancellationToken
	) {
		return ( this as IFileSystem ).CreateFileAsync(
			_root,
			fileName,
			contentWriter,
			cancellationToken
		);
	}

	Task<FileIdentifier> IFileSystem.CreateFileAsync(
		FolderIdentifier folderIdentifier,
		string fileName,
		Func<Stream, CancellationToken, Task> contentWriter,
		CancellationToken cancellationToken
	) {
		List<MappedFileSystem<IFileSystem>> lmfs = GetReadWrite( folderIdentifier );

		if( lmfs.Count == 1 ) {
			MappedFileSystem<IFileSystem> mfs = lmfs[ 0 ];
			if( !mfs.PathMapper.TryMapFromVirtual(
				folderIdentifier.FolderId,
				out FolderIdentifier mappedFolderIdentifier
			) ) {
				throw new InvalidPathException();
			}
			return mfs.FileSystem.CreateFileAsync(
				mappedFolderIdentifier,
				fileName,
				contentWriter,
				cancellationToken
			);
		}
		// There isn't a file system mounted at that path, so nothing to do
		throw new InvalidPathException();
	}

	FolderIdentifier IFileSystem.CreateFolder(
		FolderIdentifier folderIdentifier,
		string folderName
	) {
		List<MappedFileSystem<IFileSystem>> lmfs = GetReadWrite( folderIdentifier );
		if( lmfs.Count == 1 ) {
			MappedFileSystem<IFileSystem> mfs = lmfs[ 0 ];
			if( !mfs.PathMapper.TryMapFromVirtual(
				folderIdentifier.FolderId,
				out FolderIdentifier mappedFolderIdentifier
			) ) {
				throw new InvalidPathException();
			}

			FolderIdentifier newFolderIdentifier = mfs.FileSystem.CreateFolder(
				mappedFolderIdentifier,
				folderName
			);
			if( !mfs.PathMapper.TryMapToVirtual(
				newFolderIdentifier,
				out FolderId virtualFolderId
			) ) {
				throw new InvalidPathException();
			}

			return new FolderIdentifier( _fileSystemId, virtualFolderId );
		}
		// There isn't a file system mounted at that path, so nothing to do
		throw new InvalidPathException();
	}

	void IFileSystem.DeleteFolder(
		FolderIdentifier folderIdentifier
	) {
		List<MappedFileSystem<IFileSystem>> lmfs = GetReadWrite( folderIdentifier );
		if( lmfs.Count == 1 ) {
			MappedFileSystem<IFileSystem> mfs = lmfs[ 0 ];
			if( !mfs.PathMapper.TryMapFromVirtual(
				folderIdentifier.FolderId,
				out FolderIdentifier mappedFolderIdentifier
			) ) {
				throw new InvalidPathException();
			}

			mfs.FileSystem.DeleteFolder(
				mappedFolderIdentifier
			);
		}
		// There isn't a file system mounted at that path, so nothing to do
		throw new InvalidPathException();
	}

	Task IReadOnlyFileSystem.GetContentAsync(
		FileIdentifier fileIdentifier,
		Func<Stream, CancellationToken, Task> contentReader,
		CancellationToken cancellationToken
	) {
		List<MappedFileSystem<IFileSystem>> lmfs = GetReadWrite( fileIdentifier.FileId );
		if( lmfs.Count == 1 ) {
			MappedFileSystem<IFileSystem> mfs = lmfs[ 0 ];
			if( !mfs.PathMapper.TryMapFromVirtual(
				fileIdentifier.FileId,
				out FileIdentifier mappedFileIdentifier
			) ) {
				throw new InvalidPathException();
			}

			return mfs.FileSystem.GetContentAsync(
				mappedFileIdentifier,
				contentReader,
				cancellationToken
			);
		}
		// There isn't a file system mounted at that path, so nothing to do
		throw new InvalidPathException();
	}

	FileIdentifier IReadOnlyFileSystem.GetFileIdentifier(
		string fileName
	) {
		return ( this as IReadOnlyFileSystem ).GetFileIdentifier(
			_root,
			fileName
		);
	}

	FileIdentifier IReadOnlyFileSystem.GetFileIdentifier(
		FolderIdentifier folderIdentifier,
		string fileName
	) {
		List<MappedFileSystem<IReadOnlyFileSystem>> lmfs = GetReadOnly( folderIdentifier.FolderId );
		if (lmfs.Count == 1) {
			MappedFileSystem<IReadOnlyFileSystem> mfs = lmfs[ 0 ];
			if( !mfs.PathMapper.TryMapFromVirtual(
				folderIdentifier.FolderId,
				out FolderIdentifier mappedFolderIdentifier
			) ) {
				throw new InvalidPathException();
			}

			FileIdentifier fileIdentifier = mfs.FileSystem.GetFileIdentifier(
				mappedFolderIdentifier,
				fileName
			);

			if( !mfs.PathMapper.TryMapToVirtual(
				fileIdentifier,
				out FileId virtualFileId
			) ) {
				throw new InvalidPathException();
			}
			return new FileIdentifier( _fileSystemId, virtualFileId );
		}
		throw new PathNotFoundException();
	}

	IEnumerable<FileIdentifier> IReadOnlyFileSystem.GetFileIdentifiers(
	) {
		return ( this as IReadOnlyFileSystem ).GetFileIdentifiers( _root );
	}

	IEnumerable<FileIdentifier> IReadOnlyFileSystem.GetFileIdentifiers(
		FolderIdentifier folderIdentifier
	) {
		List<MappedFileSystem<IReadOnlyFileSystem>> lmfs = GetReadOnly( folderIdentifier.FolderId );
		if( lmfs.Count == 1 ) {
			MappedFileSystem<IReadOnlyFileSystem> mfs = lmfs[ 0 ];
			if( !mfs.PathMapper.TryMapFromVirtual(
				folderIdentifier.FolderId,
				out FolderIdentifier mappedFolderIdentifier
			) ) {
				throw new InvalidPathException();
			}

			IEnumerable<FileIdentifier> fileIdentifiers = mfs.FileSystem.GetFileIdentifiers(
				mappedFolderIdentifier
			);

			foreach( FileIdentifier fileIdentifier in fileIdentifiers ) {
				if( !mfs.PathMapper.TryMapToVirtual(
					fileIdentifier,
					out FileId virtualFileId
				) ) {
					throw new InvalidPathException();
				}
				yield return new FileIdentifier( _fileSystemId, virtualFileId );
			}
		}
		// There isn't a file system mounted at that path, so nothing to do
	}

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers() {
		return ( this as IReadOnlyFileSystem ).GetFolderIdentifiers( _root );
	}

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers(
		FolderIdentifier folderIdentifier
	) {
		List<MappedFileSystem<IFileSystem>> lmfs = GetReadWrite( folderIdentifier );
		if( lmfs.Count == 1 ) {
			MappedFileSystem<IFileSystem> mfs = lmfs[ 0 ];
			if( !mfs.PathMapper.TryMapFromVirtual(
				folderIdentifier.FolderId,
				out FolderIdentifier mappedFolderIdentifier
			) ) {
				throw new InvalidPathException();
			}

			IEnumerable<FolderIdentifier> folderIdentifiers = mfs.FileSystem.GetFolderIdentifiers( mappedFolderIdentifier );
			foreach( FolderIdentifier folder in folderIdentifiers ) {
				if( !mfs.PathMapper.TryMapToVirtual(
					folder,
					out FolderId virtualFolderId
				) ) {
					throw new InvalidPathException();
				}
				yield return new FolderIdentifier( _fileSystemId, virtualFolderId );
			}
			yield break;
		}
		Node? root = _tree.Find( folderIdentifier.FolderId.AsSpan() );
		if (root is null) {
			yield break;
		}
		foreach ( Node child in root.Children ) {
			yield return new FolderIdentifier(
				_fileSystemId,
				$"{folderIdentifier.FolderId}{child.Segment}{_virtualPathHandler.Separator}"
			);
		}
		/*
		IEnumerable<FolderIdentifier> fids = lmfs.Select( mfs => new FolderIdentifier( _fileSystemId, mfs.MountAtFolder ) );
		foreach( FolderIdentifier fid in fids ) {
			yield return fid;
		}
		*/
	}

	IEnumerable<char> IFileSystem.GetInvalidPathChars() {
		return _invalidPathChars;
	}

	IEnumerable<char> IFileSystem.GetInvalidFileNameChars() {
		return _invalidFileNameChars;
	}

	IEnumerable<string> IFileSystem.GetInvalidNames() {
		return _invalidNames;
	}

	FolderIdentifier IReadOnlyFileSystem.GetRoot() {
		return _root;
	}

	FolderIdentifier IReadOnlyFileSystem.GetFolderIdentifier(
		string folderName
	) {
		return ( this as IReadOnlyFileSystem ).GetFolderIdentifier(
			_root,
			folderName
		);
	}

	FolderIdentifier IReadOnlyFileSystem.GetFolderIdentifier(
		FolderIdentifier parentFolderIdentifier,
		string folderName
	) {
		FolderId folderId = _virtualPathHandler.Combine(
				parentFolderIdentifier.FolderId,
				folderName,
				_virtualPathHandler.Separator
			);

		List<MappedFileSystem<IReadOnlyFileSystem>> lmfs = GetReadOnly( folderId );
		if( lmfs.Count == 1 ) {
			// There exists a file system to handle this folder
			MappedFileSystem<IReadOnlyFileSystem> mfs = lmfs[ 0 ];
			if( !mfs.PathMapper.TryMapFromVirtual(
				folderId,
				out FolderIdentifier mappedFolderIdentifier
			) ) {
				throw new InvalidPathException();
			}
			return new FolderIdentifier( _fileSystemId, folderId );
		}
		// This is a pure virtual path
		IEnumerable<FolderIdentifier> fids = lmfs.Select( mfs => new FolderIdentifier( _fileSystemId, mfs.MountAtFolder ) );
		return fids.First( fid => fid.FolderId == folderId ) ?? throw new PathNotFoundException();
	}

	private List<MappedFileSystem<IFileSystem>> GetReadWrite(
		FolderIdentifier folderIdentifier
	) {
		Node node = _tree.Find( folderIdentifier.FolderId.AsSpan() );
		if( !_readWrite.TryGetValue( node, out List<MappedFileSystem<IFileSystem>>? fileSystem ) ) {
			return _unmappedRW;
		}
		return fileSystem;
	}

	private List<MappedFileSystem<IFileSystem>> GetReadWrite(
		FileId fileId
	) {
		ReadOnlySpan<char> path = fileId.AsSpan();
		ReadOnlySpan<char> virtualPath = path[ ..path.LastIndexOf( _virtualPathHandler.Separator ) ];
		Node node = _tree.Find( virtualPath );
		if( !_readWrite.TryGetValue( node, out List<MappedFileSystem<IFileSystem>>? fileSystem ) ) {
			return _unmappedRW;
		}
		return fileSystem;
	}

	private List<MappedFileSystem<IReadOnlyFileSystem>> GetReadOnly(
		FolderId folderId
	) {
		Node node = _tree.Find( folderId.AsSpan() );
		if( !_readOnly.TryGetValue( node, out List<MappedFileSystem<IReadOnlyFileSystem>>? fileSystem ) ) {
			return _unmappedRO;
		}
		return fileSystem;
	}

	/*
	private List<MappedFileSystem<IReadOnlyFileSystem>> GetReadOnly(
		FileIdentifier fileIdentifier
	) {
		ReadOnlySpan<char> path = fileIdentifier.FileId.AsSpan();
		ReadOnlySpan<char> virtualPath = path[ ..path.LastIndexOf( Separator ) ];
		Node node = _tree.Find( virtualPath );
		if( !_readOnly.TryGetValue( node, out List<MappedFileSystem<IReadOnlyFileSystem>>? fileSystem ) ) {
			throw new UnmappedFileSystemException();
		}
		return fileSystem;
	}
	*/
}
