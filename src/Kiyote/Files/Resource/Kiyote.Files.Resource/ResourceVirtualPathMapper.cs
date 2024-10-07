using Kiyote.Files.Virtual;

namespace Kiyote.Files.Resource;

internal sealed class ResourceVirtualPathMapper : IVirtualPathMapper {

	private readonly ResourceFileSystem _fileSystem;
	private readonly string _virtualRoot;

	public ResourceVirtualPathMapper(
		ResourceFileSystem resourceFileSystem,
		string virtualRoot
	) {
		_fileSystem = resourceFileSystem;
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
			virtualFileIdentifier.FileId.AsSpan()[ _virtualRoot.Length.. ].ToString().Replace( '/', _fileSystem.Separator )
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
			virtualFolderIdentifier.FolderId.AsSpan()[ _virtualRoot.Length.. ].ToString().Replace( '/', _fileSystem.Separator )
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
		string virtualPath = physicalPath.Replace( _fileSystem.Separator, '/' );
		virtualFileIdentifier = new FileIdentifier(
			fileIdentifier.FileSystemId,
			$"{_virtualRoot}/{virtualPath}"
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
		string virtualPath = physicalPath.Replace( _fileSystem.Separator, '/' );
		virtualFolderIdentifier = new FolderIdentifier(
			folderIdentifier.FileSystemId,
			$"{_virtualRoot}/{virtualPath}"
		);
		return true;
	}
}
