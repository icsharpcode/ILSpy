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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy.Processes
{
	/// <summary>
	/// Lists running .NET processes and their loaded assemblies. CoreCLR processes are found
	/// and inspected through the runtime's own diagnostics endpoint, which works identically
	/// on Windows, Linux and macOS and sees assemblies no OS-level module list can report.
	/// On Windows the list additionally covers .NET Framework processes, which predate that
	/// endpoint and are inspected through their native module list instead.
	/// </summary>
	public sealed class ProcessExplorer : IProcessExplorer
	{
		public Task<IReadOnlyList<RunningDotNetProcess>> GetProcessesAsync(CancellationToken cancellationToken)
			=> Task.Run(() => EnumerateProcesses(cancellationToken), cancellationToken);

		public Task<IReadOnlyList<ProcessModuleInfo>> GetModulesAsync(
			RunningDotNetProcess process, CancellationToken cancellationToken)
			=> Task.Run(() => EnumerateModules(process, cancellationToken), cancellationToken);

		static async Task<IReadOnlyList<RunningDotNetProcess>> EnumerateProcesses(CancellationToken cancellationToken)
		{
			var pids = DiagnosticsPortScanner.GetProcessIds();
			// The runtimes are queried concurrently: one unresponsive process would
			// otherwise hold up the whole listing for its share of the timeout.
			var queries = pids.Select(pid => DescribeCoreClrProcessAsync(pid, cancellationToken));
			var processes = (await Task.WhenAll(queries).ConfigureAwait(false))
				.OfType<RunningDotNetProcess>()
				.ToList();

			if (OperatingSystem.IsWindows())
			{
				var known = processes.Select(p => p.Pid).ToHashSet();
				processes.AddRange(NetFrameworkProcesses.Enumerate(known, cancellationToken));
			}

			return processes
				.OrderBy(p => p.ProcessName, StringComparer.CurrentCultureIgnoreCase)
				.ThenBy(p => p.Pid)
				.ToList();
		}

		static async Task<RunningDotNetProcess?> DescribeCoreClrProcessAsync(int pid, CancellationToken cancellationToken)
		{
			string? processName = TryGetProcessName(pid);
			if (processName == null)
				return null; // Exited between the port scan and now.
			try
			{
				var info = await DiagnosticsIpcClient.GetProcessInfoAsync(pid, cancellationToken).ConfigureAwait(false);
				return new RunningDotNetProcess(pid, processName, RuntimeKind.CoreClr,
					info.ClrVersion, info.Architecture, info.CommandLine, info.EntryAssemblyName);
			}
			catch (Exception ex) when (IsUnreachable(ex))
			{
				// The endpoint exists but did not answer - a suspended runtime, a stale
				// transport, or a process shutting down. Still worth listing.
				return new RunningDotNetProcess(pid, processName, RuntimeKind.CoreClr,
					RuntimeVersion: null, Architecture: null, CommandLine: null, EntryAssemblyName: null);
			}
		}

		static async Task<IReadOnlyList<ProcessModuleInfo>> EnumerateModules(
			RunningDotNetProcess process, CancellationToken cancellationToken)
		{
			if (process.Kind == RuntimeKind.NetFramework)
			{
				if (!OperatingSystem.IsWindows())
					return Array.Empty<ProcessModuleInfo>();
				return NetFrameworkProcesses.GetModules(process.Pid);
			}

			using var rundown = await DiagnosticsIpcClient
				.CollectModuleRundownAsync(process.Pid, cancellationToken).ConfigureAwait(false);
			return NettraceRundownReader.ReadModules(rundown);
		}

		static string? TryGetProcessName(int pid)
		{
			try
			{
				using var process = Process.GetProcessById(pid);
				return process.ProcessName;
			}
			catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
			{
				return null;
			}
		}

		/// <summary>
		/// Whether an exception means "that process did not answer" rather than "this code is
		/// wrong". Such a process is still listed, only without the metadata that none but its
		/// own runtime could supply; anything else propagates, because a process explorer that
		/// swallows its own defects reports an empty machine.
		/// </summary>
		/// <remarks>
		/// The endpoints are queried concurrently, so this classification decides between
		/// losing one row and losing the listing. Both transports are covered:
		/// <see cref="Win32Exception"/> is the base of <see cref="System.Net.Sockets.SocketException"/>
		/// and thus not an <see cref="IOException"/> - a unix socket file whose runtime is gone
		/// refuses the connection and raises it - while a Windows named pipe with no server
		/// behind it surfaces as <see cref="TimeoutException"/>.
		/// </remarks>
		internal static bool IsUnreachable(Exception ex)
			=> ex is IOException or Win32Exception or TimeoutException or UnauthorizedAccessException;

		/// <summary>
		/// Whether the file at <paramref name="path"/> is a managed assembly, i.e. a PE image
		/// carrying a CLI header. Used to keep native libraries out of the module list of a
		/// .NET Framework process, whose OS module list mixes both.
		/// </summary>
		internal static bool IsManagedAssembly(string path)
		{
			try
			{
				using var stream = File.OpenRead(path);
				using var peReader = new PEReader(stream);
				return peReader.HasMetadata;
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or BadImageFormatException or ArgumentException)
			{
				return false;
			}
		}
	}
}
