namespace Kiyote.Files;

public interface IFilesReader {

	Task<TFileContent> GetContentAsync<TFileContent>(
		FileId fileId,
		Func<Stream, CancellationToken, Task<TFileContent>> contentReader,
		CancellationToken cancellationToken
	);

	Task<FileMetadata> GetMetadataAsync(
		FileId fileId,
		CancellationToken cancellationToken
	);

}
