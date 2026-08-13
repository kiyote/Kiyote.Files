
# Kiyote.Files.Resource

## Overview

Provides a read-only file system around embedded resource files.  This allows embedded
files to be used in a structured way, similar to how files are used on disk from Kiyote.Files.Disk.

## Getting Started

### Structured file system

To have a structured file system that matches the structure of the embedded resources, the resource
assembly must reference the nuget package `Microsoft.Extensions.FileProviders.Embedded` in the csproj
file, as well as specify `<GenerateEmbeddedFilesManifest>true</GenerateEmbeddedFilesManifest>`.

For example:
```xml
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

Without this embedded manifest, the resources will appear as a flat structure in the "root folder" of the
file system.  The name of the files in that case will be:
```
{Folder}.{SubFolder}.{File Name}
```
From the above example:
```
ResourceFolder.SubFolder.subfolder.txt
root.txt
```
