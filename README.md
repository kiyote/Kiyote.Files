# Kiyote.Files

Provides a file system abstraction layer for various back-ends.  These file systems can either be Read/Write or Read-Only.  The method by which the files are placed, retrieved or enumerated is abstracted from the consumer to ensure a broader compatibility between backends.

## Kiyote.Files.Disk

Provides a read-write file system wrapper around a physical disk.

### Usage

Register the location on the disk you want to use as file repository by defining a class that can be used to differentiate your file system, and then register it using the `AddDiskReadWriteFileSystem` or `AddDiskReadOnlyFileSystem` extension method.

#### Example

Create a file system identifier for your repository:
```
public sealed record Temp : FileSystemIdentifier {

	public const string FileSystemId = "Temp";

	public Temp() : base( FileSystemId ) { }
}
```

Register the file system:
```
  services.AddDiskReadWriteFileSystem<Test>( @"c:\temp" );
```

Inject the file system in to a calss to consume it:
```
   public sealed class Foo( IReadWriteFileSystem<Temp> tempFileSystem ) {
```

Alternatively, retrieve the file system from the `IFileSystemProvider`:
```
   public IReadWritefileSystem GetFileSystem( IFileSystemProvider fileSystemProvider ) {
       return fileSystemProvider.GetReadWrite( Temp.FileSystemId )
   }
```


## Kiyote.Files.Resource

Provides a read-only file system wrapper around embedded resource files.

For more information see [Kiyote.Files.Resource](src/Kiyote/Files/Resource/Kiyote.Files.Resource/README.md).
