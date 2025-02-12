﻿namespace Kiyote.Files.Virtual;

public interface IVirtualPathHandler {

	char Separator { get; }

	FolderId GetCommonParent(
		FolderId[] virtualPaths
	);

	bool IsRelativeTo(
		FolderId folderId,
		FolderId baseFolderId
	);

	string Combine(
		FolderId folderId,
		string folderName,
		char separator
	);

	FolderId Create(
		string path = ""
	);
}
