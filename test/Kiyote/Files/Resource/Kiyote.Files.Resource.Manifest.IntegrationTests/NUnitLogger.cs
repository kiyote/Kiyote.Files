using Microsoft.Extensions.Logging;

namespace Kiyote.Files.Resource.Manifest.IntegrationTests;

#nullable disable
public class NUnitLogger : ILogger {
	private readonly Func<string, LogLevel, bool> _filter;
	private readonly string _name;

	public NUnitLogger( string name )
		: this( name, filter: null ) {
	}

	public NUnitLogger( string name, Func<string, LogLevel, bool> filter ) {
		_name = string.IsNullOrEmpty( name ) ? nameof( NUnitLogger ) : name;
		_filter = filter;
	}

	public IDisposable BeginScope<TState>( TState state ) {
		return NoopDisposable.Instance;
	}

	public bool IsEnabled( LogLevel logLevel ) {
		bool runningInNUnitContext = TestContext.Progress != null;
		return RunningInNUnitContext() && logLevel != LogLevel.None
			&& ( _filter == null || _filter( _name, logLevel ) );
	}

	public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter ) {
		if( !IsEnabled( logLevel ) ) {
			return;
		}

		ArgumentNullException.ThrowIfNull( formatter );

		string message = formatter( state, exception );

		if( string.IsNullOrEmpty( message ) ) {
			return;
		}

		message = $"{logLevel}: {message}";

		if( exception != null ) {
			message += Environment.NewLine + Environment.NewLine + exception.ToString();
		}

		WriteMessage( message, _name );
	}

	private static bool RunningInNUnitContext() {
		return TestContext.Progress != null;
	}

	private static void WriteMessage( string message, string _ ) {
		TestContext.Progress.WriteLine( message );
	}

	private sealed class NoopDisposable : IDisposable {
		public static NoopDisposable Instance = new NoopDisposable();

		public void Dispose() {
		}
	}
}
#nullable restore
