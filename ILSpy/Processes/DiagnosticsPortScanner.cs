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

namespace ICSharpCode.ILSpy.Processes
{
	/// <summary>
	/// Finds the processes that expose a CoreCLR diagnostics IPC endpoint - the same
	/// discovery <c>dotnet-trace ps</c> performs. The endpoint's mere existence identifies a
	/// process as .NET (Core 3.0+): a named pipe <c>dotnet-diagnostic-{pid}</c> on Windows, a
	/// unix domain socket <c>dotnet-diagnostic-{pid}-*-socket</c> in the temp directory on
	/// Linux/macOS. Only same-user processes are visible, and processes started with
	/// <c>DOTNET_EnableDiagnostics=0</c> expose no endpoint at all - both limits are inherent
	/// to the mechanism. The per-OS scans live in the platform partials of this class.
	/// </summary>
	static partial class DiagnosticsPortScanner
	{
		const string TransportPrefix = "dotnet-diagnostic-";

		public static IReadOnlyList<int> GetProcessIds()
		{
			var pids = new HashSet<int>();
			try
			{
				if (OperatingSystem.IsWindows())
					ScanWindowsPipes(pids);
				else
					ScanUnixSockets(pids);
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
			{
				// A transient error enumerating the transport directory yields an empty (or
				// partial) list rather than a failed refresh.
			}
			return pids.OrderBy(pid => pid).ToList();
		}
	}
}
