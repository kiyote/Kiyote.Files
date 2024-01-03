namespace Kiyote.Files.Virtual;

public interface IVirtualPathMapper {

	bool TryMapFromVirtual(
		FileIdentifier virtualFileIdentifier,
		out FileIdentifier fileIdentifier
	);

	bool TryMapToVirtual(
		FileIdentifier fileIdentifier,
		out FileIdentifier virtualFileIdentifier
	);

	bool TryMapFromVirtual(
		FolderIdentifier virtualFolderIdentifier,
		out FolderIdentifier folderIdentifier
	);

	bool TryMapToVirtual(
		FolderIdentifier folderIdentifier,
		out FolderIdentifier virtualFolderIdentifier
	);

}
