namespace Kiyote.Files;

public sealed record FileIdentifier(
	FileSystemId FileSystemId,
	FileId FileId
) {
	public static readonly FileIdentifier None = new FileIdentifier( FileSystemId.None, FileId.None );

	public override string ToString() {
		return $"{FileSystemId}::{FileId}";
	}
}
