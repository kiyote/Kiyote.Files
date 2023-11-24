using System.Reflection;

namespace Kiyote.Files.Resource;

public interface IResourceFileSystemFactory {

	IReadOnlyFileSystem CreateReadOnlyFileSystem(
		string fileSystemId,
		Assembly assembly
	);
}
