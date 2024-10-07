using System.Buffers;

namespace Kiyote.Files.Disk;

internal sealed class DiskFileSystem : IFileSystem {

	private readonly string _rootPath;
	private readonly FolderIdentifier _root;
	private readonly char[] _invalidPathChars;
	private readonly char[] _invalidFileNameChars;
	private readonly SearchValues<char> _searchPathChars;
	private readonly SearchValues<char> _searchFileNameChars;
	private readonly List<string> _invalidNames;

	public DiskFileSystem(
		FileSystemId fileSystemId,
		System.IO.Abstractions.IFileSystem fileSystem,
		string rootPath
	) {
		FileSystemId = fileSystemId;
		FileSystem = fileSystem;
		_rootPath = rootPath;
		_root = new FolderIdentifier( FileSystemId, rootPath );
		_invalidPathChars = [
			.. FileSystem.Path.GetInvalidPathChars(),
			FileSystem.Path.DirectorySeparatorChar,
			FileSystem.Path.AltDirectorySeparatorChar
		];
		_searchPathChars = SearchValues.Create( _invalidPathChars );

		_invalidFileNameChars = FileSystem.Path.GetInvalidFileNameChars();
		_searchFileNameChars = SearchValues.Create( _invalidFileNameChars );

		_invalidNames = [
			".",
			"..",
			"AUX",
			"CLOCK$",
			"COM1",
			"COM2",
			"COM3",
			"COM4",
			"COM5",
			"COM6",
			"COM7",
			"COM8",
			"COM9",
			"CON",
			"LPT1",
			"LPT2",
			"LPT3",
			"LPT4",
			"LPT5",
			"LPT6",
			"LPT7",
			"LPT8",
			"LPT9",
			"NUL",
			"PRN"
		];
	}

	internal System.IO.Abstractions.IFileSystem FileSystem { get; }

	internal FileSystemId FileSystemId { get; }

	async Task<FileIdentifier> IFileSystem.CreateFileAsync(
		FolderIdentifier folderIdentifier,
		string fileName,
		Func<Stream, CancellationToken, Task> contentWriter,
		CancellationToken cancellationToken
	) {
		if( fileName.AsSpan().ContainsAny( _searchFileNameChars )
			|| _invalidNames.BinarySearch( fileName, StringComparer.OrdinalIgnoreCase ) >= 0
		) {
			throw new InvalidNameException();
		}

		FileId fileId = ToFileId( folderIdentifier.FolderId, fileName );
		string path = ToPhysicalPath( fileId );
		try {
			using Stream fs = FileSystem.FileStream.New(
				path,
				FileMode.CreateNew,
				FileAccess.Write,
				FileShare.None
			);
			await contentWriter( fs, cancellationToken );

			return new FileIdentifier( FileSystemId, fileId );
		} catch( DirectoryNotFoundException ) {
			throw new FolderNotFoundException();
		} catch( Exception ex ) {
			throw new ContentUnavailableException( "Unable to write content.", ex );
		}
	}

	FolderIdentifier IFileSystem.CreateFolder(
		FolderIdentifier folderIdentifier,
		string folderName
	) {
		if( folderName.AsSpan().ContainsAny( _searchPathChars )
			|| _invalidNames.BinarySearch( folderName, StringComparer.OrdinalIgnoreCase ) >= 0
		) {
			throw new InvalidNameException();
		}

		FolderId newFolderId = ToFolderId( folderIdentifier.FolderId, folderName );
		string path = ToPhysicalPath( newFolderId );
		if( FileSystem.Path.Exists( path ) ) {
			throw new FolderExistsException();
		}

		_ = FileSystem.Directory.CreateDirectory( path );
		return new FolderIdentifier( FileSystemId, newFolderId );
	}

	void IFileSystem.DeleteFolder(
		FolderIdentifier folderIdentifier
	) {
		string path = ToPhysicalPath( folderIdentifier.FolderId );
		FileSystem.Directory.Delete( path, true );
	}

