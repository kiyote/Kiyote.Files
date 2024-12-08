using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kiyote.Files.Resource.IntegrationTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public class ResourceFileSystemTests {

	private IReadOnlyFileSystem _fileSystem;
	private AsyncServiceScope? _scope;
	private string _separator;

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
		_fileSystem = _scope.Value.ServiceProvider.GetRequiredKeyedService<IReadOnlyFileSystem>( "Test" );
		_separator = Path.DirectorySeparatorChar.ToString();
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
		List<FileIdentifier> files = _fileSystem.GetFileIdentifiers().ToList();

		Assert.That( files.Count, Is.EqualTo( 2 ) );
		Assert.That( files.ElementAt( 0 ).FileId.ToString(), Is.EqualTo( $"{_separator}Kiyote.Files.Resource.IntegrationTests.TestResources.Folder.subitem.txt" ) );
		Assert.That( files.ElementAt( 1 ).FileId.ToString(), Is.EqualTo( $"{_separator}Kiyote.Files.Resource.IntegrationTests.TestResources.item.txt" ) );
	}

	[Test]
	public void GetFileIdentifier_ExistingFile_ReturnsCorrectFileIdentifier() {
		FileIdentifier fileIdentifier = _fileSystem.GetFileIdentifier( "Kiyote.Files.Resource.IntegrationTests.TestResources.item.txt" );

		Assert.That( fileIdentifier.FileId.ToString(), Is.EqualTo( $"{_separator}Kiyote.Files.Resource.IntegrationTests.TestResources.item.txt" ) );
	}

	[Test]
	public void GetFileIdentifier_NonExistantFile_ThrowsPathNotFoundException() {
		_ = Assert.Throws<PathNotFoundException>( () => _fileSystem.GetFileIdentifier( "garbage" ) );
	}

	[Test]
	public async Task GetContentAsync_ExistingFile_ContentMatches() {
		FileIdentifier fileIdentifier = _fileSystem.GetFileIdentifier( "Kiyote.Files.Resource.IntegrationTests.TestResources.item.txt" );

		await _fileSystem.GetContentAsync(
			fileIdentifier,
			async ( stream, cancellationToken ) => {
				TextReader reader = new StreamReader( stream );
				string content = await reader.ReadToEndAsync( cancellationToken );
				Assert.That( content.Trim(), Is.EqualTo( "item" ) ); // Ignore line-ending issues
			},
			CancellationToken.None
		);
	}
}
