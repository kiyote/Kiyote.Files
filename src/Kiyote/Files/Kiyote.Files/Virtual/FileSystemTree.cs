namespace Kiyote.Files.Virtual;

internal sealed class Node {

	public Node(
		string segment
	) {
		Segment = segment;
		Children = [];
	}

	internal Node() {
		Segment = "";
		Children = [];
	}

	public string Segment { get; }

	public List<Node> Children { get; }
}

internal sealed class FileSystemTree {

	public FileSystemTree() {
		Root = new Node();
	}

	public Node Root { get; }

	public Node Insert(
		string path
	) {
		int start = 1;
		int end = path.AsSpan()[ start.. ].IndexOf( VirtualFileSystem.Separator ) + 1;

		if( end <= start ) {
			return Root;
		}

		Node current = Root;
		while( true ) {
			ReadOnlySpan<char> segment = path.AsSpan()[ start..end ];
			current = GetOrCreateNode( current, segment );

			start = end + 1;
			end = path.AsSpan()[ start.. ].IndexOf( VirtualFileSystem.Separator );
			if( end == -1 ) {
				break;
			}
			end += start;
		}

		return current;
	}

	public Node Find(
		string path
	) {
		return Find( path.AsSpan() );
	}

	public Node Find(
		ReadOnlySpan<char> path
	) {
		int start = 1;
		int end = path[ start.. ].IndexOf( VirtualFileSystem.Separator ) + 1;

		if( end <= start ) {
			return Root;
		}

		Node? current = Root;
		while( true ) {
			ReadOnlySpan<char> segment = path[ start..end ];
			Node? child = GetNode( current, segment );

			if( child is null ) {
				return current;
			}
			current = child;
			start = end + 1;
			end = path[ start.. ].IndexOf( VirtualFileSystem.Separator );
			if( end == -1 ) {
				break;
			}
			end += start;
		}

		return current;
	}

	private static Node? GetNode(
		Node node,
		ReadOnlySpan<char> segment
	) {
		for( int i = 0; i < node.Children.Count; i++ ) {
			Node child = node.Children[ i ];
			if( segment.CompareTo( child.Segment, StringComparison.OrdinalIgnoreCase ) == 0 ) {
				return child;
			}
		}

		return null;
	}

	private static Node GetOrCreateNode(
		Node node,
		ReadOnlySpan<char> segment
	) {
		for( int i = 0; i < node.Children.Count; i++ ) {
			Node child = node.Children[ i ];
			if( segment.CompareTo( child.Segment, StringComparison.OrdinalIgnoreCase ) == 0 ) {
				return child;
			}
		}
		Node newNode = new Node( segment.ToString() );
		node.Children.Add( newNode );
		return newNode;
	}
}
