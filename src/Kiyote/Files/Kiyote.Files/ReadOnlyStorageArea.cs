namespace Kiyote.Files.Disk;

internal sealed class ReadOnlyStorageArea : IReadOnlyStorageArea {

	private readonly string _storageAreaId;
	private readonly IReadOnlyFileSystem _fileSystem;

	public ReadOnlyStorageArea(
		string storageAreaId,
		IReadOnlyFileSystem fileSystem
	) {
		_storageAreaId = storageAreaId;
		_fileSystem = fileSystem;
	}

	string IStorageAreaIdentifier.StorageAreaId => _storageAreaId;

	IReadOnlyFileSystem IReadOnlyStorageArea.Read => _fileSystem;
}
