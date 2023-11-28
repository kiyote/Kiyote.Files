namespace Kiyote.Files;

public interface IReadOnlyFileSystem: IFilesReader, IFoldersReader {

	public string FileSystemId { get; }

}

public interface IReadOnlyFileSystem<T> : IReadOnlyFileSystem {
}
