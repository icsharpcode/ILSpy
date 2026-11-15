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

namespace Redecomp.Bench;

public static class ProjectBuilder
{
	/// <summary>Builds a project directory and returns the path of the produced assembly.</summary>
	public static string BuildProject(string projectDirectory, string assemblyName)
	{
		string output = Path.Combine(projectDirectory, "bin-redecomp");
		SampleBuilder.Run("dotnet", $"build \"{projectDirectory}\" -c Release -o \"{output}\"");
		return Path.Combine(output, assemblyName + ".dll");
	}

	/// <summary>Runs an assembly with its directory as working directory and returns its standard output.</summary>
	public static string RunAssembly(string assemblyPath)
		=> SampleBuilder.Run("dotnet", $"\"{assemblyPath}\"", Path.GetDirectoryName(assemblyPath));

	public static string NewTempDirectory()
	{
		string dir = Path.Combine(Path.GetTempPath(), "redecomp-tests", Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(dir);
		return dir;
	}
}
