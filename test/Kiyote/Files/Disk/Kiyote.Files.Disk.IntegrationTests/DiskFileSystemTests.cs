namespace Kiyote.Files.Disk.IntegrationTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public sealed class DiskFileSystemTests {

	private IFileSystem _files;
	private System.IO.Abstractions.IFileSystem _fileSystem;
	private string _testFolder;

	[SetUp]
	public void SetUp() {
		_fileSystem = new FileSystem();
		string tempPath = _fileSystem.Path.GetTempPath();
		_ = _fileSystem.Directory.CreateDirectory( tempPath );
		_testFolder = _fileSystem.Path.Combine( tempPath, Guid.NewGuid().ToString( "N" ) );
		_ = _fileSystem.Directory.CreateDirectory( _testFolder );

		_files = new DiskFileSystem(
			"Test",
			_fileSystem,
			_testFolder
		);
	}

	[TearDown]
	public void TearDown() {
		_fileSystem.Directory.Delete( _testFolder, true );
	}

	[Test]
	public void GetFileIdentifiers_NoFiles_ReturnsNoFiles() {
		IEnumerable<FileIdentifier> files = _files.GetFileIdentifiers( _files.GetRoot() );

		Assert.That( files, Is.Empty );
	}

	[Test]
	public void GetFileIdentifiers_OneFile_ReturnsOneFile() {
		_fileSystem.File.WriteAllText( Path.Combine( _testFolder, "filename.txt" ), "contents" );

		IEnumerable<FileIdentifier> files = _files.GetFileIdentifiers( _files.GetRoot() );

		Assert.That( files.Count(), Is.EqualTo( 1 ) );
	}

	[Test]
	public async Task GetContentAsync_FileExists_ContentLoaded() {
		string expected = "contents";
		await _fileSystem.File.WriteAllTextAsync( Path.Combine( _testFolder, "filename.txt" ), expected );
		FileIdentifier fileIdentifier = _files.GetFileIdentifiers( _files.GetRoot() ).First();

		string actual = "";
		await _files.GetContentAsync(
			fileIdentifier,
			async ( Stream stream, CancellationToken token ) => {
				using TextReader reader = new StreamReader( stream );
				actual = await reader.ReadToEndAsync( token );
			},
			CancellationToken.None
		);

		Assert.That( actual, Is.EqualTo( expected ) );
	}

	[Test]
	public void GetContentAsync_FileDoesNotExist_ThrowsContentUnavailableException() {
		string expected = "contents";
		string path = Path.Combine( _testFolder, "filename.txt" );
		_fileSystem.File.WriteAllText( path, expected );
		FileIdentifier fileIdentifier = _files.GetFileIdentifiers( _files.GetRoot() ).First();
		_fileSystem.File.Delete( path );

		_ = Assert.ThrowsAsync<ContentUnavailableException>(
			() => _files.GetContentAsync(
				fileIdentifier,
				( Stream stream, CancellationToken token ) => {
					Assert.Fail( "Should not be trying to read content." );
					return Task.CompletedTask;
				},
				CancellationToken.None
			)
		 );
	}

	[Test]
	public async Task CreateFileAsync_ValidFile_FileWritten() {
		string fileName = "test.txt";
		string expected = "contents";
		FileIdentifier fileIdentifier = await _files.CreateFileAsync(
			_files.GetRoot(),
			fileName,
			async ( Stream stream, CancellationToken token ) => {
				using TextWriter writer = new StreamWriter( stream );
				await writer.WriteAsync( expected.AsMemory(), token );
			},
			CancellationToken.None
		);

		string actual = await _fileSystem.File.ReadAllTextAsync( Path.Combine( _testFolder, fileName ), CancellationToken.None );

		Assert.That( actual, Is.EqualTo( expected ) );
	}

	[Test]
	public void CreateFileAsync_DirectoryNotValue_ThrowsFolderNotFoundException() {
		FolderIdentifier folderIdentifier = _files.CreateFolder( _files.GetRoot(), "subfolder" );
		_files.DeleteFolder( folderIdentifier );

		string fileName = "test.txt";
		string expected = "contents";
		_ = Assert.ThrowsAsync<FolderNotFoundException>(
			() => _files.CreateFileAsync(
				folderIdentifier,
				fileName,
				async ( Stream stream, CancellationToken token ) => {
					using TextWriter writer = new StreamWriter( stream );
					await writer.WriteAsync( expected.AsMemory(), token );
				},
				CancellationToken.None
			)
		);
	}

	[Test]
	public void GetFolderIdentifiers_EmptyFolder_NoFoldersReturned() {
		IEnumerable<FolderIdentifier> folders = _files.GetFolderIdentifiers();

		Assert.That( folders.Any(), Is.False );
	}

	[Test]
	public void GetFolderIdentifiers_OneFolder_OneFolderReturned() {
		string folderName = Guid.NewGuid().ToString( "N" );
		string newFolder = Path.Combine( _testFolder, folderName );
		_ = _fileSystem.Directory.CreateDirectory( newFolder );

		IEnumerable<FolderIdentifier> folders = _files.GetFolderIdentifiers();

		Assert.That( folders.Count(), Is.EqualTo( 1 ) );
	}

	[Test]
	public void GetFolderIdentifiers_SubFolder_NoFoldersReturned() {
		string folderName = Guid.NewGuid().ToString( "N" );
		string newFolder = Path.Combine( _testFolder, folderName );
		_ = _fileSystem.Directory.CreateDirectory( newFolder );

		FolderIdentifier folderIdentifier = _files.GetFolderIdentifiers().First();
		IEnumerable<FolderIdentifier> folders = _files.GetFolderIdentifiers( folderIdentifier );

		Assert.That( folders.Any(), Is.False );
	}

	[Test]
	public void GetFolderIdentifiers_SubFolderWithChild_OneFoldersReturned() {
		string folderName = Guid.NewGuid().ToString( "N" );
		string newFolder = Path.Combine( _testFolder, folderName );
		_ = _fileSystem.Directory.CreateDirectory( newFolder );

		string subFolderName = Guid.NewGuid().ToString( "N" );
		string subFolder = Path.Combine( newFolder, subFolderName );
		_ = _fileSystem.Directory.CreateDirectory( subFolder );

		FolderIdentifier newFolderIdentifier = _files.GetFolderIdentifiers().First();

		IEnumerable<FolderIdentifier> folders = _files.GetFolderIdentifiers( newFolderIdentifier );

		Assert.That( folders.Count(), Is.EqualTo( 1 ) );
	}

	[Test]
	public void CreateFolder_ValidName_FolderCreated() {
		string folderName = "folder_name";

		Assert.That( _files.GetFolderIdentifiers().Count(), Is.EqualTo( 0 ) );

		FolderIdentifier folderIdentifier = _files.CreateFolder(
			_files.GetRoot(),
			folderName
		);

		Assert.That( folderIdentifier, Is.Not.EqualTo( _files.GetRoot() ) );
		Assert.That( _files.GetFolderIdentifiers().Count(), Is.EqualTo( 1 ) );
	}

	[Test]
	public void CreateFolder_InvalidCharacters_ThrowsInvalidCharactersException() {
		foreach( char c in _files.GetInvalidPathChars() ) {
			string folderName = $"te{c}st";

			_ = Assert.Throws<InvalidNameException>(
				() => _files.CreateFolder(
					_files.GetRoot(),
					folderName
				)
			);
		}
	}

	[Test]
	public void CreateFolder_FolderExists_ThrowsFolderExistsException() {
		string folderName = "folder_name";

		Assert.That( _files.GetFolderIdentifiers().Count(), Is.EqualTo( 0 ) );
		FolderIdentifier folderIdentifier = _files.CreateFolder(
			_files.GetRoot(),
			folderName
		);
		Assert.That( _files.GetFolderIdentifiers().Count(), Is.EqualTo( 1 ) );

		_ = Assert.Throws<FolderExistsException>(
			() => _files.CreateFolder(
				_files.GetRoot(),
				folderName
			)
		);
	}

	[Test]
	public void Delete_FolderExists_FolderIsDeleted() {
		string folderName = "folder_name";

		FolderIdentifier folderIdentifier = _files.CreateFolder(
			_files.GetRoot(),
			folderName
		);
		Assert.That( _files.GetFolderIdentifiers().Count(), Is.EqualTo( 1 ) );
		_files.DeleteFolder(
			folderIdentifier
		);
		Assert.That( _files.GetFolderIdentifiers().Count(), Is.EqualTo( 0 ) );
	}

}
