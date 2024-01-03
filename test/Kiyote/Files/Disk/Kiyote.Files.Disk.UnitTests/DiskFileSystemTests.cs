using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Moq;

namespace Kiyote.Files.Disk.UnitTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public sealed class DiskFileSystemTests {

	public const char Separator = '\\';
	public const string Root = @"FAKE:\FOLDER";
	public static readonly char[] InvalidPathChars = ['/', '\\'];
	public static readonly char[] InvalidFileNameChars = [ '/', '\\', ':' ];
	private Mock<System.IO.Abstractions.IFileSystem> _fileSystem;
	private Mock<IPath> _path;
	private IFileSystem _files;

	[SetUp]
	public void SetUp() {
		_path = new Mock<IPath>( MockBehavior.Strict );
		_ = _path
			.Setup( p => p.DirectorySeparatorChar )
			.Returns( Separator );
		_ = _path
			.Setup( p => p.GetInvalidPathChars() )
			.Returns( InvalidPathChars );
		_ = _path
			.Setup( p => p.DirectorySeparatorChar )
			.Returns( '\\' );
		_ = _path
			.Setup( p => p.AltDirectorySeparatorChar )
			.Returns( '/' );
		_ = _path
			.Setup( p => p.GetInvalidFileNameChars() )
			.Returns( InvalidFileNameChars );
		_fileSystem = new Mock<System.IO.Abstractions.IFileSystem>( MockBehavior.Strict );
		_ = _fileSystem
			.Setup( fs => fs.Path )
			.Returns( _path.Object );
		_files = new DiskFileSystem(
			"Test",
			_fileSystem.Object,
			Root
		);
	}

	[TearDown]
	public void TearDown() {
		_path.VerifyAll();
		_fileSystem.VerifyAll();
	}

	[Test]
	public void GetRootId_ValidReader_RootReturned() {
		Assert.That( _files.GetRoot().FolderId, Is.EqualTo( Root ) );
	}
}
