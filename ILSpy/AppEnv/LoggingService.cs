// Copyright (c) 2026 Masroor
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
