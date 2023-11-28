namespace Kiyote.Files;

public interface IFoldersReader {

	FolderId Root { get; }

	IEnumerable<FileId> GetFilesInFolder(
		FolderId folderId
	);

	IEnumerable<FolderId> GetFoldersInFolder(
		FolderId folderId
	);

}
