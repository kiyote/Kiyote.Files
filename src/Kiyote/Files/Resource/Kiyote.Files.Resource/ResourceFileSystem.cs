namespace Kiyote.Files.Resource;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in DI" )]
internal sealed class ResourceFileSystem : IReadOnlyFileSystem {

	private readonly IFilesReader _filesReader;
	private readonly IFoldersReader _foldersReader;
	private readonly string _fileSystemId;

	public ResourceFileSystem(
		string fileSystemId,
		IFilesReader filesReader,
		IFoldersReader foldersReader
	) {
		_fileSystemId = fileSystemId;
		_filesReader = filesReader;
		_foldersReader = foldersReader;
	}

	FolderId IFoldersReader.Root => _foldersReader.Root;

	FileSystemIdentifier IFilesReader.Id => _filesReader.Id;

	FileSystemIdentifier IFoldersReader.Id => _foldersReader.Id;

	string IReadOnlyFileSystem.FileSystemId => _fileSystemId;

	Task<TFileContent> IFilesReader.GetContentAsync<TFileContent>(
		FileId fileId,
		Func<Stream, CancellationToken, Task<TFileContent>> contentReader,
		CancellationToken cancellationToken
	) {
		return _filesReader.GetContentAsync( fileId, contentReader, cancellationToken );
	}

	IEnumerable<FileId> IFoldersReader.GetFilesInFolder(
		FolderId folderId
	) {
		return _foldersReader.GetFilesInFolder( folderId );
	}

	IEnumerable<FolderId> IFoldersReader.GetFoldersInFolder(
		FolderId folderId
	) {
		return _foldersReader.GetFoldersInFolder( folderId );
	}

	Task<FileMetadata> IFilesReader.GetMetadataAsync(
		FileId fileId,
		CancellationToken cancellationToken
	) {
		return _filesReader.GetMetadataAsync( fileId, cancellationToken );
	}
}
