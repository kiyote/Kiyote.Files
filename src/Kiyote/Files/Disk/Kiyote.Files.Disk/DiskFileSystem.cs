using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Kiyote.Files.Disk;

internal sealed class DiskFileSystem : IReadWriteFileSystem {

	private readonly IFileSystem _fileSystem;
	private readonly string _root;
	private readonly string _fileSystemId;
	private readonly FolderId _rootFolder;

	public DiskFileSystem(
		IFileSystem fileSystem,
		string fileSystemId,
		string root
	) {
		_fileSystem = fileSystem;
		_fileSystemId = fileSystemId;
		if( !root.EndsWith( Path.DirectorySeparatorChar ) ) {
			_root = root + Path.DirectorySeparatorChar;
		} else {
			_root = root;
		}
		_rootFolder = new FolderId( _fileSystemId, "\\" );
	}

	string IFileSystemIdentifier.Id => _fileSystemId;

	FolderId IReadOnlyFileSystem.Root => _rootFolder;

	async Task<FileMetadata> IReadOnlyFileSystem.GetMetadataAsync(
		FileId fileId,
		CancellationToken cancellationToken
	) {
		string path = ToPath( fileId );
		string metadataFile = path + ".metadata";
		if( _fileSystem.File.Exists( metadataFile ) ) {
			using Stream fs = _fileSystem.FileStream.New(
				metadataFile,
				FileMode.Open,
				FileAccess.Read,
				FileShare.Read
			);
			FileMetadata? metadata = await JsonSerializer.DeserializeAsync<FileMetadata>(
				fs,
				JsonSerializerOptions.Default,
				cancellationToken
			) ?? throw new InvalidOperationException( "Unable to locate metadata for file." );
			return metadata;
		} else {
			IFileInfo fi = _fileSystem.FileInfo.New( path );
			return
				new FileMetadata(
					fileId,
					fi.Name,
					MimeTypes.GetMimeType( fi.Extension ),
					fi.Length,
					fi.CreationTimeUtc
				);
		}

		throw new InvalidOperationException( "Unable to locate metadata for file." );
	}

	async Task<FileId> IReadWriteFileSystem.PutContentAsync(
		Func<Stream, CancellationToken, Task> asyncWriter,
		CancellationToken cancellationToken
	) {
		FileId fileId = CreateFileId();
		string path = ToPath( fileId );
		string directory = _fileSystem.Path.GetDirectoryName( path ) ?? throw new DirectoryNotFoundException();
		_ = _fileSystem.Directory.CreateDirectory( directory! );
		using Stream fs = _fileSystem.FileStream.New(
			path,
			FileMode.CreateNew,
			FileAccess.Write,
			FileShare.None
		);
		await asyncWriter( fs, cancellationToken );

		return fileId;
	}

	async Task<T> IReadOnlyFileSystem.GetContentAsync<T>(
		FileId fileId,
		Func<Stream, CancellationToken, Task<T>> contentReader,
		CancellationToken cancellationToken
	) {
		if( !IsValidFileId( fileId ) ) {
			throw new InvalidOperationException( "File identifier for different file system." );
		}
		string path = ToPath( fileId );
		using Stream fs = _fileSystem.FileStream.New(
			path,
			FileMode.Open,
			FileAccess.Read,
			FileShare.Read
		);
		return await contentReader( fs, cancellationToken );
	}

	async Task IReadWriteFileSystem.RenameFileAsync(
		FileId fileId,
		string name,
		CancellationToken cancellationToken
	) {
		if( !IsValidFileId( fileId ) ) {
			throw new InvalidOperationException( "File identifier for different file system." );
		}
		FileMetadata oldMetadata = await ( this as IReadOnlyFileSystem ).GetMetadataAsync( fileId, cancellationToken );
		FileMetadata newMetadata = oldMetadata with {
			Name = name
		};
		await PutMetadataAsync(
			fileId,
			newMetadata,
			cancellationToken
		);
	}

	IEnumerable<FileId> IReadOnlyFileSystem.GetFilesInFolder(
		FolderId folderId
	) {
		if( !IsValidFolderId( folderId ) ) {
			throw new InvalidOperationException( "Folder identifier for different file system." );
		}
		string path = ToPath( folderId );
		foreach( string fileName in _fileSystem.Directory.EnumerateFiles( path ) ) {
			yield return ToFileId( folderId, fileName );
		}
	}

	IEnumerable<FolderId> IReadOnlyFileSystem.GetFoldersInFolder(
		FolderId folderId
	) {
		if( !IsValidFolderId( folderId ) ) {
			throw new InvalidOperationException( "Folder identifier for different file system." );
		}
		string path = ToPath( folderId );
		foreach( string folderName in _fileSystem.Directory.EnumerateDirectories( path ) ) {
			yield return ToFolderId( folderId, folderName );
		}
	}

	private async Task PutMetadataAsync(
		FileId fileId,
		FileMetadata metadata,
		CancellationToken cancellationToken
	) {
		string path = ToPath( fileId );
		string metadataFile = path + ".metadata";
		using Stream fs = _fileSystem.FileStream.New(
			metadataFile,
			FileMode.Create,
			FileAccess.Write,
			FileShare.None
		);
		await JsonSerializer.SerializeAsync(
			fs,
			metadata,
			JsonSerializerOptions.Default,
			cancellationToken
		);
	}

	private string ToPath(
		FileId fileId
	) {
		string result = Path.Combine( _root, fileId.Id[ 1.. ] );
		if( Path.DirectorySeparatorChar != '\\' ) {
			result = result.ToString().Replace( '\\', Path.DirectorySeparatorChar );
		}
		return result;
	}

	private string ToPath(
		FolderId folderId
	) {
		string result = Path.Combine( _root, folderId.Id[ 1.. ] );
		if( Path.DirectorySeparatorChar != '\\' ) {
			result = result.ToString().Replace( '\\', Path.DirectorySeparatorChar );
		}
		return result;
	}

	private FileId ToFileId(
		FolderId folderId,
		string name
	) {
		string id = folderId.Id + name;
		return new FileId( _fileSystemId, id );
	}

	private FolderId ToFolderId(
		FolderId folderId,
		string name
	) {
		string id = folderId.Id + name + '\\';
		return new FolderId( _fileSystemId, id );
	}

	private FileId CreateFileId() {
		byte[] bytes = Guid.NewGuid().ToByteArray();
		StringBuilder result = new StringBuilder();
		for( int i = 0; i < bytes.Length; i += 2 ) {
			ushort value = BitConverter.ToUInt16( bytes, i );
			_ = result
				.Append( '\\' )
				.Append( value.ToString( "X4", CultureInfo.InvariantCulture ) );
		}
		return new FileId( _fileSystemId, result.ToString() );
	}

	private bool IsValidFileId(
		FileId fileId
	) {
		return string.Equals( fileId.FileSystemId, _fileSystemId, StringComparison.Ordinal );
	}

	private bool IsValidFolderId(
		FolderId folderId
	) {
		return string.Equals( folderId.FileSystemId, _fileSystemId, StringComparison.Ordinal );
	}

}
