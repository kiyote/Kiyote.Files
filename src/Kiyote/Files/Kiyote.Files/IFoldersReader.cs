namespace Kiyote.Files;

public interface IFoldersReader {

	FileSystemIdentifier Id { get; }

	FolderId Root { get; }

	IEnumerable<FileId> GetFilesInFolder(
		FolderId folderId
	);

	IEnumerable<FolderId> GetFoldersInFolder(
		FolderId folderId
	);

}
