namespace Kiyote.Files.Resource;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in DI" )]
internal sealed class ResourceFoldersReaderFactory : IResourceFoldersReaderFactory {
	ResourceFoldersReader IResourceFoldersReaderFactory.Create(
		ResourceFileSystemConfiguration config
	) {
		return new ResourceFoldersReader( config );
	}
}
