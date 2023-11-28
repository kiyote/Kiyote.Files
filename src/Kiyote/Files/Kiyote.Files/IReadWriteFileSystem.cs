namespace Kiyote.Files;

public interface IReadWriteFileSystem: IFilesWriter, IReadOnlyFileSystem {
}

public interface IReadWriteFileSystem<T>: IReadWriteFileSystem {
}
