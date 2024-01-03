using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Kiyote.Files.Resource.IntegrationTests;


[TestFixture]
[ExcludeFromCodeCoverage]
public class ResourceFileSystemTests {

	private IReadOnlyFileSystem _fileSystem;

	[SetUp]
	public void SetUp() {
		_fileSystem = new ResourceFileSystem(
			"Test",
			Assembly.GetExecutingAssembly()
		);
	}

	[TearDown]
	public void TearDown() {
	}

	[Test]
	public void GetFolderIdentifiers_Root_ReturnsNoFolders() {
		IEnumerable<FolderIdentifier> folders = _fileSystem.GetFolderIdentifiers();

		Assert.That( folders, Is.Empty );
	}
}
