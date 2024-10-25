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

	async Task IReadOnlyFileSystem.GetContentAsync(
		FileIdentifier fileIdentifier,
		Func<Stream, CancellationToken, Task> contentReader,
		CancellationToken cancellationToken
	) {
		if( _provider is null ) {
			using Stream stream = _assembly.GetManifestResourceStream( fileIdentifier.FileId.ToString()[ 1.. ] ) ?? throw new ContentUnavailableException();
			await contentReader( stream, cancellationToken );
			return;
		} else {
			string path = ToPath( fileIdentifier.FileId );
			IFileInfo fileInfo = _provider.GetFileInfo( path );
			using Stream stream = fileInfo.CreateReadStream();
			await contentReader( stream, cancellationToken );
		}
	}

	FileIdentifier IReadOnlyFileSystem.GetFileIdentifier(
		string fileName
	) {
		return ( this as IReadOnlyFileSystem ).GetFileIdentifier( _root, fileName );
	}

	FileIdentifier IReadOnlyFileSystem.GetFileIdentifier(
		FolderIdentifier folderIdentifier,
		string fileName
	) {
		if( _provider is null ) {

			if( folderIdentifier != _root ) {
				_logger?.GetFlatFolderFileIdentifier( folderIdentifier, fileName );
				throw new PathNotFoundException();
			}
			foreach( string resourceName in _assembly.GetManifestResourceNames() ) {
				if( resourceName.Equals( fileName, StringComparison.Ordinal ) ) {
					return new FileIdentifier(
						FileSystemId,
						ToFileId( folderIdentifier.FolderId, resourceName )
					);
				}
			}
			_logger?.MissingFileIdentifier( folderIdentifier, fileName );
			throw new PathNotFoundException();

		} else {

			string path = ToPath( folderIdentifier.FolderId );
			IDirectoryContents contents = _provider.GetDirectoryContents( path );
			foreach( IFileInfo item in contents ) {
				if( item.IsDirectory ) {
					continue;
				}
				if( item.Name.Equals( fileName, StringComparison.Ordinal ) ) {
					return new FileIdentifier(
						FileSystemId,
						ToFileId( folderIdentifier.FolderId, item.Name )
					);
				}
			}
			_logger?.MissingFileIdentifier( folderIdentifier, fileName );
			throw new PathNotFoundException();
		}
	}

	IEnumerable<FileIdentifier> IReadOnlyFileSystem.GetFileIdentifiers(
	) {
		return ( this as IReadOnlyFileSystem ).GetFileIdentifiers( _root );
	}

	IEnumerable<FileIdentifier> IReadOnlyFileSystem.GetFileIdentifiers(
		FolderIdentifier folderIdentifier
	) {
		if( _provider is null ) {

			if( folderIdentifier != _root ) {
				_logger?.GetFlatFileIdentifiers( folderIdentifier );
				throw new PathNotFoundException();
			}
			foreach( string resourceName in _assembly.GetManifestResourceNames() ) {
				yield return new FileIdentifier(
					FileSystemId,
					ToFileId( folderIdentifier.FolderId, resourceName )
				);
			}
			yield break;

		} else {

			string path = ToPath( folderIdentifier.FolderId );
			IDirectoryContents contents = _provider.GetDirectoryContents( path );
			foreach( IFileInfo item in contents ) {
				if( item.IsDirectory ) {
					continue;
				}
				yield return new FileIdentifier(
					FileSystemId,
					ToFileId( folderIdentifier.FolderId, item.Name )
				);
			}

		}
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

		} else {

			string folder = ToPath( folderIdentifier.FolderId );

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
			|| string.Equals( folderName, _root.FolderId, StringComparison.Ordinal )
		) {
			return _root;
		}
		if( _provider is null ) {
			_logger?.GetFlatFolder( folderName );
			throw new PathNotFoundException();
		}

		FolderId folderId = ToFolderId( parentFolderIdentifier.FolderId, folderName );
		string folder = ToPath( folderId );
		IDirectoryContents contents = _provider.GetDirectoryContents( folder );
		if( contents is NotFoundDirectoryContents ) {
			throw new PathNotFoundException();
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

	internal string ToPath(
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

	internal string ToPath(
		FileId fileId
	) {
		string path = $"{_root.FolderId.AsSpan()[..^1]}{fileId}";

		return path;
	}
}
