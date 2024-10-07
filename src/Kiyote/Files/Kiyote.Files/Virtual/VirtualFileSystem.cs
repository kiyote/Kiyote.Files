namespace Kiyote.Files.Virtual;

internal sealed class VirtualFileSystem : IFileSystem {

	public const char Separator = '/';

	private readonly FileSystemId _fileSystemId;
	private readonly FileSystemTree _tree;
	private readonly Dictionary<Node, List<MappedFileSystem<IFileSystem>>> _readWrite;
	private readonly Dictionary<Node, List<MappedFileSystem<IReadOnlyFileSystem>>> _readOnly;
	private readonly char[] _invalidPathChars;
	private readonly char[] _invalidFileNameChars;
	private readonly string[] _invalidNames;
	private readonly FolderIdentifier _root;
	private readonly IVirtualPathHandler _virtualPathHandler;

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

		_tree = new FileSystemTree();
		_readWrite = [];
		_readOnly = [];
		foreach( KeyValuePair<FolderId, MappedFileSystem<IFileSystem>> pair in readWrite ) {
			string path = pair.Key;
			Node node = _tree.Insert( path );

			if( !_readWrite.TryGetValue( node, out List<MappedFileSystem<IFileSystem>>? rwfs ) ) {
				_readWrite[ node ] = [];
			}
			_readWrite[ node ].Add( pair.Value );

			if( !_readOnly.TryGetValue( node, out List<MappedFileSystem<IReadOnlyFileSystem>>? rofs ) ) {
				_readOnly[ node ] = [];
			}
			_readOnly[ node ].Add( new MappedFileSystem<IReadOnlyFileSystem>(
				pair.Value.VirtualRoot,
				pair.Value.PathMapper,
				pair.Value.FileSystem
			) );
		}

		foreach( KeyValuePair<FolderId, MappedFileSystem<IReadOnlyFileSystem>> pair in readOnly ) {
			string path = pair.Key;
			Node node = _tree.Insert( path );

			if( !_readOnly.TryGetValue( node, out List<MappedFileSystem<IReadOnlyFileSystem>>? rofs ) ) {
				_readOnly[ node ] = [];
			}
			_readOnly[ node ].Add( pair.Value );
		}

