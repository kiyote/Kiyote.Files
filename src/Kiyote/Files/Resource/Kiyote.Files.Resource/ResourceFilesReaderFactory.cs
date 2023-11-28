namespace Kiyote.Files.Resource;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in DI" )]
internal sealed class ResourceFilesReaderFactory : IResourceFilesReaderFactory {

	ResourceFilesReader IResourceFilesReaderFactory.Create(
		ConfiguredResourceFileSystem config
	) {
		return new ResourceFilesReader( config );
	}
}
