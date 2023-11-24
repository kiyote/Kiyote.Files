using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Kiyote.Files.Resource.IntegrationTests;

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
			@"ResourceFolder.folder.txt",
			@"ResourceFolder.SubFolder.subfolder.txt",
			@"root.txt",
			@"ResourceFolder.noextension"
		];
		IEnumerable<FileId> actual = _fileSystem.GetFilesInFolder( _fileSystem.Root );
		CollectionAssert.AreEquivalent( expected, actual.Select( f => f.Id ) );
	}

	[Test]
	public void GetFoldersInFolder_RootFolder_NoFoldersReturned() {
		IEnumerable<FolderId> folders = _fileSystem.GetFoldersInFolder( _fileSystem.Root );
		CollectionAssert.IsEmpty( folders );
	}
}
