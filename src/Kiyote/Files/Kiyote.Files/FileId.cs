namespace Kiyote.Files;

public sealed class FileId : IEquatable<string>, IEquatable<FileId> {

	private readonly string _id;

	public static readonly FileId None = new FileId();

	public FileId(
		string id
	) {
		ArgumentException.ThrowIfNullOrWhiteSpace( id );
		_id = id;
	}

	internal FileId() {
		_id = "";
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Usage", "CA2225:Operator overloads have named alternates", Justification = "ToString already exists" )]
	public static implicit operator FileId(
		string id
	) {
		return new FileId( id );
	}

	public static implicit operator string(
		FileId id
	) {
		ArgumentNullException.ThrowIfNull( id );
		return id._id;
	}

	bool IEquatable<string>.Equals(
		string? other
	) {
		if( other is null ) {
			return false;
		}
		return _id.Equals( other, StringComparison.Ordinal );
	}

	public override int GetHashCode() {
		return HashCode.Combine( _id );
	}

	public override bool Equals( object? obj ) {
		if( obj is FileId f ) {
			return _id.Equals( f._id, StringComparison.Ordinal );
		} else if( obj is string s ) {
			return _id.Equals( s, StringComparison.Ordinal );
		}
		return false;
	}

	public static bool operator ==(
		FileId x,
		string y
	) {
		return x?._id.Equals( y, StringComparison.Ordinal ) ?? false;
	}

	public static bool operator !=(
		FileId x,
		string y
	) {
		return !( x?._id.Equals( y, StringComparison.Ordinal ) ?? false );
	}

	public static bool operator ==(
		string x,
		FileId y
	) {
		return y?._id.Equals( x, StringComparison.Ordinal ) ?? false;
	}

	public static bool operator !=(
		string x,
		FileId y
	) {
		return !( y?._id.Equals( x, StringComparison.Ordinal ) ?? false );
	}

	public static bool operator ==(
		FileId x,
		FileId y
	) {
		return y?._id.Equals( x, StringComparison.Ordinal ) ?? false;
	}

	public static bool operator !=(
		FileId x,
		FileId y
	) {
		return !( y?._id.Equals( x, StringComparison.Ordinal ) ?? false );
	}

	public override string ToString() {
		return _id;
	}

	bool IEquatable<FileId>.Equals(
		FileId? other
	) {
		return _id.Equals( other?._id, StringComparison.Ordinal );
	}

	public ReadOnlySpan<char> AsSpan() {
		return _id.AsSpan();
	}
}

