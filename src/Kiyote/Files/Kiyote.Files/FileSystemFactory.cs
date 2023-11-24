namespace Kiyote.Files;

[System.Diagnostics.CodeAnalysis.SuppressMessage( "Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Registered in DI" )]
internal sealed class FileSystemFactory : IFileSystemFactory {

	IReadOnlyFileSystem<T> IFileSystemFactory.Create<T>(
		IReadOnlyFileSystem fileSystem
	) {
		return new ReadOnlyFileSystemAdapter<T>( fileSystem );
	}

	IReadWriteFileSystem<T> IFileSystemFactory.Create<T>(
		IReadWriteFileSystem fileSystem
	) {
		return new ReadWriteFileSystemAdapter<T>( fileSystem );
	}

}
