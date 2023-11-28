namespace Kiyote.Files;

public interface IFilesWriter {

	Task<FileId> PutContentAsync(
		Func<Stream, CancellationToken, Task> asyncWriter,
		CancellationToken cancellationToken
	);

	Task RenameFileAsync(
		FileId fileId,
		string name,
		CancellationToken cancellationToken
	);

}
