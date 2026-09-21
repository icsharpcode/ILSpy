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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Processes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Processes;

/// <summary>
/// The .NET Framework side of the process listing, verified against a Windows PowerShell
/// child - the one .NET Framework program every Windows installation carries.
/// </summary>
[TestFixture]
[Platform("Win")]
[SupportedOSPlatform("windows")]
public class NetFrameworkProcessesTests
{
	[Test]
	public void A_Process_Without_A_Desktop_Clr_Is_Ruled_Out_Without_Reading_Its_Modules()
	{
		// The test host runs on CoreCLR, which publishes no desktop IPC block.
		NetFrameworkProcesses.MayHostDesktopClr(Environment.ProcessId).Should().BeFalse();
	}

	[Test]
	public void A_Net_Framework_Process_Is_A_Candidate()
	{
		using var child = StartWindowsPowerShell();
		try
		{
			NetFrameworkProcesses.MayHostDesktopClr(child.Id).Should().BeTrue();
		}
		finally
		{
			child.Kill(entireProcessTree: true);
		}
	}

	[Test]
	public void A_Net_Framework_Process_Is_Listed_With_Its_Runtime()
	{
		using var child = StartWindowsPowerShell();
		try
		{
			var listed = NetFrameworkProcesses.Enumerate(new HashSet<int>(), CancellationToken.None).ToList();

			var row = listed.Should().ContainSingle(p => p.Pid == child.Id).Subject;
			row.Kind.Should().Be(RuntimeKind.NetFramework);
			row.RuntimeVersion.Should().StartWith("4.");
			listed.Should().NotContain(p => p.Pid == Environment.ProcessId, "the test host runs on CoreCLR");
		}
		finally
		{
			child.Kill(entireProcessTree: true);
		}
	}

	/// <summary>
	/// A Windows PowerShell 5.1 process that has provably started its runtime: the line it
	/// prints is written by managed code, so no polling for the CLR to appear is needed.
	/// </summary>
	static Process StartWindowsPowerShell()
	{
		string exe = Path.Combine(Environment.SystemDirectory, @"WindowsPowerShell\v1.0\powershell.exe");
		File.Exists(exe).Should().BeTrue("Windows PowerShell ships with every Windows installation");
		var child = Process.Start(new ProcessStartInfo(exe, "-NoProfile -NonInteractive -Command \"'ready'; Start-Sleep 120\"") {
			UseShellExecute = false,
			RedirectStandardOutput = true,
			CreateNoWindow = true,
		});
		child.Should().NotBeNull();
		child!.StandardOutput.ReadLine().Should().Be("ready");
		return child;
	}
}
