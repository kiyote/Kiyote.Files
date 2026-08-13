# Kiyote.Files.Disk

## Overview

Provides a file system wrapper that is read-only, or read-write around a physical disk.  This allows the user
to "mount" directories and treat them as their own root file system. 

### Usage

Register the location on the disk you want to use as file repository by defining a class that can be used to differentiate your file system, and then register it using the `AddDiskReadWriteFileSystem` or `AddDiskReadOnlyFileSystem` extension method.

For example:
```csharp
  services.AddReadWriteDisk<FS.Test>( @"c:\temp" );
```
