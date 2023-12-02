// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.Tests.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Semantics
{
	// assign short names to the fake reflection types
	using C = Conversion;
	using dynamic = ICSharpCode.Decompiler.TypeSystem.ReflectionHelper.Dynamic;
	using nint = ICSharpCode.Decompiler.TypeSystem.ReflectionHelper.NInt;
	using nuint = ICSharpCode.Decompiler.TypeSystem.ReflectionHelper.NUInt;
	using Null = ICSharpCode.Decompiler.TypeSystem.ReflectionHelper.Null;

	[TestFixture, Parallelizable(ParallelScope.All)]
	public unsafe class ConversionTest
	{
		CSharpConversions conversions;
		ICompilation compilation;

		[OneTimeSetUp]
		public void SetUp()
		{
			compilation = new SimpleCompilation(TypeSystemLoaderTests.TestAssembly,
				TypeSystemLoaderTests.Mscorlib,
				TypeSystemLoaderTests.SystemCore);
			conversions = new CSharpConversions(compilation);
		}

		Conversion ImplicitConversion(Type from, Type to)
		{
			IType from2 = compilation.FindType(from);
			IType to2 = compilation.FindType(to);
			return conversions.ImplicitConversion(from2, to2);
		}

		Conversion ExplicitConversion(Type from, Type to)
		{
			IType from2 = compilation.FindType(from);
			IType to2 = compilation.FindType(to);
			return conversions.ExplicitConversion(from2, to2);
		}

		[Test]
		public void IdentityConversions()
		{
			Assert.That(ImplicitConversion(typeof(char), typeof(char)), Is.EqualTo(C.IdentityConversion));
			Assert.That(ImplicitConversion(typeof(string), typeof(string)), Is.EqualTo(C.IdentityConversion));
			Assert.That(ImplicitConversion(typeof(object), typeof(object)), Is.EqualTo(C.IdentityConversion));
			Assert.That(ImplicitConversion(typeof(bool), typeof(char)), Is.EqualTo(C.None));

			Assert.That(conversions.ImplicitConversion(SpecialType.Dynamic, SpecialType.Dynamic), Is.EqualTo(C.IdentityConversion));
			Assert.That(conversions.ImplicitConversion(SpecialType.UnknownType, SpecialType.UnknownType), Is.EqualTo(C.IdentityConversion));
			Assert.That(conversions.ImplicitConversion(SpecialType.NullType, SpecialType.NullType), Is.EqualTo(C.IdentityConversion));
		}

		[Test]
		public void DynamicIdentityConversions()
		{
			Assert.That(ImplicitConversion(typeof(object), typeof(dynamic)), Is.EqualTo(C.IdentityConversion));
			Assert.That(ImplicitConversion(typeof(dynamic), typeof(object)), Is.EqualTo(C.IdentityConversion));
		}

		[Test]
		public void ComplexDynamicIdentityConversions()
		{
			Assert.That(ImplicitConversion(typeof(List<object>), typeof(List<dynamic>)), Is.EqualTo(C.IdentityConversion));
			Assert.That(ImplicitConversion(typeof(List<dynamic>), typeof(List<object>)), Is.EqualTo(C.IdentityConversion));
			Assert.That(ImplicitConversion(typeof(List<string>), typeof(List<dynamic>)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(List<dynamic>), typeof(List<string>)), Is.EqualTo(C.None));

			Assert.That(ImplicitConversion(typeof(List<List<dynamic>[]>), typeof(List<List<object>[]>)), Is.EqualTo(C.IdentityConversion));
			Assert.That(ImplicitConversion(typeof(List<List<object>[]>), typeof(List<List<dynamic>[]>)), Is.EqualTo(C.IdentityConversion));
			Assert.That(ImplicitConversion(typeof(List<List<object>[,]>), typeof(List<List<dynamic>[]>)), Is.EqualTo(C.None));
		}

		[Test]
		public void TupleIdentityConversions()
		{
			var intType = compilation.FindType(typeof(int));
			var stringType = compilation.FindType(typeof(string));
			Assert.That(conversions.ImplicitConversion(
				new TupleType(compilation, ImmutableArray.Create(intType, stringType), ImmutableArray.Create("a", "b")),
				new TupleType(compilation, ImmutableArray.Create(intType, stringType), ImmutableArray.Create("a", "c"))), Is.EqualTo(C.IdentityConversion));

			Assert.That(conversions.ImplicitConversion(
				new TupleType(compilation, ImmutableArray.Create(intType, stringType), ImmutableArray.Create("a", "b")),
				new TupleType(compilation, ImmutableArray.Create(stringType, intType), ImmutableArray.Create("a", "b"))), Is.EqualTo(C.None));
		}

		[Test]
		public void TupleConversions()
		{
			Assert.That(
				ImplicitConversion(typeof((int, string)), typeof((long, object))), Is.EqualTo(C.TupleConversion(ImmutableArray.Create(C.ImplicitNumericConversion, C.ImplicitReferenceConversion))));

			Assert.That(
				ImplicitConversion(typeof(ValueTuple<float>), typeof(ValueTuple<double>)), Is.EqualTo(C.TupleConversion(ImmutableArray.Create(C.ImplicitNumericConversion))));
		}

		[Test]
		public void PrimitiveConversions()
		{
			Assert.That(ImplicitConversion(typeof(char), typeof(ushort)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ImplicitConversion(typeof(byte), typeof(char)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(int), typeof(long)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ImplicitConversion(typeof(long), typeof(int)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(int), typeof(float)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ImplicitConversion(typeof(bool), typeof(float)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(float), typeof(double)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ImplicitConversion(typeof(float), typeof(decimal)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(char), typeof(long)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ImplicitConversion(typeof(uint), typeof(long)), Is.EqualTo(C.ImplicitNumericConversion));
		}

		[Test]
		public void EnumerationConversion()
		{
			ResolveResult zero = new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), 0);
			ResolveResult one = new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), 1);
			C implicitEnumerationConversion = C.EnumerationConversion(true, false);
			Assert.That(conversions.ImplicitConversion(zero, compilation.FindType(typeof(StringComparison))), Is.EqualTo(implicitEnumerationConversion));
			Assert.That(conversions.ImplicitConversion(one, compilation.FindType(typeof(StringComparison))), Is.EqualTo(C.None));
		}

		[Test]
		public void NullableConversions()
		{
			Assert.That(ImplicitConversion(typeof(char), typeof(ushort?)), Is.EqualTo(C.ImplicitLiftedNumericConversion));
			Assert.That(ImplicitConversion(typeof(byte), typeof(char?)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(int), typeof(long?)), Is.EqualTo(C.ImplicitLiftedNumericConversion));
			Assert.That(ImplicitConversion(typeof(long), typeof(int?)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(int), typeof(float?)), Is.EqualTo(C.ImplicitLiftedNumericConversion));
			Assert.That(ImplicitConversion(typeof(bool), typeof(float?)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(float), typeof(double?)), Is.EqualTo(C.ImplicitLiftedNumericConversion));
			Assert.That(ImplicitConversion(typeof(float), typeof(decimal?)), Is.EqualTo(C.None));
		}

		[Test]
		public void NullableConversions2()
		{
			Assert.That(ImplicitConversion(typeof(char?), typeof(ushort?)), Is.EqualTo(C.ImplicitLiftedNumericConversion));
			Assert.That(ImplicitConversion(typeof(byte?), typeof(char?)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(int?), typeof(long?)), Is.EqualTo(C.ImplicitLiftedNumericConversion));
			Assert.That(ImplicitConversion(typeof(long?), typeof(int?)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(int?), typeof(float?)), Is.EqualTo(C.ImplicitLiftedNumericConversion));
			Assert.That(ImplicitConversion(typeof(bool?), typeof(float?)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(float?), typeof(double?)), Is.EqualTo(C.ImplicitLiftedNumericConversion));
			Assert.That(ImplicitConversion(typeof(float?), typeof(decimal?)), Is.EqualTo(C.None));
		}

		[Test]
		public void NullLiteralConversions()
		{
			Assert.That(ImplicitConversion(typeof(Null), typeof(int?)), Is.EqualTo(C.NullLiteralConversion));
			Assert.That(ImplicitConversion(typeof(Null), typeof(char?)), Is.EqualTo(C.NullLiteralConversion));
			Assert.That(ImplicitConversion(typeof(Null), typeof(int)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(Null), typeof(object)), Is.EqualTo(C.NullLiteralConversion));
			Assert.That(ImplicitConversion(typeof(Null), typeof(dynamic)), Is.EqualTo(C.NullLiteralConversion));
			Assert.That(ImplicitConversion(typeof(Null), typeof(string)), Is.EqualTo(C.NullLiteralConversion));
			Assert.That(ImplicitConversion(typeof(Null), typeof(int[])), Is.EqualTo(C.NullLiteralConversion));
		}

		[Test]
		public void SimpleReferenceConversions()
		{
			Assert.That(ImplicitConversion(typeof(string), typeof(object)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(BitArray), typeof(ICollection)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(IList), typeof(IEnumerable)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(object), typeof(string)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(ICollection), typeof(BitArray)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(IEnumerable), typeof(IList)), Is.EqualTo(C.None));
		}

		[Test]
		public void ConversionToDynamic()
		{
			Assert.That(ImplicitConversion(typeof(string), typeof(dynamic)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(int), typeof(dynamic)), Is.EqualTo(C.BoxingConversion));
		}

		[Test]
		public void ConversionFromDynamic()
		{
			// There is no conversion from the type 'dynamic' to other types (except the identity conversion to object).
			// Such conversions only exists from dynamic expression.
			// This is an important distinction for type inference (see TypeInferenceTests.IEnumerableCovarianceWithDynamic)
			Assert.That(ImplicitConversion(typeof(dynamic), typeof(string)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(dynamic), typeof(int)), Is.EqualTo(C.None));

			var dynamicRR = new ResolveResult(SpecialType.Dynamic);
			Assert.That(conversions.ImplicitConversion(dynamicRR, compilation.FindType(typeof(string))), Is.EqualTo(C.ImplicitDynamicConversion));
			Assert.That(conversions.ImplicitConversion(dynamicRR, compilation.FindType(typeof(int))), Is.EqualTo(C.ImplicitDynamicConversion));
		}

		[Test]
		public void ParameterizedTypeConversions()
		{
			Assert.That(ImplicitConversion(typeof(List<string>), typeof(ICollection<string>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(IList<string>), typeof(ICollection<string>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(List<string>), typeof(ICollection<object>)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(IList<string>), typeof(ICollection<object>)), Is.EqualTo(C.None));
		}

		[Test]
		public void ArrayConversions()
		{
			Assert.That(ImplicitConversion(typeof(string[]), typeof(object[])), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(string[,]), typeof(object[,])), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(string[]), typeof(object[,])), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(object[]), typeof(string[])), Is.EqualTo(C.None));

			Assert.That(ImplicitConversion(typeof(string[]), typeof(IList<string>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(string[,]), typeof(IList<string>)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(string[]), typeof(IList<object>)), Is.EqualTo(C.ImplicitReferenceConversion));

			Assert.That(ImplicitConversion(typeof(string[]), typeof(Array)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(string[]), typeof(ICloneable)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(Array), typeof(string[])), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(object), typeof(object[])), Is.EqualTo(C.None));
		}

		[Test]
		public void VarianceConversions()
		{
			Assert.That(ImplicitConversion(typeof(List<string>), typeof(IEnumerable<object>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(List<object>), typeof(IEnumerable<string>)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(IEnumerable<string>), typeof(IEnumerable<object>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(ICollection<string>), typeof(ICollection<object>)), Is.EqualTo(C.None));

			Assert.That(ImplicitConversion(typeof(Comparer<object>), typeof(IComparer<string>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(Comparer<object>), typeof(IComparer<Array>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(Comparer<object>), typeof(Comparer<string>)), Is.EqualTo(C.None));

			Assert.That(ImplicitConversion(typeof(List<object>), typeof(IEnumerable<string>)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(IEnumerable<string>), typeof(IEnumerable<object>)), Is.EqualTo(C.ImplicitReferenceConversion));

			Assert.That(ImplicitConversion(typeof(Func<ICollection, ICollection>), typeof(Func<IList, IEnumerable>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(Func<IEnumerable, IList>), typeof(Func<ICollection, ICollection>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(Func<ICollection, ICollection>), typeof(Func<IEnumerable, IList>)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(Func<IList, IEnumerable>), typeof(Func<ICollection, ICollection>)), Is.EqualTo(C.None));
		}

		[Test]
		public void ImplicitPointerConversion()
		{
			Assert.That(ImplicitConversion(typeof(Null), typeof(int*)), Is.EqualTo(C.ImplicitPointerConversion));
			Assert.That(ImplicitConversion(typeof(int*), typeof(void*)), Is.EqualTo(C.ImplicitPointerConversion));
		}

		[Test]
		public void NoConversionFromPointerTypeToObject()
		{
			Assert.That(ImplicitConversion(typeof(int*), typeof(object)), Is.EqualTo(C.None));
			Assert.That(ImplicitConversion(typeof(int*), typeof(dynamic)), Is.EqualTo(C.None));
		}

		[Test]
		public void ConversionToNInt()
		{
			// Test based on the table in https://github.com/dotnet/csharplang/blob/master/proposals/native-integers.md
			Assert.That(ExplicitConversion(typeof(object), typeof(nint)), Is.EqualTo(C.UnboxingConversion));
			Assert.That(ExplicitConversion(typeof(void*), typeof(nint)), Is.EqualTo(C.ExplicitPointerConversion));
			Assert.That(ExplicitConversion(typeof(sbyte), typeof(nint)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(byte), typeof(nint)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(short), typeof(nint)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(ushort), typeof(nint)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(int), typeof(nint)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(uint), typeof(nint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(long), typeof(nint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(ulong), typeof(nint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(char), typeof(nint)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(float), typeof(nint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(double), typeof(nint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(decimal), typeof(nint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(IntPtr), typeof(nint)), Is.EqualTo(C.IdentityConversion));
			Assert.That(ExplicitConversion(typeof(UIntPtr), typeof(nint)), Is.EqualTo(C.None));
		}

		[Test]
		public void ConversionToNUInt()
		{
			// Test based on the table in https://github.com/dotnet/csharplang/blob/master/proposals/native-integers.md
			Assert.That(ExplicitConversion(typeof(object), typeof(nuint)), Is.EqualTo(C.UnboxingConversion));
			Assert.That(ExplicitConversion(typeof(void*), typeof(nuint)), Is.EqualTo(C.ExplicitPointerConversion));
			Assert.That(ExplicitConversion(typeof(sbyte), typeof(nuint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(byte), typeof(nuint)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(short), typeof(nuint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(ushort), typeof(nuint)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(int), typeof(nuint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(uint), typeof(nuint)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(long), typeof(nuint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(ulong), typeof(nuint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(char), typeof(nuint)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(float), typeof(nuint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(double), typeof(nuint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(decimal), typeof(nuint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(IntPtr), typeof(nuint)), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(UIntPtr), typeof(nuint)), Is.EqualTo(C.IdentityConversion));
		}

		[Test]
		public void ConversionFromNInt()
		{
			// Test based on the table in https://github.com/dotnet/csharplang/blob/master/proposals/native-integers.md
			Assert.That(ExplicitConversion(typeof(nint), typeof(object)), Is.EqualTo(C.BoxingConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(void*)), Is.EqualTo(C.ExplicitPointerConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(nuint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(sbyte)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(byte)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(short)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(ushort)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(int)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(uint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(long)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(ulong)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(char)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(float)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(double)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(decimal)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(IntPtr)), Is.EqualTo(C.IdentityConversion));
			Assert.That(ExplicitConversion(typeof(nint), typeof(UIntPtr)), Is.EqualTo(C.None));
		}

		[Test]
		public void ConversionFromNUInt()
		{
			// Test based on the table in https://github.com/dotnet/csharplang/blob/master/proposals/native-integers.md
			Assert.That(ExplicitConversion(typeof(nuint), typeof(object)), Is.EqualTo(C.BoxingConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(void*)), Is.EqualTo(C.ExplicitPointerConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(nint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(sbyte)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(byte)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(short)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(ushort)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(int)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(uint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(long)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(ulong)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(char)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(float)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(double)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(decimal)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(IntPtr)), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(UIntPtr)), Is.EqualTo(C.IdentityConversion));
		}


		[Test]
		public void NIntEnumConversion()
		{
			var explicitEnumConversion = C.EnumerationConversion(isImplicit: false, isLifted: false);
			Assert.That(ExplicitConversion(typeof(nint), typeof(StringComparison)), Is.EqualTo(explicitEnumConversion));
			Assert.That(ExplicitConversion(typeof(nuint), typeof(StringComparison)), Is.EqualTo(explicitEnumConversion));
			Assert.That(ExplicitConversion(typeof(StringComparison), typeof(nint)), Is.EqualTo(explicitEnumConversion));
			Assert.That(ExplicitConversion(typeof(StringComparison), typeof(nuint)), Is.EqualTo(explicitEnumConversion));
		}

		[Test]
		public void IntegerLiteralToNIntConversions()
		{
			Assert.That(IntegerLiteralConversion(0, typeof(nint)));
			Assert.That(IntegerLiteralConversion(-1, typeof(nint)));
			Assert.That(!IntegerLiteralConversion(uint.MaxValue, typeof(nint)));
			Assert.That(!IntegerLiteralConversion(long.MaxValue, typeof(nint)));
		}


		[Test]
		public void IntegerLiteralToNUIntConversions()
		{
			Assert.That(IntegerLiteralConversion(0, typeof(nuint)));
			Assert.That(!IntegerLiteralConversion(-1, typeof(nuint)));
			Assert.That(IntegerLiteralConversion(uint.MaxValue, typeof(nuint)));
			Assert.That(!IntegerLiteralConversion(long.MaxValue, typeof(nuint)));
		}

		[Test]
		public void UnconstrainedTypeParameter()
		{
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.TypeDefinition, 0, "T");
			ITypeParameter t2 = new DefaultTypeParameter(compilation, SymbolKind.TypeDefinition, 1, "T2");
			ITypeParameter tm = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "TM");

			Assert.That(conversions.ImplicitConversion(SpecialType.NullType, t), Is.EqualTo(C.None));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(KnownTypeCode.Object)), Is.EqualTo(C.BoxingConversion));
			Assert.That(conversions.ImplicitConversion(t, SpecialType.Dynamic), Is.EqualTo(C.BoxingConversion));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(typeof(ValueType))), Is.EqualTo(C.None));

			Assert.That(conversions.ImplicitConversion(t, t), Is.EqualTo(C.IdentityConversion));
			Assert.That(conversions.ImplicitConversion(t2, t), Is.EqualTo(C.None));
			Assert.That(conversions.ImplicitConversion(t, t2), Is.EqualTo(C.None));
			Assert.That(conversions.ImplicitConversion(t, tm), Is.EqualTo(C.None));
			Assert.That(conversions.ImplicitConversion(tm, t), Is.EqualTo(C.None));
		}

		[Test]
		public void TypeParameterWithReferenceTypeConstraint()
		{
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.TypeDefinition, 0, "T", hasReferenceTypeConstraint: true);

			Assert.That(conversions.ImplicitConversion(SpecialType.NullType, t), Is.EqualTo(C.NullLiteralConversion));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(KnownTypeCode.Object)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(conversions.ImplicitConversion(t, SpecialType.Dynamic), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(typeof(ValueType))), Is.EqualTo(C.None));
		}

		[Test]
		public void TypeParameterWithValueTypeConstraint()
		{
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.TypeDefinition, 0, "T", hasValueTypeConstraint: true);

			Assert.That(conversions.ImplicitConversion(SpecialType.NullType, t), Is.EqualTo(C.None));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(KnownTypeCode.Object)), Is.EqualTo(C.BoxingConversion));
			Assert.That(conversions.ImplicitConversion(t, SpecialType.Dynamic), Is.EqualTo(C.BoxingConversion));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(typeof(ValueType))), Is.EqualTo(C.BoxingConversion));
		}

		[Test]
		public void TypeParameterWithClassConstraint()
		{
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.TypeDefinition, 0, "T",
														constraints: new[] { compilation.FindType(typeof(StringComparer)) });

			Assert.That(conversions.ImplicitConversion(SpecialType.NullType, t), Is.EqualTo(C.NullLiteralConversion));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(KnownTypeCode.Object)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(conversions.ImplicitConversion(t, SpecialType.Dynamic), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(typeof(ValueType))), Is.EqualTo(C.None));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(typeof(StringComparer))), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(typeof(IComparer))), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(typeof(IComparer<int>))), Is.EqualTo(C.None));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(typeof(IComparer<string>))), Is.EqualTo(C.ImplicitReferenceConversion));
		}

		[Test]
		public void TypeParameterWithInterfaceConstraint()
		{
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.TypeDefinition, 0, "T",
														constraints: new[] { compilation.FindType(typeof(IList)) });

			Assert.That(conversions.ImplicitConversion(SpecialType.NullType, t), Is.EqualTo(C.None));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(KnownTypeCode.Object)), Is.EqualTo(C.BoxingConversion));
			Assert.That(conversions.ImplicitConversion(t, SpecialType.Dynamic), Is.EqualTo(C.BoxingConversion));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(typeof(ValueType))), Is.EqualTo(C.None));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(typeof(IList))), Is.EqualTo(C.BoxingConversion));
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(typeof(IEnumerable))), Is.EqualTo(C.BoxingConversion));
		}

		[Test]
		public void UserDefinedImplicitConversion()
		{
			Conversion c = ImplicitConversion(typeof(DateTime), typeof(DateTimeOffset));
			Assert.That(c.IsImplicit && c.IsUserDefined);
			Assert.That(c.Method.FullName, Is.EqualTo("System.DateTimeOffset.op_Implicit"));

			Assert.That(ImplicitConversion(typeof(DateTimeOffset), typeof(DateTime)), Is.EqualTo(C.None));
		}

		[Test]
		public void UserDefinedImplicitNullableConversion()
		{
			// User-defined conversion followed by nullable conversion
			Conversion c = ImplicitConversion(typeof(DateTime), typeof(DateTimeOffset?));
			Assert.That(c.IsValid && c.IsUserDefined);
			Assert.That(!c.IsLifted);
			// Lifted user-defined conversion
			c = ImplicitConversion(typeof(DateTime?), typeof(DateTimeOffset?));
			Assert.That(c.IsValid && c.IsUserDefined && c.IsLifted);
			// User-defined conversion doesn't drop the nullability
			c = ImplicitConversion(typeof(DateTime?), typeof(DateTimeOffset));
			Assert.That(!c.IsValid);
		}

		bool IntegerLiteralConversion(object value, Type to)
		{
			IType fromType = compilation.FindType(value.GetType());
			ConstantResolveResult crr = new ConstantResolveResult(fromType, value);
			IType to2 = compilation.FindType(to);
			return conversions.ImplicitConversion(crr, to2).IsValid;
		}

		[Test]
		public void IntegerLiteralToEnumConversions()
		{
			Assert.That(IntegerLiteralConversion(0, typeof(LoaderOptimization)));
			Assert.That(IntegerLiteralConversion(0L, typeof(LoaderOptimization)));
			Assert.That(IntegerLiteralConversion(0, typeof(LoaderOptimization?)));
			Assert.That(!IntegerLiteralConversion(0, typeof(string)));
			Assert.That(!IntegerLiteralConversion(1, typeof(LoaderOptimization)));
		}

		[Test]
		public void ImplicitConstantExpressionConversion()
		{
			Assert.That(IntegerLiteralConversion(0, typeof(int)));
			Assert.That(IntegerLiteralConversion(0, typeof(ushort)));
			Assert.That(IntegerLiteralConversion(0, typeof(sbyte)));

			Assert.That(IntegerLiteralConversion(-1, typeof(int)));
			Assert.That(!IntegerLiteralConversion(-1, typeof(ushort)));
			Assert.That(IntegerLiteralConversion(-1, typeof(sbyte)));

			Assert.That(IntegerLiteralConversion(200, typeof(int)));
			Assert.That(IntegerLiteralConversion(200, typeof(ushort)));
			Assert.That(!IntegerLiteralConversion(200, typeof(sbyte)));
		}

		[Test]
		public void ImplicitLongConstantExpressionConversion()
		{
			Assert.That(!IntegerLiteralConversion(0L, typeof(int)));
			Assert.That(!IntegerLiteralConversion(0L, typeof(short)));
			Assert.That(IntegerLiteralConversion(0L, typeof(long)));
			Assert.That(IntegerLiteralConversion(0L, typeof(ulong)));

			Assert.That(IntegerLiteralConversion(-1L, typeof(long)));
			Assert.That(!IntegerLiteralConversion(-1L, typeof(ulong)));
		}

		[Test]
		public void ImplicitConstantExpressionConversionToNullable()
		{
			Assert.That(IntegerLiteralConversion(0, typeof(uint?)));
			Assert.That(IntegerLiteralConversion(0, typeof(short?)));
			Assert.That(IntegerLiteralConversion(0, typeof(byte?)));

			Assert.That(!IntegerLiteralConversion(-1, typeof(uint?)));
			Assert.That(IntegerLiteralConversion(-1, typeof(short?)));
			Assert.That(!IntegerLiteralConversion(-1, typeof(byte?)));

			Assert.That(IntegerLiteralConversion(200, typeof(uint?)));
			Assert.That(IntegerLiteralConversion(200, typeof(short?)));
			Assert.That(IntegerLiteralConversion(200, typeof(byte?)));

			Assert.That(!IntegerLiteralConversion(0L, typeof(uint?)));
			Assert.That(IntegerLiteralConversion(0L, typeof(long?)));
			Assert.That(IntegerLiteralConversion(0L, typeof(ulong?)));

			Assert.That(IntegerLiteralConversion(-1L, typeof(long?)));
			Assert.That(!IntegerLiteralConversion(-1L, typeof(ulong?)));
		}

		[Test]
		public void ImplicitConstantExpressionConversionNumberInterfaces()
		{
			Assert.That(IntegerLiteralConversion(0, typeof(IFormattable)));
			Assert.That(IntegerLiteralConversion(0, typeof(IComparable<int>)));
			Assert.That(!IntegerLiteralConversion(0, typeof(IComparable<short>)));
			Assert.That(!IntegerLiteralConversion(0, typeof(IComparable<long>)));
		}

		int BetterConversion(Type s, Type t1, Type t2)
		{
			IType sType = compilation.FindType(s);
			IType t1Type = compilation.FindType(t1);
			IType t2Type = compilation.FindType(t2);
			return conversions.BetterConversion(sType, t1Type, t2Type);
		}

		int BetterConversion(object value, Type t1, Type t2)
		{
			IType fromType = compilation.FindType(value.GetType());
			ConstantResolveResult crr = new ConstantResolveResult(fromType, value);
			IType t1Type = compilation.FindType(t1);
			IType t2Type = compilation.FindType(t2);
			return conversions.BetterConversion(crr, t1Type, t2Type);
		}

		[Test]
		public void BetterConversion()
		{
			Assert.That(BetterConversion(typeof(string), typeof(string), typeof(object)), Is.EqualTo(1));
			Assert.That(BetterConversion(typeof(string), typeof(object), typeof(IComparable<string>)), Is.EqualTo(2));
			Assert.That(BetterConversion(typeof(string), typeof(IEnumerable<char>), typeof(IComparable<string>)), Is.EqualTo(0));
		}

		[Test]
		public void BetterPrimitiveConversion()
		{
			Assert.That(BetterConversion(typeof(short), typeof(int), typeof(long)), Is.EqualTo(1));
			Assert.That(BetterConversion(typeof(short), typeof(int), typeof(uint)), Is.EqualTo(1));
			Assert.That(BetterConversion(typeof(ushort), typeof(uint), typeof(int)), Is.EqualTo(2));
			Assert.That(BetterConversion(typeof(char), typeof(short), typeof(int)), Is.EqualTo(1));
			Assert.That(BetterConversion(typeof(char), typeof(ushort), typeof(int)), Is.EqualTo(1));
			Assert.That(BetterConversion(typeof(sbyte), typeof(long), typeof(ulong)), Is.EqualTo(1));
			Assert.That(BetterConversion(typeof(byte), typeof(ushort), typeof(short)), Is.EqualTo(2));

			Assert.That(BetterConversion(1, typeof(sbyte), typeof(byte)), Is.EqualTo(1));
			Assert.That(BetterConversion(1, typeof(ushort), typeof(sbyte)), Is.EqualTo(2));
		}

		[Test]
		public void BetterNullableConversion()
		{
			Assert.That(BetterConversion(typeof(byte), typeof(int), typeof(uint?)), Is.EqualTo(0));
			Assert.That(BetterConversion(typeof(byte?), typeof(int?), typeof(uint?)), Is.EqualTo(0));
			Assert.That(BetterConversion(typeof(byte), typeof(ushort?), typeof(uint?)), Is.EqualTo(1));
			Assert.That(BetterConversion(typeof(byte?), typeof(ulong?), typeof(uint?)), Is.EqualTo(2));
			Assert.That(BetterConversion(typeof(byte), typeof(ushort?), typeof(uint)), Is.EqualTo(0));
			Assert.That(BetterConversion(typeof(byte), typeof(ushort?), typeof(int)), Is.EqualTo(0));
			Assert.That(BetterConversion(typeof(byte), typeof(ulong?), typeof(uint)), Is.EqualTo(2));
			Assert.That(BetterConversion(typeof(byte), typeof(ulong?), typeof(int)), Is.EqualTo(0));
			Assert.That(BetterConversion(typeof(ushort?), typeof(long?), typeof(int?)), Is.EqualTo(2));
			Assert.That(BetterConversion(typeof(sbyte), typeof(int?), typeof(uint?)), Is.EqualTo(0));
		}

		/* TODO: we should probably revive these tests somehow
		[Test]
		public void ExpansiveInheritance()
		{
			var a = new DefaultUnresolvedTypeDefinition(string.Empty, "A");
			var b = new DefaultUnresolvedTypeDefinition(string.Empty, "B");
			// interface A<in U>
			a.Kind = TypeKind.Interface;
			a.TypeParameters.Add(new DefaultUnresolvedTypeParameter(SymbolKind.TypeDefinition, 0, "U") { Variance = VarianceModifier.Contravariant });
			// interface B<X> : A<A<B<X>>> { }
			b.TypeParameters.Add(new DefaultUnresolvedTypeParameter(SymbolKind.TypeDefinition, 0, "X"));
			b.BaseTypes.Add(new ParameterizedTypeReference(
				a, new[] { new ParameterizedTypeReference(
					a, new [] { new ParameterizedTypeReference(
						b, new [] { new TypeParameterReference(SymbolKind.TypeDefinition, 0) }
					) } ) }));

			ICompilation compilation = TypeSystemHelper.CreateCompilation(a, b);
			ITypeDefinition resolvedA = compilation.MainAssembly.GetTypeDefinition(a.FullTypeName);
			ITypeDefinition resolvedB = compilation.MainAssembly.GetTypeDefinition(b.FullTypeName);

			IType type1 = new ParameterizedType(resolvedB, new[] { compilation.FindType(KnownTypeCode.Double) });
			IType type2 = new ParameterizedType(resolvedA, new[] { new ParameterizedType(resolvedB, new[] { compilation.FindType(KnownTypeCode.String) }) });
			Assert.That(!conversions.ImplicitConversion(type1, type2).IsValid);
		}

		[Test]
		public void ImplicitTypeParameterConversion()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T t) where T : U {
		U u = $t$;
	}
}";
			Assert.AreEqual(C.BoxingConversion, GetConversion(program));
		}

		[Test]
		public void InvalidImplicitTypeParameterConversion()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T t) where U : T {
		U u = $t$;
	}
}";
			Assert.AreEqual(C.None, GetConversion(program));
		}

		[Test]
		public void ImplicitTypeParameterArrayConversion()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T[] t) where T : U {
		U[] u = $t$;
	}
}";
			// invalid, e.g. T=int[], U=object[]
			Assert.AreEqual(C.None, GetConversion(program));
		}

		[Test]
		public void ImplicitTypeParameterConversionWithClassConstraint()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T t) where T : class, U where U : class {
		U u = $t$;
	}
}";
			Assert.AreEqual(C.ImplicitReferenceConversion, GetConversion(program));
		}

		[Test]
		public void ImplicitTypeParameterArrayConversionWithClassConstraint()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T[] t) where T : class, U where U : class {
		U[] u = $t$;
	}
}";
			Assert.AreEqual(C.ImplicitReferenceConversion, GetConversion(program));
		}

		[Test]
		public void ImplicitTypeParameterConversionWithClassConstraintOnlyOnT()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T t) where T : class, U {
		U u = $t$;
	}
}";
			Assert.AreEqual(C.ImplicitReferenceConversion, GetConversion(program));
		}

		[Test]
		public void ImplicitTypeParameterArrayConversionWithClassConstraintOnlyOnT()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T[] t) where T : class, U {
		U[] u = $t$;
	}
}";
			Assert.AreEqual(C.ImplicitReferenceConversion, GetConversion(program));
		}

		[Test]
		public void MethodGroupConversion_Void()
		{
			string program = @"using System;
delegate void D();
class Test {
	D d = $M$;
	public static void M() {}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
			Assert.That(!c.DelegateCapturesFirstArgument);
			Assert.IsNotNull(c.Method);
		}

		[Test]
		public void MethodGroupConversion_Void_InstanceMethod()
		{
			string program = @"using System;
delegate void D();
class Test {
	D d;
	public void M() {
		d = $M$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
			Assert.That(c.DelegateCapturesFirstArgument);
			Assert.IsNotNull(c.Method);
		}

		[Test]
		public void MethodGroupConversion_MatchingSignature()
		{
			string program = @"using System;
delegate object D(int argument);
class Test {
	D d = $M$;
	public static object M(int argument) {}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_InvalidReturnType()
		{
			string program = @"using System;
delegate object D(int argument);
class Test {
	D d = $M$;
	public static int M(int argument) {}
}";
			var c = GetConversion(program);
			Assert.That(!c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_CovariantReturnType()
		{
			string program = @"using System;
delegate object D(int argument);
class Test {
	D d = $M$;
	public static string M(int argument) {}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_RefArgumentTypesEqual()
		{
			string program = @"using System;
delegate void D(ref object o);
class Test {
	D d = $M$;
	public static void M(ref object o) {}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_RefArgumentObjectVsDynamic()
		{
			string program = @"using System;
delegate void D(ref object o);
class Test {
	D d = $M$;
	public static void M(ref dynamic o) {}
}";
			var c = GetConversion(program);
			Assert.That(!c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_RefVsOut()
		{
			string program = @"using System;
delegate void D(ref object o);
class Test {
	D d = $M$;
	public static void M(out object o) {}
}";
			var c = GetConversion(program);
			Assert.That(!c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_RefVsNormal()
		{
			string program = @"using System;
delegate void D(ref object o);
class Test {
	D d = $M$;
	public static void M(object o) {}
}";
			var c = GetConversion(program);
			Assert.That(!c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_NormalVsOut()
		{
			string program = @"using System;
delegate void D(object o);
class Test {
	D d = $M$;
	public static void M(out object o) {}
}";
			var c = GetConversion(program);
			Assert.That(!c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_MatchingNormalParameter()
		{
			string program = @"using System;
delegate void D(object o);
class Test {
	D d = $M$;
	public static void M(object o) {}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_IdentityConversion()
		{
			string program = @"using System;
delegate void D(object o);
class Test {
	D d = $M$;
	public static void M(dynamic o) {}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_Contravariance()
		{
			string program = @"using System;
delegate void D(string o);
class Test {
	D d = $M$;
	public static void M(object o) {}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);

		}

		[Test, Ignore("Not sure if this conversion should be valid or not... NR and mcs both accept it as valid, csc treats it as invalid")]
		public void MethodGroupConversion_NoContravarianceDynamic()
		{
			string program = @"using System;
delegate void D(string o);
class Test {
	D d = $M$;
	public static void M(dynamic o) {}
}";
			var c = GetConversion(program);
			//Assert.IsFrue(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_ExactMatchIsBetter()
		{
			string program = @"using System;
class Test {
	delegate void D(string a);
	D d = $M$;
	static void M(object x) {}
	static void M(string x = null) {}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
			Assert.AreEqual("System.String", c.Method.Parameters.Single().Type.FullName);
		}

		[Test]
		public void MethodGroupConversion_CannotLeaveOutOptionalParameters()
		{
			string program = @"using System;
class Test {
	delegate void D(string a);
	D d = $M$;
	static void M(object x) {}
	static void M(string x, string y = null) {}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
			Assert.AreEqual("System.Object", c.Method.Parameters.Single().Type.FullName);
		}

		[Test]
		public void MethodGroupConversion_CannotUseExpandedParams()
		{
			string program = @"using System;
class Test {
	delegate void D(string a);
	D d = $M$;
	static void M(object x) {}
	static void M(params string[] x) {}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
			Assert.AreEqual("System.Object", c.Method.Parameters.Single().Type.FullName);
		}

		[Test]
		public void MethodGroupConversion_ExtensionMethod()
		{
			string program = @"using System;
static class Ext {
	public static void M(this string s, int x) {}
}
class Test {
	delegate void D(int a);
	void F() {
		string s = """";
		D d = $s.M$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
			Assert.That(c.DelegateCapturesFirstArgument);
		}

		[Test]
		public void MethodGroupConversion_ExtensionMethodUsedAsStaticMethod()
		{
			string program = @"using System;
static class Ext {
	public static void M(this string s, int x) {}
}
class Test {
	delegate void D(string s, int a);
	void F() {
		D d = $Ext.M$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
			Assert.That(!c.DelegateCapturesFirstArgument);
		}

		[Test]
		public void MethodGroupConversion_ObjectToDynamic()
		{
			string program = @"using System;
class Test {
	public void F(object o) {}
	public void M() {
		Action<dynamic> x = $F$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_ObjectToDynamicGenericArgument()
		{
			string program = @"using System;
using System.Collections.Generic;
class Test {
	public void F(List<object> l) {}
	public void M() {
		Action<List<dynamic>> x = $F$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_ObjectToDynamicReturnValue()
		{
			string program = @"using System;
class Test {
	public object F() {}
	public void M() {
		Func<dynamic> x = $F$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_DynamicToObject()
		{
			string program = @"using System;
class Test {
	public void F(dynamic o) {}
	public void M() {
		Action<object> x = $F$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_DynamicToObjectGenericArgument()
		{
			string program = @"using System;
using System.Collections.Generic;
class Test {
	public void F(List<dynamic> l) {}
	public void M() {
		Action<List<object>> x = $F$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_DynamicToObjectReturnValue()
		{
			string program = @"using System;
class Test {
	public dynamic F() {}
	public void M() {
		Func<object> x = $F$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
		}

		[Test]
		public void UserDefined_IntLiteral_ViaUInt_ToCustomStruct()
		{
			string program = @"using System;
struct T {
	public static implicit operator T(uint a) { return new T(); }
}
class Test {
	static void M() {
		T t = $1$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
		}

		[Test]
		public void UserDefined_NullLiteral_ViaString_ToCustomStruct()
		{
			string program = @"using System;
struct T {
	public static implicit operator T(string a) { return new T(); }

}
class Test {
	static void M() {
		T t = $null$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
		}


		[Test]
		public void UserDefined_CanUseLiftedEvenIfReturnTypeAlreadyNullable()
		{
			string program = @"using System;
struct S {
	public static implicit operator short?(S s) { return 0; }
}

class Test {
	static void M(S? s) {
		int? i = $s$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.IsLifted);
		}

		[Test]
		public void UserDefinedImplicitConversion_PicksExactSourceTypeIfPossible()
		{
			string program = @"using System;
class Convertible {
	public static implicit operator Convertible(int i) {return new Convertible(); }
	public static implicit operator Convertible(short s) {return new Convertible(); }
}
class Test {
	public void M() {
		Convertible a = $33$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.AreEqual("i", c.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedImplicitConversion_PicksMostEncompassedSourceType()
		{
			string program = @"using System;
class Convertible {
	public static implicit operator Convertible(long l) {return new Convertible(); }
	public static implicit operator Convertible(uint ui) {return new Convertible(); }
}
class Test {
	public void M() {
		Convertible a = $(ushort)33$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.AreEqual("ui", c.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedImplicitConversion_NoMostEncompassedSourceTypeIsInvalid()
		{
			string program = @"using System;
class Convertible {
	public static implicit operator Convertible(ulong l) {return new Convertible(); }
	public static implicit operator Convertible(int ui) {return new Convertible(); }
}
class Test {
	public void M() {
		Convertible a = $(ushort)33$;
	}
}";
			var c = GetConversion(program);
			Assert.That(!c.IsValid);
		}

		[Test]
		public void UserDefinedImplicitConversion_PicksExactTargetTypeIfPossible()
		{
			string program = @"using System;
class Convertible {
	public static implicit operator int(Convertible i) {return 0; }
	public static implicit operator short(Convertible s) {return 0; }
}
class Test {
	public void M() {
		int a = $new Convertible()$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.AreEqual("i", c.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedImplicitConversion_PicksMostEncompassingTargetType()
		{
			string program = @"using System;
class Convertible {
	public static implicit operator int(Convertible i) {return 0; }
	public static implicit operator ushort(Convertible us) {return 0; }
}
class Test {
	public void M() {
		ulong a = $new Convertible()$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.AreEqual("us", c.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedImplicitConversion_NoMostEncompassingTargetTypeIsInvalid()
		{
			string program = @"using System;
class Convertible {
	public static implicit operator uint(Convertible i) {return 0; }
	public static implicit operator short(Convertible us) {return 0; }
}
class Test {
	public void M() {
		long a = $new Convertible()$;
	}
}";
			var c = GetConversion(program);
			Assert.That(!c.IsValid);
		}

		[Test]
		public void UserDefinedImplicitConversion_AmbiguousIsInvalid()
		{
			string program = @"using System;
class Convertible1 {
	public static implicit operator Convertible2(Convertible1 c) {return 0; }
}
class Convertible2 {
	public static implicit operator Convertible2(Convertible1 c) {return 0; }
}
class Test {
	public void M() {
		Convertible2 a = $new Convertible1()$;
	}
}";
			var c = GetConversion(program);
			Assert.That(!c.IsValid);
		}

		[Test]
		public void UserDefinedImplicitConversion_DefinedNullableTakesPrecedenceOverLifted()
		{
			string program = @"using System;
struct Convertible {
	public static implicit operator Convertible(int i) {return new Convertible(); }
	public static implicit operator Convertible?(int? ni) {return new Convertible(); }
}
class Test {
	public void M() {
		Convertible? a = $(int?)33$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(!c.IsLifted);
			Assert.AreEqual("ni", c.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedImplicitConversion_UIntConstant()
		{
			string program = @"using System;
class Convertible {
	public static implicit operator Convertible(long l) {return new Convertible(); }
	public static implicit operator Convertible(uint ui) {return new Convertible(); }
}
class Test {
	public void M() {
		Convertible a = $33$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.AreEqual("ui", c.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedImplicitConversion_NullableUIntConstant()
		{
			string program = @"using System;
class Convertible {
	public static implicit operator Convertible(long? l) {return new Convertible(); }
	public static implicit operator Convertible(uint? ui) {return new Convertible(); }
}
class Test {
	public void M() {
		Convertible a = $33$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.AreEqual("ui", c.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedImplicitConversion_UseShortResult_BecauseNullableCannotBeUnpacked()
		{
			string program = @"using System;
class Test {
	public static implicit operator int?(Test i) { return 0; }
	public static implicit operator short(Test s) { return 0; }
}
class Program {
	public static void Main(string[] args)
	{
		int x = $new Test()$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.AreEqual("System.Int16", c.Method.ReturnType.FullName);
		}

		[Test]
		public void UserDefinedImplicitConversion_Short_Or_NullableByte_Target()
		{
			string program = @"using System;
class Test {
	public static implicit operator short(Test s) { return 0; }
	public static implicit operator byte?(Test b) { return 0; }
}
class Program {
	public static void Main(string[] args)
	{
		int? x = $new Test()$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.AreEqual("System.Int16", c.Method.ReturnType.FullName);
		}

		[Test]
		public void UserDefinedImplicitConversion_Byte_Or_NullableShort_Target()
		{
			string program = @"using System;
class Test {
	public static implicit operator byte(Test b) { return 0; }
	public static implicit operator short?(Test s) { return 0; }
}
class Program {
	public static void Main(string[] args)
	{
		int? x = $new Test()$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.AreEqual("s", c.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedImplicitConversion_Int_Or_NullableLong_Source()
		{
			string program = @"using System;
class Test {
	public static implicit operator Test(int i) { return new Test(); }
	public static implicit operator Test(long? l) { return new Test(); }
}
class Program {
	static void Main() {
		short s = 0;
		Test t = $s$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.AreEqual("i", c.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedImplicitConversion_NullableInt_Or_Long_Source()
		{
			string program = @"using System;
class Test {
	public static implicit operator Test(int? i) { return new Test(); }
	public static implicit operator Test(long l) { return new Test(); }
}
class Program {
	static void Main() {
		short s = 0;
		Test t = $s$;
	}
}";
			var c = GetConversion(program);
			Assert.That(!c.IsValid);
			Assert.That(c.IsUserDefined);
		}

		[Test]
		public void UserDefinedImplicitConversion_NullableInt_Or_Long_Constant_Source()
		{
			string program = @"using System;
class Test {
	public static implicit operator Test(int? i) { return new Test(); }
	public static implicit operator Test(long l) { return new Test(); }
}
class Program {
	static void Main() {
		Test t = $1$;
	}
}";
			var c = GetConversion(program);
			Assert.That(!c.IsValid);
			Assert.That(c.IsUserDefined);
		}

		[Test]
		public void UserDefinedImplicitConversion_NullableInt_Or_NullableLong_Source()
		{
			string program = @"using System;
class Test {
	public static implicit operator Test(int? i) { return new Test(); }
	public static implicit operator Test(long? l) { return new Test(); }
}
class Program {
	static void Main() {
		short s = 0;
		Test t = $s$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.AreEqual("i", c.Method.Parameters[0].Name);
		}

		[Test]
		public void PreferUserDefinedConversionOverReferenceConversion()
		{
			// actually this is not because user-defined conversions are better;
			// but because string is a better conversion target
			string program = @"
class AA {
	public static implicit operator string(AA a) { return null; }
}
class Test {
	static void M(object obj) {}
	static void M(string str) {}
	
	static void Main() {
		$M(new AA())$;
	}
}";

			var rr = Resolve<CSharpInvocationResolveResult>(program);
			Assert.That(!rr.IsError);
			Assert.AreEqual("str", rr.Member.Parameters[0].Name);
		}

		[Test]
		public void PreferAmbiguousConversionOverReferenceConversion()
		{
			// Ambiguous conversions are a compiler error; but they are not
			// preventing the overload from being chosen.

			// The user-defined conversion wins because BB is a better conversion target than object.
			string program = @"
class AA {
	public static implicit operator BB(AA a) { return null; }
}
class BB {
	public static implicit operator BB(AA a) { return null; }
}

class Test {
	static void M(BB b) {}
	static void M(object o) {}
	
	static void Main() {
		M($new AA()$);
	}
}";

			var c = GetConversion(program);
			Assert.That(c.IsUserDefined);
			Assert.That(!c.IsValid);
		}

		[Test]
		public void UserDefinedImplicitConversion_ConversionBeforeUserDefinedOperatorIsCorrect()
		{
			string program = @"using System;
class Convertible {
	public static implicit operator Convertible(long l) {return new Convertible(); }
}
class Test {
	public void M() {
		int i = 33;
		Convertible a = $i$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.ConversionBeforeUserDefinedOperator.IsImplicit);
			Assert.That(c.ConversionBeforeUserDefinedOperator.IsNumericConversion);
			Assert.That(c.ConversionBeforeUserDefinedOperator.IsValid);
			Assert.That(c.ConversionAfterUserDefinedOperator.IsIdentityConversion);
		}

		[Test]
		public void UserDefinedImplicitConversion_ConversionAfterUserDefinedOperatorIsCorrect()
		{
			string program = @"using System;
class Convertible {
	public static implicit operator int(Convertible i) {return 0; }
}
class Test {
	public void M() {
		long a = $new Convertible()$;
	}
}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.ConversionBeforeUserDefinedOperator.IsIdentityConversion);
			Assert.That(c.ConversionAfterUserDefinedOperator.IsImplicit);
			Assert.That(c.ConversionAfterUserDefinedOperator.IsNumericConversion);
			Assert.That(c.ConversionAfterUserDefinedOperator.IsValid);
		}

		[Test]
		public void UserDefinedImplicitConversion_IsImplicit()
		{
			// Bug icsharpcode/NRefactory#183: conversions from constant expressions were incorrectly marked as explicit
			string program = @"using System;
	class Test {
		void Hello(JsNumber3 x) {
			Hello($7$);
		}
	}
	public class JsNumber3 {
		public static implicit operator JsNumber3(int d) {
			return null;
		}
	}";
			var c = GetConversion(program);
			Assert.That(c.IsValid);
			Assert.That(c.IsImplicit);
			Assert.That(!c.IsExplicit);
			Assert.AreEqual(Conversion.IdentityConversion, c.ConversionBeforeUserDefinedOperator);
			Assert.AreEqual(Conversion.IdentityConversion, c.ConversionAfterUserDefinedOperator);
		}
		*/
	}
}
