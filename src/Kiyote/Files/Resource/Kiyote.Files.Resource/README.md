# Resource File System

## Structured file system

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

## Unstructured file system

Without this embedded manifest, the resources will appear as a flat structure in the "root folder" of the file system.  The name of the files in that case will be:
```
{Folder}.{SubFolder}.{File Name}
```
From the above example:
```
ResourceFolder.SubFolder.subfolder.txt
root.txt
```
