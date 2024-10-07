using System.Diagnostics.CodeAnalysis;
using Kiyote.Files.Virtual;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kiyote.Files.Disk;

[ExcludeFromCodeCoverage]
public static class ExtensionMethods {

	public static IServiceCollection AddReadWriteDiskFileSystem(
		this IServiceCollection services,
		FileSystemId fileSystemId,
		string rootFolder
	) {
		ArgumentNullException.ThrowIfNull( fileSystemId );

		services
			.AddDiskFileSystem()
			.TryAddKeyedSingleton<IFileSystem>(
				fileSystemId.ToString(),
				( IServiceProvider sp, object? serviceKey ) => {
					ArgumentException.ThrowIfNullOrWhiteSpace( rootFolder );
					return new DiskFileSystem(
						fileSystemId,
						sp.GetRequiredService<System.IO.Abstractions.IFileSystem>(),
						rootFolder
					);
				}
			);
		return services;
	}

	public static IServiceCollection AddReadWriteDiskFileSystem<T>(
		this IServiceCollection services,
		string rootFolder
	) where T : IFileSystemIdentifier, new() {
		IFileSystemIdentifier fsid = Activator.CreateInstance<T>();
		services
			.AddReadWriteDiskFileSystem( fsid.FileSystemId, rootFolder )
			.TryAddSingleton<IFileSystem<T>>(
				(IServiceProvider sp) => {
					return new FileSystemAdapter<T>(
						sp.GetRequiredKeyedService<IFileSystem>( fsid.FileSystemId.ToString() )
					);
				}
			);
		return services;
	}

	public static IServiceCollection AddReadOnlyDiskFileSystem(
		this IServiceCollection services,
		FileSystemId fileSystemId,
		string rootFolder
	) {
		ArgumentNullException.ThrowIfNull( fileSystemId );

		services
			.AddDiskFileSystem()
			.TryAddKeyedSingleton<IReadOnlyFileSystem>(
				fileSystemId.ToString(),
				( IServiceProvider sp, object? serviceKey ) => {
					ArgumentException.ThrowIfNullOrWhiteSpace( rootFolder );
					return new DiskFileSystem(
						fileSystemId,
						sp.GetRequiredService<System.IO.Abstractions.IFileSystem>(),
						rootFolder
					);
				}
			);
		return services;
	}

	public static IServiceCollection AddReadOnlyDiskFileSystem<T>(
		this IServiceCollection services,
		string rootFolder
	) where T : IFileSystemIdentifier, new() {
		IFileSystemIdentifier fsid = Activator.CreateInstance<T>();
		services
			.AddReadOnlyDiskFileSystem( fsid.FileSystemId, rootFolder )
			.TryAddSingleton<IReadOnlyFileSystem<T>>(
				( IServiceProvider sp ) => {
					return new ReadOnlyFileSystemAdapter<T>(
						sp.GetRequiredKeyedService<IReadOnlyFileSystem>( fsid.FileSystemId.ToString() )
					);
				}
			);
		return services;
	}

	public static IServiceCollection AddDiskFileSystem(
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
	) where T: IFileSystemIdentifier, new() {
		ArgumentNullException.ThrowIfNull( builder );
		IFileSystemIdentifier fsid = Activator.CreateInstance<T>();
		System.IO.Abstractions.IFileSystem fs = services.GetRequiredService<System.IO.Abstractions.IFileSystem>();
		DiskFileSystem fileSystem = new DiskFileSystem(
			fsid.FileSystemId,
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
