using Microsoft.Extensions.Logging;

namespace Kiyote.Files.Resource.Manifest.IntegrationTests;

#nullable disable
[ProviderAlias( "NUnit" )]
public sealed class NUnitLoggerProvider : ILoggerProvider {
	private readonly Func<string, LogLevel, bool> _filter;
	private bool _disposedValue;

	public NUnitLoggerProvider() {
		_filter = null;
	}
	public ILogger CreateLogger( string categoryName ) {
		return new NUnitLogger( categoryName, _filter );
	}

	ILogger ILoggerProvider.CreateLogger( string categoryName ) {
		return new NUnitLogger( categoryName, _filter );
	}

	private void Dispose( bool disposing ) {
		if( !_disposedValue ) {
			if( disposing ) {
				// TODO: dispose managed state (managed objects)
			}
			_disposedValue = true;
		}
	}

	public void Dispose() {
		// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
		Dispose( disposing: true );
		GC.SuppressFinalize( this );
	}
}
#nullable restore
