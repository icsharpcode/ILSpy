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

namespace ICSharpCode.ILSpy.Processes
{
	static partial class DiagnosticsPortScanner
	{
		// The runtime places its socket in TMPDIR (else /tmp), which is exactly what
		// Path.GetTempPath() resolves on unix. Socket files of exited processes linger, so a
		// name match alone is not proof of a live process - callers see those filtered out.
		static void ScanUnixSockets(HashSet<int> pids)
		{
			foreach (string path in Directory.EnumerateFiles(Path.GetTempPath(), TransportPrefix + "*"))
			{
				if (TryParseSocketPid(Path.GetFileName(path)) is int pid && IsProcessAlive(pid))
					pids.Add(pid);
			}
		}

		/// <summary>
		/// Returns the socket file to connect to for <paramref name="pid"/>, or null if the
		/// process exposes none. If a stale file of a previous process with the same pid
		/// coexists with the live one, the newest file is the live runtime's.
		/// </summary>
		public static string? GetUnixSocketPath(int pid)
		{
			try
			{
				string? best = null;
				DateTime bestTime = DateTime.MinValue;
				foreach (string path in Directory.EnumerateFiles(Path.GetTempPath(), TransportPrefix + pid + "-*"))
				{
					if (TryParseSocketPid(Path.GetFileName(path)) != pid)
						continue;
					DateTime writeTime = File.GetLastWriteTimeUtc(path);
					if (best == null || writeTime > bestTime)
					{
						best = path;
						bestTime = writeTime;
					}
				}
				return best;
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
			{
				return null;
			}
		}

		// Socket files are named "dotnet-diagnostic-{pid}-{disambiguation}-socket".
		static int? TryParseSocketPid(string fileName)
		{
			if (!fileName.StartsWith(TransportPrefix, StringComparison.Ordinal) || !fileName.EndsWith("-socket", StringComparison.Ordinal))
				return null;
			ReadOnlySpan<char> rest = fileName.AsSpan(TransportPrefix.Length);
			int dash = rest.IndexOf('-');
			if (dash <= 0 || !int.TryParse(rest[..dash], out int pid))
				return null;
			return pid;
		}

		static bool IsProcessAlive(int pid)
		{
			try
			{
				using var process = System.Diagnostics.Process.GetProcessById(pid);
				return !process.HasExited;
			}
			catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
			{
				return false;
			}
		}
	}
}
