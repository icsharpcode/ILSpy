// Copyright (c) 2026 Franck
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

using System.Diagnostics;

namespace Redecomp.Core.Verification;

/// <summary>Builds a reconstructed project and compares its IL with the original assembly.</summary>
public static class FidelityVerifier
{
	/// <param name="originalAssembly">The assembly the project was reconstructed from.</param>
	/// <param name="projectFile">The reconstructed project file.</param>
	/// <param name="restoreSources">NuGet sources to restore from; null uses the configured ones.</param>
	public static IlComparison Verify(string originalAssembly, string projectFile, string? restoreSources = null)
	{
		string output = Path.Combine(Path.GetTempPath(), "redecomp-verify", Guid.NewGuid().ToString("N"));
		try
		{
			var arguments = new List<string> { "build", projectFile, "-c", "Release", "-o", output, "-p:DebugType=none" };
			if (restoreSources != null)
				arguments.Add($"-p:RestoreSources={restoreSources}");
			var psi = new ProcessStartInfo("dotnet", arguments) {
				RedirectStandardOutput = true,
				RedirectStandardError = true,
			};
			using var process = Process.Start(psi)!;
			string stdout = process.StandardOutput.ReadToEnd();
			string stderr = process.StandardError.ReadToEnd();
			process.WaitForExit();
			if (process.ExitCode != 0)
				throw new InvalidOperationException($"The reconstructed project does not build:\n{stdout}\n{stderr}");
			return IlComparer.Compare(originalAssembly, Path.Combine(output, Path.GetFileName(originalAssembly)));
		}
		finally
		{
			if (Directory.Exists(output))
				Directory.Delete(output, recursive: true);
		}
	}
}
