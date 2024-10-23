using Kiyote.Files.Virtual;

namespace Kiyote.Files.Resource;

internal sealed class ResourceVirtualPathMapper : IVirtualPathMapper {

	private readonly ResourceFileSystem _fileSystem;
	private readonly string _virtualRoot;
	private readonly char _virtualSeparator;

	public ResourceVirtualPathMapper(
		ResourceFileSystem resourceFileSystem,
		string virtualRoot,
		char virtualSeparator
	) {
		_fileSystem = resourceFileSystem;
		_virtualRoot = virtualRoot;
		_virtualSeparator = virtualSeparator;
	}

	bool IVirtualPathMapper.TryMapFromVirtual(
		FileId virtualFileId,
		out FileIdentifier fileIdentifier
	) {
		if( !virtualFileId.ToString().StartsWith( _virtualRoot, StringComparison.OrdinalIgnoreCase ) ) {
			fileIdentifier = FileIdentifier.None;
			return false;
		}
		fileIdentifier = new FileIdentifier(
			_fileSystem.FileSystemId,
			virtualFileId.AsSpan()[ _virtualRoot.Length.. ].ToString().Replace( _virtualSeparator, _fileSystem.Separator )
		);
		return true;
	}

	bool IVirtualPathMapper.TryMapFromVirtual(
		FolderId virtualFolderId,
		out FolderIdentifier folderIdentifier
	) {
		if( !virtualFolderId.ToString().StartsWith( _virtualRoot, StringComparison.OrdinalIgnoreCase ) ) {
			folderIdentifier = FolderIdentifier.None;
			return false;
		}
		folderIdentifier = new FolderIdentifier(
			_fileSystem.FileSystemId,
			virtualFolderId.AsSpan()[ _virtualRoot.Length.. ].ToString().Replace( _virtualSeparator, _fileSystem.Separator )
		);
		return true;
	}

	bool IVirtualPathMapper.TryMapToVirtual(
		FileIdentifier fileIdentifier,
		out FileId virtualFileId
	) {
		if( fileIdentifier.FileSystemId != _fileSystem.FileSystemId ) {
			virtualFileId = FileId.None;
			return false;
		}

		string physicalPath = fileIdentifier.FileId;
		string virtualPath = physicalPath.Replace( _fileSystem.Separator, _virtualSeparator );
		virtualFileId = $"{_virtualRoot}{virtualPath}";
		return true;
	}

	bool IVirtualPathMapper.TryMapToVirtual(
		FolderIdentifier folderIdentifier,
		out FolderId virtualFolderId
	) {
		if( folderIdentifier.FileSystemId != _fileSystem.FileSystemId ) {
			virtualFolderId = FolderId.None;
			return false;
		}

		string physicalPath = folderIdentifier.FolderId;
		string virtualPath = physicalPath.Replace( _fileSystem.Separator, _virtualSeparator );
		virtualFolderId = $"{_virtualRoot}{virtualPath}";
		return true;
	}
}
