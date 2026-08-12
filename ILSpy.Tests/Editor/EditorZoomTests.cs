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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class EditorZoomTests
{
	[Test]
	public void ZoomIn_Multiplies_By_Factor()
	{
		EditorZoom.ZoomIn(1.0).Should().BeApproximately(1.1, 0.001);
	}

	[Test]
	public void ZoomOut_Divides_By_Factor()
	{
		EditorZoom.ZoomOut(1.1).Should().BeApproximately(1.0, 0.001);
	}

	[Test]
	public void Reset_Returns_Default_Zoom()
	{
		EditorZoom.Reset().Should().Be(EditorZoom.DefaultZoom);
	}

	[Test]
	public void Zoom_Round_Trip_Returns_To_Default_Without_Floating_Point_Drift()
	{
		// 1.1 * (1/1.1) is bit-equal-ish but not exactly equal in float; without the
		// snap-to-default heuristic, the round-tripped value would be 1.0000000001
		// instead of 1.0 and the user would see a sticky "non-100% zoom" even after
		// they explicitly returned to default. The Reset path bypasses this entirely,
		// but Ctrl+Wheel round trips need the snap.
		var step1 = EditorZoom.ZoomIn(EditorZoom.DefaultZoom);
		var step2 = EditorZoom.ZoomOut(step1);
		step2.Should().Be(EditorZoom.DefaultZoom);
	}

	[Test]
	public void ZoomIn_Clamps_At_Upper_Bound()
	{
		EditorZoom.ZoomIn(EditorZoom.MaxZoom).Should().Be(EditorZoom.MaxZoom);
	}

	[Test]
	public void ZoomOut_Clamps_At_Lower_Bound()
	{
		EditorZoom.ZoomOut(EditorZoom.MinZoom).Should().Be(EditorZoom.MinZoom);
	}

	[Test]
	public void ZoomOut_Of_Slightly_Above_Min_Saturates_Not_Below()
	{
		// Slightly above MinZoom zoomed out by the standard factor should land AT min,
		// not below. Guards against an off-by-clamp bug where the post-divide value is
		// below min but clamping only runs on the multiplier output.
		EditorZoom.ZoomOut(EditorZoom.MinZoom * 1.05).Should().Be(EditorZoom.MinZoom);
	}

	[Test]
	public void EffectiveFontSize_Scales_The_Configured_Size()
	{
		var settings = new DisplaySettings { SelectedFontSize = 20, EditorZoomFactor = 1.5 };
		EditorZoom.EffectiveFontSize(settings).Should().BeApproximately(30, 0.001);
	}

	[AvaloniaTest]
	public void Changing_The_Configured_Font_Size_Leaves_The_Zoom_Buttons_Hidden()
	{
		// The options dialog moves the base font size; that is not a zoom, so the
		// overlay must stay hidden and the label must keep reading 100%.
		var settings = new DisplaySettings();
		var buttons = new ZoomButtons();
		buttons.Bind(settings);

		settings.SelectedFontSize = 24;

		buttons.IsVisible.Should().BeFalse();
		buttons.ZoomPercentText.Should().Be("100%");
	}

	[AvaloniaTest]
	public void Zooming_Shows_The_Zoom_Buttons()
	{
		var settings = new DisplaySettings();
		var buttons = new ZoomButtons();
		buttons.Bind(settings);

		settings.EditorZoomFactor = 1.5;

		buttons.IsVisible.Should().BeTrue();
		buttons.ZoomPercentText.Should().Be("150%");
	}
}
