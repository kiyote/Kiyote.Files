namespace Kiyote.Files.Resource.IntegrationTests;

public sealed record Test : IFileSystemIdentifier {

	public const string TestFileSystemId = "Test";

	string IFileSystemIdentifier.FileSystemId => TestFileSystemId;
}
