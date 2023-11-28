
namespace Kiyote.Files;

internal sealed class ReadWriteFileSystemAdapter<T> : IReadWriteFileSystem<T> {

	private readonly IReadWriteFileSystem _fileSystem;

	public ReadWriteFileSystemAdapter(
		IReadWriteFileSystem fileSystem
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

	Task<FileId> IFilesWriter.PutContentAsync(
		Func<Stream, CancellationToken, Task> asyncWriter,
		CancellationToken cancellationToken
	) {
		return _fileSystem.PutContentAsync( asyncWriter, cancellationToken );
	}

	Task<TFileContent> IFilesReader.GetContentAsync<TFileContent>(
		FileId fileId,
		Func<Stream, CancellationToken, Task<TFileContent>> contentReader,
		CancellationToken cancellationToken
	) {
		return _fileSystem.GetContentAsync( fileId, contentReader, cancellationToken );
	}

	Task IFilesWriter.RenameFileAsync(
		FileId fileId,
		string name,
		CancellationToken cancellationToken
	) {
		return _fileSystem.RenameFileAsync( fileId, name, cancellationToken );
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
