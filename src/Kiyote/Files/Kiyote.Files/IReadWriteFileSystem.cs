namespace Kiyote.Files;

public interface IReadWriteFileSystem: IReadOnlyFileSystem {

	Task PutContentAsync(
		FileId fileId,
		Func<Stream, Task> asyncWriter,
		CancellationToken cancellationToken
	);

}

