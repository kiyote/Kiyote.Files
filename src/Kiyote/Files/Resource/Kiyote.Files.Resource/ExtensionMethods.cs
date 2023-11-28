using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kiyote.Files.Resource;

public static class ExtensionMethods {

	public static IServiceCollection AddResourceFiles(
		this IServiceCollection services
	) {
		_ = services.AddFiles();
		services.TryAddSingleton<IResourceFileSystemFactory, ResourceFileSystemFactory>();
		services.TryAddSingleton<IResourceFilesReaderFactory, ResourceFilesReaderFactory>();
		services.TryAddSingleton<IResourceFoldersReaderFactory, ResourceFoldersReaderFactory>();
		return services;
	}

	public static IServiceCollection AddResourceReadOnlyFileSystem<T>(
		this IServiceCollection services,
		Assembly assembly
	) where T: FileSystemIdentifier {
		string fsid = Activator.CreateInstance<T>().FileSystemId;

		_ = services.AddResourceFiles();

		// Register instance as IReadOnlyFileSystem
		_ = services.AddKeyedSingleton(
				fsid + FileSystemIdentifier.ReadOnly,
				( IServiceProvider services, object? key ) => {
					IResourceFileSystemFactory factory = services.GetRequiredService<IResourceFileSystemFactory>();
					return factory.CreateReadOnlyFileSystem( fsid, assembly );
				}
			);

		// Register instance as IReadOnlyFileSystem<T>
		_ = services.AddSingleton(
				( IServiceProvider services ) => {
					IReadOnlyFileSystem fileSystem = services.GetRequiredKeyedService<IReadOnlyFileSystem>( fsid + FileSystemIdentifier.ReadOnly );
					IFileSystemFactory wrapperFactory = services.GetRequiredService<IFileSystemFactory>();
					IReadOnlyFileSystem<T> wrappedFileSystem = wrapperFactory.Create<T>( fileSystem );
					return wrappedFileSystem;
				}
			);

		return services;
	}
	
}
