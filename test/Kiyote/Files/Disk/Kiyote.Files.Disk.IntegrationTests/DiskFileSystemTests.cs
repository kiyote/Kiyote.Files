using Microsoft.Extensions.DependencyInjection;

namespace Kiyote.Files.Disk.IntegrationTests;

[TestFixture]
public sealed class DiskFileSystemTests {

	public const string TestStorageAreadId = "Test";

	private IServiceScope _scope;
	private IReadWriteFileSystem _fileSystem;

	[SetUp]
	public void Setup() {
		IServiceCollection serviceCollection = new ServiceCollection();
		_ = serviceCollection
			.AddDiskFileSystem()
			.AddStorageAreas( ( IServiceProvider services, IStorageAreaBuilder builder ) => {
				_ = builder.AddReadWrite(
					TestStorageAreadId,
					services.CreateReadWriteDiskFileSystem( TestContext.CurrentContext.TestDirectory )
				);
			} );
		IServiceProvider services = serviceCollection.BuildServiceProvider();
		_scope = services.CreateScope();

		IStorageAreaProvider provider = _scope.ServiceProvider.GetRequiredService<IStorageAreaProvider>();
		IReadWriteStorageArea storageArea = provider.GetReadWriteStorageArea( TestStorageAreadId );
		_fileSystem = storageArea.ReadWrite;
	}

	[TearDown]
	public void TearDown() {
		_scope.Dispose();
	}

	[Test]
	public void Test1() {		
		Assert.Pass();
	}

}
