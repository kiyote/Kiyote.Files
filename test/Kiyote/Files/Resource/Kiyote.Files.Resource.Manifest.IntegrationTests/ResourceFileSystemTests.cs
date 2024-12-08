using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Kiyote.Files.Resource.Manifest.IntegrationTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public class ResourceFileSystemTests {

	private IReadOnlyFileSystem _fileSystem;
	private AsyncServiceScope? _scope;
	private string _separator;

	[SetUp]
	public void SetUp() {
		IServiceCollection serviceCollection = new ServiceCollection();
		serviceCollection
			.AddLogging( ( ILoggingBuilder configure ) => {
				_ = configure.SetMinimumLevel( LogLevel.Debug );
			} )
			.AddResourceFileSystem(
				"Test",
				Assembly.GetExecutingAssembly(),
				"TestResources"
			)
			.TryAddEnumerable( ServiceDescriptor.Singleton<ILoggerProvider, NUnitLoggerProvider>() );

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
	public void GetFolderIdentifier_BadFolder_ThrowsFolderNotFoundException() {
		_ = Assert.Throws<PathNotFoundException>( () => _fileSystem.GetFolderIdentifier( "BadFolder" ) );
	}

	[Test]
	public void GetFolderIdentifier_GoodFolder_ReturnsOneFolderIdentifier() {
		FolderIdentifier folder = _fileSystem.GetFolderIdentifier( "Folder" );

		Assert.That( folder.FolderId, Is.EqualTo( $"{_separator}Folder{_separator}" ) );
	}

	[Test]
	public void GetFolderIdentifiers_RootFolder_OneFolderReturned() {
		List<FolderIdentifier> folders = _fileSystem.GetFolderIdentifiers().ToList();

		Assert.That( folders, Has.Exactly( 1 ).Items );
	}

	[Test]
	public void GetFolderIdentifiers_ResourcesFolder_OneFolderReturned() {
		List<FolderIdentifier> folderIdentifiers = _fileSystem.GetFolderIdentifiers().ToList();
		Assert.That( folderIdentifiers.Count, Is.EqualTo( 1 ), "Unable to get folder identifiers for resource assembly root." );

		FolderIdentifier folderIdentifier = folderIdentifiers.First();
		List<FolderIdentifier> folders = _fileSystem.GetFolderIdentifiers( folderIdentifier ).ToList();

		Assert.That( folders.Count, Is.EqualTo( 1 ) );
		Assert.That( folders.ElementAt( 0 ).FolderId, Is.EqualTo( $"{_separator}Folder{_separator}SubFolder{_separator}" ) );
	}

	[Test]
	public void GetFileIdentifiers_Root_Returns1File() {
		List<FileIdentifier> fileIdentifiers = _fileSystem.GetFileIdentifiers().ToList();

		Assert.That( fileIdentifiers.Count, Is.EqualTo( 1 ) );
		Assert.That( fileIdentifiers.ElementAt( 0 ).FileId.ToString(), Is.EqualTo( $"{_separator}item.txt" ) );
	}

	[Test]
	public void GetFileIdentifiers_Folder_Returns1File() {
		FolderIdentifier folderIdentifier = _fileSystem.GetFolderIdentifier( "Folder" );
		List<FileIdentifier> fileIdentifiers = _fileSystem.GetFileIdentifiers( folderIdentifier ).ToList();

		Assert.That( fileIdentifiers.Count, Is.EqualTo( 1 ) );
		Assert.That( fileIdentifiers.ElementAt( 0 ).FileId.ToString(), Is.EqualTo( $"{_separator}Folder{_separator}subitem.txt" ) );
	}

	[Test]
	public void GetFileIdentifier_RootExistingFile_ReturnsCorrectFileIdentifier() {
		FileIdentifier fileIdentifier = _fileSystem.GetFileIdentifier( "item.txt" );

		Assert.That( fileIdentifier.FileId.ToString(), Is.EqualTo( $"{_separator}item.txt" ) );
	}

	[Test]
	public void GetFileIdentifier_FolderExistingFile_ReturnsCorrectFileIdentifier() {
		FolderIdentifier folderIdentifier = _fileSystem.GetFolderIdentifier( "Folder" );
		FileIdentifier fileIdentifier = _fileSystem.GetFileIdentifier( folderIdentifier, "subitem.txt" );

		Assert.That( fileIdentifier.FileId.ToString(), Is.EqualTo( $"{_separator}Folder{_separator}subitem.txt" ) );
	}

	[Test]
	public void GetFileIdentifier_RootNonExistantFile_ThrowsPathNotFoundException() {
		_ = Assert.Throws<PathNotFoundException>( () => _fileSystem.GetFileIdentifier( "garbage" ) );
	}

	[Test]
	public void GetFileIdentifier_FolderNonExistantFile_ThrowsPathNotFoundException() {
		FolderIdentifier folderIdentifier = _fileSystem.GetFolderIdentifier( "Folder" );
		_ = Assert.Throws<PathNotFoundException>( () => _fileSystem.GetFileIdentifier( folderIdentifier, "garbage" ) );
	}

	[Test]
	public async Task GetContentAsync_ExistingFile_ContentMatches() {
		FileIdentifier fileIdentifier = _fileSystem.GetFileIdentifier( "item.txt" );

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

	[Test]
	public async Task GetContentAsync_SubFolderExistingFile_ContentMatches() {
		FolderIdentifier folderIdentifier = _fileSystem.GetFolderIdentifier( "Folder" );
		FileIdentifier fileIdentifier = _fileSystem.GetFileIdentifier( folderIdentifier, "subitem.txt" );

		await _fileSystem.GetContentAsync(
			fileIdentifier,
			async ( stream, cancellationToken ) => {
				TextReader reader = new StreamReader( stream );
				string content = await reader.ReadToEndAsync( cancellationToken );
				Assert.That( content.Trim(), Is.EqualTo( "subitem" ) ); // Ignore line-ending issues
			},
			CancellationToken.None
		);
	}
}
