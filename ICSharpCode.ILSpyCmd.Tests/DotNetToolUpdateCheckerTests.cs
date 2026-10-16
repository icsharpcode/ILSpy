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

using System;

using NUnit.Framework;

namespace ICSharpCode.ILSpyCmd.Tests
{
	/// <summary>
	/// The update check compares the running tool against the versions nuget.org lists for the
	/// package; only the selection logic is tested here, the request itself is not.
	/// </summary>
	[TestFixture]
	public class DotNetToolUpdateCheckerTests
	{
		[Test]
		public void LatestStableVersionIgnoresPrereleases()
		{
			// nuget.org's flat container lists every version lowercase, prereleases included.
			var latest = DotNetToolUpdateChecker.LatestStableVersion(new[] { "11.0.0.9283", "11.1.0.9900-preview1", "11.1.0.9950-pr4241" });

			Assert.That(latest, Is.EqualTo(new Version(11, 0, 0, 9283)));
		}

		[Test]
		public void LatestStableVersionIsTheHighestNotTheLastListed()
		{
			var latest = DotNetToolUpdateChecker.LatestStableVersion(new[] { "9.1.0.7988", "11.0.0.9283", "10.0.0.8800", "not-a-version" });

			Assert.That(latest, Is.EqualTo(new Version(11, 0, 0, 9283)));
		}

		[Test]
		public void LatestStableVersionIsNullWithoutAnyStableVersion()
		{
			Assert.That(DotNetToolUpdateChecker.LatestStableVersion(new[] { "11.1.0.9900-preview1" }), Is.Null);
			Assert.That(DotNetToolUpdateChecker.LatestStableVersion(Array.Empty<string>()), Is.Null);
		}

		[Test]
		public void LatestStableVersionTreatsMissingComponentsAsZero()
		{
			// System.Version alone would rank "11.1.0" below "11.0.9.1" because a missing component compares as -1.
			var latest = DotNetToolUpdateChecker.LatestStableVersion(new[] { "11.0.9.1", "11.1.0" });

			Assert.That(latest, Is.EqualTo(new Version(11, 1, 0, 0)));
		}

		[Test]
		public void RunningVersionDropsPrereleaseLabelAndBuildMetadata()
		{
			// The informational version is DecompilerVersionInfo.FullVersionWithCommitHash,
			// e.g. "11.1.0.9927-pr4241+0dab0067ec".
			Assert.That(DotNetToolUpdateChecker.RunningVersion("11.1.0.9927-pr4241+0dab0067ec"), Is.EqualTo(new Version(11, 1, 0, 9927)));
			Assert.That(DotNetToolUpdateChecker.RunningVersion("11.1.0.9927+0dab0067ec"), Is.EqualTo(new Version(11, 1, 0, 9927)));
			Assert.That(DotNetToolUpdateChecker.RunningVersion("11.1.0.9927"), Is.EqualTo(new Version(11, 1, 0, 9927)));
		}
	}
}
