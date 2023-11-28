namespace Kiyote.Files.Disk.Tests;

public class DiskFileSystemTests {

	public const string FileSystemId = "fsid";
	public const string Root = "root";
	private IReadWriteFileSystem _disk;
	private Mock<IFilesReader> _filesReader;
	private Mock<IFilesWriter> _filesWriter;
	private Mock<IFoldersReader> _foldersReader;

	[SetUp]
	public void Setup() {
		_filesReader = new Mock<IFilesReader>( MockBehavior.Strict );
		_filesWriter = new Mock<IFilesWriter>( MockBehavior.Strict );
		_foldersReader = new Mock<IFoldersReader>( MockBehavior.Strict );
		_disk = new DiskFileSystem(
			_filesReader.Object,
			_filesWriter.Object,
			_foldersReader.Object
		);
	}

	[TearDown]
	public void TearDown() {
		_filesReader.VerifyAll();
		_filesWriter.VerifyAll();
		_foldersReader.VerifyAll();
	}

	[Test]
	public async Task GetContentAsync_ConfiguredFileSystem_PassthroughCalled() {
		FileId fileId = new FileId( "fsid", "root" );
		string expected = "GetContentAsync";

		_ = _filesReader
			.Setup( fr => fr.GetContentAsync(
				fileId,
				It.IsAny<Func<Stream, CancellationToken, Task<string>>>(),
				It.IsAny<CancellationToken>()
			) )
			.ReturnsAsync( expected );

		string actual = await _disk.GetContentAsync(
				fileId,
				( Stream s, CancellationToken c ) => Task.FromResult( "" ),
				CancellationToken.None
			);

		Assert.AreEqual( expected, actual );
	}

	[Test]
	public async Task GetMetadataAsync_ConfiguredFileSystem_PassthroughCalled() {
		FileId fileId = new FileId( "fsid", "root" );
		DateTime createdOn = DateTime.UtcNow;
		FileMetadata expected = new FileMetadata(
			fileId,
			"expected",
			"expec/ted",
			123L,
			createdOn
		);

		_ = _filesReader
			.Setup( fr => fr.GetMetadataAsync(
				fileId,
				It.IsAny<CancellationToken>()
			) )
			.ReturnsAsync( expected );

		FileMetadata actual = await _disk.GetMetadataAsync(
				fileId,
				CancellationToken.None
			);

		Assert.AreEqual( expected, actual );
	}

	[Test]
	public async Task PutContentAsync_ConfiguredFileSystem_PassthroughCalled() {
		FileId expected = new FileId( "fsid", "root" );

		_ = _filesWriter
			.Setup( fw => fw.PutContentAsync(
				It.IsAny<Func<Stream, CancellationToken, Task>>(),
				It.IsAny<CancellationToken>()
			) )
			.ReturnsAsync( expected );

		FileId actual = await _disk.PutContentAsync(
				( Stream s, CancellationToken c ) => Task.CompletedTask,
				CancellationToken.None
			);

		Assert.AreEqual( expected, actual );
	}

	[Test]
	public async Task RenameFileAsync_InvalidFileId_ConfiguredFileSystem_PassthroughCalled() {
		FileId fileId = new FileId( "fsid", "root" );
		string expected = "RenameFileAsync";

		_ = _filesWriter
			.Setup( fw => fw.RenameFileAsync(
				fileId,
				expected,
				It.IsAny<CancellationToken>()
			) )
			.Returns( Task.CompletedTask );

		await _disk.RenameFileAsync(				
				fileId,
				expected,
				CancellationToken.None
			);

		Assert.Pass();
	}
}
