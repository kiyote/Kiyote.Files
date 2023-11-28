namespace Kiyote.Files.Disk;

internal sealed class DiskFoldersReaderFactory : IDiskFoldersReaderFactory {
	DiskFoldersReader IDiskFoldersReaderFactory.Create(
		IFileSystem fileSystem,
		ConfiguredDiskFileSystem config
	) {
		return new DiskFoldersReader(
			fileSystem,
			config
		);
	}
}
