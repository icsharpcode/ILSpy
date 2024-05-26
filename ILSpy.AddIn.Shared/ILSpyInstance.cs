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

			var process = new Process() {
				StartInfo = new ProcessStartInfo() {
					FileName = ilSpyExe,
					UseShellExecute = false,
					Arguments = argumentsToPass
				}
			};
			process.Start();
		}
	}
}
