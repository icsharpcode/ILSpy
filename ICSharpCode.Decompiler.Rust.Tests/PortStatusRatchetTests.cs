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
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Rust.Interop;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// G-ratchet: rust/port-status.json records how much of the pipeline the engine serves, and
/// may only rise. The committed values must equal what this run measures, so a drop fails and
/// a rise has to be committed.
/// </summary>
[TestFixture]
[NonParallelizable]
public class PortStatusRatchetTests
{
	sealed record PortStatus(int IlPrefix, int AstPrefix, double MethodsServedPct);

	static string RepositoryRoot([CallerFilePath] string thisFile = "")
		=> Path.GetFullPath(Path.Combine(Path.GetDirectoryName(thisFile)!, ".."));

	static PortStatus Committed() => JsonSerializer.Deserialize<PortStatus>(
		File.ReadAllText(Path.Combine(RepositoryRoot(), "rust", "port-status.json")),
		new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower })!;

	/// <summary>
	/// The share of method bodies seam 1 serves when decompiling this test assembly and
	/// ICSharpCode.ILSpyX, in percent rounded down to one decimal.
	/// </summary>
	static double MeasureServedPct()
	{
		RustSeam1.ResetCounters();
		foreach (var type in new[] { typeof(Corpus), typeof(ICSharpCode.ILSpyX.PdbProvider.DebugInfoUtils) })
		{
			// A method that fails is not counted at all, so failures must be ruled out first.
			string text = new CSharpDecompiler(type.Assembly.Location, new DecompilerSettings()).DecompileWholeModuleAsSingleFile().ToString();
			var error = Regex.Match(text, "Error decompiling @06[0-9A-F]{6}");
			Assert.That(error.Success, Is.False, () => text.Substring(error.Index, Math.Min(3000, text.Length - error.Index)));
		}
		long served = RustSeam1.Served, total = served + RustSeam1.Fallbacks;
		Assert.That(total, Is.GreaterThan(0), "no method body went through seam 1");
		return Math.Floor(served * 1000.0 / total) / 10;
	}

	[Test]
	public void PortStatusRatchet()
	{
		var committed = Committed();
		Assert.That(RustEngine.ILPrefix, Is.EqualTo(committed.IlPrefix), "il_prefix must equal the engine's IL_PREFIX");
		Assert.That(committed.AstPrefix, Is.Zero, "seam 2 serves no AST transforms yet");
		double measured = MeasureServedPct();
		Assert.That(measured, Is.GreaterThanOrEqualTo(committed.MethodsServedPct), "methods_served_pct dropped");
		Assert.That(measured, Is.EqualTo(committed.MethodsServedPct), "methods_served_pct rose: commit the measured value");
	}
}
