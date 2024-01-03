using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kiyote.Files.Resource.Manifest.IntegrationTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public class ResourceFileSystemTests {

	private IReadOnlyFileSystem _fileSystem;
	private IServiceScope _scope;

	[SetUp]
	public void SetUp() {
		IServiceCollection serviceCollection = new ServiceCollection();
		_ = serviceCollection
			.AddLogging()
			.AddReadOnlyResource(
				"Test",
				Assembly.GetExecutingAssembly(),
				"TestResources"
			);
		IServiceProvider services = serviceCollection.BuildServiceProvider();
		_scope = services.CreateAsyncScope();
		_fileSystem = services.GetRequiredKeyedService<IReadOnlyFileSystem>( "Test" );
	}

	[TearDown]
	public void TearDown() {
		_scope?.Dispose();
	}

	[Test]
	public void GetFolderIds_RootFolder_OneFolderReturned() {
		IEnumerable<FolderIdentifier> folders = _fileSystem.GetFolderIdentifiers();

		Assert.That( folders.Count(), Is.EqualTo( 1 ) );
	}

	[Test]
	public void GetFolderIds_ResourcesFolder_OneFolderReturned() {
		IEnumerable<FolderIdentifier> folderIdentifiers = _fileSystem.GetFolderIdentifiers();
		Assert.That( folderIdentifiers.Count, Is.EqualTo( 1 ), "Unable to get folder identifiers for resource assembly root." );

		FolderIdentifier folderIdentifier = folderIdentifiers.First();
		IEnumerable<FolderIdentifier> folders = _fileSystem.GetFolderIdentifiers( folderIdentifier );

		Assert.That( folders.Count(), Is.EqualTo( 0 ) );
	}
}
