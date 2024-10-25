namespace Kiyote.Files;

public interface IFileSystem: IReadOnlyFileSystem {

	Task<FileIdentifier> CreateFileAsync(
		string fileName,
		Func<Stream, CancellationToken, Task> contentWriter,
		CancellationToken cancellationToken
	);

	Task<FileIdentifier> CreateFileAsync(
		FolderIdentifier folderIdentifier,
		string fileName,
		Func<Stream, CancellationToken, Task> contentWriter,
		CancellationToken cancellationToken
	);

	FolderIdentifier CreateFolder(
		FolderIdentifier folderIdentifier,
		string folderName
	);

	void DeleteFolder(
		FolderIdentifier folderIdentifier
	);

	IEnumerable<char> GetInvalidPathChars();

	IEnumerable<char> GetInvalidFileNameChars();

	IEnumerable<string> GetInvalidNames();

}

public interface IFileSystem<T>: IFileSystem where T : IFileSystemIdentifier {
};
