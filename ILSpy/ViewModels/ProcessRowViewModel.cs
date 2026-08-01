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
using System.Globalization;

using ICSharpCode.ILSpy.Processes;

namespace ICSharpCode.ILSpy.ViewModels
{
	/// <summary>
	/// One row of the process list: a running .NET process, in the shape the grid displays
	/// and the filter box searches.
	/// </summary>
	public sealed class ProcessRowViewModel
	{
		public ProcessRowViewModel(RunningDotNetProcess process)
		{
			Process = process;
		}

		public RunningDotNetProcess Process { get; }

		public int Pid => Process.Pid;

		public string ProcessName => Process.ProcessName;

		public string? Architecture => Process.Architecture;

		public string? EntryAssembly => Process.EntryAssemblyName;

		/// <summary>
		/// The runtime flavor and version, e.g. ".NET 10.0.3" or ".NET Framework 4.8.9032.0".
		/// </summary>
		public string Runtime {
			get {
				string product = Process.Kind == RuntimeKind.NetFramework ? ".NET Framework" : ".NET";
				return string.IsNullOrWhiteSpace(Process.RuntimeVersion)
					? product
					: product + " " + Process.RuntimeVersion;
			}
		}

		/// <summary>
		/// Whether this process answers to what the user typed in the filter box. The pid is
		/// matched as well as the names, since it is often the only thing that tells two
		/// instances of the same program apart.
		/// </summary>
		public bool Matches(string filter)
		{
			if (string.IsNullOrWhiteSpace(filter))
				return true;
			filter = filter.Trim();
			return Contains(ProcessName, filter)
				|| Contains(EntryAssembly, filter)
				|| Contains(Pid.ToString(CultureInfo.InvariantCulture), filter);
		}

		static bool Contains(string? value, string filter)
			=> value != null && value.Contains(filter, StringComparison.OrdinalIgnoreCase);
	}
}
