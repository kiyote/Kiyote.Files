namespace Kiyote.Files;

public interface IReadOnlyFileSystem : IFileSystemIdentifier {

	Task<bool> TryGetContentAsync(
		FileId fileId,
		Func<Stream, Task> contentReader,
		CancellationToken cancellationToken
	);

	Task<FileMetadata> GetMetadataAsync(
		FileId fileId,
		CancellationToken cancellationToken
	);

}
