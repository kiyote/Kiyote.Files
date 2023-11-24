namespace Kiyote.Files;

public interface IReadOnlyFileSystem: IFileSystemIdentifier {

	FolderId Root { get; }

	Task<TFileContent> GetContentAsync<TFileContent>(
		FileId fileId,
		Func<Stream, CancellationToken, Task<TFileContent>> contentReader,
		CancellationToken cancellationToken
	);

	Task<FileMetadata> GetMetadataAsync(
		FileId fileId,
		CancellationToken cancellationToken
	);

	IEnumerable<FileId> GetFilesInFolder(
		FolderId folderId
	);

	IEnumerable<FolderId> GetFoldersInFolder(
		FolderId folderId
	);

}

public interface IReadOnlyFileSystem<T> : IReadOnlyFileSystem {
}
