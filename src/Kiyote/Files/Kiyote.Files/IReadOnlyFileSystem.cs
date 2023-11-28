namespace Kiyote.Files;

public interface IReadOnlyFileSystem: IFilesReader, IFoldersReader, IFileSystemIdentifier {
}

public interface IReadOnlyFileSystem<T> : IReadOnlyFileSystem {
}
