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

	[Test]
	public void FollowingAppendScrollsToTail()
	{
		AIFollowTailPolicy.ShouldFollowAfterAppend(true).Should().BeTrue();
	}

	[Test]
	public void InactiveAppendRetainsPosition()
	{
		AIFollowTailPolicy.ShouldFollowAfterAppend(false).Should().BeFalse();
	}

	[TestCase(true)]
	[TestCase(false)]
	public void ClearOrNewStreamResetsFromResultingViewport(bool nearBottom)
	{
		AIFollowTailPolicy.ResetAfterLifecycle(nearBottom).Should().Be(nearBottom);
	}

	[Test]
	public void CompletionDoesNotForceScroll()
	{
		AIFollowTailPolicy.ShouldForceScrollOnCompletion().Should().BeFalse();
	}

	[Test]
	public void ReturningToBottomResumesFollowing()
	{
		AIFollowTailPolicy.ShouldFollowAfterAppend(
			AIFollowTailPolicy.ResetAfterLifecycle(nearBottom: true)).Should().BeTrue();
	}

	[Test]
	public void DetachReattachDoesNotRestoreStaleState()
	{
		AIFollowTailPolicy.ResetAfterLifecycle(nearBottom: false).Should().BeFalse();
	}

	[Test]
	public void DelayedRestoreCannotOverrideNewerAttach()
	{
		AIFollowTailPolicy.ResetAfterLifecycle(nearBottom: true).Should().BeTrue();
	}
}
