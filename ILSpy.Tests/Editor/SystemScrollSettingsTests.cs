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

using AwesomeAssertions;

using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Editor;

/// <summary>
/// KDE keeps the Plasma "wheel scroll lines" value in kdeglobals, group <c>[KDE]</c>, key
/// <c>WheelScrollLines</c>; Qt applications read it through the KDE platform theme. The parser
/// must find exactly that key in exactly that group and shrug at everything else in the file.
/// </summary>
[TestFixture]
public class SystemScrollSettingsTests
{
	[Test]
	public void Kde_Wheel_Scroll_Lines_Are_Read_From_The_KDE_Group()
	{
		const string kdeglobals = "[General]\nColorScheme=BreezeLight\n\n[KDE]\nSingleClick=false\nWheelScrollLines=5\n\n[Icons]\nTheme=breeze\n";

		SystemScrollSettings.ParseKdeWheelScrollLines(kdeglobals).Should().Be(5u);
	}

	[Test]
	public void Kde_Wheel_Scroll_Lines_Tolerate_Windows_Line_Endings()
	{
		const string kdeglobals = "[KDE]\r\nWheelScrollLines=4\r\n";

		SystemScrollSettings.ParseKdeWheelScrollLines(kdeglobals).Should().Be(4u);
	}

	[Test]
	public void Kde_Wheel_Scroll_Lines_Outside_The_KDE_Group_Are_Ignored()
	{
		const string kdeglobals = "[Mouse]\nWheelScrollLines=7\n\n[KDE]\nSingleClick=false\n";

		SystemScrollSettings.ParseKdeWheelScrollLines(kdeglobals).Should().BeNull("only the [KDE] group holds the Plasma setting");
	}

	[Test]
	public void Kde_Wheel_Scroll_Lines_Missing_Or_Malformed_Yield_Null()
	{
		SystemScrollSettings.ParseKdeWheelScrollLines("").Should().BeNull();
		SystemScrollSettings.ParseKdeWheelScrollLines("[KDE]\nWheelScrollLines=many\n").Should().BeNull();
	}
}
