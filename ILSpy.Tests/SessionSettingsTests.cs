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

using System.Xml.Linq;

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

	[Test]
	public void Save_then_Load_round_trips_ActiveTreeViewPath()
	{
		var original = new SessionSettings {
			ActiveTreeViewPath = ["My.Assembly.dll", "MyNamespace.MyType", "M:MyMethod"],
		};

		var xml = original.SaveToXml();

		var loaded = new SessionSettings();
		loaded.LoadFromXml(xml);

		loaded.ActiveTreeViewPath.Should().Equal("My.Assembly.dll", "MyNamespace.MyType", "M:MyMethod");
	}

	// Legacy SessionSettings XML emitted by the WPF host (ILSpy 10.x) must still load.
	// WindowBounds was a CSV element value (Rect TypeConverter format "L,T,W,H"); the new
	// attribute form is forward-only. ActiveTreeViewPath node values were \xNNNN-hex-escaped
	// so non-alphanumeric chars survive XML round-trips, and the restored path must compare
	// equal to the live tree-node ToString()s — escaped strings will never match.
	[Test]
	public void Load_accepts_legacy_WindowBounds_csv_body()
	{
		var section = XElement.Parse(@"<SessionSettings>
			<WindowState>Normal</WindowState>
			<WindowBounds>882.6666666666666,342,750,550</WindowBounds>
		</SessionSettings>");

		var loaded = new SessionSettings();
		loaded.LoadFromXml(section);

		loaded.WindowPosition.Should().Be(new PixelPoint(882, 342));
		loaded.WindowSize.Should().Be(new Size(750, 550));
	}

	[Test]
	public void Load_unescapes_legacy_ActiveTreeViewPath_node_values()
	{
		// \x002E -> '.', \x003A -> ':', \x005C -> '\' (the escape the WPF SessionSettings.Escape
		// helper applied to every non-letter-or-digit char).
		var section = XElement.Parse(@"<SessionSettings>
			<ActiveTreeViewPath>
				<Node>D\x003A\x005CProjects\x005CILSpy\x005CILSpy\x002Edll</Node>
				<Node>TomsToolbox\x002EWpf</Node>
				<Node>TomsToolbox\x002EWpf\x002EDelegateCommand</Node>
			</ActiveTreeViewPath>
		</SessionSettings>");

		var loaded = new SessionSettings();
		loaded.LoadFromXml(section);

		loaded.ActiveTreeViewPath.Should().Equal(
			@"D:\Projects\ILSpy\ILSpy.dll",
			"TomsToolbox.Wpf",
			"TomsToolbox.Wpf.DelegateCommand");
	}

	[Test]
	public void Load_accepts_full_legacy_SessionSettings_section()
	{
		// Top-level structure the WPF host wrote to ILSpy.xml. DockLayout is intentionally
		// not asserted — the AvalonDock schema doesn't translate to the Avalonia Dock host
		// and is rebuilt from the new layout descriptors.
		var section = XElement.Parse(@"<SessionSettings>
			<FilterSettings>
				<ShowAPILevel>1</ShowAPILevel>
				<Language></Language>
				<LanguageVersion></LanguageVersion>
			</FilterSettings>
			<ActiveAssemblyList>.NET 4 (WPF)</ActiveAssemblyList>
			<ActiveTreeViewPath>
				<Node>D\x003A\x005CProjects\x005CILSpy\x005CILSpy\x002Edll</Node>
				<Node>TomsToolbox\x002EWpf</Node>
			</ActiveTreeViewPath>
			<WindowState>Normal</WindowState>
			<WindowBounds>882.6666666666666,342,750,550</WindowBounds>
			<SelectedSearchMode>TypeAndMember</SelectedSearchMode>
			<Theme>Light</Theme>
		</SessionSettings>");

		var loaded = new SessionSettings();
		loaded.LoadFromXml(section);

		loaded.ActiveAssemblyList.Should().Be(".NET 4 (WPF)");
		loaded.WindowState.Should().Be(WindowState.Normal);
		loaded.WindowPosition.Should().Be(new PixelPoint(882, 342));
		loaded.WindowSize.Should().Be(new Size(750, 550));
		loaded.Theme.Should().Be("Light");
		loaded.ActiveTreeViewPath.Should().Equal(
			@"D:\Projects\ILSpy\ILSpy.dll",
			"TomsToolbox.Wpf");
		loaded.LanguageSettings.Should().NotBeNull();
		loaded.LanguageSettings.ShowApiLevel.Should().Be(ICSharpCode.ILSpyX.ApiVisibility.PublicAndInternal);
	}

	// Forward-compat with retired or future SessionSettings children: unknown elements at
	// load time must be silently ignored, and SaveToXml must not echo them back. The dock
	// layout below is the AvalonDock schema (WPF host); it's incompatible with the Avalonia
	// Dock host and would persist forever as dead state if we round-tripped it.
	[Test]
	public void Load_silently_ignores_unknown_children()
	{
		var section = XElement.Parse(@"<SessionSettings>
			<ActiveAssemblyList>my-list</ActiveAssemblyList>
			<DockLayout>
				<LayoutRoot>
					<RootPanel Orientation=""Horizontal"">
						<LayoutAnchorablePaneGroup Orientation=""Horizontal"" DockWidth=""300"" />
					</RootPanel>
				</LayoutRoot>
			</DockLayout>
			<SelectedSearchMode>TypeAndMember</SelectedSearchMode>
			<ActiveAutoLoadedAssembly>foo.dll</ActiveAutoLoadedAssembly>
			<FutureFeatureFlag value=""xyz"">
				<Nested>123</Nested>
			</FutureFeatureFlag>
		</SessionSettings>");

		var loaded = new SessionSettings();
		var act = () => loaded.LoadFromXml(section);

		act.Should().NotThrow();
		loaded.ActiveAssemblyList.Should().Be("my-list");
	}

	[Test]
	public void Save_does_not_emit_unknown_children_seen_at_load()
	{
		var loaded = new SessionSettings();
		loaded.LoadFromXml(XElement.Parse(@"<SessionSettings>
			<ActiveAssemblyList>my-list</ActiveAssemblyList>
			<DockLayout><LayoutRoot /></DockLayout>
			<SelectedSearchMode>TypeAndMember</SelectedSearchMode>
			<FutureFeatureFlag value=""xyz"" />
		</SessionSettings>"));

		var saved = loaded.SaveToXml();

		saved.Element("DockLayout").Should().BeNull();
		saved.Element("SelectedSearchMode").Should().BeNull();
		saved.Element("FutureFeatureFlag").Should().BeNull();
	}
}
