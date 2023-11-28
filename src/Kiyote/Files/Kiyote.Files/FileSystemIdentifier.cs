namespace Kiyote.Files;

public abstract record FileSystemIdentifier(
	string FileSystemType,
	string FileSystemId
) {

	public const string ReadOnly = "RO";
	public const string ReadWrite = "RW";

}
