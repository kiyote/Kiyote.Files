namespace Kiyote.Files;

public sealed class FileSystemId : IEquatable<string>, IEquatable<FileSystemId> {

	private readonly string _id;

	public static readonly FileSystemId None = new FileSystemId();

	public FileSystemId(
		string id
	) {
		ArgumentException.ThrowIfNullOrWhiteSpace( id );
		_id = id;
	}

	internal FileSystemId() {
		_id = "";
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Usage", "CA2225:Operator overloads have named alternates", Justification = "ToString already exists" )]
	public static implicit operator FileSystemId(
		string id
	) {
		return new FileSystemId( id );
	}

	public static implicit operator string(
		FileSystemId id
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
		return _id == other;
	}

	public override int GetHashCode() {
		return HashCode.Combine( _id );
	}

	public override bool Equals( object? obj ) {
		if( obj is FileSystemId f ) {
			return f == _id;
		} else if( obj is string s ) {
			return s == _id;
		}
		return false;
	}

	public static bool operator ==(
		FileSystemId x,
		string y
	) {
		return x?._id.Equals( y, StringComparison.OrdinalIgnoreCase ) ?? false;
	}

	public static bool operator !=(
		FileSystemId x,
		string y
	) {
		return !( x?._id.Equals( y, StringComparison.OrdinalIgnoreCase ) ?? false );
	}

	public static bool operator ==(
		string x,
		FileSystemId y
	) {
		return y?._id.Equals( x, StringComparison.OrdinalIgnoreCase ) ?? false;
	}

	public static bool operator !=(
		string x,
		FileSystemId y
	) {
		return !( y?._id.Equals( x, StringComparison.OrdinalIgnoreCase ) ?? false );
	}

	public static bool operator ==(
		FileSystemId x,
		FileSystemId y
	) {
		return y?._id.Equals( x, StringComparison.OrdinalIgnoreCase ) ?? false;
	}

	public static bool operator !=(
		FileSystemId x,
		FileSystemId y
	) {
		return !( y?._id.Equals( x, StringComparison.OrdinalIgnoreCase ) ?? false );
	}

	public override string ToString() {
		return _id;
	}

	bool IEquatable<FileSystemId>.Equals(
		FileSystemId? other
	) {
		return _id.Equals( other?._id, StringComparison.OrdinalIgnoreCase );
	}
}

