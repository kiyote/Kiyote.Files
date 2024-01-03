using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kiyote.Files.Virtual;

[ExcludeFromCodeCoverage]
public static class ExtensionMethods {

	public static IServiceCollection BuildFileSystem<T>(
		this IServiceCollection services,
		Action<IServiceProvider, IFileSystemBuilder<T>> action
	) where T: IFileSystemIdentifier {
		services.TryAddSingleton<IVirtualPathHandler, VirtualPathHandler>();
		_ = services.AddSingleton(
			(IServiceProvider serviceProvider) => {
				T identifier = Activator.CreateInstance<T>();
				IVirtualPathHandler virtualPathHandler = serviceProvider.GetRequiredService<IVirtualPathHandler>();
				FileSystemBuilder<T> builder = new FileSystemBuilder<T>(
					virtualPathHandler,
					identifier.FileSystemId
				);
				action( serviceProvider, builder );
				return builder.Build();
			}
		);
		return services;
	}
}
