using System.Reflection;

namespace Kiyote.Files.Resource;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in DI" )]
internal sealed class ResourceFileSystemFactory : IResourceFileSystemFactory {
	IReadOnlyFileSystem IResourceFileSystemFactory.CreateReadOnlyFileSystem(
		string fileSystemId,
		Assembly assembly
	) {
		return new ResourceFileSystem( assembly, fileSystemId );
	}
}
