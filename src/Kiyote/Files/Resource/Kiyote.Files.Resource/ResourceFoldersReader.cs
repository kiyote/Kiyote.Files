using Microsoft.Extensions.FileProviders;

namespace Kiyote.Files.Resource;

public sealed class ResourceFoldersReader : IFoldersReader {

	private readonly ResourceFileSystemConfiguration _config;
	private readonly FolderId _rootFolder;
	private readonly ManifestEmbeddedFileProvider? _provider;

	public ResourceFoldersReader(
		ResourceFileSystemConfiguration config
	) {
		ArgumentNullException.ThrowIfNull( config );
		_config = config;
		string prefix;
		try {
			_provider = new ManifestEmbeddedFileProvider( config.Assembly );
			prefix = "\\";
		} catch( InvalidOperationException ) {
			_provider = null;
			prefix = "";
		}
		_rootFolder = new FolderId( config.FileSystemId, prefix );
	}

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
