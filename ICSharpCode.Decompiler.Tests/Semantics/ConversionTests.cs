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
			Assert.AreEqual(C.IdentityConversion, ImplicitConversion(typeof(char), typeof(char)));
			Assert.AreEqual(C.IdentityConversion, ImplicitConversion(typeof(string), typeof(string)));
			Assert.AreEqual(C.IdentityConversion, ImplicitConversion(typeof(object), typeof(object)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(bool), typeof(char)));

			Assert.AreEqual(C.IdentityConversion, conversions.ImplicitConversion(SpecialType.Dynamic, SpecialType.Dynamic));
			Assert.AreEqual(C.IdentityConversion, conversions.ImplicitConversion(SpecialType.UnknownType, SpecialType.UnknownType));
			Assert.AreEqual(C.IdentityConversion, conversions.ImplicitConversion(SpecialType.NullType, SpecialType.NullType));
		}

		[Test]
		public void DynamicIdentityConversions()
		{
			Assert.AreEqual(C.IdentityConversion, ImplicitConversion(typeof(object), typeof(dynamic)));
			Assert.AreEqual(C.IdentityConversion, ImplicitConversion(typeof(dynamic), typeof(object)));
		}

		[Test]
		public void ComplexDynamicIdentityConversions()
		{
			Assert.AreEqual(C.IdentityConversion, ImplicitConversion(typeof(List<object>), typeof(List<dynamic>)));
			Assert.AreEqual(C.IdentityConversion, ImplicitConversion(typeof(List<dynamic>), typeof(List<object>)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(List<string>), typeof(List<dynamic>)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(List<dynamic>), typeof(List<string>)));

			Assert.AreEqual(C.IdentityConversion, ImplicitConversion(typeof(List<List<dynamic>[]>), typeof(List<List<object>[]>)));
			Assert.AreEqual(C.IdentityConversion, ImplicitConversion(typeof(List<List<object>[]>), typeof(List<List<dynamic>[]>)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(List<List<object>[,]>), typeof(List<List<dynamic>[]>)));
		}

		[Test]
		public void TupleIdentityConversions()
		{
			var intType = compilation.FindType(typeof(int));
			var stringType = compilation.FindType(typeof(string));
			Assert.AreEqual(C.IdentityConversion, conversions.ImplicitConversion(
				new TupleType(compilation, ImmutableArray.Create(intType, stringType), ImmutableArray.Create("a", "b")),
				new TupleType(compilation, ImmutableArray.Create(intType, stringType), ImmutableArray.Create("a", "c"))));

			Assert.AreEqual(C.None, conversions.ImplicitConversion(
				new TupleType(compilation, ImmutableArray.Create(intType, stringType), ImmutableArray.Create("a", "b")),
				new TupleType(compilation, ImmutableArray.Create(stringType, intType), ImmutableArray.Create("a", "b"))));
		}

		[Test]
		public void TupleConversions()
		{
			Assert.AreEqual(
				C.TupleConversion(ImmutableArray.Create(C.ImplicitNumericConversion, C.ImplicitReferenceConversion)),
				ImplicitConversion(typeof((int, string)), typeof((long, object))));

			Assert.AreEqual(
				C.TupleConversion(ImmutableArray.Create(C.ImplicitNumericConversion)),
				ImplicitConversion(typeof(ValueTuple<float>), typeof(ValueTuple<double>)));
		}

		[Test]
		public void PrimitiveConversions()
		{
			Assert.AreEqual(C.ImplicitNumericConversion, ImplicitConversion(typeof(char), typeof(ushort)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(byte), typeof(char)));
			Assert.AreEqual(C.ImplicitNumericConversion, ImplicitConversion(typeof(int), typeof(long)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(long), typeof(int)));
			Assert.AreEqual(C.ImplicitNumericConversion, ImplicitConversion(typeof(int), typeof(float)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(bool), typeof(float)));
			Assert.AreEqual(C.ImplicitNumericConversion, ImplicitConversion(typeof(float), typeof(double)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(float), typeof(decimal)));
			Assert.AreEqual(C.ImplicitNumericConversion, ImplicitConversion(typeof(char), typeof(long)));
			Assert.AreEqual(C.ImplicitNumericConversion, ImplicitConversion(typeof(uint), typeof(long)));
		}

		[Test]
		public void EnumerationConversion()
		{
			ResolveResult zero = new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), 0);
			ResolveResult one = new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), 1);
			C implicitEnumerationConversion = C.EnumerationConversion(true, false);
			Assert.AreEqual(implicitEnumerationConversion, conversions.ImplicitConversion(zero, compilation.FindType(typeof(StringComparison))));
			Assert.AreEqual(C.None, conversions.ImplicitConversion(one, compilation.FindType(typeof(StringComparison))));
		}

		[Test]
		public void NullableConversions()
		{
			Assert.AreEqual(C.ImplicitLiftedNumericConversion, ImplicitConversion(typeof(char), typeof(ushort?)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(byte), typeof(char?)));
			Assert.AreEqual(C.ImplicitLiftedNumericConversion, ImplicitConversion(typeof(int), typeof(long?)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(long), typeof(int?)));
			Assert.AreEqual(C.ImplicitLiftedNumericConversion, ImplicitConversion(typeof(int), typeof(float?)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(bool), typeof(float?)));
			Assert.AreEqual(C.ImplicitLiftedNumericConversion, ImplicitConversion(typeof(float), typeof(double?)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(float), typeof(decimal?)));
		}

		[Test]
		public void NullableConversions2()
		{
			Assert.AreEqual(C.ImplicitLiftedNumericConversion, ImplicitConversion(typeof(char?), typeof(ushort?)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(byte?), typeof(char?)));
			Assert.AreEqual(C.ImplicitLiftedNumericConversion, ImplicitConversion(typeof(int?), typeof(long?)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(long?), typeof(int?)));
			Assert.AreEqual(C.ImplicitLiftedNumericConversion, ImplicitConversion(typeof(int?), typeof(float?)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(bool?), typeof(float?)));
			Assert.AreEqual(C.ImplicitLiftedNumericConversion, ImplicitConversion(typeof(float?), typeof(double?)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(float?), typeof(decimal?)));
		}

		[Test]
		public void NullLiteralConversions()
		{
			Assert.AreEqual(C.NullLiteralConversion, ImplicitConversion(typeof(Null), typeof(int?)));
			Assert.AreEqual(C.NullLiteralConversion, ImplicitConversion(typeof(Null), typeof(char?)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(Null), typeof(int)));
			Assert.AreEqual(C.NullLiteralConversion, ImplicitConversion(typeof(Null), typeof(object)));
			Assert.AreEqual(C.NullLiteralConversion, ImplicitConversion(typeof(Null), typeof(dynamic)));
			Assert.AreEqual(C.NullLiteralConversion, ImplicitConversion(typeof(Null), typeof(string)));
			Assert.AreEqual(C.NullLiteralConversion, ImplicitConversion(typeof(Null), typeof(int[])));
		}

		[Test]
		public void SimpleReferenceConversions()
		{
			Assert.AreEqual(C.ImplicitReferenceConversion, ImplicitConversion(typeof(string), typeof(object)));
			Assert.AreEqual(C.ImplicitReferenceConversion, ImplicitConversion(typeof(BitArray), typeof(ICollection)));
			Assert.AreEqual(C.ImplicitReferenceConversion, ImplicitConversion(typeof(IList), typeof(IEnumerable)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(object), typeof(string)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(ICollection), typeof(BitArray)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(IEnumerable), typeof(IList)));
		}

		[Test]
		public void ConversionToDynamic()
		{
			Assert.AreEqual(C.ImplicitReferenceConversion, ImplicitConversion(typeof(string), typeof(dynamic)));
			Assert.AreEqual(C.BoxingConversion, ImplicitConversion(typeof(int), typeof(dynamic)));
		}

		[Test]
		public void ConversionFromDynamic()
		{
			// There is no conversion from the type 'dynamic' to other types (except the identity conversion to object).
			// Such conversions only exists from dynamic expression.
			// This is an important distinction for type inference (see TypeInferenceTests.IEnumerableCovarianceWithDynamic)
			Assert.AreEqual(C.None, ImplicitConversion(typeof(dynamic), typeof(string)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(dynamic), typeof(int)));

			var dynamicRR = new ResolveResult(SpecialType.Dynamic);
			Assert.AreEqual(C.ImplicitDynamicConversion, conversions.ImplicitConversion(dynamicRR, compilation.FindType(typeof(string))));
			Assert.AreEqual(C.ImplicitDynamicConversion, conversions.ImplicitConversion(dynamicRR, compilation.FindType(typeof(int))));
		}

		[Test]
		public void ParameterizedTypeConversions()
		{
			Assert.AreEqual(C.ImplicitReferenceConversion, ImplicitConversion(typeof(List<string>), typeof(ICollection<string>)));
			Assert.AreEqual(C.ImplicitReferenceConversion, ImplicitConversion(typeof(IList<string>), typeof(ICollection<string>)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(List<string>), typeof(ICollection<object>)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(IList<string>), typeof(ICollection<object>)));
		}

		[Test]
		public void ArrayConversions()
		{
			Assert.AreEqual(C.ImplicitReferenceConversion, ImplicitConversion(typeof(string[]), typeof(object[])));
			Assert.AreEqual(C.ImplicitReferenceConversion, ImplicitConversion(typeof(string[,]), typeof(object[,])));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(string[]), typeof(object[,])));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(object[]), typeof(string[])));

			Assert.AreEqual(C.ImplicitReferenceConversion, ImplicitConversion(typeof(string[]), typeof(IList<string>)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(string[,]), typeof(IList<string>)));
			Assert.AreEqual(C.ImplicitReferenceConversion, ImplicitConversion(typeof(string[]), typeof(IList<object>)));

			Assert.AreEqual(C.ImplicitReferenceConversion, ImplicitConversion(typeof(string[]), typeof(Array)));
			Assert.AreEqual(C.ImplicitReferenceConversion, ImplicitConversion(typeof(string[]), typeof(ICloneable)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(Array), typeof(string[])));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(object), typeof(object[])));
		}

		[Test]
		public void VarianceConversions()
		{
			Assert.AreEqual(C.ImplicitReferenceConversion,
							ImplicitConversion(typeof(List<string>), typeof(IEnumerable<object>)));
			Assert.AreEqual(C.None,
							ImplicitConversion(typeof(List<object>), typeof(IEnumerable<string>)));
			Assert.AreEqual(C.ImplicitReferenceConversion,
							ImplicitConversion(typeof(IEnumerable<string>), typeof(IEnumerable<object>)));
			Assert.AreEqual(C.None,
							ImplicitConversion(typeof(ICollection<string>), typeof(ICollection<object>)));

			Assert.AreEqual(C.ImplicitReferenceConversion,
							ImplicitConversion(typeof(Comparer<object>), typeof(IComparer<string>)));
			Assert.AreEqual(C.ImplicitReferenceConversion,
							ImplicitConversion(typeof(Comparer<object>), typeof(IComparer<Array>)));
			Assert.AreEqual(C.None,
							ImplicitConversion(typeof(Comparer<object>), typeof(Comparer<string>)));

			Assert.AreEqual(C.None,
							ImplicitConversion(typeof(List<object>), typeof(IEnumerable<string>)));
			Assert.AreEqual(C.ImplicitReferenceConversion,
							ImplicitConversion(typeof(IEnumerable<string>), typeof(IEnumerable<object>)));

			Assert.AreEqual(C.ImplicitReferenceConversion,
							ImplicitConversion(typeof(Func<ICollection, ICollection>), typeof(Func<IList, IEnumerable>)));
			Assert.AreEqual(C.ImplicitReferenceConversion,
							ImplicitConversion(typeof(Func<IEnumerable, IList>), typeof(Func<ICollection, ICollection>)));
			Assert.AreEqual(C.None,
							ImplicitConversion(typeof(Func<ICollection, ICollection>), typeof(Func<IEnumerable, IList>)));
			Assert.AreEqual(C.None,
							ImplicitConversion(typeof(Func<IList, IEnumerable>), typeof(Func<ICollection, ICollection>)));
		}

		[Test]
		public void ImplicitPointerConversion()
		{
			Assert.AreEqual(C.ImplicitPointerConversion, ImplicitConversion(typeof(Null), typeof(int*)));
			Assert.AreEqual(C.ImplicitPointerConversion, ImplicitConversion(typeof(int*), typeof(void*)));
		}

		[Test]
		public void NoConversionFromPointerTypeToObject()
		{
			Assert.AreEqual(C.None, ImplicitConversion(typeof(int*), typeof(object)));
			Assert.AreEqual(C.None, ImplicitConversion(typeof(int*), typeof(dynamic)));
		}

		[Test]
		public void ConversionToNInt()
		{
			// Test based on the table in https://github.com/dotnet/csharplang/blob/master/proposals/native-integers.md
			Assert.AreEqual(C.UnboxingConversion, ExplicitConversion(typeof(object), typeof(nint)));
			Assert.AreEqual(C.ExplicitPointerConversion, ExplicitConversion(typeof(void*), typeof(nint)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(sbyte), typeof(nint)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(byte), typeof(nint)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(short), typeof(nint)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(ushort), typeof(nint)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(int), typeof(nint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(uint), typeof(nint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(long), typeof(nint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(ulong), typeof(nint)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(char), typeof(nint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(float), typeof(nint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(double), typeof(nint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(decimal), typeof(nint)));
			Assert.AreEqual(C.IdentityConversion, ExplicitConversion(typeof(IntPtr), typeof(nint)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(UIntPtr), typeof(nint)));
		}

		[Test]
		public void ConversionToNUInt()
		{
			// Test based on the table in https://github.com/dotnet/csharplang/blob/master/proposals/native-integers.md
			Assert.AreEqual(C.UnboxingConversion, ExplicitConversion(typeof(object), typeof(nuint)));
			Assert.AreEqual(C.ExplicitPointerConversion, ExplicitConversion(typeof(void*), typeof(nuint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(sbyte), typeof(nuint)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(byte), typeof(nuint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(short), typeof(nuint)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(ushort), typeof(nuint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(int), typeof(nuint)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(uint), typeof(nuint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(long), typeof(nuint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(ulong), typeof(nuint)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(char), typeof(nuint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(float), typeof(nuint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(double), typeof(nuint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(decimal), typeof(nuint)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(IntPtr), typeof(nuint)));
			Assert.AreEqual(C.IdentityConversion, ExplicitConversion(typeof(UIntPtr), typeof(nuint)));
		}

		[Test]
		public void ConversionFromNInt()
		{
			// Test based on the table in https://github.com/dotnet/csharplang/blob/master/proposals/native-integers.md
			Assert.AreEqual(C.BoxingConversion, ExplicitConversion(typeof(nint), typeof(object)));
			Assert.AreEqual(C.ExplicitPointerConversion, ExplicitConversion(typeof(nint), typeof(void*)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nint), typeof(nuint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nint), typeof(sbyte)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nint), typeof(byte)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nint), typeof(short)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nint), typeof(ushort)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nint), typeof(int)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nint), typeof(uint)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(nint), typeof(long)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nint), typeof(ulong)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nint), typeof(char)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(nint), typeof(float)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(nint), typeof(double)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(nint), typeof(decimal)));
			Assert.AreEqual(C.IdentityConversion, ExplicitConversion(typeof(nint), typeof(IntPtr)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(nint), typeof(UIntPtr)));
		}

		[Test]
		public void ConversionFromNUInt()
		{
			// Test based on the table in https://github.com/dotnet/csharplang/blob/master/proposals/native-integers.md
			Assert.AreEqual(C.BoxingConversion, ExplicitConversion(typeof(nuint), typeof(object)));
			Assert.AreEqual(C.ExplicitPointerConversion, ExplicitConversion(typeof(nuint), typeof(void*)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nuint), typeof(nint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nuint), typeof(sbyte)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nuint), typeof(byte)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nuint), typeof(short)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nuint), typeof(ushort)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nuint), typeof(int)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nuint), typeof(uint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nuint), typeof(long)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(nuint), typeof(ulong)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(nuint), typeof(char)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(nuint), typeof(float)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(nuint), typeof(double)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(nuint), typeof(decimal)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(nuint), typeof(IntPtr)));
			Assert.AreEqual(C.IdentityConversion, ExplicitConversion(typeof(nuint), typeof(UIntPtr)));
		}


		[Test]
		public void NIntEnumConversion()
		{
			var explicitEnumConversion = C.EnumerationConversion(isImplicit: false, isLifted: false);
			Assert.AreEqual(explicitEnumConversion, ExplicitConversion(typeof(nint), typeof(StringComparison)));
			Assert.AreEqual(explicitEnumConversion, ExplicitConversion(typeof(nuint), typeof(StringComparison)));
			Assert.AreEqual(explicitEnumConversion, ExplicitConversion(typeof(StringComparison), typeof(nint)));
			Assert.AreEqual(explicitEnumConversion, ExplicitConversion(typeof(StringComparison), typeof(nuint)));
		}

		[Test]
		public void IntegerLiteralToNIntConversions()
		{
			Assert.IsTrue(IntegerLiteralConversion(0, typeof(nint)));
			Assert.IsTrue(IntegerLiteralConversion(-1, typeof(nint)));
			Assert.IsFalse(IntegerLiteralConversion(uint.MaxValue, typeof(nint)));
			Assert.IsFalse(IntegerLiteralConversion(long.MaxValue, typeof(nint)));
		}


		[Test]
		public void IntegerLiteralToNUIntConversions()
		{
			Assert.IsTrue(IntegerLiteralConversion(0, typeof(nuint)));
			Assert.IsFalse(IntegerLiteralConversion(-1, typeof(nuint)));
			Assert.IsTrue(IntegerLiteralConversion(uint.MaxValue, typeof(nuint)));
			Assert.IsFalse(IntegerLiteralConversion(long.MaxValue, typeof(nuint)));
		}

		[Test]
		public void UnconstrainedTypeParameter()
		{
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.TypeDefinition, 0, "T");
			ITypeParameter t2 = new DefaultTypeParameter(compilation, SymbolKind.TypeDefinition, 1, "T2");
			ITypeParameter tm = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "TM");

			Assert.AreEqual(C.None, conversions.ImplicitConversion(SpecialType.NullType, t));
			Assert.AreEqual(C.BoxingConversion, conversions.ImplicitConversion(t, compilation.FindType(KnownTypeCode.Object)));
			Assert.AreEqual(C.BoxingConversion, conversions.ImplicitConversion(t, SpecialType.Dynamic));
			Assert.AreEqual(C.None, conversions.ImplicitConversion(t, compilation.FindType(typeof(ValueType))));

			Assert.AreEqual(C.IdentityConversion, conversions.ImplicitConversion(t, t));
			Assert.AreEqual(C.None, conversions.ImplicitConversion(t2, t));
			Assert.AreEqual(C.None, conversions.ImplicitConversion(t, t2));
			Assert.AreEqual(C.None, conversions.ImplicitConversion(t, tm));
			Assert.AreEqual(C.None, conversions.ImplicitConversion(tm, t));
		}

		[Test]
		public void TypeParameterWithReferenceTypeConstraint()
		{
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.TypeDefinition, 0, "T", hasReferenceTypeConstraint: true);

			Assert.AreEqual(C.NullLiteralConversion, conversions.ImplicitConversion(SpecialType.NullType, t));
			Assert.AreEqual(C.ImplicitReferenceConversion, conversions.ImplicitConversion(t, compilation.FindType(KnownTypeCode.Object)));
			Assert.AreEqual(C.ImplicitReferenceConversion, conversions.ImplicitConversion(t, SpecialType.Dynamic));
			Assert.AreEqual(C.None, conversions.ImplicitConversion(t, compilation.FindType(typeof(ValueType))));
		}

		[Test]
		public void TypeParameterWithValueTypeConstraint()
		{
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.TypeDefinition, 0, "T", hasValueTypeConstraint: true);

			Assert.AreEqual(C.None, conversions.ImplicitConversion(SpecialType.NullType, t));
			Assert.AreEqual(C.BoxingConversion, conversions.ImplicitConversion(t, compilation.FindType(KnownTypeCode.Object)));
			Assert.AreEqual(C.BoxingConversion, conversions.ImplicitConversion(t, SpecialType.Dynamic));
			Assert.AreEqual(C.BoxingConversion, conversions.ImplicitConversion(t, compilation.FindType(typeof(ValueType))));
		}

		[Test]
		public void TypeParameterWithClassConstraint()
		{
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.TypeDefinition, 0, "T",
														constraints: new[] { compilation.FindType(typeof(StringComparer)) });

			Assert.AreEqual(C.NullLiteralConversion,
							conversions.ImplicitConversion(SpecialType.NullType, t));
			Assert.AreEqual(C.ImplicitReferenceConversion,
							conversions.ImplicitConversion(t, compilation.FindType(KnownTypeCode.Object)));
			Assert.AreEqual(C.ImplicitReferenceConversion,
							conversions.ImplicitConversion(t, SpecialType.Dynamic));
			Assert.AreEqual(C.None, conversions.ImplicitConversion(t, compilation.FindType(typeof(ValueType))));
			Assert.AreEqual(C.ImplicitReferenceConversion,
							conversions.ImplicitConversion(t, compilation.FindType(typeof(StringComparer))));
			Assert.AreEqual(C.ImplicitReferenceConversion,
							conversions.ImplicitConversion(t, compilation.FindType(typeof(IComparer))));
			Assert.AreEqual(C.None, conversions.ImplicitConversion(t, compilation.FindType(typeof(IComparer<int>))));
			Assert.AreEqual(C.ImplicitReferenceConversion,
							conversions.ImplicitConversion(t, compilation.FindType(typeof(IComparer<string>))));
		}

		[Test]
		public void TypeParameterWithInterfaceConstraint()
		{
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.TypeDefinition, 0, "T",
														constraints: new[] { compilation.FindType(typeof(IList)) });

			Assert.AreEqual(C.None, conversions.ImplicitConversion(SpecialType.NullType, t));
			Assert.AreEqual(C.BoxingConversion,
							conversions.ImplicitConversion(t, compilation.FindType(KnownTypeCode.Object)));
			Assert.AreEqual(C.BoxingConversion,
							conversions.ImplicitConversion(t, SpecialType.Dynamic));
			Assert.AreEqual(C.None, conversions.ImplicitConversion(t, compilation.FindType(typeof(ValueType))));
			Assert.AreEqual(C.BoxingConversion,
							conversions.ImplicitConversion(t, compilation.FindType(typeof(IList))));
			Assert.AreEqual(C.BoxingConversion,
							conversions.ImplicitConversion(t, compilation.FindType(typeof(IEnumerable))));
		}

		[Test]
		public void UserDefinedImplicitConversion()
		{
			Conversion c = ImplicitConversion(typeof(DateTime), typeof(DateTimeOffset));
			Assert.IsTrue(c.IsImplicit && c.IsUserDefined);
			Assert.AreEqual("System.DateTimeOffset.op_Implicit", c.Method.FullName);

			Assert.AreEqual(C.None, ImplicitConversion(typeof(DateTimeOffset), typeof(DateTime)));
		}

		[Test]
		public void UserDefinedImplicitNullableConversion()
		{
			// User-defined conversion followed by nullable conversion
			Conversion c = ImplicitConversion(typeof(DateTime), typeof(DateTimeOffset?));
			Assert.IsTrue(c.IsValid && c.IsUserDefined);
			Assert.IsFalse(c.IsLifted);
			// Lifted user-defined conversion
			c = ImplicitConversion(typeof(DateTime?), typeof(DateTimeOffset?));
			Assert.IsTrue(c.IsValid && c.IsUserDefined && c.IsLifted);
			// User-defined conversion doesn't drop the nullability
			c = ImplicitConversion(typeof(DateTime?), typeof(DateTimeOffset));
			Assert.IsFalse(c.IsValid);
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
			Assert.IsTrue(IntegerLiteralConversion(0, typeof(LoaderOptimization)));
			Assert.IsTrue(IntegerLiteralConversion(0L, typeof(LoaderOptimization)));
			Assert.IsTrue(IntegerLiteralConversion(0, typeof(LoaderOptimization?)));
			Assert.IsFalse(IntegerLiteralConversion(0, typeof(string)));
			Assert.IsFalse(IntegerLiteralConversion(1, typeof(LoaderOptimization)));
		}

		[Test]
		public void ImplicitConstantExpressionConversion()
		{
			Assert.IsTrue(IntegerLiteralConversion(0, typeof(int)));
			Assert.IsTrue(IntegerLiteralConversion(0, typeof(ushort)));
			Assert.IsTrue(IntegerLiteralConversion(0, typeof(sbyte)));

			Assert.IsTrue(IntegerLiteralConversion(-1, typeof(int)));
			Assert.IsFalse(IntegerLiteralConversion(-1, typeof(ushort)));
			Assert.IsTrue(IntegerLiteralConversion(-1, typeof(sbyte)));

			Assert.IsTrue(IntegerLiteralConversion(200, typeof(int)));
			Assert.IsTrue(IntegerLiteralConversion(200, typeof(ushort)));
			Assert.IsFalse(IntegerLiteralConversion(200, typeof(sbyte)));
		}

		[Test]
		public void ImplicitLongConstantExpressionConversion()
		{
			Assert.IsFalse(IntegerLiteralConversion(0L, typeof(int)));
			Assert.IsFalse(IntegerLiteralConversion(0L, typeof(short)));
			Assert.IsTrue(IntegerLiteralConversion(0L, typeof(long)));
			Assert.IsTrue(IntegerLiteralConversion(0L, typeof(ulong)));

			Assert.IsTrue(IntegerLiteralConversion(-1L, typeof(long)));
			Assert.IsFalse(IntegerLiteralConversion(-1L, typeof(ulong)));
		}

		[Test]
		public void ImplicitConstantExpressionConversionToNullable()
		{
			Assert.IsTrue(IntegerLiteralConversion(0, typeof(uint?)));
			Assert.IsTrue(IntegerLiteralConversion(0, typeof(short?)));
			Assert.IsTrue(IntegerLiteralConversion(0, typeof(byte?)));

			Assert.IsFalse(IntegerLiteralConversion(-1, typeof(uint?)));
			Assert.IsTrue(IntegerLiteralConversion(-1, typeof(short?)));
			Assert.IsFalse(IntegerLiteralConversion(-1, typeof(byte?)));

			Assert.IsTrue(IntegerLiteralConversion(200, typeof(uint?)));
			Assert.IsTrue(IntegerLiteralConversion(200, typeof(short?)));
			Assert.IsTrue(IntegerLiteralConversion(200, typeof(byte?)));

			Assert.IsFalse(IntegerLiteralConversion(0L, typeof(uint?)));
			Assert.IsTrue(IntegerLiteralConversion(0L, typeof(long?)));
			Assert.IsTrue(IntegerLiteralConversion(0L, typeof(ulong?)));

			Assert.IsTrue(IntegerLiteralConversion(-1L, typeof(long?)));
			Assert.IsFalse(IntegerLiteralConversion(-1L, typeof(ulong?)));
		}

		[Test]
		public void ImplicitConstantExpressionConversionNumberInterfaces()
		{
			Assert.IsTrue(IntegerLiteralConversion(0, typeof(IFormattable)));
			Assert.IsTrue(IntegerLiteralConversion(0, typeof(IComparable<int>)));
			Assert.IsFalse(IntegerLiteralConversion(0, typeof(IComparable<short>)));
			Assert.IsFalse(IntegerLiteralConversion(0, typeof(IComparable<long>)));
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
			Assert.AreEqual(1, BetterConversion(typeof(string), typeof(string), typeof(object)));
			Assert.AreEqual(2, BetterConversion(typeof(string), typeof(object), typeof(IComparable<string>)));
			Assert.AreEqual(0, BetterConversion(typeof(string), typeof(IEnumerable<char>), typeof(IComparable<string>)));
		}

		[Test]
		public void BetterPrimitiveConversion()
		{
			Assert.AreEqual(1, BetterConversion(typeof(short), typeof(int), typeof(long)));
			Assert.AreEqual(1, BetterConversion(typeof(short), typeof(int), typeof(uint)));
			Assert.AreEqual(2, BetterConversion(typeof(ushort), typeof(uint), typeof(int)));
			Assert.AreEqual(1, BetterConversion(typeof(char), typeof(short), typeof(int)));
			Assert.AreEqual(1, BetterConversion(typeof(char), typeof(ushort), typeof(int)));
			Assert.AreEqual(1, BetterConversion(typeof(sbyte), typeof(long), typeof(ulong)));
			Assert.AreEqual(2, BetterConversion(typeof(byte), typeof(ushort), typeof(short)));

			Assert.AreEqual(1, BetterConversion(1, typeof(sbyte), typeof(byte)));
			Assert.AreEqual(2, BetterConversion(1, typeof(ushort), typeof(sbyte)));
		}

		[Test]
		public void BetterNullableConversion()
		{
			Assert.AreEqual(0, BetterConversion(typeof(byte), typeof(int), typeof(uint?)));
			Assert.AreEqual(0, BetterConversion(typeof(byte?), typeof(int?), typeof(uint?)));
			Assert.AreEqual(1, BetterConversion(typeof(byte), typeof(ushort?), typeof(uint?)));
			Assert.AreEqual(2, BetterConversion(typeof(byte?), typeof(ulong?), typeof(uint?)));
			Assert.AreEqual(0, BetterConversion(typeof(byte), typeof(ushort?), typeof(uint)));
			Assert.AreEqual(0, BetterConversion(typeof(byte), typeof(ushort?), typeof(int)));
			Assert.AreEqual(2, BetterConversion(typeof(byte), typeof(ulong?), typeof(uint)));
			Assert.AreEqual(0, BetterConversion(typeof(byte), typeof(ulong?), typeof(int)));
			Assert.AreEqual(2, BetterConversion(typeof(ushort?), typeof(long?), typeof(int?)));
			Assert.AreEqual(0, BetterConversion(typeof(sbyte), typeof(int?), typeof(uint?)));
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
			Assert.IsFalse(conversions.ImplicitConversion(type1, type2).IsValid);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsMethodGroupConversion);
			Assert.IsFalse(c.DelegateCapturesFirstArgument);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsMethodGroupConversion);
			Assert.IsTrue(c.DelegateCapturesFirstArgument);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsMethodGroupConversion);
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
			Assert.IsFalse(c.IsValid);
			Assert.IsTrue(c.IsMethodGroupConversion);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsMethodGroupConversion);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsMethodGroupConversion);
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
			Assert.IsFalse(c.IsValid);
			Assert.IsTrue(c.IsMethodGroupConversion);
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
			Assert.IsFalse(c.IsValid);
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
			Assert.IsFalse(c.IsValid);
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
			Assert.IsFalse(c.IsValid);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsMethodGroupConversion);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsMethodGroupConversion);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsMethodGroupConversion);

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
			Assert.IsTrue(c.IsMethodGroupConversion);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsMethodGroupConversion);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsMethodGroupConversion);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsMethodGroupConversion);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsMethodGroupConversion);
			Assert.IsTrue(c.DelegateCapturesFirstArgument);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsMethodGroupConversion);
			Assert.IsFalse(c.DelegateCapturesFirstArgument);
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
			Assert.IsTrue(c.IsValid);
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
			Assert.IsTrue(c.IsValid);
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
			Assert.IsTrue(c.IsValid);
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
			Assert.IsTrue(c.IsValid);
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
			Assert.IsTrue(c.IsValid);
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
			Assert.IsTrue(c.IsValid);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
			Assert.IsTrue(c.IsLifted);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
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
			Assert.IsFalse(c.IsValid);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
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
			Assert.IsFalse(c.IsValid);
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
			Assert.IsFalse(c.IsValid);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
			Assert.IsFalse(c.IsLifted);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
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
			Assert.IsFalse(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
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
			Assert.IsFalse(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsUserDefined);
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
			Assert.IsFalse(rr.IsError);
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
			Assert.IsTrue(c.IsUserDefined);
			Assert.IsFalse(c.IsValid);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.ConversionBeforeUserDefinedOperator.IsImplicit);
			Assert.IsTrue(c.ConversionBeforeUserDefinedOperator.IsNumericConversion);
			Assert.IsTrue(c.ConversionBeforeUserDefinedOperator.IsValid);
			Assert.IsTrue(c.ConversionAfterUserDefinedOperator.IsIdentityConversion);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.ConversionBeforeUserDefinedOperator.IsIdentityConversion);
			Assert.IsTrue(c.ConversionAfterUserDefinedOperator.IsImplicit);
			Assert.IsTrue(c.ConversionAfterUserDefinedOperator.IsNumericConversion);
			Assert.IsTrue(c.ConversionAfterUserDefinedOperator.IsValid);
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
			Assert.IsTrue(c.IsValid);
			Assert.IsTrue(c.IsImplicit);
			Assert.IsFalse(c.IsExplicit);
			Assert.AreEqual(Conversion.IdentityConversion, c.ConversionBeforeUserDefinedOperator);
			Assert.AreEqual(Conversion.IdentityConversion, c.ConversionAfterUserDefinedOperator);
		}
		*/
	}
}
