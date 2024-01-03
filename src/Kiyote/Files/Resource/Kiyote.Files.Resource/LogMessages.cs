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

}
