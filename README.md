![CI](https://github.com/kiyote/Files/actions/workflows/ci.yml/badge.svg?branch=main)
![coverage](https://github.com/kiyotes/Files/blob/badges/.badges/main/coverage.svg?raw=true)

# Kiyote.Files

Provides a file system abstraction layer for various back-ends.  These file systems can either be Read/Write or Read-Only.  The method by which the files are placed, retrieved or enumerated is abstracted from the consumer to ensure a broader compatibility between backends.

## Kiyote.Files.Disk

Provides a read-write file system wrapper around a physical disk.

### Usage

Register the location on the disk you want to use as file repository by defining a class that can be used to differentiate your file system, and then register it using the `AddDiskReadWriteFileSystem` or `AddDiskReadOnlyFileSystem` extension method.

#### Example

Create a file system identifier for your repository:
```
public abstract class FS {
	public sealed class Test : IFileSystemIdentifier {

		public const string TestFileSystemId = "Test";

		FileSystemId IFileSystemIdentifier.FileSystemId => TestFileSystemId;
	}
}
```

Register the file system:
```
  services.AddReadWriteDisk<FS.Test>( @"c:\temp" );
```

Inject the file system in to a calss to consume it:
```
   public sealed class Foo( IFileSystem<FS.Temp> tempFileSystem ) {
```


## Kiyote.Files.Resource

Provides a read-only file system wrapper around embedded resource files.

### Structured file system

To have a structured file system that matches the structure of the embedded resources, the resource assembly must reference the nuget package `Microsoft.Extensions.FileProviders.Embedded` in the csproj file, as well as specify `<GenerateEmbeddedFilesManifest>true</GenerateEmbeddedFilesManifest>`.

For example:
```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <GenerateEmbeddedFilesManifest>true</GenerateEmbeddedFilesManifest>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.FileProviders.Embedded" />
  <ItemGroup>

  <ItemGroup>
    <EmbeddedResource Include="ResourceFolder\SubFolder\subfolder.txt" />
    <EmbeddedResource Include="root.txt" />
  </ItemGroup>

...
```

### Unstructured file system

Without this embedded manifest, the resources will appear as a flat structure in the "root folder" of the file system.  The name of the files in that case will be:
```
{Folder}.{SubFolder}.{File Name}
```
From the above example:
```
ResourceFolder.SubFolder.subfolder.txt
root.txt
```

# Notes

Github action failing with permission denied?
```
git update-index --chmod=+x ./create-orphan-branch.sh
```
Still failing?  Make sure to run the `./create-orphan-branch.sh` locally once.
