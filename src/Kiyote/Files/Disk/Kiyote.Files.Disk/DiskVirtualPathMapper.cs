using Kiyote.Files.Virtual;

namespace Kiyote.Files.Disk;

internal sealed class DiskVirtualPathMapper : IVirtualPathMapper {

	private readonly DiskFileSystem _fileSystem;
	private readonly string _virtualRoot;

	public DiskVirtualPathMapper(
		DiskFileSystem diskFileSystem,
		string virtualRoot
	) {
		_fileSystem = diskFileSystem;
		_virtualRoot = virtualRoot;
	}

	bool IVirtualPathMapper.TryMapFromVirtual(
		FileIdentifier virtualFileIdentifier,
		out FileIdentifier fileIdentifier
	) {
		if( virtualFileIdentifier.FileSystemId != _fileSystem.FileSystemId
			|| !virtualFileIdentifier.FileId.ToString().StartsWith( _virtualRoot, StringComparison.OrdinalIgnoreCase
		) ) {
			fileIdentifier = FileIdentifier.None;
			return false;
		}
		fileIdentifier = new FileIdentifier(
			virtualFileIdentifier.FileSystemId,
			virtualFileIdentifier.FileId.AsSpan()[ _virtualRoot.Length.. ].ToString().Replace( '/', _fileSystem.FileSystem.Path.DirectorySeparatorChar )
		);
		return true;
	}

	bool IVirtualPathMapper.TryMapFromVirtual(
		FolderIdentifier virtualFolderIdentifier,
		out FolderIdentifier folderIdentifier
	) {
		if( virtualFolderIdentifier.FileSystemId != _fileSystem.FileSystemId
			|| !virtualFolderIdentifier.FolderId.ToString().StartsWith( _virtualRoot, StringComparison.OrdinalIgnoreCase ) ) {
			folderIdentifier = FolderIdentifier.None;
			return false;
		}
		folderIdentifier = new FolderIdentifier(
			virtualFolderIdentifier.FileSystemId,
			virtualFolderIdentifier.FolderId.AsSpan()[ _virtualRoot.Length.. ].ToString().Replace( '/', _fileSystem.FileSystem.Path.DirectorySeparatorChar )
		);
		return true;
	}

	bool IVirtualPathMapper.TryMapToVirtual(
		FileIdentifier fileIdentifier,
		out FileIdentifier virtualFileIdentifier
	) {
		if( fileIdentifier.FileSystemId != _fileSystem.FileSystemId ) {
			virtualFileIdentifier = FileIdentifier.None;
			return false;
		}

		string physicalPath = fileIdentifier.FileId;
		string virtualPath = physicalPath.Replace( _fileSystem.FileSystem.Path.DirectorySeparatorChar, '/' );
		virtualFileIdentifier = new FileIdentifier(
			fileIdentifier.FileSystemId,
			$"{_virtualRoot}{virtualPath}"
		);
		return true;
	}

	bool IVirtualPathMapper.TryMapToVirtual(
		FolderIdentifier folderIdentifier,
		out FolderIdentifier virtualFolderIdentifier
	) {
		if( folderIdentifier.FileSystemId != _fileSystem.FileSystemId ) {
			virtualFolderIdentifier = FolderIdentifier.None;
			return false;
		}

		string physicalPath = folderIdentifier.FolderId;
		string virtualPath = physicalPath.Replace( _fileSystem.FileSystem.Path.DirectorySeparatorChar, '/' );
		virtualFolderIdentifier = new FolderIdentifier(
			folderIdentifier.FileSystemId,
			$"{_virtualRoot}{virtualPath}"
		);
		return true;
	}
}
