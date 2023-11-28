namespace Kiyote.Files.Resource;

public interface IResourceFoldersReaderFactory {

	ResourceFoldersReader Create(
		ConfiguredResourceFileSystem config
	);

}
