namespace Kiyote.Files.Virtual;

public sealed class ReadOnlyFileSystemAdapter<T> : IReadOnlyFileSystem<T> where T: IFileSystemIdentifier {

	private readonly IReadOnlyFileSystem _fileSystem;

	public ReadOnlyFileSystemAdapter(
		IReadOnlyFileSystem fileSystem
	) {
		_fileSystem = fileSystem;
	}

	Task IReadOnlyFileSystem.GetContentAsync(
		FileIdentifier fileIdentifier,
		Func<Stream, CancellationToken, Task> contentReader,
		CancellationToken cancellationToken
	) {
		return _fileSystem.GetContentAsync(
			fileIdentifier,
			contentReader,
			cancellationToken
		);
	}

	IEnumerable<FileIdentifier> IReadOnlyFileSystem.GetFileIdentifiers(
		FolderIdentifier folderIdentifier
	) {
		return _fileSystem.GetFileIdentifiers(
			folderIdentifier
		);
	}

	FolderIdentifier IReadOnlyFileSystem.GetFolderIdentifier(
		string folderName
	) {
		return _fileSystem.GetFolderIdentifier( folderName );
	}

	FolderIdentifier IReadOnlyFileSystem.GetFolderIdentifier(
		FolderIdentifier parentFolderIdentifier,
		string folderName
	) {
		return _fileSystem.GetFolderIdentifier(
			parentFolderIdentifier,
			folderName
		);
	}

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers() {
		return _fileSystem.GetFolderIdentifiers();
	}

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers(
		FolderIdentifier folderIdentifier
	) {
		return _fileSystem.GetFolderIdentifiers();
	}

	FolderIdentifier IReadOnlyFileSystem.GetRoot() {
		return _fileSystem.GetRoot();
	}
}
