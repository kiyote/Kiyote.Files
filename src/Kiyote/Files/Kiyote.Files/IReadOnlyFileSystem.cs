namespace Kiyote.Files;

public interface IReadOnlyFileSystem {

	Task GetContentAsync(
		FileIdentifier fileIdentifier,
		Func<Stream, CancellationToken, Task> contentReader,
		CancellationToken cancellationToken
	);

	IEnumerable<FileIdentifier> GetFileIdentifiers(
		FolderIdentifier folderIdentifier
	);

	FolderIdentifier GetRoot();

	IEnumerable<FolderIdentifier> GetFolderIdentifiers();

	IEnumerable<FolderIdentifier> GetFolderIdentifiers(
		FolderIdentifier folderIdentifier
	);

	FolderIdentifier GetFolderIdentifier(
		string folderName
	);
}

public interface IReadOnlyFileSystem<T> : IReadOnlyFileSystem where T: IFileSystemIdentifier {
};
