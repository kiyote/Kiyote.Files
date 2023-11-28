using System.Reflection;

namespace Kiyote.Files.Resource;

public sealed record ConfiguredResourceFileSystem(
	Assembly Assembly,
	string FileSystemId
) {
	public FileId ToFileId(
		FolderId folderId,
		string name
	) {
		ArgumentNullException.ThrowIfNull( folderId );
		return new FileId( FileSystemId, $"{folderId.Id}{name}" );
	}

	public FolderId ToFolderId(
		FolderId folderId,
		string name
	) {
		ArgumentNullException.ThrowIfNull( folderId );
		return new FolderId( FileSystemId, $"{folderId.Id}{name}\\" );
	}
}
