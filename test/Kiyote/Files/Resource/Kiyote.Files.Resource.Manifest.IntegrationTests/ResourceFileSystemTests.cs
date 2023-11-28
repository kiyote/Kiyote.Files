using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kiyote.Files.Resource.IntegrationTests;

[TestFixture]
public class ResourceFileSystemTests {

	private IServiceScope _scope;
	private IReadOnlyFileSystem<Test> _fileSystem;

	[SetUp]
	public void SetUp() {
		IServiceCollection serviceCollection = new ServiceCollection();
		_ = serviceCollection
			.AddResourceReadOnlyFileSystem<Test>( Assembly.GetExecutingAssembly() );

		IServiceProvider services = serviceCollection.BuildServiceProvider();
		_scope = services.CreateScope();

		_fileSystem = _scope.ServiceProvider.GetRequiredService<IReadOnlyFileSystem<Test>>();
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	public void GetFilesInFolder_RootFolder_ExpectedFilesReturned() {
		string[] expected = [
			@"\Microsoft.Extensions.FileProviders.Embedded.Manifest.xml",
			@"\root.txt"
		];
		IEnumerable<FileId> actual = _fileSystem.GetFilesInFolder( _fileSystem.Root );
		CollectionAssert.AreEquivalent( expected, actual.Select( f => f.Id ) );
	}


	[Test]
	public void GetFilesInFolder_SubFolder_ExpectedFilesReturned() {
		string[] expected = [
			@"\ResourceFolder\folder.txt",
			@"\ResourceFolder\noextension"
		];
		FolderId subFolder = _fileSystem.GetFoldersInFolder( _fileSystem.Root ).First();
		IEnumerable<FileId> actual = _fileSystem.GetFilesInFolder( subFolder );
		CollectionAssert.AreEquivalent( expected, actual.Select( f => f.Id ) );
	}

	[Test]
	public void GetFoldersInFolder_RootFolder_ExpectedFoldersReturned() {
		string[] expected = [
			@"\ResourceFolder\"
		];
		IEnumerable<FolderId> folders = _fileSystem.GetFoldersInFolder( _fileSystem.Root );
		CollectionAssert.AreEquivalent( expected, folders.Select( f => f.Id ) );
	}

	[Test]
	public void GetFoldersInFolder_SubFolder_ExpectedFoldersReturned() {
		string[] expected = [
			@"\ResourceFolder\SubFolder\"
		];
		FolderId subFolder = _fileSystem.GetFoldersInFolder( _fileSystem.Root ).First();
		IEnumerable<FolderId> folders = _fileSystem.GetFoldersInFolder( subFolder );
		CollectionAssert.AreEquivalent( expected, folders.Select( f => f.Id ) );
	}
}
