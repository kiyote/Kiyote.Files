namespace Kiyote.Files.Disk;

internal sealed class DiskFoldersReaderFactory : IDiskFoldersReaderFactory {
	DiskFoldersReader IDiskFoldersReaderFactory.Create(
		IFileSystem fileSystem,
		DiskFileSystemConfiguration config
	) {
		return new DiskFoldersReader(
			fileSystem,
			config
		);
	}
}
