namespace Kiyote.Files.Disk.IntegrationTests;

public sealed record Test : IFileSystemIdentifier {

	public const string TestFileSystemId = "Test";

	string IFileSystemIdentifier.FileSystemId => TestFileSystemId;
}
