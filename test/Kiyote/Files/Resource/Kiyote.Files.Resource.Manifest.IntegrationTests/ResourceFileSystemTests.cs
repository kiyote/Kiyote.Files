using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Kiyote.Files.Resource.Manifest.IntegrationTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public class ResourceFileSystemTests {

	private IReadOnlyFileSystem _fileSystem;

	[SetUp]
	public void SetUp() {
		_fileSystem = new ResourceFileSystem(
			"Test",
			Assembly.GetExecutingAssembly(),
			"TestResources"
		);
	}

	[Test]
	public void GetFolderIds_RootFolder_OneFolderReturned() {
		IEnumerable<FolderIdentifier> folders = _fileSystem.GetFolderIdentifiers();

		Assert.That( folders.Count(), Is.EqualTo( 1 ) );
	}

	[Test]
	public void GetFolderIds_ResourcesFolder_OneFolderReturned() {
		FolderIdentifier folderIdentifier = _fileSystem.GetFolderIdentifiers().First();
		IEnumerable<FolderIdentifier> folders = _fileSystem.GetFolderIdentifiers( folderIdentifier );

		Assert.That( folders.Count(), Is.EqualTo( 0 ) );
	}
}
