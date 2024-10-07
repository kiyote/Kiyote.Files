using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Files.Virtual.IntegrationTests;

[ExcludeFromCodeCoverage]
public abstract class FS {
	public sealed class Test : IFileSystemIdentifier {

		public static readonly FileSystemId TestFileSystemId = "Test";

		FileSystemId IFileSystemIdentifier.FileSystemId => TestFileSystemId;
	}
}
