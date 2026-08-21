using AwesomeAssertions;

using ICSharpCode.ILSpy.AI.Decompiler;
using ICSharpCode.ILSpy.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class BatchRenameItemViewModelTests
{
	[TestCase(0.59, false)]
	[TestCase(0.60, true)]
	[TestCase(1.0, true)]
	public void AutoSelect_UsesSixtyPercentThreshold(double confidence, bool expected)
	{
		BatchRenameItemViewModel.ShouldAutoSelect(new RenameSuggestion("UsefulName", confidence, "reason")).Should().Be(expected);
	}

	[Test]
	public void AutoSelect_RejectsMissingSuggestion()
	{
		BatchRenameItemViewModel.ShouldAutoSelect(null).Should().BeFalse();
	}
}
