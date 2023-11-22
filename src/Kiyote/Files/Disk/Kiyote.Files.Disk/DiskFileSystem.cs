using MimeTypes;

namespace Kiyote.Files.Disk;

internal sealed class DiskFileSystem : IDiskFileSystem {

	public const string FileSystemId = "Disk";

	private readonly IFileSystem _fileSystem;
	private readonly string _root;

	public DiskFileSystem(
		IFileSystem fileSystem,
		string root
	) {
		_fileSystem = fileSystem;
		_root = root;
	}

	string IFileSystemIdentifier.FileSystemId => FileSystemId;

	Task<FileMetadata> IReadOnlyFileSystem.GetMetadataAsync(
		FileId fileId,
		CancellationToken cancellationToken
	) {
		if( !IsValidFileId( fileId ) ) {
			throw new ArgumentException( "File identifier not for this file system.", nameof( fileId ) );
		}

		IFileInfo fi = _fileSystem.FileInfo.New( fileId.Id );

		return Task.FromResult(
			new FileMetadata(
				fileId,
				fi.Name,
				MimeTypeMap.GetMimeType( fi.Extension ),
				fi.Length,
				fi.CreationTimeUtc
			)
		);
	}

	async Task IReadWriteFileSystem.PutContentAsync(
		FileId fileId,
		Func<Stream, Task> asyncWriter,
		CancellationToken cancellationToken
	) {
		if( !IsValidFileId( fileId ) ) {
			throw new ArgumentException( "File identifier not for this file system.", nameof( fileId ) );
		}

		using Stream fs = _fileSystem.FileStream.New( fileId.Id, FileMode.Create, FileAccess.Write, FileShare.None );
		await asyncWriter( fs );
	}

	async Task<bool> IReadOnlyFileSystem.TryGetContentAsync(
		FileId fileId,
		Func<Stream, Task> contentReader,
		CancellationToken cancellationToken
	) {
		if( !IsValidFileId( fileId ) ) {
			throw new ArgumentException( "File identifier not for this file system.", nameof( fileId ) );
		}

		using Stream fs = _fileSystem.FileStream.New( fileId.Id, FileMode.Open, FileAccess.Read, FileShare.Read );
		await contentReader( fs );
		return true;
	}

	private bool IsValidFileId(
		FileId fileId
	) {
		if( !string.Equals( fileId.Id, FileSystemId, StringComparison.Ordinal ) ) {
			return false;
		}

		if( !fileId.Id.StartsWith( _root, StringComparison.OrdinalIgnoreCase ) ) {
			return false;
		}

		return true;
	}
}
