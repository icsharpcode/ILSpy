// Copyright (c) 2018 Andreas Weizel
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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ICSharpCode.ILSpy.AddIn
{
	class ILSpyParameters
	{
		public ILSpyParameters(IEnumerable<string> assemblyFileNames, params string[] arguments)
		{
			this.AssemblyFileNames = assemblyFileNames.ToArray();
			this.Arguments = arguments;
		}

		public string[] AssemblyFileNames { get; private set; }
		public string[] Arguments { get; private set; }
	}

	class ILSpyInstance
	{
		internal static readonly ConcurrentStack<string> TempFiles = new ConcurrentStack<string>();

		readonly ILSpyParameters parameters;
		public ILSpyInstance(ILSpyParameters parameters = null)
		{
			this.parameters = parameters;
		}

		static string GetILSpyPath()
		{
			// Only VS2022 supports arm64, so we can gloss over 2017-2019 support
			string archPathSegment = "x64";
			if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
			{
				archPathSegment = "arm64";
			}

			var basePath = Path.GetDirectoryName(typeof(ILSpyAddInPackage).Assembly.Location);
			return Path.Combine(basePath, archPathSegment, "ILSpy", "ILSpy.exe");
		}

		public void Start()
		{
			var commandLineArguments = parameters?.AssemblyFileNames?.Concat(parameters.Arguments);
			string ilSpyExe = GetILSpyPath();

			const string defaultOptions = "--navigateto:none";
			string argumentsToPass = defaultOptions;

			if ((commandLineArguments != null) && commandLineArguments.Any())
			{
				string assemblyArguments = string.Join("\r\n", commandLineArguments);

				string filepath = Path.GetTempFileName();
				File.WriteAllText(filepath, assemblyArguments);

				TempFiles.Push(filepath);

				argumentsToPass = $"@\"{filepath}\"";
			}

			// Target only this bundled ILSpy build: --instanceid names the exe we launch, so ILSpy
			// reuses a running instance iff it is that same executable, and otherwise opens its own
			// window instead of joining a different ILSpy the user may have open.
			string instanceId = Path.GetFullPath(ilSpyExe);

			var process = new Process() {
				StartInfo = new ProcessStartInfo() {
					FileName = ilSpyExe,
					UseShellExecute = false,
					Arguments = $"--instanceid \"{instanceId}\" {argumentsToPass}"
				}
			};
			process.Start();
		}
	}
}
