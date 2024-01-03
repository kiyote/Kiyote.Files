using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace Kiyote.Files.Resource;

internal sealed class ResourceFileSystem : IReadOnlyFileSystem {

	private readonly FileSystemId _fileSystemId;
	private readonly ManifestEmbeddedFileProvider? _provider;
	private readonly FolderIdentifier _root;

	public ResourceFileSystem(
		FileSystemId fileSystemId,
		Assembly assembly
	) : this( fileSystemId, assembly, "" ) {
	}

	public ResourceFileSystem(
		FileSystemId fileSystemId,
		Assembly assembly,
		string rootFolder
	) {
		ArgumentNullException.ThrowIfNull( assembly );
		ArgumentNullException.ThrowIfNull( rootFolder );

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
