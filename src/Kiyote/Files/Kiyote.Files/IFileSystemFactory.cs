namespace Kiyote.Files;

public interface IFileSystemFactory {
	IReadOnlyFileSystem<T> Create<T>( IReadOnlyFileSystem fileSystem ) where T: IFileSystemIdentifier;

	IReadWriteFileSystem<T> Create<T>( IReadWriteFileSystem fileSystem ) where T: IFileSystemIdentifier;

}
