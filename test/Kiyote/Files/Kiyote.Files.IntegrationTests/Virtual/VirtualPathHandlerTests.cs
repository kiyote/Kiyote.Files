using System.Diagnostics.CodeAnalysis;
using Kiyote.Files.Virtual;

namespace Kiyote.Files.IntegrationTests.Virtual;

[TestFixture]
[ExcludeFromCodeCoverage]
public sealed class VirtualPathHandlerTests {

	private IVirtualPathHandler _handler;

	[SetUp]
	public void SetUp() {
		_handler = new VirtualPathHandler();
	}

	[Test]
	public void GetCommonParent_NoPaths_ThrowsArgumentException() {
		_ = Assert.Throws<ArgumentException>( () => _handler.GetCommonParent( [] ) );
	}

	[Test]
	public void GetCommonParent_SinglePath_ReturnsPath() {
		FolderId expected = "/test/";

		FolderId result = _handler.GetCommonParent(
			[
				expected
			]
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void GetCommonParent_TwoChildPaths_ReturnsParent() {
		FolderId expected = "/test/";

		FolderId result = _handler.GetCommonParent(
			[
				"/test/1/",
				"/test/2/"
			]
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}

	[Test]
	public void GetCommonParent_TwoDifferentChildren_ReturnsRoot() {
		FolderId expected = "/";

		FolderId result = _handler.GetCommonParent(
			[
				"/test/1/",
				"/sub/2/"
			]
		);

		Assert.That( result, Is.EqualTo( expected ) );
	}
}
