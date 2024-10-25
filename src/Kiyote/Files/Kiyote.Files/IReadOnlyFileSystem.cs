namespace Kiyote.Files;

public interface IReadOnlyFileSystem {

	Task GetContentAsync(
		FileIdentifier fileIdentifier,
		Func<Stream, CancellationToken, Task> contentReader,
		CancellationToken cancellationToken
	);

	IEnumerable<FileIdentifier> GetFileIdentifiers(
	);

	IEnumerable<FileIdentifier> GetFileIdentifiers(
		FolderIdentifier folderIdentifier
	);

	FileIdentifier GetFileIdentifier(
		string fileName
	);

	FileIdentifier GetFileIdentifier(
		FolderIdentifier folderIdentifier,
		string fileName
	);

	FolderIdentifier GetRoot();

	IEnumerable<FolderIdentifier> GetFolderIdentifiers();

	IEnumerable<FolderIdentifier> GetFolderIdentifiers(
		FolderIdentifier folderIdentifier
	);

	FolderIdentifier GetFolderIdentifier(
		string folderName
	);

	FolderIdentifier GetFolderIdentifier(
		FolderIdentifier parentFolderIdentifier,
		string folderName
	);
}

public interface IReadOnlyFileSystem<T> : IReadOnlyFileSystem where T: IFileSystemIdentifier {
};
