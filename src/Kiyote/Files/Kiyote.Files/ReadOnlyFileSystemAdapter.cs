namespace Kiyote.Files;

internal sealed class ReadOnlyFileSystemAdapter<T> : IReadOnlyFileSystem<T> {

	private readonly IReadOnlyFileSystem _fileSystem;

	public ReadOnlyFileSystemAdapter(
		IReadOnlyFileSystem fileSystem
	) {
		_fileSystem = fileSystem;
	}

	FolderId IFoldersReader.Root => _fileSystem.Root;

	string IFileSystemIdentifier.FileSystemId => _fileSystem.FileSystemId;

	Task<FileMetadata> IFilesReader.GetMetadataAsync(
		FileId fileId,
		CancellationToken cancellationToken
	) {
		return _fileSystem.GetMetadataAsync( fileId, cancellationToken );
	}

	Task<TFileContent> IFilesReader.GetContentAsync<TFileContent>(
		FileId fileId,
		Func<Stream, CancellationToken, Task<TFileContent>> contentReader,
		CancellationToken cancellationToken
	) {
		return _fileSystem.GetContentAsync( fileId, contentReader, cancellationToken );
	}

	IEnumerable<FileId> IFoldersReader.GetFilesInFolder(
		FolderId folderId
	) {
		return _fileSystem.GetFilesInFolder( folderId );
	}

	IEnumerable<FolderId> IFoldersReader.GetFoldersInFolder(
		FolderId folderId
	) {
		return _fileSystem.GetFoldersInFolder( folderId );
	}
}
