namespace Kiyote.Files;

public sealed record FileMetadata(
	FileId FileId,
	string Name,
	string MimeType,
	long Size,
	DateTime CreatedOn
);
