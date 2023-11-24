namespace Kiyote.Files;

public interface IFileSystemFactory {
	IReadOnlyFileSystem<T> Create<T>( IReadOnlyFileSystem fileSystem );

	IReadWriteFileSystem<T> Create<T>( IReadWriteFileSystem fileSystem );

}
