using Kiyote.Files.Virtual;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kiyote.Files.Resource;

[ExcludeFromCodeCoverage]
public static class ExtensionMethods {

	public static IServiceCollection AddReadOnlyResource(
		this IServiceCollection services,
		string fileSystemId,
		Assembly assembly,
		string rootFolder = ""
	) {
		ArgumentNullException.ThrowIfNull( assembly );

		services.TryAddKeyedSingleton(
			fileSystemId,
			( IServiceProvider sp, object? serviceKey ) => {
				ILogger<ResourceFileSystem> logger = sp.GetRequiredService<ILogger<ResourceFileSystem>>();
				IReadOnlyFileSystem fileSystem = new ResourceFileSystem(
					logger,
					fileSystemId,
					assembly,
					rootFolder
				);
				return fileSystem;
			}
		);
		return services;
	}

	public static IServiceCollection AddReadOnlyResource<T>(
		this IServiceCollection services,
		string fileSystemId,
		Assembly assembly,
		string rootFolder
	) where T: IFileSystemIdentifier {
		return services
			.AddReadOnlyResource( fileSystemId, assembly, rootFolder )
			.AddSingleton<IReadOnlyFileSystem<T>>(
				( IServiceProvider sp ) => {
					return new ReadOnlyFileSystemAdapter<T>(
						sp.GetRequiredKeyedService<IReadOnlyFileSystem>( fileSystemId )
					);
				}
			);
	}

}
