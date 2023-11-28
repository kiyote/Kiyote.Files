namespace Kiyote.Files.Disk;

public abstract record DiskFileSystemIdentifier(
	string FileSystemId
) : FileSystemIdentifier( DiskFileSystemType, FileSystemId ) {
	public const string DiskFileSystemType = "Disk";
}
