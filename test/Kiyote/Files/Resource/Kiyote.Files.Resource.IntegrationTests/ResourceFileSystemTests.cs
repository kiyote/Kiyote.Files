using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kiyote.Files.Resource.IntegrationTests;

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
			.AddResourceFileSystem(
				"Test",
				Assembly.GetExecutingAssembly()
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
	public void GetFolderIdentifiers_Root_ReturnsNoFolders() {
		IEnumerable<FolderIdentifier> folders = _fileSystem.GetFolderIdentifiers();

		Assert.That( folders, Is.Empty );
	}

	[Test]
	public void GetFileIdentifiers_Root_Returns2Files() {
		FolderIdentifier root = _fileSystem.GetRoot();
		List<FileIdentifier> files = _fileSystem.GetFileIdentifiers( root ).ToList();

		Assert.That( files.Count, Is.EqualTo( 2 ) );
	}

}
