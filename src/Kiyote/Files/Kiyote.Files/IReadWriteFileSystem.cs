namespace Kiyote.Files;

public interface IReadWriteFileSystem: IReadOnlyFileSystem {

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

public interface IReadWriteFileSystem<T>: IReadWriteFileSystem {
}
