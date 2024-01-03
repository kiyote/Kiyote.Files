using System.Diagnostics.CodeAnalysis;
using Kiyote.Files.Disk;
using Microsoft.Extensions.DependencyInjection;

namespace Kiyote.Files.Virtual.IntegrationTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public sealed class FileSystemTests {

	private string _folder1;
	private string _folder2;
	private IServiceScope? _scope;
	private IFileSystem<FS.Test>? _fileSystem;

	[SetUp]
	public void SetUp() {
		string tempPath = Path.GetTempPath();
		_folder1 = $"{Path.Combine( tempPath, Guid.NewGuid().ToString( "N" ) )}{Path.DirectorySeparatorChar}";
		_ = Directory.CreateDirectory( _folder1 );
		_folder2 = $"{Path.Combine( tempPath, Guid.NewGuid().ToString( "N" ) )}{Path.DirectorySeparatorChar}";
		_ = Directory.CreateDirectory( _folder2 );
	}

	[TearDown]
	public void TearDown() {
		_scope!.Dispose();
		Directory.Delete( _folder1, true );
		Directory.Delete( _folder2, true );
	}

	[Test]
	public void GetFolderIds_OneRootNoChildFolders_NoFoldersReturned() {
		CreateSingleRootedFileSystem();
		IEnumerable<FolderIdentifier> folderIdentifiers = _fileSystem!.GetFolderIdentifiers();

		Assert.That( folderIdentifiers, Is.Empty );
	}

	[Test]
	public void GetFolderIds_TwoRootsNoChildFolders_NoFoldersReturned() {
		CreateTwoRootedFileSystem();
		IEnumerable<FolderIdentifier> folderIdentifiers = _fileSystem!.GetFolderIdentifiers();

		Assert.That( folderIdentifiers.Count, Is.EqualTo( 2 ) );
	}

	[Test]
	public void CreateFolder_ValidFolder_FolderCreated() {
		CreateSingleRootedFileSystem();
		FolderIdentifier rootIdentifier = _fileSystem!.GetRoot();

		Assert.That( Directory.GetDirectories( _folder1 ).Length, Is.EqualTo( 0 ) );

		_ = _fileSystem.CreateFolder( rootIdentifier, "test" );

		Assert.That( Directory.GetDirectories( _folder1 ).Length, Is.EqualTo( 1 ) );
	}

	[Test]
	public void CreateFolderId_ChildFolder_CorrectVirtualPathReturned() {
		CreateSingleRootedFileSystem();
		FolderIdentifier rootIdentifier = _fileSystem!.GetRoot();
		FolderIdentifier childIdentifier = _fileSystem.CreateFolder( rootIdentifier, "test" );

		Assert.That( childIdentifier.FolderId.ToString(), Is.EqualTo( "/test/" ) );
	}

	[Test]
	public void CreateFolderId_NestedChildFolder_CorrectVirtualPathReturned() {
		CreateSingleRootedFileSystem();
		FolderIdentifier rootIdentifier = _fileSystem!.GetRoot();
		FolderIdentifier child1 = _fileSystem.CreateFolder( rootIdentifier, "child1" );
		FolderIdentifier child2 = _fileSystem.CreateFolder( child1, "child2" );

		Assert.That( child2.FolderId.ToString(), Is.EqualTo( "/child1/child2/" ) );
	}

	private void CreateSingleRootedFileSystem() {
		IServiceCollection collection = new ServiceCollection();
		_ = collection
			.AddDiskFiles()
			.BuildFileSystem(
				( IServiceProvider services, IFileSystemBuilder<FS.Test> builder ) => {
					_ = builder.AddReadWriteDisk(
						services,
						"/",
						_folder1
					);
				}
			);

		IServiceProvider services = collection.BuildServiceProvider();
		_scope = services.CreateScope();
		_fileSystem = _scope.ServiceProvider.GetRequiredService<IFileSystem<FS.Test>>();
	}

	private void CreateTwoRootedFileSystem() {
		IServiceCollection collection = new ServiceCollection();
		_ = collection
			.AddDiskFiles()
			.BuildFileSystem(
				( IServiceProvider services, IFileSystemBuilder<FS.Test> builder ) => {
					_ = builder.AddReadWriteDisk(
						services,
						"/root1",
						_folder1
					).AddReadWriteDisk(
						services,
						"/root2",
						_folder2
					);
				}
			);

		IServiceProvider services = collection.BuildServiceProvider();
		_scope = services.CreateScope();
		_fileSystem = _scope.ServiceProvider.GetRequiredService<IFileSystem<FS.Test>>();
	}
}
