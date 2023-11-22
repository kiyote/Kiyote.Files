namespace Kiyote.Files;

public interface IReadWriteStorageArea: IReadOnlyStorageArea {

	IReadWriteFileSystem ReadWrite { get; }

}
