
namespace Kiyote.Files;

internal sealed class ReadOnlyFileSystemAdapter<T> : IReadOnlyFileSystem<T> {

	private readonly IReadOnlyFileSystem _fileSystem;


	public ReadOnlyFileSystemAdapter(
		IReadOnlyFileSystem fileSystem
	) {
		_fileSystem = fileSystem;
	}
	string IFileSystemIdentifier.Id => _fileSystem.Id;

	FolderId IReadOnlyFileSystem.Root => _fileSystem.Root;

	Task<FileMetadata> IReadOnlyFileSystem.GetMetadataAsync(
		FileId fileId,
		CancellationToken cancellationToken
	) {
		return _fileSystem.GetMetadataAsync( fileId, cancellationToken );
	}

	Task<TFileContent> IReadOnlyFileSystem.GetContentAsync<TFileContent>(
		FileId fileId,
		Func<Stream, CancellationToken, Task<TFileContent>> contentReader,
		CancellationToken cancellationToken
	) {
		return _fileSystem.GetContentAsync( fileId, contentReader, cancellationToken );
	}

	IEnumerable<FileId> IReadOnlyFileSystem.GetFilesInFolder(
		FolderId folderId
	) {
		return _fileSystem.GetFilesInFolder( folderId );
	}

	IEnumerable<FolderId> IReadOnlyFileSystem.GetFoldersInFolder(
		FolderId folderId
	) {
		return _fileSystem.GetFoldersInFolder( folderId );
	}
}
