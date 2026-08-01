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
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Processes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Processes;

/// <summary>
/// The facade the dialog talks to, verified against the running test host, plus the pure
/// helpers it relies on: telling a managed file from a native one, and working out which
/// assembly is the entry point behind a native apphost.
/// </summary>
[TestFixture]
public class ProcessExplorerTests
{
	static readonly ProcessExplorer Explorer = new();

	[Test]
	public async Task The_Current_Process_Is_Listed_As_A_CoreClr_Process()
	{
		using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

		var processes = await Explorer.GetProcessesAsync(cts.Token);

		var self = processes.Should().ContainSingle(p => p.Pid == Environment.ProcessId).Subject;
		self.Kind.Should().Be(RuntimeKind.CoreClr);
		self.ProcessName.Should().NotBeNullOrWhiteSpace();
		// A null here means the process was listed but did not answer: the diagnostics query
		// failed or ran out its budget. It is not an "unknown entry assembly" - the runtime
		// always knows its own.
		self.EntryAssemblyName.Should().Be(Assembly.GetEntryAssembly()!.GetName().Name,
			"the runtime answered the process-info query");
		self.RuntimeVersion.Should().NotBeNullOrWhiteSpace(
			"the runtime answered the process-info query");
	}

	[Test]
	public void Every_Way_An_Endpoint_Can_Fail_To_Answer_Counts_As_Unreachable()
	{
		// One process that cannot be reached must cost that one row, not the listing: the
		// queries run concurrently, so an exception that escapes the classification fails the
		// whole enumeration and the dialog shows an empty machine.
		ProcessExplorer.IsUnreachable(new SocketException((int)SocketError.ConnectionRefused))
			.Should().BeTrue("a refused unix socket - a stale file, or a runtime already torn down");
		ProcessExplorer.IsUnreachable(new IOException("broken pipe")).Should().BeTrue();
		ProcessExplorer.IsUnreachable(new EndOfStreamException()).Should().BeTrue();
		ProcessExplorer.IsUnreachable(new TimeoutException()).Should().BeTrue("a named pipe with no server");
		ProcessExplorer.IsUnreachable(new UnauthorizedAccessException()).Should().BeTrue("another user's endpoint");

		ProcessExplorer.IsUnreachable(new InvalidDataException("the reader misread a block"))
			.Should().BeFalse("a defect in this code must not be disguised as an unreachable process");
	}

	[Test]
	[Platform(Exclude = "Win", Reason = "Unix domain sockets are the transport being planted; Windows uses named pipes.")]
	public async Task A_Socket_That_Refuses_The_Connection_Costs_One_Row_Not_The_Listing()
	{
		// The reachable case is covered above; this is the same scan with one endpoint that
		// exists as a file but has nobody listening behind it - what a process leaves behind
		// when it exits between the port scan and the connect.
		using var child = StartNonDotNetChildProcess();
		string socketPath = Path.Combine(Path.GetTempPath(), $"dotnet-diagnostic-{child.Id}-1-socket");
		using var abandoned = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
		abandoned.Bind(new UnixDomainSocketEndPoint(socketPath));
		try
		{
			// Not listening, so a connect is refused. Asserted directly, because a planted
			// socket that failed some other way would make the scan below prove nothing.
			var connect = async () => await DiagnosticsIpcClient.ConnectAsync(child.Id, CancellationToken.None);
			await connect.Should().ThrowAsync<SocketException>(
				"a bound socket with no listener refuses connections");

			using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
			var processes = await Explorer.GetProcessesAsync(cts.Token);

			processes.Should().Contain(p => p.Pid == Environment.ProcessId,
				"the reachable processes must still be listed");
			processes.Should().Contain(p => p.Pid == child.Id && p.EntryAssemblyName == null,
				"the unreachable one is listed without the metadata only its runtime could give");
		}
		finally
		{
			abandoned.Close();
			File.Delete(socketPath);
			child.Kill(entireProcessTree: true);
		}
	}

