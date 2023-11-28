namespace Kiyote.Files.Resource;

public interface IResourceFilesReaderFactory {

	ResourceFilesReader Create(
		ResourceFileSystemConfiguration config
	);

}
