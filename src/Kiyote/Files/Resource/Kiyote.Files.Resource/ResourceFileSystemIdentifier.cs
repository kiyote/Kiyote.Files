namespace Kiyote.Files.Resource;

public abstract record ResourceFileSystemIdentifier(
	string FileSystemId
): FileSystemIdentifier( ResourceFileSystemType, FileSystemId ) {

	public const string ResourceFileSystemType = "Resource";
}
