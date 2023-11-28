namespace Kiyote.Files.Resource.IntegrationTests;

public sealed record Test : ResourceFileSystemIdentifier {

	public const string TestFileSystemId = "Test";

	public Test() : base( TestFileSystemId ) { }
}
