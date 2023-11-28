namespace Kiyote.Files.Resource;

internal sealed class ResourceFileSystem : IReadOnlyFileSystem {

	private readonly IFilesReader _filesReader;
	private readonly IFoldersReader _foldersReader;

	public ResourceFileSystem(
		IFilesReader filesReader,
		IFoldersReader foldersReader
	) {
		_filesReader = filesReader;
		_foldersReader = foldersReader;
	}

	FolderId IFoldersReader.Root => _foldersReader.Root;

	string IFilesReader.FileSystemId => _filesReader.FileSystemId;

	string IFoldersReader.FileSystemId => _foldersReader.FileSystemId;

	Task<TFileContent> IFilesReader.GetContentAsync<TFileContent>(
		FileId fileId,
		Func<Stream, CancellationToken, Task<TFileContent>> contentReader,
		CancellationToken cancellationToken
	) {
		return _filesReader.GetContentAsync( fileId, contentReader, cancellationToken );
	}

	IEnumerable<FileId> IFoldersReader.GetFilesInFolder(
		FolderId folderId
	) {
		return _foldersReader.GetFilesInFolder( folderId );
	}

	IEnumerable<FolderId> IFoldersReader.GetFoldersInFolder(
		FolderId folderId
	) {
		return _foldersReader.GetFoldersInFolder( folderId );
	}

	Task<FileMetadata> IFilesReader.GetMetadataAsync(
		FileId fileId,
		CancellationToken cancellationToken
	) {
		return _filesReader.GetMetadataAsync( fileId, cancellationToken );
	}
}
