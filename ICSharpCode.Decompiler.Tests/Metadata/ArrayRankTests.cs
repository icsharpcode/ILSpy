// Copyright (c) 2026 Siegfried Pammer
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
using System.Collections.Immutable;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Metadata
{
	[TestFixture]
	public class ArrayRankTests
	{
		static ArrayShape Shape(int rank) => new ArrayShape(rank, ImmutableArray<int>.Empty, ImmutableArray<int>.Empty);

		[TestCase(1)]
		[TestCase(ArrayType.MaxArrayRank)]
		public void TypeProvider_AcceptsValidRank(int rank)
		{
			var compilation = new SimpleCompilation(TypeSystem.TypeSystemLoaderTests.Mscorlib);
			var type = new TypeProvider(compilation).GetArrayType(compilation.FindType(KnownTypeCode.Int32), Shape(rank));
			Assert.That(((ArrayType)type).Dimensions, Is.EqualTo(rank));
		}

		[TestCase(0, 1)]
		[TestCase(ArrayType.MaxArrayRank + 1, ArrayType.MaxArrayRank)]
		[TestCase(0x1FFFFFFF, ArrayType.MaxArrayRank)]
		public void TypeProvider_ClampsInvalidRank(int rank, int expectedRank)
		{
			var compilation = new SimpleCompilation(TypeSystem.TypeSystemLoaderTests.Mscorlib);
			var type = (ArrayType)new TypeProvider(compilation).GetArrayType(compilation.FindType(KnownTypeCode.Int32), Shape(rank));
			using (Assert.EnterMultipleScope())
			{
				Assert.That(type.Dimensions, Is.EqualTo(expectedRank));
				Assert.That(type.OriginalDimensions, Is.EqualTo(rank));
			}
		}

		[TestCase(1, "int32[]")]
		[TestCase(3, "int32[, , ]")]
		[TestCase(0)]
		[TestCase(ArrayType.MaxArrayRank + 1)]
		public void Disassembler_ClampsInvalidRank(int rank, string expectedPrefix = null)
		{
			var output = new PlainTextOutput();
			var provider = new DisassemblerSignatureTypeProvider(TypeSystem.TypeSystemLoaderTests.Mscorlib, output);
			provider.GetArrayType(provider.GetPrimitiveType(PrimitiveTypeCode.Int32), Shape(rank))(ILNameSyntax.Signature);
			if (expectedPrefix == null)
				expectedPrefix = $"int32[/* invalid rank: {rank}, clamped to {Math.Min(Math.Max(rank, 1), ArrayType.MaxArrayRank)} */";
			Assert.That(output.ToString(), Does.StartWith(expectedPrefix));
			Assert.That(output.ToString().Length, Is.LessThan(200));
		}
	}
}
