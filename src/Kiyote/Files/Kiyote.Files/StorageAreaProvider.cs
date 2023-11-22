namespace Kiyote.Files;

internal sealed class StorageAreaProvider : IStorageAreaProvider {

	private readonly IDictionary<string, IReadOnlyStorageArea> _readable;
	private readonly IDictionary<string, IReadWriteStorageArea> _readWrite;

	public StorageAreaProvider(
		IDictionary<string, IReadOnlyStorageArea> readable,
		IDictionary<string, IReadWriteStorageArea> readWrite
	) {
		_readable = readable;
		_readWrite = readWrite;
	}

	IReadOnlyStorageArea IStorageAreaProvider.GetReadableStorageArea(
		FileId fileId
	) {
		if( _readable.TryGetValue( fileId.StorageAreaId, out IReadOnlyStorageArea? value ) ) {
			return value;
		}
		throw new ArgumentException( $"Storage area '{fileId.StorageAreaId}' not registered.", nameof( fileId ) );
	}

	IReadOnlyStorageArea IStorageAreaProvider.GetReadableStorageArea(
		FolderId folderId
	) {
		if( _readable.TryGetValue( folderId.StorageAreaId, out IReadOnlyStorageArea? value ) ) {
			return value;
		}
		throw new ArgumentException( $"Storage area '{folderId.StorageAreaId}' not registered.", nameof( folderId ) );
	}

	IReadOnlyStorageArea IStorageAreaProvider.GetReadableStorageArea(
		string storageAreaId
	) {
		if( _readable.TryGetValue( storageAreaId, out IReadOnlyStorageArea? value ) ) {
			return value;
		}
		throw new ArgumentException( $"Storage area '{storageAreaId}' not registered.", nameof( storageAreaId ) );
	}

	IReadWriteStorageArea IStorageAreaProvider.GetReadWriteStorageArea(
		FileId fileId
	) {
		if( _readWrite.TryGetValue( fileId.StorageAreaId, out IReadWriteStorageArea? value ) ) {
			return value;
		}
		throw new ArgumentException( $"Storage area '{fileId.StorageAreaId}' not registered.", nameof( fileId ) );
	}

	IReadWriteStorageArea IStorageAreaProvider.GetReadWriteStorageArea(
		FolderId folderId
	) {
		if( _readWrite.TryGetValue( folderId.StorageAreaId, out IReadWriteStorageArea? value ) ) {
			return value;
		}
		throw new ArgumentException( $"Storage area '{folderId.StorageAreaId}' not registered.", nameof( folderId ) );
	}

	IReadWriteStorageArea IStorageAreaProvider.GetReadWriteStorageArea(
		string storageAreaId
	) {
		if( _readWrite.TryGetValue( storageAreaId, out IReadWriteStorageArea? value ) ) {
			return value;
		}
		throw new ArgumentException( $"Storage area '{storageAreaId}' not registered.", nameof( storageAreaId ) );
	}

	bool IStorageAreaProvider.ReadableStorageAreaExists(
		string storageAreaId
	) {
		return _readable.ContainsKey( storageAreaId );
	}

	bool IStorageAreaProvider.ReadWriteStorageAreaExists(
		string storageAreaId
	) {
		return _readWrite.ContainsKey( storageAreaId );
	}
}
