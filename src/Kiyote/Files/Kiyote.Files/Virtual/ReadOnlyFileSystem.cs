namespace Kiyote.Files.Virtual;

internal sealed class ReadOnlyFileSystem : IReadOnlyFileSystem {
	Task IReadOnlyFileSystem.GetContentAsync(
		FileIdentifier fileIdentifier,
		Func<Stream, CancellationToken, Task> contentReader,
		CancellationToken cancellationToken
	) {
		throw new NotImplementedException();
	}

	IEnumerable<FileIdentifier> IReadOnlyFileSystem.GetFileIdentifiers(
		FolderIdentifier folderIdentifier
	) {
		throw new NotImplementedException();
	}

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers() {
		throw new NotImplementedException();
	}

	IEnumerable<FolderIdentifier> IReadOnlyFileSystem.GetFolderIdentifiers(
		FolderIdentifier folderIdentifier
	) {
		throw new NotImplementedException();
	}

	FolderIdentifier IReadOnlyFileSystem.GetRoot() {
		throw new NotImplementedException();
	}
}
