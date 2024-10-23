using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Kiyote.Files.Virtual;
using Moq;

namespace Kiyote.Files.Disk.UnitTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public sealed class DiskVirtualPathMapperTests {

	public const string VirtualRoot = @"/root/";
	public const string PhysicalRoot = @"DRIVE:\PATH";
	public const char Separator = '\\';
	public const char VirtualSeparator = '/';
	public static readonly char[] InvalidPathChars = [ '/', '\\' ];
	public static readonly char[] InvalidFileNameChars = [ '/', '\\', ':' ];

	private Mock<System.IO.Abstractions.IFileSystem> _fileSystem;
	private Mock<IPath> _path;
	private IVirtualPathMapper _pathMapper;

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
		DiskFileSystem fileSystem = new DiskFileSystem(
			"Test",
			_fileSystem.Object,
			PhysicalRoot
		);
		_pathMapper = new DiskVirtualPathMapper(
			fileSystem,
			VirtualRoot,
			VirtualSeparator
		);
	}

	[TearDown]
	public void TearDown() {
		_path.VerifyAll();
		_fileSystem.VerifyAll();
	}

	[Test]
	public void TryMapFromVirtual_ValidFolderId_Success() {
		bool result = _pathMapper.TryMapFromVirtual(
			VirtualRoot,
			out FolderIdentifier mappedFolderIdentifier
		);

		Assert.That( result, Is.True );
		Assert.That( mappedFolderIdentifier.FolderId.ToString(), Is.EqualTo( "" ) );
	}
}