	async Task IReadOnlyFileSystem.GetContentAsync(
		FileIdentifier fileIdentifier,
		Func<Stream, CancellationToken, Task> contentReader,
		CancellationToken cancellationToken
	) {
		string path = ToPhysicalPath( fileIdentifier.FileId );
		try {
			using Stream fs = FileSystem.FileStream.New(
				path,
				FileMode.Open,
				FileAccess.Read,
				FileShare.Read
			);
			await contentReader( fs, cancellationToken );
		} catch( Exception ex ) {
			throw new ContentUnavailableException( "Unable to open stream for file.", ex );
		}
	}

	IEnumerable<FileIdentifier> IReadOnlyFileSystem.GetFileIdentifiers(
		FolderIdentifier folderIdentifier
	) {
		string path = ToPhysicalPath( folderIdentifier.FolderId );
		IEnumerable<string> files = FileSystem.Directory.EnumerateFiles( path );

		foreach( string file in files ) {
			int last = file.AsSpan().LastIndexOf( FileSystem.Path.DirectorySeparatorChar ) + 1;
			ReadOnlySpan<char> fileName = file.AsSpan()[ last.. ];
			yield return new FileIdentifier( FileSystemId, ToFileId( folderIdentifier.FolderId, fileName ) );
		}
	}

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers() {
		return ( this as IReadOnlyFileSystem ).GetFolderIdentifiers( _root );
	}

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers(
		FolderIdentifier folderIdentifier
	) {
		string physicalPath = ToPhysicalPath( folderIdentifier.FolderId );
		IEnumerable<string> names = FileSystem.Directory.EnumerateDirectories( physicalPath );

		foreach( string name in names ) {
			int last = name.AsSpan().LastIndexOf( FileSystem.Path.DirectorySeparatorChar ) + 1;
			ReadOnlySpan<char> folderName = name.AsSpan()[ last.. ];
			yield return new FolderIdentifier( FileSystemId, ToFolderId( folderIdentifier.FolderId, folderName ) );
		}
	}

	IEnumerable<char> IFileSystem.GetInvalidPathChars() {
		return _invalidPathChars;
	}

	IEnumerable<char> IFileSystem.GetInvalidFileNameChars() {
		return _invalidFileNameChars;
	}

	IEnumerable<string> IFileSystem.GetInvalidNames() {
		return _invalidNames;
	}

	FolderIdentifier IReadOnlyFileSystem.GetRoot() {
		return _root;
	}

	FolderIdentifier IReadOnlyFileSystem.GetFolderIdentifier(
		string folderName
	) {
		FolderId folderId = ToFolderId(
				_root.FolderId,
				folderName
			);
		string physicalPath = ToPhysicalPath( folderId );
		if (FileSystem.Directory.Exists( physicalPath )) {
			return new FolderIdentifier(
				FileSystemId,
				folderId
			);
		}
		throw new FolderNotFoundException();
	}

	private string ToPhysicalPath(
		FolderId folderId
	) {
		if( folderId == _root.FolderId ) {
			return _rootPath;
		}

		return Path.Combine( _rootPath, folderId );
	}

	private string ToPhysicalPath(
		FileId fileId
	) {
		return Path.Combine( _rootPath, fileId );
	}

	private FileId ToFileId(
		FolderId folderId,
		string fileName
	) {
		if( folderId == _root.FolderId ) {
			return fileName;
		}
		return $"{folderId}{fileName}";
	}

	private FileId ToFileId(
		FolderId folderId,
		ReadOnlySpan<char> fileName
	) {
		if( folderId == _root.FolderId ) {
			return $"{fileName}";
		}
		return $"{folderId}{fileName}";
	}

	private FolderId ToFolderId(
		FolderId folderId,
		string folderName
	) {
		if( folderId == _root.FolderId ) {
			return $"{folderName}{FileSystem.Path.DirectorySeparatorChar}";
		}
		return $"{folderId}{folderName}{FileSystem.Path.DirectorySeparatorChar}";
	}

	private FolderId ToFolderId(
		FolderId folderId,
		ReadOnlySpan<char> folderName
	) {
		if( folderId == _root.FolderId ) {
			return $"{folderName}{FileSystem.Path.DirectorySeparatorChar}";
		}
		return $"{folderId}{folderName}{FileSystem.Path.DirectorySeparatorChar}";
	}
}
