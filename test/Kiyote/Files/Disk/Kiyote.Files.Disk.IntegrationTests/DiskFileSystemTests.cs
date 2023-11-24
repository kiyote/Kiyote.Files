using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Kiyote.Files.Disk.IntegrationTests;

[TestFixture]
public sealed class DiskFileSystemTests {

	private IServiceScope _scope;
	private IReadWriteFileSystem<Test> _fileSystem;
	private string _root;

	[SetUp]
	public void Setup() {
		_root = $"{TestContext.CurrentContext.TestDirectory}\\{Guid.NewGuid():N}";
		IServiceCollection serviceCollection = new ServiceCollection();
		_ = serviceCollection
			.AddDiskReadWriteFileSystem<Test>( _root );

		IServiceProvider services = serviceCollection.BuildServiceProvider();
		_scope = services.CreateScope();

		_fileSystem = _scope.ServiceProvider.GetRequiredService<IReadWriteFileSystem<Test>>();
	}

	[TearDown]
	public void TearDown() {
		Directory.Delete( _root, true );
		_scope.Dispose();
	}

	[Test]
	public async Task PutContentAsync_ValidContent_FileCreated() {
		string expected = "Test";
		FileId fileId = await _fileSystem.PutContentAsync(
			async ( Stream stream, CancellationToken token ) => {
				using TextWriter writer = new StreamWriter( stream );
				char[] text = Encoding.UTF8.GetChars( Encoding.UTF8.GetBytes( expected ) );
				await writer.WriteAsync( text, token );
			},
			CancellationToken.None
		);

		string actual = await File.ReadAllTextAsync( Path.Combine( _root, fileId.Id[1..] ), CancellationToken.None );

		Assert.AreEqual( expected, actual );
	}

	[Test]
	public async Task GetContentAsync_ValidContent_ContentRead() {
		string expected = "AnotherTest";
		FileId fileId = await _fileSystem.PutContentAsync(
			async ( Stream stream, CancellationToken token ) => {
				using TextWriter writer = new StreamWriter( stream );
				char[] text = Encoding.UTF8.GetChars( Encoding.UTF8.GetBytes( expected ) );
				await writer.WriteAsync( text, token );
			},
			CancellationToken.None
		);

		string actual = await _fileSystem.GetContentAsync(
			fileId,
			async ( Stream stream, CancellationToken token ) => {
				using TextReader reader = new StreamReader( stream );
				return await reader.ReadToEndAsync( token );
			},
			CancellationToken.None
		);

		Assert.AreEqual( expected, actual );
	}

	[Test]
	public async Task GetMetadataAsync_FileExistsNoMetadata_FileMetadataRead() {
		string expected = "AnotherTest";
		FileId fileId = await _fileSystem.PutContentAsync(
			async ( Stream stream, CancellationToken token ) => {
				using TextWriter writer = new StreamWriter( stream );
				char[] text = Encoding.UTF8.GetChars( Encoding.UTF8.GetBytes( expected ) );
				await writer.WriteAsync( text, token );
			},
			CancellationToken.None
		);

		FileMetadata metadata = await _fileSystem.GetMetadataAsync(
			fileId,
			CancellationToken.None
		);

		Assert.AreEqual( Path.GetFileName( fileId.Id ), metadata.Name );
		Assert.AreEqual( Encoding.UTF8.GetBytes( expected ).Length, metadata.Size );
	}

	[Test]
	public async Task GetMetadataAsync_FileExistsWithMetadata_FileMetadataRead() {
		string expected = "AnotherTest";
		FileId fileId = await _fileSystem.PutContentAsync(
			async ( Stream stream, CancellationToken token ) => {
				using TextWriter writer = new StreamWriter( stream );
				char[] text = Encoding.UTF8.GetChars( Encoding.UTF8.GetBytes( expected ) );
				await writer.WriteAsync( text, token );
			},
			CancellationToken.None
		);

		FileMetadata metadata = await _fileSystem.GetMetadataAsync(
			fileId,
			CancellationToken.None
		);

		Assert.AreEqual( Path.GetFileName( fileId.Id ), metadata.Name );
		Assert.AreEqual( Encoding.UTF8.GetBytes( expected ).Length, metadata.Size );
	}

	[Test]
	public async Task RenameFileAsync_FileExistsWithMetadata_FileMetadataRead() {
		string expected = "AnotherTest";
		FileId fileId = await _fileSystem.PutContentAsync(
			async ( Stream stream, CancellationToken token ) => {
				using TextWriter writer = new StreamWriter( stream );
				char[] text = Encoding.UTF8.GetChars( Encoding.UTF8.GetBytes( expected ) );
				await writer.WriteAsync( text, token );
			},
			CancellationToken.None
		);
		await _fileSystem.RenameFileAsync(
			fileId,
			expected,
			CancellationToken.None
		);

		FileMetadata metadata = await _fileSystem.GetMetadataAsync(
			fileId,
			CancellationToken.None
		);

		Assert.AreEqual( expected, metadata.Name );
		Assert.AreEqual( Encoding.UTF8.GetBytes( expected ).Length, metadata.Size );
	}
}
