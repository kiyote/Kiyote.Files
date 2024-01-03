using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Files.Virtual.IntegrationTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public sealed class FileSystemTreeTests {

	private FileSystemTree _tree;

	[SetUp]
	public void SetUp() {
		_tree = new FileSystemTree();
	}

	[Test]
	public void Insert_ValidPath_TreeCreated() {
		string path = @"/root/child/leaf/";

		Node leaf = _tree.Insert( path );

		Assert.That( leaf.Segment, Is.EqualTo( "leaf" ) );
	}

	[Test]
	public void Insert_ExistingPath_ChildReturned() {
		string path1 = @"/root/child/leaf/";
		string path2 = @"/root/child/";

		_ = _tree.Insert( path1 );
		Node child = _tree.Insert( path2 );

		Assert.That( child.Segment, Is.EqualTo( "child" ) );
		Assert.That( _tree.Root.Children.Count, Is.EqualTo( 1 ) );
	}

	[Test]
	public void Insert_NewPath_OrphanReturned() {
		string path1 = @"/root/child/leaf/";
		string path2 = @"/root/orphan/";

		_ = _tree.Insert( path1 );
		Node orphan = _tree.Insert( path2 );

		Assert.That( orphan.Segment, Is.EqualTo( "orphan" ) );
		Assert.That( _tree.Root.Children.Count, Is.EqualTo( 1 ) );
	}

	[Test]
	public void Insert_TwoBases_OrphanReturned() {
		string path1 = @"/root1/";
		string path2 = @"/root2/orphan/";

		_ = _tree.Insert( path1 );
		Node orphan = _tree.Insert( path2 );

		Assert.That( orphan.Segment, Is.EqualTo( "orphan" ) );
		Assert.That( _tree.Root.Children.Count, Is.EqualTo( 2 ) );
	}

	[Test]
	public void Find_MatchingPath_NodeReturned() {
		string path1 = @"/root1/";
		string path2 = @"/root1/orphan/";

		_ = _tree.Insert( path1 );
		Node root1 = _tree.Find( path2 );

		Assert.That( root1.Segment, Is.EqualTo( "root1" ) );
	}
}
