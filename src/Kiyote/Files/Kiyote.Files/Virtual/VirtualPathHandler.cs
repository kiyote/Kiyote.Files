﻿using System.Globalization;
using System.Text;

namespace Kiyote.Files.Virtual;

internal sealed class VirtualPathHandler : IVirtualPathHandler {
	public char Separator => '/';

	FolderId IVirtualPathHandler.GetCommonParent(
		FolderId[] virtualPaths
	) {
		ArgumentNullException.ThrowIfNull( virtualPaths );
		if( virtualPaths.Length == 0 ) {
			throw new ArgumentException( "No possible common parent.", nameof( virtualPaths ) );
		}
		if( virtualPaths.Length == 1 ) {
			return virtualPaths[ 0 ];
		}

		Dictionary<string, string[]> pathSegments = [];

		foreach( FolderId virtualPath in virtualPaths ) {
			string[] segments = virtualPath.ToString().Split( Separator, StringSplitOptions.RemoveEmptyEntries );
			pathSegments[ virtualPath ] = segments;
		}

		string[] shortest = pathSegments.First( ps => ps.Value.Length == pathSegments.Min( ps => ps.Value.Length ) ).Value;

		int i = 0;
		while( true ) {
			bool allMatch = true;

			// Check that each path matches at the new segment
			foreach( string path in pathSegments.Keys ) {
				string[] segments = pathSegments[ path ];
				if( !string.Equals( shortest[ i ], segments[ i ], StringComparison.OrdinalIgnoreCase ) ) {
					allMatch = false;
					break;
				}
			}

			// If it doesn't match at this point, or we've reached the end
			// of the shortest path, then we copy the shortest path up to the
			// point it stopped matching.
			if( !allMatch
				|| i >= shortest.Length
			) {
				StringBuilder result = new StringBuilder( Separator.ToString() );
				for( int j = 0; j < i; j++ ) {
					_ = result
						.Append( shortest[ j ] )
						.Append( Separator );
				}
				return result.ToString();
			}
			i++;
		}
	}

	bool IVirtualPathHandler.IsRelativeTo(
		FolderId folderId,
		FolderId baseFolderId
	) {
		return folderId.ToString().StartsWith(
			baseFolderId.ToString(),
			ignoreCase: true,
			CultureInfo.InvariantCulture
		);
	}

	string IVirtualPathHandler.Combine(
		FolderId folderId,
		string folderName,
		char separator
	) {
		if( folderName.StartsWith( separator ) ) {
			throw new InvalidPathException();
		}
		string path = $"{folderId}{folderName}";
		if( !path.EndsWith( separator ) ) {
			return $"{path}{separator}";
		} else {
			return path;
		}
	}

	FolderId IVirtualPathHandler.Create(
		string path
	) {
		path = path.Trim();
		if (string.IsNullOrWhiteSpace( path )) {
			return $"{Separator}";
		}
		string[] segments = path.Split( Separator );
		if( segments.Length == 0 ) {
			return Separator.ToString();
		} else if( segments.Length == 1 ) {
			return $"{Separator}{segments[ 0 ]}{Separator}";
		} else {
			return $"{Separator}{segments.Aggregate( ( c, n ) => $"{c}{Separator}{n}" )}{Separator}";
		}
	}
}
