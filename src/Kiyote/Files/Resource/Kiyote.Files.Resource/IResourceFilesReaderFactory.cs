namespace Kiyote.Files.Resource;

public interface IResourceFilesReaderFactory {

	ResourceFilesReader Create(
		ConfiguredResourceFileSystem config
	);

}
