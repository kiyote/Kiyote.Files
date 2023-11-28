namespace Kiyote.Files.Disk;

public sealed record DiskFileSystemConfiguration(
	string FileSystemId,
	string RootFolder
) {
	public const string DiskFileSystemType = "Disk";

	public void EnsureValidId(
		FileId fileId
	) {
		if( !IsValidId( fileId ) ) {
			throw new ArgumentException(
				"File identifier for different file system.",
				nameof( fileId )
			);
		}
	}

	public bool IsValidId(
		FileId fileId
	) {
		ArgumentNullException.ThrowIfNull( fileId );
		return string.CompareOrdinal( FileSystemId, fileId.FileSystemId ) == 0;
	}

	public void EnsureValidId(
		FolderId folderId
	) {
		if( !IsValidId( folderId ) ) {
			throw new ArgumentException(
				"File identifier for different file system.",
				nameof( folderId )
			);
		}
	}

	public bool IsValidId(
		FolderId folderId
	) {
		ArgumentNullException.ThrowIfNull( folderId );
		return string.CompareOrdinal( FileSystemId, folderId.FileSystemId ) == 0;
	}

	public string ToPath(
		FileId fileId
	) {
		ArgumentNullException.ThrowIfNull( fileId );
		string result = Path.Combine( RootFolder, fileId.Id[ 1.. ] );
		if( Path.DirectorySeparatorChar != '\\' ) {
			result = result.ToString().Replace( '\\', Path.DirectorySeparatorChar );
		}
		return result;
	}

	public string ToPath(
		FolderId folderId
	) {
		ArgumentNullException.ThrowIfNull( folderId );
		string result = Path.Combine( RootFolder, folderId.Id[ 1.. ] );
		if( Path.DirectorySeparatorChar != '\\' ) {
			result = result.ToString().Replace( '\\', Path.DirectorySeparatorChar );
		}
		return result;
	}

	public FolderId ToFolderId(
		FolderId folderId,
		string name
	) {
		ArgumentNullException.ThrowIfNull( folderId );
		string id = folderId.Id + name + '\\';
		return new FolderId( FileSystemId, id );
	}

	public FileId ToFileId(
		FolderId folderId,
		string name
	) {
		ArgumentNullException.ThrowIfNull( folderId );
		if( string.IsNullOrWhiteSpace( name ) ) {
			throw new ArgumentNullException( nameof( name ) );
		}
		string id = folderId.Id + name;
		return new FileId( FileSystemId, id );
	}
}
