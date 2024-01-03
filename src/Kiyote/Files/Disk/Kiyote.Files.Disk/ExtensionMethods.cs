using System.Diagnostics.CodeAnalysis;
using Kiyote.Files.Virtual;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kiyote.Files.Disk;

[ExcludeFromCodeCoverage]
public static class ExtensionMethods {

	public static IServiceCollection AddReadWriteDisk(
		this IServiceCollection services,
		string rootFolder
	) {
		services.AddDiskFiles();
		services.TryAddKeyedSingleton<IFileSystem>(
			rootFolder,
			( IServiceProvider sp, object? serviceKey ) => {
				string? physicalPath = serviceKey as string;
				ArgumentException.ThrowIfNullOrWhiteSpace( physicalPath );
				return new DiskFileSystem(
					rootFolder,
					sp.GetRequiredService<System.IO.Abstractions.IFileSystem>(),
					physicalPath
				);
			}
		);
		return services;
	}

	public static IServiceCollection AddReadWriteDisk<T>(
		this IServiceCollection services,
		string rootFolder
	) where T : IFileSystemIdentifier {
		services
			.AddReadWriteDisk( rootFolder )
			.TryAddSingleton<IFileSystem<T>>(
				(IServiceProvider sp) => {
					return new FileSystemAdapter<T>(
						sp.GetRequiredKeyedService<IFileSystem>( rootFolder )
					);
				}
			);
		return services;
	}

	public static IServiceCollection AddReadOnlyDisk(
		this IServiceCollection services,
		string rootFolder
	) {
		services.AddDiskFiles();
		services.TryAddKeyedSingleton<IReadOnlyFileSystem>(
			rootFolder,
			( IServiceProvider sp, object? serviceKey ) => {
				string? physicalPath = serviceKey as string;
				ArgumentException.ThrowIfNullOrWhiteSpace( physicalPath );
				return new DiskFileSystem(
					rootFolder,
					sp.GetRequiredService<System.IO.Abstractions.IFileSystem>(),
					physicalPath
				);
			}
		);
		return services;
	}

	public static IServiceCollection AddReadOnlyDisk<T>(
		this IServiceCollection services,
		string rootFolder
	) where T : IFileSystemIdentifier {
		services
			.AddReadOnlyDisk( rootFolder )
			.TryAddSingleton<IReadOnlyFileSystem<T>>(
				( IServiceProvider sp ) => {
					return new ReadOnlyFileSystemAdapter<T>(
						sp.GetRequiredKeyedService<IReadOnlyFileSystem>( rootFolder )
					);
				}
			);
		return services;
	}

	public static IServiceCollection AddDiskFiles(
		this IServiceCollection services
	) {
		services.TryAddSingleton<System.IO.Abstractions.IFileSystem, FileSystem>();

		return services;
	}

	public static IFileSystemBuilder<T> AddReadWriteDisk<T>(
		this IFileSystemBuilder<T> builder,
		IServiceProvider services,
		FolderId virtualRoot,
		string physicalPath
	) where T: IFileSystemIdentifier {
		ArgumentNullException.ThrowIfNull( builder );
		IFileSystemIdentifier fileSystemIdentifier = Activator.CreateInstance<T>();
		System.IO.Abstractions.IFileSystem fs = services.GetRequiredService<System.IO.Abstractions.IFileSystem>();
		DiskFileSystem fileSystem = new DiskFileSystem(
			fileSystemIdentifier.FileSystemId,
			fs,
			physicalPath
		);
		DiskVirtualPathMapper pathMapper = new DiskVirtualPathMapper(
			fileSystem,
			virtualRoot
		);
		builder.AddReadWrite(
			virtualRoot,
			fileSystem,
			pathMapper
		);

		return builder;
	}
}
