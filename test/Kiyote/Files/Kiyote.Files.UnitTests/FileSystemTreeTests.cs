using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Files.Virtual.UnitTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public sealed class FileSystemTreeTests {

	private FileSystemTree _tree;

	[SetUp]
	public void SetUp() {
		_tree = new FileSystemTree( '/' );
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

	[Test]
	public void GetNode_BuiltTree_NodeWithChildrenReturned() {
		string path1 = @"/root1/";
		string path2 = @"/root1/child1/";
		string path3 = @"/root2/";
		string path4 = @"/root2/child2/";

		_tree.Insert( [path1, path2, path3, path4 ] );

		Node root = _tree.Find( "/" );

		Assert.That( string.IsNullOrEmpty( root.Segment ) );
		Assert.That( root.Children.Count, Is.EqualTo( 2 ) );
		Assert.That( root.Children.ElementAt( 0 ).Segment, Is.EqualTo( "root1" ) );
		Assert.That( root.Children.ElementAt( 0 ).Children.Count, Is.EqualTo( 1 ) );
		Assert.That( root.Children.ElementAt( 0 ).Children.ElementAt( 0 ).Segment, Is.EqualTo( "child1" ) );
		Assert.That( root.Children.ElementAt( 1 ).Segment, Is.EqualTo( "root2" ) );
		Assert.That( root.Children.ElementAt( 1 ).Children.Count, Is.EqualTo( 1 ) );
		Assert.That( root.Children.ElementAt( 1 ).Children.ElementAt( 0 ).Segment, Is.EqualTo( "child2" ) );
	}
}
