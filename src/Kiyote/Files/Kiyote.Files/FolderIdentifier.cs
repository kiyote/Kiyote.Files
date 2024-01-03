namespace Kiyote.Files;

public sealed record FolderIdentifier(
	FileSystemId FileSystemId,
	FolderId FolderId
) {
	public static readonly FolderIdentifier None = new FolderIdentifier( FileSystemId.None, FolderId.None );
}
