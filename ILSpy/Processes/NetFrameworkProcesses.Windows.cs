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

namespace ICSharpCode.ILSpy.Processes
{
	/// <summary>
	/// Finds and inspects .NET Framework processes, which exist on Windows only and predate
	/// the diagnostics endpoint every CoreCLR process exposes. Both answers come from the OS
	/// module list: the desktop CLR is present in it as clr.dll (or mscorwks.dll before .NET
	/// 4), and - unlike CoreCLR - the desktop loader registers the managed images it maps via
	/// LoadLibrary there too, so filtering that list to files carrying a CLI header yields
	/// the managed set. That covers NGen native images and mixed-mode assemblies only: an
	/// IL-only assembly is memory-mapped without a loader entry (the runtime falls back to
	/// that whenever no valid native image exists), and assemblies loaded from a byte array
	/// have no file anywhere - both are invisible on this path.
	/// </summary>
	[SupportedOSPlatform("windows")]
	static class NetFrameworkProcesses
	{
		static readonly string[] DesktopClrModules = { "clr.dll", "mscorwks.dll", "mscorsvr.dll" };

		public static IEnumerable<RunningDotNetProcess> Enumerate(ISet<int> alreadyListed, CancellationToken cancellationToken)
		{
			// Every process on the machine is inspected, and reading one module list is a few
			// hundred cross-process calls. Done one process at a time on a loaded machine that
			// adds up to minutes, so the processes are inspected concurrently - as the CoreCLR
			// half of the explorer already queries its runtimes.
			return Process.GetProcesses()
				.AsParallel()
				.WithCancellation(cancellationToken)
				.Select(process => {
					using (process)
					{
						return alreadyListed.Contains(process.Id) ? null : TryDescribe(process);
					}
				})
				.OfType<RunningDotNetProcess>();
		}

		static RunningDotNetProcess? TryDescribe(Process process)
		{
			try
			{
				var clr = process.Modules.Cast<ProcessModule>()
					.FirstOrDefault(m => DesktopClrModules.Contains(m.ModuleName, StringComparer.OrdinalIgnoreCase));
				if (clr == null)
					return null;

				string? mainModule = TryGetMainModuleFileName(process);
				return new RunningDotNetProcess(process.Id, process.ProcessName, RuntimeKind.NetFramework,
					RuntimeVersion: clr.FileVersionInfo.FileVersion,
					// The desktop CLR lives under Framework64 in a 64-bit process and under
					// Framework in a 32-bit one, which settles the architecture without
					// having to open the process for a bitness query.
					Architecture: clr.FileName.Contains(@"\Framework64\", StringComparison.OrdinalIgnoreCase) ? "x64" : "x86",
					CommandLine: mainModule,
					// A .NET Framework executable is itself managed - there is no separate
					// native host to see through.
					EntryAssemblyName: mainModule == null ? null : Path.GetFileNameWithoutExtension(mainModule));
			}
			catch (Exception ex) when (IsProcessAccessFailure(ex))
			{
				// Processes of other users, elevated processes, and processes that exit
				// mid-scan are simply not listed.
				return null;
			}
		}

		public static IReadOnlyList<ProcessModuleInfo> GetModules(int pid)
		{
			try
			{
				using var process = Process.GetProcessById(pid);
				return process.Modules.Cast<ProcessModule>()
					.Select(m => m.FileName)
					.Where(f => !string.IsNullOrEmpty(f) && ProcessExplorer.IsManagedAssembly(f))
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.Select(f => new ProcessModuleInfo(Path.GetFileName(f), f, IsInMemory: false))
					.OrderBy(m => m.Name, StringComparer.OrdinalIgnoreCase)
					.ToList();
			}
			catch (Exception ex) when (IsProcessAccessFailure(ex))
			{
				return Array.Empty<ProcessModuleInfo>();
			}
		}

		static string? TryGetMainModuleFileName(Process process)
		{
			try
			{
				return process.MainModule?.FileName;
			}
			catch (Exception ex) when (IsProcessAccessFailure(ex))
			{
				return null;
			}
		}

		static bool IsProcessAccessFailure(Exception ex)
			=> ex is System.ComponentModel.Win32Exception or InvalidOperationException
				or NotSupportedException or ArgumentException;
	}
}
