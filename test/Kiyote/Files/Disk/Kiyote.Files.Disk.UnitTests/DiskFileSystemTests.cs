namespace Kiyote.Files.Disk.Tests;

public class DiskFileSystemTests {

	public const string FileSystemId = "fsid";
	public const string Root = "root";
	private IReadWriteFileSystem _disk;
	private Mock<IFileSystem> _fileSystem;

	[SetUp]
	public void Setup() {
		_fileSystem = new Mock<IFileSystem>( MockBehavior.Strict );
		_disk = new DiskFileSystem(
			_fileSystem.Object,
			FileSystemId,
			Root
		);
	}

	[TearDown]
	public void TearDown() {
		_fileSystem.VerifyAll();
	}

	[Test]
	public void FileSystemId_FileSystemIdSet_IdMatches() {
		Assert.AreEqual( FileSystemId, _disk.Id );
	}

	[Test]
	public void GetContentAsync_InvalidFileId_ThrowsInvalidOperationException() {
		FileId fileId = new FileId( "invalid", "id" );

		_ = Assert.ThrowsAsync<InvalidOperationException>( () =>
			_disk.GetContentAsync(
				fileId,
				( Stream s, CancellationToken c ) => Task.FromResult( "" ),
				CancellationToken.None
			)
		);
	}

	[Test]
	public void RenameFileAsync_InvalidFileId_ThrowsInvalidOperationException() {
		FileId fileId = new FileId( "invalid", "id" );

		_ = Assert.ThrowsAsync<InvalidOperationException>( () =>
			_disk.RenameFileAsync(
				fileId,
				"name",
				CancellationToken.None
			)
		);
	}
}
