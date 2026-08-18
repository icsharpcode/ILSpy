// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Composition;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ICSharpCode.ILSpy.AppEnv
{
	/// <summary>
	/// MEF-exported ILoggerFactory that bridges to AppLog and Debug output.
	/// </summary>
	[Export(typeof(ILoggerFactory))]
	[Shared]
	public sealed class LoggingService : ILoggerFactory
	{
		public ILogger CreateLogger(string categoryName)
		{
			return new AppLogger(categoryName);
		}

		public void AddProvider(ILoggerProvider provider)
		{
			// Not implemented: this factory uses a fixed AppLog bridge
		}

		public void Dispose()
		{
		}

		private sealed class AppLogger : ILogger
		{
			private readonly string categoryName;

			public AppLogger(string categoryName)
			{
				this.categoryName = categoryName;
			}

			public IDisposable? BeginScope<TState>(TState state) where TState : notnull
			{
				return null;
			}

			public bool IsEnabled(LogLevel logLevel)
			{
				return logLevel >= LogLevel.Information;
			}

			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
			{
				if (!IsEnabled(logLevel))
					return;

				var message = formatter(state, exception);
				var logEntry = $"[{DateTime.Now:HH:mm:ss.fff}] [{logLevel}] [{categoryName}] {message}";

				if (exception != null)
					logEntry += Environment.NewLine + exception;

				// Write to debug output
				Debug.WriteLine(logEntry);

				// Write to AppLog file
				AppLog.Write(AppLog.Category.AI, logEntry);
			}
		}
	}
}
