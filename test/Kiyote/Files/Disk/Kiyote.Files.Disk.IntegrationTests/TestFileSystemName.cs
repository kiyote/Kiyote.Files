namespace Kiyote.Files.Disk.IntegrationTests;

public sealed record Test : DiskFileSystemIdentifier {

	public const string TestFileSystemId = "Test";

	public Test() : base( TestFileSystemId ) { }
}