		_root = new FolderIdentifier(
			_fileSystemId,
			_virtualPathHandler.GetCommonParent(
				readOnly
					.Select( ro => ro.Key )
					.Union(
						readWrite
							.Select( rw => rw.Key )
					).ToArray()
			)
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
				folderIdentifier,
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
				folderIdentifier,
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
				out FolderIdentifier virtualFolderIdentifier
			) ) {
				throw new InvalidPathException();
			}

			return virtualFolderIdentifier;
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
				folderIdentifier,
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
		List<MappedFileSystem<IFileSystem>> lmfs = GetReadWrite( fileIdentifier );
		if( lmfs.Count == 1 ) {
			MappedFileSystem<IFileSystem> mfs = lmfs[ 0 ];
			if( !mfs.PathMapper.TryMapFromVirtual(
				fileIdentifier,
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

	IEnumerable<FileIdentifier> IReadOnlyFileSystem.GetFileIdentifiers(
		FolderIdentifier folderIdentifier
	) {
		List<MappedFileSystem<IReadOnlyFileSystem>> lmfs = GetReadOnly( folderIdentifier );
		if( lmfs.Count == 1 ) {
			MappedFileSystem<IReadOnlyFileSystem> mfs = lmfs[ 0 ];
			if( !mfs.PathMapper.TryMapFromVirtual(
				folderIdentifier,
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
					out FileIdentifier virtualFileIdentifier
				) ) {
					throw new InvalidPathException();
				}
				yield return virtualFileIdentifier;
			}
		}
		// There isn't a file system mounted at that path, so nothing to do
	}

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers() {
		List<MappedFileSystem<IReadOnlyFileSystem>> lmfs = GetReadOnly( _root );
		if( lmfs.Count == 1 ) {
			MappedFileSystem<IReadOnlyFileSystem> mfs = lmfs[ 0 ];
			if( !mfs.PathMapper.TryMapFromVirtual(
				_root,
				out FolderIdentifier mappedFolderIdentifier
			) ) {
				throw new InvalidPathException();
			}

			IEnumerable<FolderIdentifier> folderIdentifiers = mfs.FileSystem.GetFolderIdentifiers(
				mappedFolderIdentifier
			);

			foreach( FolderIdentifier folderIdentifier in folderIdentifiers ) {
				if( !mfs.PathMapper.TryMapToVirtual(
					folderIdentifier,
					out FolderIdentifier virtualFolderIdentifier
				) ) {
					throw new InvalidPathException();
				}
				yield return virtualFolderIdentifier;
			}
			yield break;
		}
		IEnumerable<FolderIdentifier> fids = lmfs.Select( mfs => new FolderIdentifier( _fileSystemId, mfs.VirtualRoot ) );
		foreach( FolderIdentifier fid in fids ) {
			yield return fid;
		}
	}

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers(
		FolderIdentifier folderIdentifier
	) {
		List<MappedFileSystem<IFileSystem>> lmfs = GetReadWrite( folderIdentifier );
		if( lmfs.Count == 1 ) {
			MappedFileSystem<IFileSystem> mfs = lmfs[ 0 ];
			if( !mfs.PathMapper.TryMapFromVirtual(
				folderIdentifier,
				out FolderIdentifier mappedFolderIdentifier
			) ) {
				throw new InvalidPathException();
			}

			IEnumerable<FolderIdentifier> folderIdentifiers = mfs.FileSystem.GetFolderIdentifiers( mappedFolderIdentifier );
			foreach( FolderIdentifier folder in folderIdentifiers ) {
				if( !mfs.PathMapper.TryMapToVirtual(
					folder,
					out FolderIdentifier virtualFolderIdentifier
				) ) {
					throw new InvalidPathException();
				}
				yield return virtualFolderIdentifier;
			}
			yield break;
		}
		IEnumerable<FolderIdentifier> fids = lmfs.Select( mfs => new FolderIdentifier( _fileSystemId, mfs.VirtualRoot ) );
		foreach( FolderIdentifier fid in fids) {
			yield return fid;
		}
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
		FolderIdentifier target = new FolderIdentifier( _fileSystemId, $"{_root.FolderId}/{folderName}/" );
		List<MappedFileSystem<IReadOnlyFileSystem>> lmfs = GetReadOnly( target );
		if( lmfs.Count == 1 ) {
			// There exists a file system to handle this folder
			MappedFileSystem<IReadOnlyFileSystem> mfs = lmfs[ 0 ];
			if( !mfs.PathMapper.TryMapFromVirtual(
				_root,
				out FolderIdentifier mappedFolderIdentifier
			) ) {
				throw new InvalidPathException();
			}
			return mfs.FileSystem.GetFolderIdentifier( folderName );
		}
		// This is a pure virtual path
		IEnumerable<FolderIdentifier> fids = lmfs.Select( mfs => new FolderIdentifier( _fileSystemId, mfs.VirtualRoot ) );
		return fids.First( fid => fid.FolderId == target.FolderId ) ?? throw new FolderNotFoundException();
	}

	private List<MappedFileSystem<IFileSystem>> GetReadWrite(
		FolderIdentifier folderIdentifier
	) {
		Node node = _tree.Find( folderIdentifier.FolderId.AsSpan() );
		if( !_readWrite.TryGetValue( node, out List<MappedFileSystem<IFileSystem>>? fileSystem ) ) {
			throw new UnmappedFileSystemException();
		}
		return fileSystem;
	}

	private List<MappedFileSystem<IFileSystem>> GetReadWrite(
		FileIdentifier fileIdentifier
	) {
		ReadOnlySpan<char> path = fileIdentifier.FileId.AsSpan();
		ReadOnlySpan<char> virtualPath = path[ ..path.LastIndexOf( Separator ) ];
		Node node = _tree.Find( virtualPath );
		if( !_readWrite.TryGetValue( node, out List<MappedFileSystem<IFileSystem>>? fileSystem ) ) {
			throw new UnmappedFileSystemException();
		}
		return fileSystem;
	}

	private List<MappedFileSystem<IReadOnlyFileSystem>> GetReadOnly(
		FolderIdentifier folderIdentifier
	) {
		Node node = _tree.Find( folderIdentifier.FolderId.AsSpan() );
		if( !_readOnly.TryGetValue( node, out List<MappedFileSystem<IReadOnlyFileSystem>>? fileSystem ) ) {
			throw new UnmappedFileSystemException();
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
