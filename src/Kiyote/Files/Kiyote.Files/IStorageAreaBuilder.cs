namespace Kiyote.Files;

public interface IStorageAreaBuilder {

	IStorageAreaBuilder AddReadWrite(
		IReadWriteStorageArea storageArea
	);

	IStorageAreaBuilder AddReadWrite(
		string storageAreaId,
		IReadWriteFileSystem fileSystem
	);

	IStorageAreaBuilder AddReadOnly(
		IReadOnlyStorageArea storageArea
	);

	IStorageAreaBuilder AddReadOnly(
		string storageAreaId,
		IReadOnlyFileSystem fileSystem
	);

	IStorageAreaProvider Build();

}
