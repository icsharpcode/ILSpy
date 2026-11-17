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

using System.Collections.Generic;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.Rust.Interop;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// The request-level check of seam 2: which AST transform lists the engine may serve, and which
/// early transforms become engine flags.
/// </summary>
[TestFixture]
public class AstRequestTests
{
	// Stand-ins for the test helpers of ICSharpCode.Decompiler.Tests, which the check knows by name.
	sealed class RemoveEmbeddedAttributes : IAstTransform { public void Run(AstNode rootNode, TransformContext context) { } }
	sealed class RemoveCompilerAttribute : IAstTransform { public void Run(AstNode rootNode, TransformContext context) { } }
	sealed class RemoveNamespaceMy : IAstTransform { public void Run(AstNode rootNode, TransformContext context) { } }
	sealed class SomethingElse : IAstTransform { public void Run(AstNode rootNode, TransformContext context) { } }

	[Test]
	public void BuiltInTransformsAreServed()
	{
		Assert.That(RustSeam2.AstRequest(CSharpDecompiler.GetAstTransforms(), recordSteps: false), Is.EqualTo(AstRequestFlags.None));
	}

	[Test]
	public void AppendedTransformsAreServed()
	{
		var transforms = CSharpDecompiler.GetAstTransforms();
		transforms.Add(new SomethingElse());
		Assert.That(RustSeam2.AstRequest(transforms, recordSteps: false), Is.EqualTo(AstRequestFlags.None));
	}

	[Test]
	public void RecognizedEarlyTransformsBecomeFlags()
	{
		var transforms = CSharpDecompiler.GetAstTransforms();
		transforms.Insert(0, new RemoveEmbeddedAttributes());
		transforms.Insert(0, new RemoveCompilerAttribute());
		transforms.Insert(0, new RemoveNamespaceMy());
		Assert.That(RustSeam2.AstRequest(transforms, recordSteps: false),
			Is.EqualTo(AstRequestFlags.RemoveEmbeddedAttributes | AstRequestFlags.RemoveCompilerAttribute | AstRequestFlags.RemoveNamespaceMy));
	}

	[Test]
	public void OtherRequestsStayWithTheHost()
	{
		var unknownEarly = CSharpDecompiler.GetAstTransforms();
		unknownEarly.Insert(0, new SomethingElse());
		var missing = CSharpDecompiler.GetAstTransforms();
		missing.RemoveAt(1);
		var reordered = CSharpDecompiler.GetAstTransforms();
		(reordered[0], reordered[1]) = (reordered[1], reordered[0]);
		Assert.Multiple(() => {
			Assert.That(RustSeam2.AstRequest(CSharpDecompiler.GetAstTransforms(), recordSteps: true), Is.Null);
			Assert.That(RustSeam2.AstRequest(unknownEarly, recordSteps: false), Is.Null);
			Assert.That(RustSeam2.AstRequest(missing, recordSteps: false), Is.Null);
			Assert.That(RustSeam2.AstRequest(reordered, recordSteps: false), Is.Null);
		});
		Assert.That(RustSeam2.RequestFallbackReasons.Keys, Has.Member("the early AST transform SomethingElse"));
	}
}
