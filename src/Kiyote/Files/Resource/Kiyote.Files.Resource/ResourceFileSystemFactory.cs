using System.Reflection;

namespace Kiyote.Files.Resource;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in DI" )]
internal sealed class ResourceFileSystemFactory : IResourceFileSystemFactory {

	private readonly IResourceFoldersReaderFactory _foldersReaderFactory;
	private readonly IResourceFilesReaderFactory _filesReaderFactory;

	public ResourceFileSystemFactory(
		IResourceFilesReaderFactory filesReaderFactory,
		IResourceFoldersReaderFactory foldersReaderFactory
	) {
		_filesReaderFactory = filesReaderFactory;
		_foldersReaderFactory = foldersReaderFactory;
	}

	IReadOnlyFileSystem IResourceFileSystemFactory.CreateReadOnlyFileSystem(
		string fileSystemId,
		Assembly assembly
	) {
		ConfiguredResourceFileSystem config = new ConfiguredResourceFileSystem(
			assembly,
			fileSystemId
		);
		IFilesReader filesReader = _filesReaderFactory.Create( config );
		IFoldersReader foldersReader = _foldersReaderFactory.Create( config );
		return new ResourceFileSystem(
			filesReader,
			foldersReader
		);
	}
}
