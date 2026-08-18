// Copyright (c) 2026 Dr. Masroor Ehsan

using AwesomeAssertions;

using ICSharpCode.ILSpy.AI.Controls;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class AIEditorScrollStateTests
{
	[TestCase(100, 50, 50, true)]
	[TestCase(100, 49, 50, true)]
	[TestCase(100, 26, 50, true)]
	[TestCase(100, 25.9, 50, false)]
	[TestCase(100, 0, 50, false)]
	[TestCase(10, 0, 50, true)]
	[TestCase(100, -100, 50, false)]
	public void IsNearBottom_UsesTwentyFourDipThreshold(double extent, double offset, double viewport, bool expected)
	{
		AIEditorScrollState.IsNearBottom(extent, offset, viewport).Should().Be(expected);
	}

	[Test]
	public void IsNearBottom_CoercesNegativeRemainingHeightToZero()
	{
		AIEditorScrollState.IsNearBottom(100, 100, 50).Should().BeTrue();
	}
}