	/// <summary>
	/// A live process that hosts no .NET runtime, so the only diagnostics endpoint it appears
	/// to have is the one the test plants for it.
	/// </summary>
	static System.Diagnostics.Process StartNonDotNetChildProcess()
	{
		var child = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
			"/bin/sh", "-c \"sleep 120\"") { UseShellExecute = false });
		child.Should().NotBeNull();
		return child!;
	}

	[Test]
	public async Task Modules_Of_The_Current_Process_Include_Its_Own_Assemblies()
	{
		using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
		var processes = await Explorer.GetProcessesAsync(cts.Token);
		var self = processes.Single(p => p.Pid == Environment.ProcessId);

		var modules = await Explorer.GetModulesAsync(self, cts.Token);

		modules.Should().Contain(m => m.Name.Equals("ILSpy.dll", StringComparison.OrdinalIgnoreCase),
			"the assembly under test is loaded in the test host");
	}

	[Test]
	public async Task The_Entry_Assembly_Of_The_Current_Process_Resolves_To_A_Real_File()
	{
		using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
		var processes = await Explorer.GetProcessesAsync(cts.Token);
		var self = processes.Single(p => p.Pid == Environment.ProcessId);
		var modules = await Explorer.GetModulesAsync(self, cts.Token);

		string? entryPath = self.ResolveEntryAssemblyPath(modules);

		File.Exists(entryPath).Should().BeTrue();
		Path.GetFileNameWithoutExtension(entryPath).Should().Be(Assembly.GetEntryAssembly()!.GetName().Name);
	}

	[Test]
	public void The_Entry_Assembly_Falls_Back_To_The_Dll_Beside_The_Apphost()
	{
		// A modern app's process shows an .exe that holds no IL at all; the assembly to
		// decompile is the dll of the same name next to it. This is the path taken when the
		// module list is unavailable (an old runtime, or a rundown that failed).
		string apphost = Path.Combine(TestContext.CurrentContext.TestDirectory, "ILSpy.Tests.exe");
		string expected = Path.Combine(TestContext.CurrentContext.TestDirectory, "ILSpy.Tests.dll");
		var process = new RunningDotNetProcess(1, "ILSpy.Tests", RuntimeKind.CoreClr,
			RuntimeVersion: null, Architecture: null, CommandLine: $"\"{apphost}\" --a b",
			EntryAssemblyName: null);

		process.ResolveEntryAssemblyPath(Array.Empty<ProcessModuleInfo>())
			.Should().Be(expected);
	}

	[Test]
	public void The_Entry_Assembly_Is_Taken_From_A_Dotnet_Command_Line()
	{
		string dll = Path.Combine(TestContext.CurrentContext.TestDirectory, "ILSpy.Tests.dll");
		var process = new RunningDotNetProcess(1, "dotnet", RuntimeKind.CoreClr,
			RuntimeVersion: null, Architecture: null, CommandLine: $"/usr/bin/dotnet {dll}",
			EntryAssemblyName: null);

		process.ResolveEntryAssemblyPath(Array.Empty<ProcessModuleInfo>()).Should().Be(dll);
	}

	[Test]
	public void An_Unresolvable_Entry_Assembly_Is_Reported_As_Missing()
	{
		var process = new RunningDotNetProcess(1, "ghost", RuntimeKind.CoreClr,
			RuntimeVersion: null, Architecture: null, CommandLine: "/no/such/path/ghost",
			EntryAssemblyName: "ghost");

		process.ResolveEntryAssemblyPath(Array.Empty<ProcessModuleInfo>()).Should().BeNull();
	}

	[Test]
	public void An_In_Memory_Module_Never_Resolves_To_A_Path()
	{
		var process = new RunningDotNetProcess(1, "host", RuntimeKind.CoreClr,
			RuntimeVersion: null, Architecture: null, CommandLine: null, EntryAssemblyName: "Dynamic");
		var modules = new[] { new ProcessModuleInfo("Dynamic", Path: null, IsInMemory: true) };

		process.ResolveEntryAssemblyPath(modules).Should().BeNull(
			"an assembly with no file cannot be opened from a path");
	}

	[Test]
	public void Managed_Files_Are_Told_Apart_From_Native_Ones()
	{
		string managed = typeof(ProcessExplorer).Assembly.Location;
		string native = NativeRuntimeHost.FullPath;
		File.Exists(native).Should().BeTrue(
			"a native file that is merely missing would pass the assertion below for the wrong reason");

		ProcessExplorer.IsManagedAssembly(managed).Should().BeTrue();
		ProcessExplorer.IsManagedAssembly(native).Should().BeFalse("the runtime host carries no IL");
		ProcessExplorer.IsManagedAssembly(Path.Combine(Path.GetTempPath(), "does-not-exist.dll"))
			.Should().BeFalse();
	}
}
