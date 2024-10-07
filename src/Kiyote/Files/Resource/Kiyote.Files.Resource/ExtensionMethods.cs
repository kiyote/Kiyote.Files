using Kiyote.Files.Virtual;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kiyote.Files.Resource;

[ExcludeFromCodeCoverage]
public static class ExtensionMethods {

	public static IServiceCollection AddResourceFileSystem(
		this IServiceCollection services,
		FileSystemId fileSystemId,
		Assembly assembly,
		string rootFolder = ""
	) {
		ArgumentNullException.ThrowIfNull( fileSystemId );
		ArgumentNullException.ThrowIfNull( assembly );

		services.TryAddKeyedSingleton(
			fileSystemId.ToString(),
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

	public static IServiceCollection AddResourceFileSystem<T>(
		this IServiceCollection services,
		Assembly assembly,
		string rootFolder = ""
	) where T: IFileSystemIdentifier, new() {
		ArgumentNullException.ThrowIfNull( assembly );

		IFileSystemIdentifier fsid = Activator.CreateInstance<T>();
		return services
			.AddResourceFileSystem ( fsid.FileSystemId, assembly, rootFolder )
			.AddSingleton<IReadOnlyFileSystem<T>>(
				( IServiceProvider sp ) => {
					return new ReadOnlyFileSystemAdapter<T>(
						sp.GetRequiredKeyedService<IReadOnlyFileSystem>( fsid.FileSystemId.ToString() )
					);
				}
			);
	}

	public static IFileSystemBuilder<T> AddResource<T>(
		this IFileSystemBuilder<T> builder,
		IServiceProvider services,
		FolderId virtualRoot,
		Assembly assembly,
		string rootFolder = ""
	) where T : IFileSystemIdentifier, new() {
		ArgumentNullException.ThrowIfNull( builder );
		IFileSystemIdentifier fsid = Activator.CreateInstance<T>();
		ILogger<ResourceFileSystem>? logger = services.GetService<ILogger<ResourceFileSystem>>();
		ResourceFileSystem fileSystem = new ResourceFileSystem(
			logger,
			fsid.FileSystemId,
			assembly,
			rootFolder
		);
		ResourceVirtualPathMapper pathMapper = new ResourceVirtualPathMapper(
			fileSystem,
			virtualRoot
		);
		builder.AddReadOnly(
			virtualRoot,
			fileSystem,
			pathMapper
		);

		return builder;
	}
}
