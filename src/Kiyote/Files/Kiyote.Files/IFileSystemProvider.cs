namespace Kiyote.Files;

public interface IFileSystemProvider {

	IReadOnlyFileSystem? GetReadOnly( string fileSystemId );

	IReadWriteFileSystem? GetReadWrite( string fileSystemId );

	IReadOnlyFileSystem? GetReadOnly( FileId fileId );

	IReadWriteFileSystem? GetReadWrite( FileId fileId );

}
