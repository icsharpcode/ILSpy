// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.TypeSystem
{
	[TestFixture]
	public class TypeSystemAstBuilderTests
	{
		ICompilation compilation;

		[OneTimeSetUp]
		public void SetUp()
		{
			compilation = new SimpleCompilation(TypeSystemLoaderTests.TestAssembly,
				TypeSystemLoaderTests.Mscorlib,
				TypeSystemLoaderTests.SystemCore);
		}

		[Test]
		public void ConvertConstantValue_NullableArrayCreation_SizeOnly()
		{
			// A nullable array type (T[]?) converts to a ComposedType that carries its array specifier
			// on the nullable wrapper's BaseType, leaving the outer type's ArraySpecifiers empty. The
			// size-only array-creation path therefore has no specifier to strip and must not assume one.
			var intType = compilation.FindType(KnownTypeCode.Int32);
			var nullableArrayType = new ArrayType(compilation, intType, dimensions: 1, nullability: Nullability.Nullable);
			var rr = new ArrayCreateResolveResult(
				nullableArrayType,
				sizeArguments: new ResolveResult[] { new ConstantResolveResult(intType, 5) },
				initializerElements: null);

			var builder = new TypeSystemAstBuilder();

			var expr = builder.ConvertConstantValue(rr);

			Assert.That(expr, Is.InstanceOf<ArrayCreateExpression>());
		}
	}
}
