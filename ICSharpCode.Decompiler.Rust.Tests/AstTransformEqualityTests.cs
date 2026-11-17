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

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Rust.Interop;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// Each ported AST transform on its own: every fixture image decompiles to the same C# whether
/// built-in transform <c>k</c> runs managed or in the engine, after the managed transforms before
/// it, independently of whether those are ported. This is how a transform is checked before the
/// engine serves the prefix up to it.
/// </summary>
[TestFixture, Parallelizable(ParallelScope.All)]
public class AstTransformEqualityTests
{
	/// <summary>Runs the engine's port of the built-in transform <see cref="Index"/>.</summary>
	sealed class EngineTransform : IAstTransform
	{
		public EngineTransform(int index) => Index = index;

		public int Index { get; }

		public void Run(AstNode rootNode, TransformContext context)
		{
			var engine = RustEngine.For(context.TypeSystem, out string? reason)
				?? throw new InvalidOperationException(reason);
			engine.RunAstTransforms(rootNode, 0, AstRequestFlags.None, single: Index);
		}
	}

	static IEnumerable<TestCaseData> Cases()
	{
		var images = Corpus.ManagedImages().Where(c => {
			var path = (string)c.Arguments[0]!;
			return path == typeof(CSharpDecompiler).Assembly.Location
				|| path.StartsWith(Corpus.TestCasesDirectory(), StringComparison.OrdinalIgnoreCase);
		}).ToList();
		var names = CSharpDecompiler.GetAstTransforms().Select(t => t.GetType().Name).ToList();
		for (int k = 0; k < names.Count; k++)
		{
			if (!RustEngine.IsAstTransformPorted(k))
				continue;
			foreach (var image in images)
			{
				string path = (string)image.Arguments[0]!;
				yield return new TestCaseData(k, path).SetArgDisplayNames(names[k], Path.GetFileName(path));
			}
		}
	}

	[TestCaseSource(nameof(Cases))]
	public void AstTransformEquality(int index, string path)
	{
		string managed = Decompile(path, index, engine: false);
		string engine = Decompile(path, index, engine: true);
		Assert.That(Corpus.FirstDifference(managed, engine), Is.Null);
	}

	static string Decompile(string path, int index, bool engine)
	{
		var module = new PEFile(path, new MemoryStream(Corpus.ReadFile(path)));
		var resolver = new UniversalAssemblyResolver(path, false, module.DetectTargetFrameworkId());
		var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
		var transforms = CSharpDecompiler.GetAstTransforms().Take(index).ToList();
		transforms.Add(engine ? new EngineTransform(index) : CSharpDecompiler.GetAstTransforms()[index]);
		decompiler.AstTransforms.Clear();
		foreach (var t in transforms)
			decompiler.AstTransforms.Add(t);
		try
		{
			return decompiler.DecompileWholeModuleAsString();
		}
		catch (Exception ex) when (ex is not RustEngineException)
		{
			// A module neither side can decompile still has to fail alike.
			return ex.GetType().FullName + ": " + ex.Message;
		}
	}

	[Test]
	public void AnUnportedTransformIsRefused()
	{
		var decompiler = new CSharpDecompiler(typeof(Corpus).Assembly.Location, new DecompilerSettings());
		var engine = RustEngine.For(decompiler.TypeSystem, out _)!;
		int unported = Enumerable.Range(0, CSharpDecompiler.GetAstTransforms().Count).First(i => !RustEngine.IsAstTransformPorted(i));
		Assert.Throws<RustEngineException>(() => engine.RunAstTransforms(new SyntaxTree(), 0, AstRequestFlags.None, single: unported));
	}
}
