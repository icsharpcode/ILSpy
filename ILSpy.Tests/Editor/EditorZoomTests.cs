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

using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class EditorZoomTests
{
	[Test]
	public void ZoomIn_Multiplies_By_Factor()
	{
		EditorZoom.ZoomIn(10.0).Should().BeApproximately(11.0, 0.001);
	}

	[Test]
	public void ZoomOut_Divides_By_Factor()
	{
		EditorZoom.ZoomOut(11.0).Should().BeApproximately(10.0, 0.001);
	}

	[Test]
	public void Reset_Returns_Default_Font_Size()
	{
		EditorZoom.Reset().Should().Be(EditorZoom.DefaultFontSize);
	}

	[Test]
	public void Zoom_Round_Trip_Returns_To_Default_Without_Floating_Point_Drift()
	{
		// 1.1 * (1/1.1) is bit-equal-ish but not exactly equal in float; without the
		// snap-to-default heuristic, the rounded-trip value would be 13.3333000001
		// instead of 13.3333333... and the user would see a sticky "non-100% zoom"
		// even after they explicitly returned to default. The Reset path bypasses
		// this entirely, but Ctrl+Wheel round trips need the snap.
		var step1 = EditorZoom.ZoomIn(EditorZoom.DefaultFontSize);
		var step2 = EditorZoom.ZoomOut(step1);
		step2.Should().Be(EditorZoom.DefaultFontSize);
	}

	[Test]
	public void ZoomIn_Clamps_At_Upper_Bound()
	{
		EditorZoom.ZoomIn(EditorZoom.MaxFontSize).Should().Be(EditorZoom.MaxFontSize);
	}

	[Test]
	public void ZoomOut_Clamps_At_Lower_Bound()
	{
		EditorZoom.ZoomOut(EditorZoom.MinFontSize).Should().Be(EditorZoom.MinFontSize);
	}

	[Test]
	public void ZoomOut_Of_Slightly_Above_Min_Saturates_Not_Below()
	{
		// Slightly above MinFontSize zoomed out by the standard factor should land
		// AT min, not below. Guards against an off-by-clamp bug where the post-divide
		// value is below min but clamping only runs on the multiplier output.
		var justAbove = EditorZoom.MinFontSize * 1.05;
		EditorZoom.ZoomOut(justAbove).Should().Be(EditorZoom.MinFontSize);
	}
}
