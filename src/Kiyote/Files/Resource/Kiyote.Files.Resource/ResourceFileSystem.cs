using Microsoft.Extensions.FileProviders;

namespace Kiyote.Files.Resource;

internal sealed class ResourceFileSystem : IReadOnlyFileSystem {

	private readonly FileSystemId _fileSystemId;
	private readonly ManifestEmbeddedFileProvider? _provider;
	private readonly FolderIdentifier _root;
	private readonly ILogger<ResourceFileSystem> _logger;

	public ResourceFileSystem(
		ILogger<ResourceFileSystem> logger,
		FileSystemId fileSystemId,
		Assembly assembly,
		string rootFolder
	) {
		ArgumentNullException.ThrowIfNull( assembly );
		ArgumentNullException.ThrowIfNull( rootFolder );
		ArgumentNullException.ThrowIfNull( logger );
		_logger = logger;

		string prefix;
		try {
			_provider = new ManifestEmbeddedFileProvider( assembly );
			if( string.IsNullOrWhiteSpace( rootFolder ) ) {
				prefix = "\\";
			} else {
				prefix = rootFolder;
			}
			if( !prefix.StartsWith( '\\' ) ) {
				prefix = "\\" + prefix;
			}
			if( !prefix.EndsWith( '\\' ) ) {
				prefix += "\\";
			}
		} catch( InvalidOperationException ) {
			_provider = null;
			prefix = "";
		}

		if (_provider is null) {
			_logger.FlatResourceAssembly( assembly.GetName().FullName );
		} else {
			_logger.ManifestResourceAssembly( assembly.GetName().FullName );
		}

		_fileSystemId = fileSystemId;
		_root = new FolderIdentifier( fileSystemId, prefix );
	}

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
		throw new NotImplementedException();
	}

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers() {
		return ( this as IReadOnlyFileSystem ).GetFolderIdentifiers( _root );
	}

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers(
		FolderIdentifier folderIdentifier
	) {
		if( _provider is null ) {
			_logger.GetFlatFolderIdentifiers();
			yield break;
		}

		IDirectoryContents directoryContents = _provider.GetDirectoryContents( folderIdentifier.FolderId );
		foreach( IFileInfo? info in directoryContents ) {
			if( info?.IsDirectory ?? false ) {
				yield return new FolderIdentifier(
					_fileSystemId,
					ToFolderId( folderIdentifier.FolderId, info.Name )
				);

			}
		}
	}

	FolderIdentifier IReadOnlyFileSystem.GetRoot() {
		return _root;
	}

	internal static FolderId ToFolderId(
		FolderId folderId,
		string folderName
	) {
		return new FolderId( $"{folderId}{folderName}\\" );
	}
}
