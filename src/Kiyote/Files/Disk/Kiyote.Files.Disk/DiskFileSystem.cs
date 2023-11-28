namespace Kiyote.Files.Disk;

internal sealed class DiskFileSystem : IReadWriteFileSystem {

	private readonly IFilesReader _filesReader;
	private readonly IFilesWriter _filesWriter;
	private readonly IFoldersReader _foldersReader;

	public DiskFileSystem(
		IFilesReader filesReader,
		IFilesWriter filesWriter,
		IFoldersReader foldersReader
	) {
		_filesReader = filesReader;
		_filesWriter = filesWriter;
		_foldersReader = foldersReader;
	}

	FolderId IFoldersReader.Root {
		get {
			return _foldersReader.Root;
		}
	}

	string IFilesWriter.FileSystemId => _filesWriter.FileSystemId;

	string IFilesReader.FileSystemId => _filesReader.FileSystemId;

	string IFoldersReader.FileSystemId => _foldersReader.FileSystemId;

	Task<T> IFilesReader.GetContentAsync<T>(
		FileId fileId,
		Func<Stream, CancellationToken, Task<T>> contentReader,
		CancellationToken cancellationToken
	) {
		return _filesReader.GetContentAsync<T>(
			fileId,
			contentReader,
			cancellationToken
		);
	}

	Task<FileMetadata> IFilesReader.GetMetadataAsync(
		FileId fileId,
		CancellationToken cancellationToken
	) {
		return _filesReader.GetMetadataAsync(
			fileId,
			cancellationToken
		);
	}

	Task<FileId> IFilesWriter.PutContentAsync(
		Func<Stream, CancellationToken, Task> asyncWriter,
		CancellationToken cancellationToken
	) {
		return _filesWriter.PutContentAsync(
			asyncWriter,
			cancellationToken
		);
	}


	Task IFilesWriter.RenameFileAsync(
		FileId fileId,
		string name,
		CancellationToken cancellationToken
	) {
		return _filesWriter.RenameFileAsync(
			fileId,
			name,
			cancellationToken
		);
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
}
