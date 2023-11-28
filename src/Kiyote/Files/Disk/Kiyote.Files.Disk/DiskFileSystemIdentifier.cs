namespace Kiyote.Files.Disk;

public record DiskFileSystemIdentifier(
	string FileSystemId
) : FileSystemIdentifier( DiskFileSystemType, FileSystemId ) {
	public const string DiskFileSystemType = "Disk";
}
