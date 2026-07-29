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
using System.Text;

namespace ICSharpCode.ILSpy.Processes
{
	/// <summary>
	/// The runtime flavor hosted by a running process. CoreCLR processes are discovered and
	/// inspected through the runtime's diagnostics IPC endpoint on every OS; .NET Framework
	/// processes exist on Windows only and are inspected through OS-level module scanning.
	/// </summary>
	public enum RuntimeKind
	{
		CoreClr,
		NetFramework,
	}

	/// <summary>
	/// A running process that hosts a .NET runtime. Deliberately free of
	/// <c>System.Diagnostics</c> types so view models and tests never touch live processes.
	/// All fields except pid, name and kind are best-effort: they come from the runtime's
	/// answer to a process-info query and may be missing on old runtimes.
	/// </summary>
	public sealed record RunningDotNetProcess(
		int Pid,
		string ProcessName,
		RuntimeKind Kind,
		string? RuntimeVersion,
		string? Architecture,
		string? CommandLine,
		string? EntryAssemblyName)
	{
		/// <summary>
		/// The file holding the assembly this process started from, or null if it cannot be
		/// reached from a path. With a modern app the executable in the process list is a
		/// native host that carries no IL, so the answer is normally a dll of the same name
		/// beside it - or, for a single-file app, the executable itself, which ILSpy opens
		/// as the bundle it is.
		/// </summary>
		public string? ResolveEntryAssemblyPath(IReadOnlyList<ProcessModuleInfo> modules)
		{
			// The runtime's own module list is authoritative: it gives the real path even
			// when the assembly was loaded from somewhere unrelated to the command line.
			if (EntryAssemblyName != null)
			{
				var match = modules.FirstOrDefault(m => !m.IsInMemory && m.Path != null
					&& string.Equals(Path.GetFileNameWithoutExtension(m.Name), EntryAssemblyName, StringComparison.OrdinalIgnoreCase));
				if (match != null)
					return match.Path;
			}
			return ResolveFromCommandLine();
		}

		string? ResolveFromCommandLine()
		{
			string? executable = FirstCommandLineToken();
			if (executable == null)
				return null;

			if (executable.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
				return File.Exists(executable) ? executable : null;

			// "dotnet app.dll" names the assembly in its second token.
			string? second = SecondCommandLineToken();
			if (second != null && second.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) && File.Exists(second))
				return second;

			string sibling = Path.ChangeExtension(executable, ".dll");
			if (File.Exists(sibling))
				return sibling;

			// No managed sibling: a single-file app has its assemblies bundled into the
			// executable itself.
			return File.Exists(executable) ? executable : null;
		}

		string? FirstCommandLineToken() => SplitCommandLine().FirstOrDefault();

		string? SecondCommandLineToken() => SplitCommandLine().Skip(1).FirstOrDefault();

		/// <summary>
		/// Splits the reported command line into tokens, honoring double quotes around paths
		/// that contain spaces.
		/// </summary>
		IEnumerable<string> SplitCommandLine()
		{
			if (string.IsNullOrWhiteSpace(CommandLine))
				yield break;
			var token = new StringBuilder();
			bool quoted = false;
			foreach (char c in CommandLine)
			{
				if (c == '"')
				{
					quoted = !quoted;
				}
				else if (char.IsWhiteSpace(c) && !quoted)
				{
					if (token.Length > 0)
					{
						yield return token.ToString();
						token.Clear();
					}
				}
				else
				{
					token.Append(c);
				}
			}
			if (token.Length > 0)
				yield return token.ToString();
		}
	}

	/// <summary>
	/// A managed assembly loaded in a running process. <see cref="Path"/> is null (and
	/// <see cref="IsInMemory"/> true) for assemblies without a file on disk - byte-array or
	/// dynamic loads - which can be listed but not opened from a path.
	/// </summary>
	public sealed record ProcessModuleInfo(
		string Name,
		string? Path,
		bool IsInMemory);
}
