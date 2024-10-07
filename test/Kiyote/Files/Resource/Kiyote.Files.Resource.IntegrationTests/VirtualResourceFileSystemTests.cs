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
			.BuildFileSystem<FS>( ( services, builder ) => {
				_ = builder.AddResource( services, VirtualRoot, Assembly.GetExecutingAssembly() );
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
	public void GetFolderIdentifiers_Root_ReturnsNoFolders() {
		IEnumerable<FolderIdentifier> folders = _fileSystem.GetFolderIdentifiers();

		Assert.That( folders, Is.Empty );
	}

	[Test]
	public void GetFileIdentifiers_RootFolder_FilesReturned() {
		IEnumerable<FileIdentifier> files = _fileSystem.GetFileIdentifiers( _fileSystem.GetRoot() );

		Assert.That( files.Count(), Is.EqualTo( 2 ) );
	}

	/*
	[Test]
	public void GetFolderIdentifier_ResourceFolder_FolderReturned() {
		Assert.DoesNotThrow( () => _fileSystem.GetFolderIdentifier( VirtualRoot ) );
	}
	*/
}

