using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kiyote.Files.Disk;

public static class ExtensionMethods {

	public static IServiceCollection AddDiskFileSystem(
		this IServiceCollection services
	) {
		services.TryAddSingleton<IFileSystem>( new FileSystem() );
		services.TryAddSingleton<IDiskFileSystemFactory, DiskFileSystemFactory>();
		return services;
	}

	public static IReadWriteFileSystem CreateReadWriteDiskFileSystem(
		this IServiceProvider services,
		string rootFolder
	) {
		IDiskFileSystemFactory factory = services.GetRequiredService<IDiskFileSystemFactory>();
		IReadWriteFileSystem fileSystem = factory.CreateReadWriteFileSystem(
			rootFolder
		);
		return fileSystem;
	}

}
