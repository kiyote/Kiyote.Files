namespace Kiyote.Files.Resource;

public record ResourceFileSystemIdentifier(
	string FileSystemId
): FileSystemIdentifier( ResourceFileSystemType, FileSystemId ) {

	public const string ResourceFileSystemType = "Resource";
}
