using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace Kiyote.Files.Resource;

internal sealed class ResourceFileSystem : IReadOnlyFileSystem {

	private readonly string _fileSystemId;
	private readonly Assembly _assembly;
	private readonly FolderId _rootFolder;
	private readonly ManifestEmbeddedFileProvider? _provider;

	public ResourceFileSystem(
		Assembly assembly,
		string fileSystemId
	) {
		_assembly = assembly;
		_fileSystemId = fileSystemId;
		try {
			_provider = new ManifestEmbeddedFileProvider( _assembly );
			_rootFolder = new FolderId( fileSystemId, "\\" );
		} catch( InvalidOperationException ) {
			_provider = null;
			_rootFolder = new FolderId( fileSystemId, "" );
		}
	}

	string IFileSystemIdentifier.Id => _fileSystemId;

	FolderId IReadOnlyFileSystem.Root => _rootFolder;

	async Task<TFileContent> IReadOnlyFileSystem.GetContentAsync<TFileContent>(
		FileId fileId,
		Func<Stream, CancellationToken, Task<TFileContent>> contentReader,
		CancellationToken cancellationToken
	) {
		using Stream? stream = _assembly.GetManifestResourceStream( fileId.Id ) ?? throw new FileNotFoundException();
		return await contentReader( stream, cancellationToken );
	}

	IEnumerable<FileId> IReadOnlyFileSystem.GetFilesInFolder(
		FolderId folderId
	) {
		if( _provider is null ) {
			string assembly = _assembly.GetName().Name ?? "";
			string[] names = _assembly.GetManifestResourceNames();
			foreach( string name in names ) {
				yield return ToFileId( folderId, name[ ( assembly.Length + 1 ).. ] );
			}
			yield break;
		}

		IDirectoryContents directoryContents = _provider.GetDirectoryContents( folderId.Id );
		foreach( IFileInfo? info in directoryContents ) {
			if( info is null ) {
				continue;
			}
			if( !info.IsDirectory ) {
				yield return ToFileId( folderId, info.Name );

			}
		}
	}

	IEnumerable<FolderId> IReadOnlyFileSystem.GetFoldersInFolder(
		FolderId folderId
	) {
		if( _provider is null ) {
			yield break;
		}

		IDirectoryContents directoryContents = _provider.GetDirectoryContents( folderId.Id );
		foreach( IFileInfo? info in directoryContents ) {
			if( info?.IsDirectory ?? false ) {
				yield return ToFolderId( folderId, info.Name );

			}
		}
	}

	Task<FileMetadata> IReadOnlyFileSystem.GetMetadataAsync(
		FileId fileId,
		CancellationToken cancellationToken
	) {
		throw new NotImplementedException();
	}

	private FileId ToFileId(
		FolderId folderId,
		string name
	) {
		return new FileId( _fileSystemId, $"{folderId.Id}{name}" );
	}

	private FolderId ToFolderId(
		FolderId folderId,
		string name
	) {
		return new FolderId( _fileSystemId, $"{folderId.Id}{name}\\" );
	}
}
