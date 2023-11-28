namespace Kiyote.Files.Resource.IntegrationTests;

public sealed record Test : FileSystemIdentifier {

	public const string TestFileSystemId = "Test";
	public const string TestFileSystemType = "TestType";

	public Test() : base( TestFileSystemId, TestFileSystemType ) { }
}
