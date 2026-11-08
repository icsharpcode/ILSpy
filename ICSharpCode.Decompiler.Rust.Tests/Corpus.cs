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
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Threading;

using ICSharpCode.Decompiler.CSharp;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// The images the equality tests run over: the decompiler itself, every file of the running
/// shared framework and, when a run of ICSharpCode.Decompiler.Tests has compiled them, every
/// test case fixture.
/// </summary>
static class Corpus
{
	internal static string TestCasesDirectory([CallerFilePath] string thisFile = "")
		=> Path.GetFullPath(Path.Combine(Path.GetDirectoryName(thisFile)!, "..", "ICSharpCode.Decompiler.Tests", "TestCases"));

	static IEnumerable<string> Assemblies(string directory, SearchOption option)
		=> Directory.Exists(directory)
			? Directory.EnumerateFiles(directory, "*.*", option)
				.Where(f => f.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
			: [];

	public static IEnumerable<TestCaseData> AllImages()
	{
		yield return new TestCaseData(typeof(CSharpDecompiler).Assembly.Location).SetArgDisplayNames("ICSharpCode.Decompiler.dll");
		string runtime = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
		foreach (var file in Assemblies(runtime, SearchOption.TopDirectoryOnly).Order())
			yield return new TestCaseData(file).SetArgDisplayNames("runtime/" + Path.GetFileName(file));
		string fixtures = TestCasesDirectory();
		foreach (var file in Assemblies(fixtures, SearchOption.AllDirectories).Order())
			yield return new TestCaseData(file).SetArgDisplayNames(Path.GetRelativePath(fixtures, file).Replace(Path.DirectorySeparatorChar, '/'));
	}

	public static IEnumerable<TestCaseData> ManagedImages()
		=> AllImages().Where(c => HasMetadata((string)c.Arguments[0]!));

	/// <summary>
	/// The managed images with a PDB next to them or embedded, plus this test assembly and
	/// ICSharpCode.ILSpyX, whose PDBs are built with them.
	/// </summary>
	public static IEnumerable<TestCaseData> ImagesWithSymbols()
	{
		foreach (var type in new[] { typeof(Corpus), typeof(ICSharpCode.ILSpyX.PdbProvider.DebugInfoUtils) })
			yield return new TestCaseData(type.Assembly.Location).SetArgDisplayNames(Path.GetFileName(type.Assembly.Location));
		foreach (var c in ManagedImages().Where(c => HasSymbols((string)c.Arguments[0]!)))
			yield return c;
	}

	static bool HasSymbols(string path)
	{
		if (File.Exists(Path.ChangeExtension(path, ".pdb")))
			return true;
		using var pe = new PEReader(File.OpenRead(path));
		return pe.ReadDebugDirectory().Any(e => e.Type == DebugDirectoryEntryType.EmbeddedPortablePdb);
	}

	static bool HasMetadata(string path)
	{
		try
		{
			using var pe = new PEReader(File.OpenRead(path));
			return pe.HasMetadata;
		}
		catch (BadImageFormatException)
		{
			return false;
		}
	}

	/// <summary>
	/// The bytes of a corpus file. In a solution test run ICSharpCode.Decompiler.Tests rewrites
	/// fixtures while this suite reads them; the compiler holds a file exclusively until it is
	/// complete, so a sharing violation is waited out.
	/// </summary>
	public static byte[] ReadFile(string path)
	{
		for (int attempt = 0; ; attempt++)
		{
			try
			{
				return File.ReadAllBytes(path);
			}
			catch (IOException) when (attempt < 100 && File.Exists(path))
			{
				Thread.Sleep(100);
			}
		}
	}

	/// <summary>The first differing line of two listings, or null when they are equal.</summary>
	public static string? FirstDifference(string expected, string actual)
	{
		string[] e = expected.Split('\n'), a = actual.Split('\n');
		int i = 0;
		while (i < e.Length && i < a.Length && e[i] == a[i])
			i++;
		if (i == e.Length && i == a.Length)
			return null;
		return $"line {i + 1}:\n  C#:   {(i < e.Length ? e[i] : "<end>")}\n  Rust: {(i < a.Length ? a[i] : "<end>")}";
	}
}
