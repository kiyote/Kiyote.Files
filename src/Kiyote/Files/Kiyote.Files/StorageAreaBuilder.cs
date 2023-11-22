using Kiyote.Files.Disk;

namespace Kiyote.Files;

internal sealed class StorageAreaBuilder : IStorageAreaBuilder {

	private readonly Dictionary<string, IReadWriteStorageArea> _mutable;
	private readonly Dictionary<string, IReadOnlyStorageArea> _immutable;

	public StorageAreaBuilder() {
		_mutable = [];
		_immutable = [];
	}

	IStorageAreaBuilder IStorageAreaBuilder.AddReadOnly(
		IReadOnlyStorageArea storageArea
	) {
		if( _immutable.ContainsKey( storageArea.StorageAreaId ) ) {
			throw new ArgumentException( $"Storage area '{storageArea.StorageAreaId}' already registered", nameof( storageArea ) );
		}
		_immutable[ storageArea.StorageAreaId ] = storageArea;

		return this;
	}

	IStorageAreaBuilder IStorageAreaBuilder.AddReadOnly(
		string storageAreaId,
		IReadOnlyFileSystem fileSystem
	) {
		IReadOnlyStorageArea storageArea = new ReadOnlyStorageArea( storageAreaId, fileSystem );
		_ = ( this as IStorageAreaBuilder ).AddReadOnly( storageArea );

		return this;
	}

	IStorageAreaBuilder IStorageAreaBuilder.AddReadWrite(
		IReadWriteStorageArea storageArea
	) {
		if( _immutable.ContainsKey( storageArea.StorageAreaId )
			|| _mutable.ContainsKey( storageArea.StorageAreaId )
		) {
			throw new ArgumentException( $"Storage area '{storageArea.StorageAreaId}' already registered", nameof( storageArea ) );
		}
		_mutable[ storageArea.StorageAreaId ] = storageArea;
		_immutable[ storageArea.StorageAreaId ] = storageArea;

		return this;
	}

	IStorageAreaBuilder IStorageAreaBuilder.AddReadWrite(
		string storageAreaId,
		IReadWriteFileSystem fileSystem
	) {
		IReadWriteStorageArea storageArea = new ReadWriteStorageArea( storageAreaId, fileSystem );
		_ = ( this as IStorageAreaBuilder ).AddReadWrite( storageArea );

		return this;
	}

	IStorageAreaProvider IStorageAreaBuilder.Build() {
		return new StorageAreaProvider(
			_immutable,
			_mutable
		);
	}
}
