using System.Reflection;
using Kiyote.Files.Virtual;
using Microsoft.Extensions.DependencyInjection;

namespace Kiyote.Files.Resource.IntegrationTests;

[TestFixture]
public sealed class VirtualResourceFileSystemTests {

	private sealed class FS : IFileSystemIdentifier {

		public static readonly FileSystemId Id = new FileSystemId( nameof( VirtualResourceFileSystemTests ) );

		FileSystemId IFileSystemIdentifier.FileSystemId => Id;
	}

	private const string VirtualRoot = "Resource";

	private IServiceScope? _scope;
	private IFileSystem<FS> _fileSystem;
	

	[SetUp]
	public void SetUp() {
		IServiceCollection services = new ServiceCollection();
		_ = services
			.BuildFileSystem<FS>( ( services, virtualPathHandler, builder ) => {
				FolderId root = virtualPathHandler.Create( VirtualRoot );
				_ = builder.AddResource( services, root, Assembly.GetExecutingAssembly() );
			} );

		IServiceProvider provider = services.BuildServiceProvider();
		_scope = provider.CreateAsyncScope();

		_fileSystem = _scope.ServiceProvider.GetRequiredService<IFileSystem<FS>>();
	}

	[TearDown]
	public void TearDown() {
		_scope?.Dispose();
	}

	[Test]
	public void GetFolderIdentifiers_Root_ReturnsResourceFolder() {
		IEnumerable<FolderIdentifier> folders = _fileSystem.GetFolderIdentifiers();

		Assert.That( folders.Count, Is.EqualTo( 1 ) );
	}

	[Test]
	public void GetFileIdentifiers_RootFolder_NoFilesReturned() {
		IEnumerable<FileIdentifier> files = _fileSystem.GetFileIdentifiers( _fileSystem.GetRoot() );

		Assert.That( files.Count(), Is.EqualTo( 0 ) );
	}

	[Test]
	public void GetFileIdentifiers_ResourceFolder_FilesReturned() {
		FolderIdentifier resourceFolder = _fileSystem.GetFolderIdentifier( VirtualRoot );
		List<FileIdentifier> files = _fileSystem.GetFileIdentifiers( resourceFolder ).ToList();

		Assert.That( files.Count, Is.EqualTo( 2 ) );
		Assert.That( files, Is.All.Matches<FileIdentifier>( fid => fid.FileId.ToString().StartsWith( "/Resource/", StringComparison.OrdinalIgnoreCase ) ) );
		Assert.That( files, Has.One.Matches<FileIdentifier>( fid => fid.FileId.ToString().EndsWith( "TestResources.item.txt", StringComparison.OrdinalIgnoreCase ) ) );
		Assert.That( files, Has.One.Matches<FileIdentifier>( fid => fid.FileId.ToString().EndsWith( "TestResources.Folder.subitem.txt", StringComparison.OrdinalIgnoreCase ) ) );
	}
}

