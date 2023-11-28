namespace Kiyote.Files;

public interface IReadOnlyFileSystem: IFilesReader, IFoldersReader {
}

public interface IReadOnlyFileSystem<T> : IReadOnlyFileSystem {
}
