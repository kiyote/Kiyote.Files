using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Files.Virtual.IntegrationTests;

[ExcludeFromCodeCoverage]
public abstract class FS {
	public sealed class Test : IFileSystemIdentifier {

		public const string TestFileSystemId = "Test";

		FileSystemId IFileSystemIdentifier.FileSystemId => TestFileSystemId;
	}
}
