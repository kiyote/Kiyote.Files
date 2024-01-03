using System.Diagnostics.CodeAnalysis;

namespace Kiyote.Files.UnitTests;

[TestFixture]
[ExcludeFromCodeCoverage]
public class FileSystemIdTests {

	[Test]
	public void Ctor() {
		string expected = "value";

		FileSystemId id = new FileSystemId( expected );

		Assert.That( id, Is.EqualTo( expected ) );
	}

	[TestCase( "value", true )]
	[TestCase( "invalid", false )]
	[TestCase( 0, false )]
	[TestCase( null, false )]
	public void Equals_Object_TestCases(
		object? testValue,
		bool areEqual
	) {
		string expected = "value";

		FileSystemId id = new FileSystemId( expected );

		Assert.That( areEqual, Is.EqualTo( id.Equals( testValue ) ) );
	}
}
