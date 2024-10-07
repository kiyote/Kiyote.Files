using Microsoft.Extensions.FileProviders;

namespace Kiyote.Files.Resource;

internal sealed class ResourceFileSystem : IReadOnlyFileSystem {

	private readonly Assembly _assembly;
	private readonly ManifestEmbeddedFileProvider? _provider;
	private readonly FolderIdentifier _root;
	private readonly ILogger<ResourceFileSystem>? _logger;

	public ResourceFileSystem(
		ILogger<ResourceFileSystem>? logger,
		FileSystemId fileSystemId,
		Assembly assembly,
		string rootFolder
	) {
		ArgumentNullException.ThrowIfNull( assembly, nameof( assembly ) );
		ArgumentNullException.ThrowIfNull( rootFolder, nameof( rootFolder ) );
		_logger = logger;
		_assembly = assembly;
		Separator = Path.DirectorySeparatorChar;

		string prefix;
		try {
			_provider = new ManifestEmbeddedFileProvider( assembly );
			if( string.IsNullOrWhiteSpace( rootFolder ) ) {
				prefix = Separator.ToString();
			} else {
				prefix = rootFolder;
			}
			if( !prefix.StartsWith( Separator ) ) {
				prefix = Separator + prefix;
			}
			if( !prefix.EndsWith( Separator ) ) {
				prefix += Separator;
			}
		} catch( InvalidOperationException ) {
			_provider = null;
			prefix = "";
		}

		if( _provider is null ) {
			_logger?.FlatResourceAssembly( assembly.GetName().FullName );
		} else {
			_logger?.ManifestResourceAssembly( assembly.GetName().FullName );
		}

		FileSystemId = fileSystemId;
		_root = new FolderIdentifier( fileSystemId, prefix );
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
			foreach( string resourceName in _assembly.GetManifestResourceNames()) {
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

		IDirectoryContents directoryContents = _provider.GetDirectoryContents( folderIdentifier.FolderId );
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
		if( _provider is null ) {
			_logger?.GetFlatFolder( folderName );
			throw new NotImplementedException();
		}

		throw new NotImplementedException();
	}

	internal FolderId ToFolderId(
		FolderId folderId,
		string folderName
	) {
		if( folderId == _root.FolderId ) {
			return $"{folderName}{Separator}";
		}
		return $"{folderId}{folderName}{Separator}";
	}

	internal FileId ToFileId(
		FolderId folderId,
		string fileName
	) {
		if( folderId == _root.FolderId ) {
			return $"{fileName}";
		}
		return $"{folderId}{fileName}";
	}
}
