namespace Kiyote.Files.Resource;

[ExcludeFromCodeCoverage]
public static partial class LogMessages {

	[LoggerMessage( 1, LogLevel.Debug, "Manifest resource assembly `{assemblyName}`." )]
	public static partial void ManifestResourceAssembly(
		this ILogger logger,
		string assemblyName
	);

	[LoggerMessage( 2, LogLevel.Debug, "Flat resource assembly `{assemblyName}`." )]
	public static partial void FlatResourceAssembly(
		this ILogger logger,
		string assemblyName
	);

	[LoggerMessage( 3, LogLevel.Error, "No folder identifiers are possible for flat resource assemblies." )]
	public static partial void GetFlatFolderIdentifiers(
		this ILogger logger
	);

	[LoggerMessage( 4, LogLevel.Error, "Trying to get file identifiers from folder `{folderIdentifier}` in flat resource assembly." )]
	public static partial void GetFlatFileIdentifiers(
		this ILogger logger,
		FolderIdentifier folderIdentifier
	);

	[LoggerMessage( 5, LogLevel.Error, "Trying to get folder `{folderName}` from flat resource assembly." )]
	public static partial void GetFlatFolder(
		this ILogger logger,
		string folderName
	);

	[LoggerMessage( 6, LogLevel.Error, "Trying to get file `{fileName}` from folder `{folderIdentifier}` in flat resource assembly." )]
	public static partial void GetFlatFolderFileIdentifier(
		this ILogger logger,
		FolderIdentifier folderIdentifier,
		string fileName
	);

	[LoggerMessage( 7, LogLevel.Warning, "Unable to locate file `{fileName}` in folder `{folderIdentifier}`." )]
	public static partial void MissingFileIdentifier(
		this ILogger logger,
		FolderIdentifier folderIdentifier,
		string fileName
	);
}
