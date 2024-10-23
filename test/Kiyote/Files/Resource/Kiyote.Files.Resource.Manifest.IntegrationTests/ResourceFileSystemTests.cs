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
	private IServiceScope _scope;

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
		_fileSystem = services.GetRequiredKeyedService<IReadOnlyFileSystem>( "Test" );
	}

	[TearDown]
	public void TearDown() {
		_scope?.Dispose();
	}

	[Test]
	public void GetFolderIdentifier_BadFolder_ThrowsFolderNotFoundException() {
		_ = Assert.Throws<FolderNotFoundException>( () => _fileSystem.GetFolderIdentifier( "BadFolder" ) );
	}

	[Test]
	public void GetFolderIdentifier_GoodFolder_ReturnsOneFolderIdentifier() {
		FolderIdentifier folder = _fileSystem.GetFolderIdentifier( "Folder" );

		Assert.That( folder.FolderId, Is.EqualTo( "\\Folder\\" ) );
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
		Assert.That( folders.ElementAt( 0 ).FolderId, Is.EqualTo( "\\Folder\\SubFolder\\" ) );
	}

	/*
	[Test]
	public void GetFileIds_RootFolder_OneFileReturned() {
		FolderIdentifier root = _fileSystem.GetRoot();
		List<FileIdentifier> fileIds = _fileSystem.GetFileIdentifiers( root ).ToList();

		Assert.That( fileIds, Has.Exactly( 1 ).Items );
	}
	*/
}
