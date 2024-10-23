namespace Kiyote.Files.Virtual;

public interface IVirtualPathHandler {

	FolderId GetCommonParent(
		FolderId[] virtualPaths
	);

	bool IsRelativeTo(
		FolderId folderId,
		FolderId baseFolderId
	);

}
