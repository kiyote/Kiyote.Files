namespace Kiyote.Files.Virtual.UnitTests;

[TestFixture]
public sealed class VirtualFileSystemTests {

	private Mock<IReadOnlyFileSystem> _readOnly;
	private Mock<IVirtualPathMapper> _readOnlyPathMapper;
	private Mock<IVirtualPathHandler> _pathHandler;
	private VirtualFileSystem _virtualFileSystem;
	private IFileSystem _vfs;

	[SetUp]
	public void SetUp() {
		_readOnly = new Mock<IReadOnlyFileSystem>( MockBehavior.Strict );
		_readOnlyPathMapper = new Mock<IVirtualPathMapper>( MockBehavior.Strict );
		var readOnly = new Dictionary<FolderId, MappedFileSystem<IReadOnlyFileSystem>>();
		readOnly[ "ReadOnly" ] = new MappedFileSystem<IReadOnlyFileSystem>( "ro", _readOnlyPathMapper.Object, _readOnly.Object );

		_pathHandler = new Mock<IVirtualPathHandler>( MockBehavior.Strict );
		_ = _pathHandler
			.Setup( ph => ph.Separator )
			.Returns( '/' );

		_virtualFileSystem = new VirtualFileSystem(
			_pathHandler.Object,
			"Test",
			readOnly,
			[]
		);
		_vfs = _virtualFileSystem;
	}

	[TearDown]
	public void TearDown() {
		_readOnly.VerifyAll();
		_readOnlyPathMapper.VerifyAll();
		_pathHandler.VerifyAll();
	}

	[Test]
	public void GetFolderIdentifier_MountFolderOfFileSYstem_MountedFolderReturned() {
		_ = _pathHandler
			.Setup( ph => ph.Combine( "/", "ro", '/' ) )
			.Returns( $"/ro/" );

		FolderIdentifier result = new FolderIdentifier( "ReadOnly", "/" );
		_ = _readOnlyPathMapper
			.Setup( ropm => ropm.TryMapFromVirtual( "/ro/", out result ) )
			.Returns( true );

		FolderIdentifier fid = _vfs.GetFolderIdentifier( "ro" );

		Assert.That( fid.FileSystemId, Is.EqualTo( "Test" ) );
		Assert.That( fid.FolderId, Is.EqualTo( "/ro/" ) );
	}
}
