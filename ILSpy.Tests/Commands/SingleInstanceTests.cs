// Copyright (c) 2026 Siegfried Pammer
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
using System.IO;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class SingleInstanceTests
{
	[Test]
	public void InstanceId_Option_Is_Parsed()
	{
		CommandLineArguments.Create(new[] { "--instanceid", "Foo" }).InstanceId.Should().Be("Foo");
	}

	[Test]
	public void InstanceId_Defaults_To_Null_When_Absent()
	{
		CommandLineArguments.Create(Array.Empty<string>()).InstanceId.Should().BeNull();
	}

	[Test]
	public void NewInstance_Option_Forces_SingleInstance_False()
	{
		// --newinstance is what disables single-instance for a single launch; lock in the mapping
		// now that a consumer finally reads CommandLineArguments.SingleInstance.
		CommandLineArguments.Create(new[] { "--newinstance" }).SingleInstance.Should().BeFalse();
		CommandLineArguments.Create(Array.Empty<string>()).SingleInstance.Should().BeNull();
	}

	[Test]
	public void GetInstanceName_Is_Deterministic_And_Contains_No_Path_Separators()
	{
		// The mutex/pipe namespace is derived from machine + user only -- no executable location --
		// so every launch of the same user shares one instance regardless of which exe path it came
		// from. The name feeds a Mutex/pipe identifier, so it must carry no path separators.
		var name = SingleInstance.GetInstanceName();

		name.Should().Be(SingleInstance.GetInstanceName());
		name.Should().StartWith("ILSpy.");
		name.Should().NotContain("/");
		name.Should().NotContain("\\");
	}

	[Test]
	public void ShouldReuse_When_No_Id_Requested()
	{
		// A launcher without --instanceid (Explorer "Open with", a plain CLI launch) reuses whatever
		// instance is running, regardless of that instance's identity.
		SingleInstance.ShouldReuse(null, null, "/opt/ilspy/ILSpy").Should().BeTrue();
		SingleInstance.ShouldReuse("", "launch-id", "/opt/ilspy/ILSpy").Should().BeTrue();
	}

	[Test]
	public void ShouldReuse_When_Requested_Id_Matches_Running_Launch_Id()
	{
		SingleInstance.ShouldReuse("Foo", "Foo", "/opt/ilspy/ILSpy").Should().BeTrue();
	}

	[Test]
	public void ShouldReuse_When_Requested_Id_Matches_Running_Executable_Path()
	{
		// The only-AddIn-installed case: VS requests its bundled exe path, and the running instance
		// was started WITHOUT an id (e.g. by a shell "Open with ILSpy" on that same exe). It must
		// still match, because the running instance reports the executable it actually is.
		SingleInstance.ShouldReuse("/opt/ilspy/ILSpy", null, "/opt/ilspy/ILSpy").Should().BeTrue();
	}

	[Test]
	public void ShouldReuse_False_When_Requested_Id_Matches_Neither()
	{
		SingleInstance.ShouldReuse("/opt/other/ILSpy", "Foo", "/opt/ilspy/ILSpy").Should().BeFalse();
	}

	[Test]
	public void FullyQualifyPath_Roots_An_Existing_Relative_File_Against_The_Base_Directory()
	{
		var dir = Path.Combine(Path.GetTempPath(), "ILSpySingleInstance_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(dir);
		try
		{
			var file = Path.Combine(dir, "some.dll");
			File.WriteAllText(file, "");

			SingleInstance.FullyQualifyPath("some.dll", dir).Should().Be(file);
		}
		finally
		{
			Directory.Delete(dir, recursive: true);
		}
	}

	[Test]
	public void FullyQualifyPath_Leaves_Non_File_Tokens_Unchanged()
	{
		var dir = Path.GetTempPath();
		var missing = "definitely-not-a-file-" + Guid.NewGuid().ToString("N");
		var rooted = Path.Combine(dir, "does-not-exist-" + Guid.NewGuid().ToString("N") + ".dll");

		// An option flag, a value that is not an existing file, and an already-rooted path all pass
		// through untouched so the receiving instance re-parses them exactly as typed.
		SingleInstance.FullyQualifyPath("--navigateto", dir).Should().Be("--navigateto");
		SingleInstance.FullyQualifyPath(missing, dir).Should().Be(missing);
		SingleInstance.FullyQualifyPath(rooted, dir).Should().Be(rooted);
	}
}
