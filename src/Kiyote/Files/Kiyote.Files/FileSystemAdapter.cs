namespace Kiyote.Files;

public sealed class FileSystemAdapter<T> : IFileSystem<T> where T : IFileSystemIdentifier {

	private readonly IFileSystem _fileSystem;

	public FileSystemAdapter(
		IFileSystem fileSystem
	) {
		_fileSystem = fileSystem;
	}

	Task<FileIdentifier> IFileSystem.CreateFileAsync(
		FolderIdentifier folderIdentifier,
		string fileName,
		Func<Stream, CancellationToken, Task> contentWriter,
		CancellationToken cancellationToken
	) {
		return _fileSystem.CreateFileAsync(
			folderIdentifier,
			fileName,
			contentWriter,
			cancellationToken
		);
	}

	FolderIdentifier IFileSystem.CreateFolder(
		FolderIdentifier folderIdentifier,
		string folderName
	) {
		return _fileSystem.CreateFolder(
			folderIdentifier,
			folderName
		);
	}

	void IFileSystem.DeleteFolder(
		FolderIdentifier folderIdentifier
	) {
		_fileSystem.DeleteFolder(
			folderIdentifier
		);
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

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers() {
		return _fileSystem.GetFolderIdentifiers();
	}

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers(
		FolderIdentifier folderIdentifier
	) {
		return _fileSystem.GetFolderIdentifiers( folderIdentifier );
	}

	IEnumerable<char> IFileSystem.GetInvalidPathChars() {
		return _fileSystem.GetInvalidPathChars();
	}

	IEnumerable<char> IFileSystem.GetInvalidFileNameChars() {
		return _fileSystem.GetInvalidFileNameChars();
	}

	IEnumerable<string> IFileSystem.GetInvalidNames() {
		return _fileSystem.GetInvalidNames();
	}

	FolderIdentifier IReadOnlyFileSystem.GetRoot() {
		return _fileSystem.GetRoot();
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
}
