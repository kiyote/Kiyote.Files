using Microsoft.Extensions.DependencyInjection;

namespace Kiyote.Files;

public static class ExtensionMethods {

	public static IServiceCollection AddStorageAreas(
		this IServiceCollection services,
		Action<IServiceProvider, IStorageAreaBuilder> action
	) {
		_ = services.AddSingleton(
			( IServiceProvider services ) => {
				IStorageAreaBuilder builder = new StorageAreaBuilder();
				action( services, builder );
				return builder.Build();
			}
		);
		return services;
	}

}
