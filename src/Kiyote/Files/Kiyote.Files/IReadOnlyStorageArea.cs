namespace Kiyote.Files;

public interface IReadOnlyStorageArea: IStorageAreaIdentifier {

	IReadOnlyFileSystem Read { get; }

}
