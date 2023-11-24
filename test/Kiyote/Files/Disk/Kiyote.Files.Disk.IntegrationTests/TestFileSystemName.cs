namespace Kiyote.Files.Disk.IntegrationTests;

public sealed record Test : FileSystemIdentifier {

	public const string FileSystemId = "Test";

	public Test() : base( FileSystemId ) { }
}
