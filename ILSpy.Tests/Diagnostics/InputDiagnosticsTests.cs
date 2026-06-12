// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Diagnostics;

[TestFixture]
public class InputDiagnosticsTests
{
	// Stands in for Tmds.DBus.Protocol.DBusException (a transitive dependency we don't reference):
	// InputDiagnostics matches it by type Name and reflects ErrorName/ErrorMessage off it.
	sealed class DBusException : Exception
	{
		public DBusException(string errorName, string errorMessage) : base(errorName + ": " + errorMessage)
		{
			ErrorName = errorName;
			ErrorMessage = errorMessage;
		}

		public string ErrorName { get; }
		public string ErrorMessage { get; }
	}

	[Test]
	public void DescribeException_Unwraps_Aggregate_And_Reflects_DBus_ErrorName()
	{
		var dbus = new DBusException("org.freedesktop.DBus.Error.ServiceUnknown", "The name is not activatable");
		var aggregate = new AggregateException("A Task's exception(s) were not observed",
			new InvalidOperationException("outer", dbus));

		InputDiagnostics.ContainsDBusException(aggregate).Should().BeTrue();

		var report = InputDiagnostics.DescribeException(aggregate);
		report.Should().Contain("InvalidOperationException");
		report.Should().Contain("DBusException");
		report.Should().Contain("ErrorName = org.freedesktop.DBus.Error.ServiceUnknown",
			"the reflected DBus ErrorName is the detail a bare ToString() buries");
		report.Should().Contain("ErrorMessage = The name is not activatable");
	}

	[Test]
	public void ContainsDBusException_Is_False_For_Ordinary_Exceptions()
	{
		InputDiagnostics.ContainsDBusException(new InvalidOperationException("nope")).Should().BeFalse();
		InputDiagnostics.ContainsDBusException(null).Should().BeFalse();
	}

	[AvaloniaTest]
	public void Attach_Records_Keyboard_Interactions_When_DBusDebug_Enabled()
	{
		AppLog.Enable(AppLog.Category.DBusDebug);
		InputDiagnostics.Record("--- test marker ---");

		var window = new Window { Width = 200, Height = 200 };
		var box = new TextBox();
		window.Content = box;
		InputDiagnostics.Attach(window);
		window.Show();
		Dispatcher.UIThread.RunJobs();
		box.Focus();
		Dispatcher.UIThread.RunJobs();

		window.KeyPress(Key.A, RawInputModifiers.None, PhysicalKey.A, keySymbol: "a");
		Dispatcher.UIThread.RunJobs();

		InputDiagnostics.DumpRecent().Should().Contain("KeyDown    A",
			"every keyboard interaction must land on the DBUSDEBUG input trail");
	}
}
