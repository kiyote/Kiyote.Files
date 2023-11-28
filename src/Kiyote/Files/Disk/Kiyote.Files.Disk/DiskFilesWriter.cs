using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Kiyote.Files.Disk;

public sealed class DiskFilesWriter : IFilesWriter {

	private readonly IFileSystem _fileSystem;
	private readonly DiskFileSystemConfiguration _config;
	private readonly IFilesReader _reader;

	public DiskFilesWriter(
		DiskFileSystemConfiguration config,
		IFilesReader reader
	) : this( new FileSystem(), config, reader ) {
	}

	public DiskFilesWriter(
		IFileSystem fileSystem,
		DiskFileSystemConfiguration config,
		IFilesReader reader
	) {
		ArgumentNullException.ThrowIfNull( fileSystem );
		ArgumentNullException.ThrowIfNull( config );
		ArgumentNullException.ThrowIfNull( reader );
		if( config.Id != reader.Id ) {
			throw new InvalidOperationException( "DiskFilesWriter must be attached to same file system reader." );
		}
		_fileSystem = fileSystem;
		_config = config;
		_reader = reader;
	}

	FileSystemIdentifier IFilesWriter.Id => _config.Id;

	async Task<FileId> IFilesWriter.PutContentAsync(
		Func<Stream, CancellationToken, Task> asyncWriter,
		CancellationToken cancellationToken
	) {
		FileId fileId = CreateFileId();
		string path = _config.ToPath( fileId );
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


	async Task IFilesWriter.RenameFileAsync(
		FileId fileId,
		string name,
		CancellationToken cancellationToken
	) {
		_config.EnsureValidId( fileId );
		FileMetadata oldMetadata = await _reader.GetMetadataAsync( fileId, cancellationToken );
		FileMetadata newMetadata = oldMetadata with {
			Name = name
		};
		await PutMetadataAsync(
			fileId,
			newMetadata,
			cancellationToken
		);
	}

	private async Task PutMetadataAsync(
		FileId fileId,
		FileMetadata metadata,
		CancellationToken cancellationToken
	) {
		string path = _config.ToPath( fileId );
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

	private FileId CreateFileId() {
		byte[] bytes = Guid.NewGuid().ToByteArray();
		StringBuilder result = new StringBuilder();
		for( int i = 0; i < bytes.Length; i += 2 ) {
			ushort value = BitConverter.ToUInt16( bytes, i );
			_ = result
				.Append( '\\' )
				.Append( value.ToString( "X4", CultureInfo.InvariantCulture ) );
		}
		return new FileId( _config.Id.FileSystemId, result.ToString() );
	}

}
