using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kiyote.Files;

public static class ExtensionMethods {

	public static IServiceCollection AddFiles(
		this IServiceCollection services
	) {
		services.TryAddSingleton<IFileSystemFactory, FileSystemFactory>();
		services.TryAddSingleton<IFileSystemProvider, FileSystemProvider>();

		return services;
	}

}
