namespace Kiyote.Files;

public interface IStorageAreaProvider {

	bool ReadableStorageAreaExists(
		string storageAreaId
	);

	IReadOnlyStorageArea GetReadableStorageArea(
		FileId fileId
	);

	IReadOnlyStorageArea GetReadableStorageArea(
		FolderId folderId
	);

	IReadOnlyStorageArea GetReadableStorageArea(
		string storageAreaId
	);

	bool ReadWriteStorageAreaExists(
		string storageAreaId
	);

	IReadWriteStorageArea GetReadWriteStorageArea(
		FileId fileId
	);

	IReadWriteStorageArea GetReadWriteStorageArea(
		FolderId folderId
	);

	IReadWriteStorageArea GetReadWriteStorageArea(
		string storageAreaId
	);

}
