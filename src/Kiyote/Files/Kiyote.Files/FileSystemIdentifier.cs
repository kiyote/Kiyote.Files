namespace Kiyote.Files;

public abstract record FileSystemIdentifier( string Id ) {

	public const string ReadOnly = "RO";
	public const string ReadWrite = "RW";

}
