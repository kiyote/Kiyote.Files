using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kiyote.Files.Resource;

[ExcludeFromCodeCoverage]
public static class ExtensionMethods {

	public static IServiceCollection AddReadOnlyResource(
		this IServiceCollection services,
		Assembly assembly
	) {
		ArgumentNullException.ThrowIfNull( assembly );

		services.TryAddKeyedSingleton(
			assembly.GetName().Name,
			( IServiceProvider sp, object? serviceKey ) => {
				IReadOnlyFileSystem fileSystem = new ResourceFileSystem(
					assembly.FullName ?? throw new InvalidOperationException(),
					assembly,
					""
				);
				return fileSystem;
			}
		);
		return services;
	}

}
