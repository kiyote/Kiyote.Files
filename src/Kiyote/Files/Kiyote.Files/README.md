# Kiyote.Files

Provides a file system abstraction layer for various back-ends.  These file systems can either be Read/Write or Read-Only.
The method by which the files are placed, retrieved or enumerated is abstracted from the consumer to ensure a broader compatibility between backends.

## Getting Started

Each registered file system must have its own distinct identifier.  This allows implementations to correctly store and retrieve from the appropriate
location invisibly to the user.  They are just aware there is an opaque IFileSystemIdentifier for their file.

Create a file system identifier for your repository:
```csharp
public abstract class FS {
  public sealed class Test : IFileSystemIdentifier {

    public const string TestFileSystemId = "Test";

    FileSystemId IFileSystemIdentifier.FileSystemId => TestFileSystemId;
  }
}
```

Register the file system:
```csharp
  services.AddReadWriteDisk<FS.Test>( @"c:\temp" );
```

Inject the file system in to a class to consume it:
```csharp
   public sealed class Foo( IFileSystem<FS.Temp> tempFileSystem ) {
```
