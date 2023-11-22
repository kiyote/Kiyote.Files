namespace Kiyote.Files.Disk;

internal sealed class ReadWriteStorageArea : IReadWriteStorageArea {

	private readonly string _storageAreaId;
	private readonly IReadWriteFileSystem _mutableFileSystem;

	public ReadWriteStorageArea(
		string storageAreaId,
		IReadWriteFileSystem mutableFileSystem
	) {
		_storageAreaId = storageAreaId;
		_mutableFileSystem = mutableFileSystem;
	}

	string IStorageAreaIdentifier.StorageAreaId => _storageAreaId;

	IReadWriteFileSystem IReadWriteStorageArea.ReadWrite => _mutableFileSystem;

	IReadOnlyFileSystem IReadOnlyStorageArea.Read => _mutableFileSystem;
}
