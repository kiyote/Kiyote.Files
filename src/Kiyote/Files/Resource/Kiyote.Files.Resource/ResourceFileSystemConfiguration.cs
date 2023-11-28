using System.Reflection;

namespace Kiyote.Files.Resource;

public sealed record ResourceFileSystemConfiguration(
	FileSystemIdentifier Id,
	Assembly Assembly
) {
	public FileId ToFileId(
		FolderId folderId,
		string name
	) {
		ArgumentNullException.ThrowIfNull( folderId );
		return new FileId( Id.FileSystemId, $"{folderId.Id}{name}" );
	}

	public FolderId ToFolderId(
		FolderId folderId,
		string name
	) {
		ArgumentNullException.ThrowIfNull( folderId );
		return new FolderId( Id.FileSystemId, $"{folderId.Id}{name}\\" );
	}
}
