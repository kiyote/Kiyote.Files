using System.Text.Json;

namespace Kiyote.Files.Disk;

public sealed class DiskFilesReader : IFilesReader {

	private readonly IFileSystem _fileSystem;
	private readonly DiskFileSystemConfiguration _config;

	public DiskFilesReader(
		DiskFileSystemConfiguration config
	) : this( new FileSystem(), config ) {
	}

	public DiskFilesReader(
		IFileSystem fileSystem,
		DiskFileSystemConfiguration config
	) {
		_fileSystem = fileSystem;
		_config = config;
	}

	FileSystemIdentifier IFilesReader.Id => _config.Id;

	async Task<TFileContent> IFilesReader.GetContentAsync<TFileContent>(
		FileId fileId,
		Func<Stream, CancellationToken, Task<TFileContent>> contentReader,
		CancellationToken cancellationToken
	) {
		_config.EnsureValidId( fileId );
		string path = _config.ToPath( fileId );
		try {
			using Stream fs = _fileSystem.FileStream.New(
				path,
				FileMode.Open,
				FileAccess.Read,
				FileShare.Read
			);
			return await contentReader( fs, cancellationToken );
		} catch( Exception ex ) {
			throw new InvalidOperationException( "Unable to open stream for file.", ex );
		}
	}

	async Task<FileMetadata> IFilesReader.GetMetadataAsync(
		FileId fileId,
		CancellationToken cancellationToken
	) {
		_config.EnsureValidId( fileId );
		string path = _config.ToPath( fileId );
		string metadataFile = path + ".metadata";
		try {
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
				) ?? throw new InvalidOperationException( "Unable to deserialize metadata file." );
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
		} catch( Exception ex ) {
			throw new InvalidOperationException( "Unable to read metadata for file.", ex );
		}
		throw new InvalidOperationException( "Unable to locate metadata for file." );
	}
}
