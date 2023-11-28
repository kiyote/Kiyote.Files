namespace Kiyote.Files.Resource;

public sealed class ResourceFilesReader : IFilesReader {

	private readonly ConfiguredResourceFileSystem _config;

	public ResourceFilesReader(
		ConfiguredResourceFileSystem config
	) {
		_config = config;
	}

	string IFilesReader.FileSystemId => _config.FileSystemId;

	async Task<TFileContent> IFilesReader.GetContentAsync<TFileContent>(
		FileId fileId,
		Func<Stream, CancellationToken, Task<TFileContent>> contentReader,
		CancellationToken cancellationToken
	) {
		try {
			using Stream? stream = _config.Assembly.GetManifestResourceStream( fileId.Id ) ?? throw new FileNotFoundException();
			return await contentReader( stream, cancellationToken );
		} catch( Exception ex ) {
			throw new InvalidOperationException( "Unable to open stream for file.", ex );
		}
	}

	Task<FileMetadata> IFilesReader.GetMetadataAsync(
		FileId fileId,
		CancellationToken cancellationToken
	) {
		try {
			return Task.FromResult( new FileMetadata(
				fileId,
				"",
				"",
				0L,
				File.GetCreationTimeUtc( _config.Assembly.Location )
			) );
		} catch( Exception ex ) {
			throw new InvalidOperationException( "Unable to read metadata for file.", ex );
		}
	}
}
