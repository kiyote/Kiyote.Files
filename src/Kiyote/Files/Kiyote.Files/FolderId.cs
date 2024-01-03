namespace Kiyote.Files;

public sealed class FolderId : IEquatable<string>, IEquatable<FolderId> {

	private readonly string? _id;

	public static readonly FolderId None = new FolderId();

	public FolderId(
		string id
	) {
		ArgumentNullException.ThrowIfNull( id );
		_id = id;
	}

	internal FolderId() {
		_id = null;
	}

	[System.Diagnostics.CodeAnalysis.SuppressMessage( "Usage", "CA2225:Operator overloads have named alternates", Justification = "ToString already exists" )]
	public static implicit operator FolderId(
		string id
	) {
		return new FolderId( id );
	}

	public static implicit operator string(
		FolderId id
	) {
		ArgumentNullException.ThrowIfNull( id?._id );
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
		if( obj is FolderId f ) {
			return f._id == _id;
		} else if( obj is string s ) {
			return s == _id;
		}
		return false;
	}

	public static bool operator ==(
		FolderId x,
		string y
	) {
		return x?._id?.Equals( y, StringComparison.OrdinalIgnoreCase ) ?? false;
	}

	public static bool operator !=(
		FolderId x,
		string y
	) {
		return !( x?._id?.Equals( y, StringComparison.OrdinalIgnoreCase ) ?? false );
	}

	public static bool operator ==(
		string x,
		FolderId y
	) {
		return y?._id?.Equals( x, StringComparison.OrdinalIgnoreCase ) ?? false;
	}

	public static bool operator !=(
		string x,
		FolderId y
	) {
		return !( y?._id?.Equals( x, StringComparison.OrdinalIgnoreCase ) ?? false );
	}

	public static bool operator ==(
		FolderId x,
		FolderId y
	) {
		return y?._id?.Equals( x, StringComparison.OrdinalIgnoreCase ) ?? false;
	}

	public static bool operator !=(
		FolderId x,
		FolderId y
	) {
		return !( y?._id?.Equals( x, StringComparison.OrdinalIgnoreCase ) ?? false );
	}

	public override string ToString() {
		return _id ?? "";
	}

	bool IEquatable<FolderId>.Equals(
		FolderId? other
	) {
		return _id?.Equals( other?._id, StringComparison.OrdinalIgnoreCase ) ?? false;
	}

	public ReadOnlySpan<char> AsSpan() {
		if( _id is null ) {
			return "".AsSpan();
		}
		return _id.AsSpan();
	}
}

