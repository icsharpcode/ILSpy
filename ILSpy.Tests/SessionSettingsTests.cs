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

using Avalonia;
using Avalonia.Controls;

using AwesomeAssertions;

using ILSpy;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

// SessionSettings persists window bounds + active list to <SessionSettings> in ILSpy.xml.
// If serialization and deserialization don't round-trip, a user's saved window position /
// active list silently resets on every launch — and the bug is invisible until they
// actually restart with a non-default layout. Round-tripping is the contract.
[TestFixture]
public class SessionSettingsTests
{
	[Test]
	public void Save_then_Load_round_trips_all_fields()
	{
		var original = new SessionSettings {
			ActiveAssemblyList = "my-list",
			WindowState = WindowState.Maximized,
			WindowPosition = new PixelPoint(200, 300),
			WindowSize = new Size(1024, 768),
		};

		var xml = original.SaveToXml();

		var loaded = new SessionSettings();
		loaded.LoadFromXml(xml);

		loaded.ActiveAssemblyList.Should().Be("my-list");
		loaded.WindowState.Should().Be(WindowState.Maximized);
		loaded.WindowPosition.Should().Be(new PixelPoint(200, 300));
		loaded.WindowSize.Should().Be(new Size(1024, 768));
	}
}
