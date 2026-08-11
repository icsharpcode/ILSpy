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
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Processes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Windows.Processes;

/// <summary>
/// The .NET Framework half of the process explorer, which exists on Windows only: those
/// processes predate the diagnostics endpoint every CoreCLR process serves, so they are found
/// by the desktop CLR in their OS module list and their assemblies are read from that same
/// list. Windows PowerShell is the fixture - a .NET Framework 4.x application present on
/// every Windows installation, including the CI runner.
/// </summary>
[TestFixture]
[Platform("Win", Reason = ".NET Framework and the OS module list this path reads exist on Windows only.")]
public class NetFrameworkProcessesTests
{
	static Process? host;

	static string WindowsPowerShellPath => Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.System),
		"WindowsPowerShell", "v1.0", "powershell.exe");

	[OneTimeSetUp]
	public void StartADotNetFrameworkProcess()
	{
		File.Exists(WindowsPowerShellPath).Should().BeTrue(
			"Windows PowerShell 5.1 is the .NET Framework process this fixture inspects");

		host = Process.Start(new ProcessStartInfo(WindowsPowerShellPath,
			"-NoProfile -NonInteractive -Command \"Start-Sleep -Seconds 300\"") {
			UseShellExecute = false,
			CreateNoWindow = true,
		});
		host.Should().NotBeNull();

		// The runtime is loaded a moment after the process exists, and nothing about this path
		// works before it is.
		WaitUntilTheDesktopClrIsLoaded(host!);
	}

	[OneTimeTearDown]
	public void StopIt()
	{
		if (host is { HasExited: false })
			host.Kill(entireProcessTree: true);
		host?.Dispose();
	}

	static void WaitUntilTheDesktopClrIsLoaded(Process process)
	{
		var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(60);
		while (DateTime.UtcNow < deadline)
		{
			process.Refresh();
			if (!process.HasExited && LoadedModuleNames(process).Contains("clr.dll"))
				return;
			Thread.Sleep(100);
		}
		Assert.Fail("Windows PowerShell did not load clr.dll within a minute.");
	}

	static HashSet<string> LoadedModuleNames(Process process)
	{
		try
		{
			return process.Modules.Cast<ProcessModule>()
				.Select(m => m.ModuleName)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
		}
		catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
		{
			return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		}
	}

	static RunningDotNetProcess TheHostAsListed(ISet<int>? alreadyListed = null)
	{
		var listed = NetFrameworkProcesses.Enumerate(
			alreadyListed ?? new HashSet<int>(), CancellationToken.None).ToList();
		return listed.Should().ContainSingle(p => p.Pid == host!.Id,
			"a running .NET Framework process is what this path exists to find").Subject;
	}

	[Test]
	public void A_Desktop_Clr_Process_Is_Found_And_Described()
	{
		var listed = TheHostAsListed();

		listed.Kind.Should().Be(RuntimeKind.NetFramework);
		listed.ProcessName.Should().Be("powershell");
		listed.RuntimeVersion.Should().StartWith("4.", "the desktop CLR in use is version 4");
		listed.Architecture.Should().Be(Environment.Is64BitOperatingSystem ? "x64" : "x86",
			"the architecture follows from which Framework directory the CLR was loaded out of");
		// Unlike a modern app, a .NET Framework executable is itself managed - there is no
		// native host in front of it, so the entry assembly is the exe.
		listed.EntryAssemblyName.Should().Be("powershell");
	}

	[Test]
	public void Processes_The_Diagnostics_Scan_Already_Listed_Are_Not_Listed_Twice()
	{
		// The two halves of the explorer are merged, and a process that answered on the
		// diagnostics endpoint must not appear again from the module scan.
		var listed = NetFrameworkProcesses.Enumerate(
			new HashSet<int> { host!.Id }, CancellationToken.None);

		listed.Should().NotContain(p => p.Pid == host!.Id);
	}

	[Test]
	public void Only_The_Managed_Modules_Of_A_Desktop_Clr_Process_Are_Listed()
	{
		var modules = NetFrameworkProcesses.GetModules(host!.Id);

		// The GAC and its NGen cache live under %windir%\assembly; an entry from there proves
		// the scan reports what the loader actually used, not merely files beside the exe.
		// PowerShell's own assemblies cannot serve as that witness: they are IL-only, and an
		// IL-only assembly appears in the OS module list only while its NGen native image is
		// valid - right after a .NET Framework servicing update it silently is not, and the
		// runtime falls back to a memory-mapped IL load this path cannot see.
		string gac = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.Windows), "assembly");
		modules.Should().Contain(m => m.Path!.StartsWith(gac, StringComparison.OrdinalIgnoreCase),
			"the framework assemblies of a desktop CLR process are loaded from the GAC, not from beside the exe");

		// The OS module list mixes native libraries in, and clr.dll is the very entry that
		// identified the process - so this proves the filtering runs, not merely that it exists.
		LoadedModuleNames(host!).Should().Contain("clr.dll");
		modules.Should().NotContain(m => string.Equals(m.Name, "clr.dll", StringComparison.OrdinalIgnoreCase));

		// Stated as the property itself rather than as a list of names to keep out: any native
		// library the filter starts admitting fails this, not just the ones thought of here.
		foreach (var module in modules)
		{
			module.IsInMemory.Should().BeFalse("everything on this path comes from a file");
			ProcessExplorer.IsManagedAssembly(module.Path!).Should().BeTrue(
				$"'{module.Name}' was listed as an assembly loaded in the process");
		}
	}

	[Test]
	public void The_Core_Library_Is_Listed_However_The_Runtime_Loaded_It()
	{
		var modules = NetFrameworkProcesses.GetModules(host!.Id);

		var mscorlib = modules.Should().ContainSingle(m => IsAssembly(m, "mscorlib"),
			"every .NET Framework process has the core library loaded").Subject;

		// Normally this is mscorlib.ni.dll out of the NGen cache rather than the IL assembly
		// in the GAC, because Windows ships pre-compiled native images for the framework. The
		// image carries metadata and is genuinely the module the process loaded, so it is
		// listed - but its IL lives in the assembly it was compiled from, and the OS module
		// list does not say where that is. Opening a native image is therefore of limited use,
		// which is the fidelity gap of this path rather than a defect in it.
		ProcessExplorer.IsManagedAssembly(mscorlib.Path!).Should().BeTrue();
		File.Exists(mscorlib.Path).Should().BeTrue();
	}

	/// <summary>
	/// Whether a listed module is the given assembly, in either of the two forms the desktop
	/// loader reports: the IL assembly, or the NGen native image compiled from it.
	/// </summary>
	static bool IsAssembly(ProcessModuleInfo module, string simpleName)
		=> string.Equals(module.Name, simpleName + ".dll", StringComparison.OrdinalIgnoreCase)
			|| string.Equals(module.Name, simpleName + ".ni.dll", StringComparison.OrdinalIgnoreCase);

	[Test]
	public async Task The_Explorer_Routes_A_Framework_Process_To_The_Module_Scan()
	{
		// The facade decides per process which mechanism answers; a .NET Framework process
		// must not be sent down the diagnostics path, where it has no endpoint at all.
		var explorer = new ProcessExplorer();
		using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
		var processes = await explorer.GetProcessesAsync(cts.Token);
		var listed = processes.Should().ContainSingle(p => p.Pid == host!.Id).Subject;
		listed.Kind.Should().Be(RuntimeKind.NetFramework);

		var modules = await explorer.GetModulesAsync(listed, cts.Token);

		modules.Should().Contain(m => IsAssembly(m, "mscorlib"));
	}

	[Test]
	public void A_Process_That_Has_Exited_Yields_No_Modules()
	{
		using var shortLived = Process.Start(new ProcessStartInfo(WindowsPowerShellPath,
			"-NoProfile -NonInteractive -Command \"exit\"") { UseShellExecute = false, CreateNoWindow = true })!;
		shortLived.WaitForExit();

		NetFrameworkProcesses.GetModules(shortLived.Id).Should().BeEmpty(
			"a process that exits mid-scan is skipped, not reported as an error");
	}
}
