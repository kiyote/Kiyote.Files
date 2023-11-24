namespace Kiyote.Files.Resource.IntegrationTests;

public sealed record Test : FileSystemIdentifier {

	public const string FileSystemId = "Test";

	public Test() : base( FileSystemId ) { }
}
