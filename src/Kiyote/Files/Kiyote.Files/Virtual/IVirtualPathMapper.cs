namespace Kiyote.Files.Virtual;

public interface IVirtualPathMapper {

	bool TryMapFromVirtual(
		FileId virtualFileId,
		out FileIdentifier fileIdentifier
	);

	bool TryMapToVirtual(
		FileIdentifier fileIdentifier,
		out FileId virtualFileId
	);

	bool TryMapFromVirtual(
		FolderId virtualFolderId,
		out FolderIdentifier folderIdentifier
	);

	bool TryMapToVirtual(
		FolderIdentifier folderIdentifier,
		out FolderId virtualFolderId
	);

}
