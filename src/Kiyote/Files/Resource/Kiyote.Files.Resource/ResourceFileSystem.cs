using Microsoft.Extensions.FileProviders;

namespace Kiyote.Files.Resource;

internal sealed class ResourceFileSystem : IReadOnlyFileSystem {

	private readonly Assembly _assembly;
	private readonly ManifestEmbeddedFileProvider? _provider;
	private readonly FolderIdentifier _root;
	private readonly ILogger<ResourceFileSystem>? _logger;
	private readonly FileSystemId _fileSystemId;

	/// <summary>
	/// Creates a new instance of the <c>ResourceFileSystem</c> class.
	/// </summary>
	/// <param name="logger">Provides the logging mechanism for the instance.</param>
	/// <param name="fileSystemId">The identifier used to disambiguate this file system from other file systems.</param>
	/// <param name="assembly">The .net assembly containing the resources to be accessed.</param>
	/// <param name="manifestRootFolder">Use to indicate what folder the resources should be accessed from if the assembly contains a manifest provider.</param>
	/// <exception cref="ArgumentException">Thrown if a root folder is specified for a non-manifest resource assembly.</exception>
	public ResourceFileSystem(
		ILogger<ResourceFileSystem>? logger,
		FileSystemId fileSystemId,
		Assembly assembly,
		string manifestRootFolder
	) {
		ArgumentNullException.ThrowIfNull( assembly, nameof( assembly ) );
		ArgumentNullException.ThrowIfNull( manifestRootFolder, nameof( manifestRootFolder ) );
		_logger = logger;
		_assembly = assembly;
		Separator = Path.DirectorySeparatorChar;

		string prefix;
		try {
			_provider = new ManifestEmbeddedFileProvider( assembly );
			if( string.IsNullOrWhiteSpace( manifestRootFolder ) ) {
				prefix = Separator.ToString();
			} else {
				prefix = manifestRootFolder;
			}
			if( !prefix.StartsWith( Separator ) ) {
				prefix = Separator + prefix;
			}
			if( !prefix.EndsWith( Separator ) ) {
				prefix += Separator;
			}
		} catch( InvalidOperationException ) {
			_provider = null;
			if( !string.IsNullOrWhiteSpace( manifestRootFolder ) ) {
				throw new ArgumentException( $"{nameof( manifestRootFolder )} can not be used on a non-manifest resource assembly." );
			}
			prefix = "";
		}

		if( _provider is null ) {
			_logger?.FlatResourceAssembly( assembly.GetName().FullName );
		} else {
			_logger?.ManifestResourceAssembly( assembly.GetName().FullName );
		}

		FileSystemId = fileSystemId;
		_root = new FolderIdentifier( fileSystemId, prefix );
		_fileSystemId = fileSystemId;
	}

	internal FileSystemId FileSystemId { get; }

	internal char Separator { get; }

	Task IReadOnlyFileSystem.GetContentAsync(
		FileIdentifier fileId,
		Func<Stream, CancellationToken, Task> contentReader,
		CancellationToken cancellationToken
	) {
		throw new NotImplementedException();
	}

	IEnumerable<FileIdentifier> IReadOnlyFileSystem.GetFileIdentifiers(
		FolderIdentifier folderIdentifier
	) {
		if( _provider is null ) {
			_logger?.GetFlatFileIdentifiers();
			foreach( string resourceName in _assembly.GetManifestResourceNames() ) {
				yield return new FileIdentifier(
					FileSystemId,
					ToFileId( folderIdentifier.FolderId, resourceName )
				);
			}
			yield break;
		}
		throw new NotImplementedException();
	}

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers() {
		return ( this as IReadOnlyFileSystem ).GetFolderIdentifiers( _root );
	}

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers(
		FolderIdentifier folderIdentifier
	) {
		if( _provider is null ) {
			_logger?.GetFlatFolderIdentifiers();
			yield break;
		}

		string folder = ToFolder( folderIdentifier.FolderId );

		IDirectoryContents directoryContents = _provider.GetDirectoryContents( folder );
		foreach( IFileInfo? info in directoryContents ) {
			if( info?.IsDirectory ?? false ) {
				yield return new FolderIdentifier(
					FileSystemId,
					ToFolderId( folderIdentifier.FolderId, info.Name )
				);
			}
		}
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
		if( string.IsNullOrWhiteSpace( folderName )
			|| string.Equals( folderName,  _root.FolderId, StringComparison.Ordinal )
		) {
			return _root;
		}
		if( _provider is null ) {
			_logger?.GetFlatFolder( folderName );
			throw new FolderNotFoundException();
		}

		FolderId folderId = ToFolderId( parentFolderIdentifier.FolderId, folderName );
		string folder = ToFolder( folderId );
		IDirectoryContents contents = _provider.GetDirectoryContents( folder );
		if (contents is NotFoundDirectoryContents) {
			throw new FolderNotFoundException();
		}

		return new FolderIdentifier(
			_fileSystemId,
			folderId
		);
	}

	internal FolderId ToFolderId(
		FolderId folderId,
		string folderName
	) {
		if( folderId == _root.FolderId ) {
			return $"{Separator}{folderName}{Separator}";
		}
		return $"{folderId}{folderName}{Separator}";
	}

	internal FileId ToFileId(
		FolderId folderId,
		string fileName
	) {
		if( folderId == _root.FolderId ) {
			return $"{Separator}{fileName}";
		}
		return $"{folderId}{fileName}";
	}

	internal string ToFolder(
		FolderId folderId
	) {
		string folder = folderId;
		if( _root != FolderIdentifier.None
			&& folderId != _root.FolderId
		) {
			folder = $"{_root.FolderId.AsSpan()[ ..^1 ]}{folder}";
		}

		return folder;
	}
}
