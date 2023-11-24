﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kiyote.Files.Disk;

public static class ExtensionMethods {

	public static IServiceCollection AddDiskFiles(
		this IServiceCollection services
	) {
		_ = services.AddFiles();
		services.TryAddSingleton<IFileSystem>( new FileSystem() );
		services.TryAddSingleton<IDiskFileSystemFactory, DiskFileSystemFactory>();
		return services;
	}

	public static IServiceCollection AddDiskReadWriteFileSystem<T>(
		this IServiceCollection services,
		string rootFolder
	) where T : FileSystemIdentifier {
		string fsid = Activator.CreateInstance<T>().Id;

		_ = services.AddDiskFiles();

		// Register instance as IReadWriteFileSystem
		_ = services.AddKeyedSingleton(
				fsid + FileSystemIdentifier.ReadWrite,
				( IServiceProvider services, object? key ) => {
					IDiskFileSystemFactory diskFactory = services.GetRequiredService<IDiskFileSystemFactory>();
					return diskFactory.CreateReadWriteFileSystem( fsid, rootFolder );
				}
			);

		// Register instance as IReadWriteFileSystem<T>
		_ = services.AddSingleton(
				( IServiceProvider services ) => {
					IReadWriteFileSystem fileSystem = services.GetRequiredKeyedService<IReadWriteFileSystem>( fsid + FileSystemIdentifier.ReadWrite );
					IFileSystemFactory wrapperFactory = services.GetRequiredService<IFileSystemFactory>();
					IReadWriteFileSystem<T> wrappedFileSystem = wrapperFactory.Create<T>( fileSystem );
					return wrappedFileSystem;
				}
			);

		// Register existing instance as IReadOnlyFileSystem
		_ = services.AddKeyedSingleton<IReadOnlyFileSystem>(
				fsid + FileSystemIdentifier.ReadOnly,
				( IServiceProvider services, object? key ) => {
					IReadWriteFileSystem fileSystem = services.GetRequiredKeyedService<IReadWriteFileSystem>( fsid + FileSystemIdentifier.ReadWrite );
					return fileSystem;
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

	public static IServiceCollection AddDiskReadOnlyFileSystem<T>(
		this IServiceCollection services,
		string rootFolder
	) where T : FileSystemIdentifier {
		string fsid = Activator.CreateInstance<T>().Id;

		_ = services.AddDiskFiles();

		// Register instance as IReadOnlyFileSystem
		_ = services.AddKeyedSingleton(
				fsid + FileSystemIdentifier.ReadOnly,
				( IServiceProvider services, object? key ) => {
					IDiskFileSystemFactory diskFactory = services.GetRequiredService<IDiskFileSystemFactory>();
					return diskFactory.CreateReadOnlyFileSystem( fsid, rootFolder );
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