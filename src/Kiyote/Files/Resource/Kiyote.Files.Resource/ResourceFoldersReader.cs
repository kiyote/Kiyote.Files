using Microsoft.Extensions.FileProviders;

namespace Kiyote.Files.Resource;

public sealed class ResourceFoldersReader : IFoldersReader {

	private readonly ConfiguredResourceFileSystem _config;
	private readonly FolderId _rootFolder;
	private readonly ManifestEmbeddedFileProvider? _provider;

	public ResourceFoldersReader(
		ConfiguredResourceFileSystem config
	) {
		ArgumentNullException.ThrowIfNull( config );
		_config = config;
		try {
			_provider = new ManifestEmbeddedFileProvider( config.Assembly );
			_rootFolder = new FolderId( config.FileSystemId, "\\" );
		} catch( InvalidOperationException ) {
			_provider = null;
			_rootFolder = new FolderId( config.FileSystemId, "" );
		}
	}

	string IFoldersReader.FileSystemId => _config.FileSystemId;

	FolderId IFoldersReader.Root => _rootFolder;

	IEnumerable<FileId> IFoldersReader.GetFilesInFolder(
		FolderId folderId
	) {
		if( _provider is null ) {
			string assembly = _config.Assembly.GetName().Name ?? "";
			string[] names = _config.Assembly.GetManifestResourceNames();
			foreach( string name in names ) {
				yield return _config.ToFileId( folderId, name[ ( assembly.Length + 1 ).. ] );
			}
			yield break;
		}

		IDirectoryContents directoryContents = _provider.GetDirectoryContents( folderId.Id );
		foreach( IFileInfo? info in directoryContents ) {
			if( info is null ) {
				continue;
			}
			if( !info.IsDirectory ) {
				yield return _config.ToFileId( folderId, info.Name );

			}
		}
	}

	IEnumerable<FolderId> IFoldersReader.GetFoldersInFolder(
		FolderId folderId
	) {
		if( _provider is null ) {
			yield break;
		}

		IDirectoryContents directoryContents = _provider.GetDirectoryContents( folderId.Id );
		foreach( IFileInfo? info in directoryContents ) {
			if( info?.IsDirectory ?? false ) {
				yield return _config.ToFolderId( folderId, info.Name );

			}
		}
	}
}
