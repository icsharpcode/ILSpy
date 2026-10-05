// Copyright (c) 2026 Christoph Wille
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


using System.Collections.Generic;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;
using Avalonia.Logging;

using AwesomeAssertions;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// A binding that fails while the main window comes up is logged by Avalonia and otherwise
/// goes unnoticed: the bound property silently keeps its default. Booting the window with a
/// capturing log sink pins that startup produces no such errors.
/// </summary>
[TestFixture]
public class StartupBindingErrorTests
{
	sealed class BindingErrorSink(ILogSink? inner) : ILogSink
	{
		public List<string> Errors { get; } = new();

		public bool IsEnabled(LogEventLevel level, string area)
			=> area == LogArea.Binding || (inner?.IsEnabled(level, area) ?? false);

		public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
			=> Log(level, area, source, messageTemplate, []);

		public void Log(LogEventLevel level, string area, object? source, string messageTemplate, params object?[] propertyValues)
		{
			if (area == LogArea.Binding && level >= LogEventLevel.Warning)
				Errors.Add($"{messageTemplate} [{string.Join(", ", propertyValues)}] ({source})");
		}
	}

	[AvaloniaTest]
	public async Task Booting_The_Main_Window_Logs_No_Binding_Errors()
	{
		var previous = Logger.Sink;
		var sink = new BindingErrorSink(previous);
		Logger.Sink = sink;
		try
		{
			await TestHarness.BootAsync();
			await Waiters.WaitForIdleAsync();
		}
		finally
		{
			Logger.Sink = previous;
		}

		sink.Errors.Should().BeEmpty("every binding in the startup UI must resolve against its data context");
	}
}
