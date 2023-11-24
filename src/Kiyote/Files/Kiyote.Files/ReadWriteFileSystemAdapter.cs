
namespace Kiyote.Files;

internal sealed class ReadWriteFileSystemAdapter<T> : IReadWriteFileSystem<T> {

	private readonly IReadWriteFileSystem _fileSystem;

	public ReadWriteFileSystemAdapter(
		IReadWriteFileSystem fileSystem
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

	Task<FileId> IReadWriteFileSystem.PutContentAsync(
		Func<Stream, CancellationToken, Task> asyncWriter,
		CancellationToken cancellationToken
	) {
		return _fileSystem.PutContentAsync( asyncWriter, cancellationToken );
	}

	Task<TFileContent> IReadOnlyFileSystem.GetContentAsync<TFileContent>(
		FileId fileId,
		Func<Stream, CancellationToken, Task<TFileContent>> contentReader,
		CancellationToken cancellationToken
	) {
		return _fileSystem.GetContentAsync( fileId, contentReader, cancellationToken );
	}

	Task IReadWriteFileSystem.RenameFileAsync(
		FileId fileId,
		string name,
		CancellationToken cancellationToken
	) {
		return _fileSystem.RenameFileAsync( fileId, name, cancellationToken );
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
