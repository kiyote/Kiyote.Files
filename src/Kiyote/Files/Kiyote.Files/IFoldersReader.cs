namespace Kiyote.Files;

public interface IFoldersReader {

	string FileSystemId { get; }

	FolderId Root { get; }

	IEnumerable<FileId> GetFilesInFolder(
		FolderId folderId
	);

	IEnumerable<FolderId> GetFoldersInFolder(
		FolderId folderId
	);

}
