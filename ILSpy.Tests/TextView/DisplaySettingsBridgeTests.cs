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

using AwesomeAssertions;

using ICSharpCode.Decompiler;

using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// The Display options that affect generated source (brace folding, debug info, indentation, and the
/// fold-expansion flags) must be bridged into the decompiler settings used for a decompile. WPF wired
/// these in DecompilationOptions; the Avalonia port previously only bridged the two expand flags, so
/// FoldBraces / ShowDebugInfo / indentation were silently dead.
/// </summary>
[TestFixture]
public class DisplaySettingsBridgeTests
{
	[Test]
	public void Brace_Folding_And_Debug_Info_Are_Bridged()
	{
		var display = new DisplaySettings { FoldBraces = true, ShowDebugInfo = true };
		var settings = new DecompilerSettings { FoldBraces = false, ShowDebugInfo = false };

		SettingsService.ApplyDisplaySettings(settings, display);

		settings.FoldBraces.Should().BeTrue();
		settings.ShowDebugInfo.Should().BeTrue();
	}

	[Test]
	public void Indentation_Uses_Spaces_When_Tabs_Are_Off()
	{
		var display = new DisplaySettings { IndentationUseTabs = false, IndentationSize = 2 };
		var settings = new DecompilerSettings();

		SettingsService.ApplyDisplaySettings(settings, display);

		settings.CSharpFormattingOptions.IndentationString.Should().Be("  ", "two spaces");
	}

	[Test]
	public void Indentation_Uses_Tabs_When_Enabled()
	{
		var display = new DisplaySettings { IndentationUseTabs = true, IndentationSize = 4, IndentationTabSize = 4 };
		var settings = new DecompilerSettings();

		SettingsService.ApplyDisplaySettings(settings, display);

		settings.CSharpFormattingOptions.IndentationString.Should().Be("\t", "one tab");
	}

	[Test]
	public void Expand_Flags_Still_Bridged()
	{
		var display = new DisplaySettings { ExpandUsingDeclarations = true, ExpandMemberDefinitions = true };
		var settings = new DecompilerSettings { ExpandUsingDeclarations = false, ExpandMemberDefinitions = false };

		SettingsService.ApplyDisplaySettings(settings, display);

		settings.ExpandUsingDeclarations.Should().BeTrue();
		settings.ExpandMemberDefinitions.Should().BeTrue();
	}
}
