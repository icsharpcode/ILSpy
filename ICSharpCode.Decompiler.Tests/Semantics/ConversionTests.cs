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
using System.Linq;

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
	using dynamic = ConversionTest.Dynamic;
	using nint = ConversionTest.NInt;
	using nuint = ConversionTest.NUInt;

	[TestFixture, Parallelizable(ParallelScope.All)]
	public unsafe class ConversionTest
	{
		/// <summary>
		/// A reflection class used to represent <c>null</c>.
		/// </summary>
		public sealed class Null { }

		/// <summary>
		/// A reflection class used to represent <c>dynamic</c>.
		/// </summary>
		public sealed class Dynamic { }

		/// <summary>
		/// A reflection class used to represent <c>nint</c>.
		/// </summary>
		public sealed class NInt { }

		/// <summary>
		/// A reflection class used to represent <c>nuint</c>.
		/// </summary>
		public sealed class NUInt { }

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

		public class ReplaceSpecialTypesVisitor : TypeVisitor
		{
			public override IType VisitTypeDefinition(ITypeDefinition type)
			{
				switch (type.FullName)
				{
					case "ICSharpCode.Decompiler.Tests.Semantics.ConversionTest.Dynamic":
						return SpecialType.Dynamic;
					case "ICSharpCode.Decompiler.Tests.Semantics.ConversionTest.Null":
						return SpecialType.NullType;
					case "ICSharpCode.Decompiler.Tests.Semantics.ConversionTest.NInt":
						return SpecialType.NInt;
					case "ICSharpCode.Decompiler.Tests.Semantics.ConversionTest.NUInt":
						return SpecialType.NUInt;
					default:
						return base.VisitTypeDefinition(type);
				}
			}
		}

		Conversion ImplicitConversion(Type from, Type to)
		{
			IType from2 = compilation.FindType(from).AcceptVisitor(new ReplaceSpecialTypesVisitor());
			IType to2 = compilation.FindType(to).AcceptVisitor(new ReplaceSpecialTypesVisitor());
			return conversions.ImplicitConversion(from2, to2);
		}

		Conversion ExplicitConversion(Type from, Type to)
		{
			IType from2 = compilation.FindType(from).AcceptVisitor(new ReplaceSpecialTypesVisitor());
			IType to2 = compilation.FindType(to).AcceptVisitor(new ReplaceSpecialTypesVisitor());
			return conversions.ExplicitConversion(from2, to2);
		}

		/// <summary>
		/// Converts a constant expression (e.g. an integer literal) to the target type.
		/// </summary>
		Conversion ConstantConversion(object value, Type to)
		{
			IType fromType = compilation.FindType(value.GetType());
			IType to2 = compilation.FindType(to).AcceptVisitor(new ReplaceSpecialTypesVisitor());
			return conversions.ImplicitConversion(new ConstantResolveResult(fromType, value), to2);
		}

		/// <summary>
		/// Builds a MethodGroupResolveResult from all methods named <paramref name="methodName"/>
		/// in <paramref name="declaringType"/> (as if the expression were "instance.M") and
		/// converts it to <paramref name="delegateType"/>.
		/// </summary>
		Conversion MethodGroupConversion(Type declaringType, string methodName, Type delegateType,
			ResolveResult targetResult = null, IMethod[] extensionMethods = null, IReadOnlyList<IType> typeArguments = null)
		{
			IType declaring = compilation.FindType(declaringType);
			var mgrr = new MethodGroupResolveResult(
				targetResult ?? new ResolveResult(declaring), methodName,
				new[] { new MethodListWithDeclaringType(declaring, declaring.GetMethods(m => m.Name == methodName)) },
				typeArguments);
			if (extensionMethods != null)
			{
				mgrr.extensionMethods = new List<List<IMethod>> { new List<IMethod>(extensionMethods) };
			}
			IType dt = compilation.FindType(delegateType).AcceptVisitor(new ReplaceSpecialTypesVisitor());
			return conversions.ImplicitConversion(mgrr, dt);
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
		public void NullableEnumerationConversion()
		{
			ResolveResult zero = new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), 0);
			ResolveResult one = new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), 1);
			C implicitEnumerationConversion = C.EnumerationConversion(true, true);
			Assert.That(conversions.ImplicitConversion(zero, compilation.FindType(typeof(StringComparison?))), Is.EqualTo(implicitEnumerationConversion));
			Assert.That(conversions.ImplicitConversion(one, compilation.FindType(typeof(StringComparison?))), Is.EqualTo(C.None));
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

			ITypeDefinition classImplementingIDisposable = compilation.FindType(typeof(ClassImplementingIDisposable)).GetDefinition();
			ITypeDefinition genericStructWithIDisposableConstraintAndImplicitConversion = compilation.FindType(typeof(GenericStructWithIDisposableConstraintAndImplicitConversion<>)).GetDefinition();
			IType genericStructIDisposableInstance = new ParameterizedType(genericStructWithIDisposableConstraintAndImplicitConversion, ImmutableArray.Create(compilation.FindType(typeof(IDisposable))));

			// C => S<I>
			Conversion c2 = conversions.ImplicitConversion(classImplementingIDisposable, genericStructIDisposableInstance);
			Assert.That(c2.IsImplicit && c2.IsUserDefined);
			Assert.That(c2.Method.FullName, Is.EqualTo("ICSharpCode.Decompiler.Tests.TypeSystem.GenericStructWithIDisposableConstraintAndImplicitConversion.op_Implicit"));

			Assert.That(conversions.ImplicitConversion(genericStructIDisposableInstance, classImplementingIDisposable), Is.EqualTo(C.None));
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
			IType fromType = compilation.FindType(value.GetType()).AcceptVisitor(new ReplaceSpecialTypesVisitor());
			ConstantResolveResult crr = new ConstantResolveResult(fromType, value);
			IType to2 = compilation.FindType(to).AcceptVisitor(new ReplaceSpecialTypesVisitor());
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
			IType sType = compilation.FindType(s).AcceptVisitor(new ReplaceSpecialTypesVisitor());
			IType t1Type = compilation.FindType(t1).AcceptVisitor(new ReplaceSpecialTypesVisitor());
			IType t2Type = compilation.FindType(t2).AcceptVisitor(new ReplaceSpecialTypesVisitor());
			return conversions.BetterConversion(sType, t1Type, t2Type);
		}

		int BetterConversion(object value, Type t1, Type t2)
		{
			IType fromType = compilation.FindType(value.GetType()).AcceptVisitor(new ReplaceSpecialTypesVisitor());
			ConstantResolveResult crr = new ConstantResolveResult(fromType, value);
			IType t1Type = compilation.FindType(t1).AcceptVisitor(new ReplaceSpecialTypesVisitor());
			IType t2Type = compilation.FindType(t2).AcceptVisitor(new ReplaceSpecialTypesVisitor());
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

		[Test]
		public void ExpansiveInheritance()
		{
			// interface A<in U> { }
			// interface B<X> : A<A<B<X>>> { }
			// Finding a conversion B<double> -> A<B<string>> must terminate even though
			// the base-type substitution keeps producing ever-larger types.
			ITypeDefinition a = compilation.FindType(typeof(ExpansiveInheritanceTestCases.A<>)).GetDefinition();
			ITypeDefinition b = compilation.FindType(typeof(ExpansiveInheritanceTestCases.B<>)).GetDefinition();

			IType type1 = new ParameterizedType(b, ImmutableArray.Create(compilation.FindType(KnownTypeCode.Double)));
			IType type2 = new ParameterizedType(a, ImmutableArray.Create<IType>(
				new ParameterizedType(b, ImmutableArray.Create(compilation.FindType(KnownTypeCode.String)))));
			Assert.That(!conversions.ImplicitConversion(type1, type2).IsValid);
		}

		[Test]
		public void ImplicitTypeParameterConversion()
		{
			// void M<T, U>(T t) where T : U  =>  "U u = t;" is a boxing conversion
			// (e.g. T = int, U = object)
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U");
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T", constraints: new[] { u });
			Assert.That(conversions.ImplicitConversion(t, u), Is.EqualTo(C.BoxingConversion));
		}

		[Test]
		public void InvalidImplicitTypeParameterConversion()
		{
			// void M<T, U>(T t) where U : T  =>  "U u = t;" is invalid
			// (the constraint points the wrong way)
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U", constraints: new[] { t });
			Assert.That(conversions.ImplicitConversion(t, u), Is.EqualTo(C.None));
		}

		[Test]
		public void ImplicitTypeParameterArrayConversion()
		{
			// void M<T, U>(T[] t) where T : U  =>  "U[] u = t;" is invalid
			// (e.g. T = int, U = object: int[] is not convertible to object[])
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U");
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T", constraints: new[] { u });
			Assert.That(conversions.ImplicitConversion(new ArrayType(compilation, t), new ArrayType(compilation, u)), Is.EqualTo(C.None));
		}

		[Test]
		public void ImplicitTypeParameterConversionWithClassConstraint()
		{
			// void M<T, U>(T t) where T : class, U where U : class  =>  "U u = t;" is a reference conversion
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U", hasReferenceTypeConstraint: true);
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T", hasReferenceTypeConstraint: true, constraints: new[] { u });
			Assert.That(conversions.ImplicitConversion(t, u), Is.EqualTo(C.ImplicitReferenceConversion));
		}

		[Test]
		public void ImplicitTypeParameterArrayConversionWithClassConstraint()
		{
			// void M<T, U>(T[] t) where T : class, U where U : class  =>  "U[] u = t;" is a
			// covariant array conversion (both are known to be reference types)
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U", hasReferenceTypeConstraint: true);
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T", hasReferenceTypeConstraint: true, constraints: new[] { u });
			Assert.That(conversions.ImplicitConversion(new ArrayType(compilation, t), new ArrayType(compilation, u)), Is.EqualTo(C.ImplicitReferenceConversion));
		}

		[Test]
		public void ImplicitTypeParameterConversionWithClassConstraintOnlyOnT()
		{
			// void M<T, U>(T t) where T : class, U  =>  "U u = t;" is a reference conversion
			// (T is known to be a reference type, so no boxing is involved)
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U");
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T", hasReferenceTypeConstraint: true, constraints: new[] { u });
			Assert.That(conversions.ImplicitConversion(t, u), Is.EqualTo(C.ImplicitReferenceConversion));
		}

		[Test]
		public void ImplicitTypeParameterArrayConversionWithClassConstraintOnlyOnT()
		{
			// void M<T, U>(T[] t) where T : class, U  =>  "U[] u = t;" is a covariant array conversion
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U");
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T", hasReferenceTypeConstraint: true, constraints: new[] { u });
			Assert.That(conversions.ImplicitConversion(new ArrayType(compilation, t), new ArrayType(compilation, u)), Is.EqualTo(C.ImplicitReferenceConversion));
		}

		[Test]
		public void MethodGroupConversion_Void()
		{
			// delegate void D();
			// D d = M;  with  public static void M() {}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.VoidStatic), "M",
				typeof(MethodGroupConversionTestCases.DVoid));
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
			Assert.That(!c.DelegateCapturesFirstArgument);
			Assert.That(c.Method, Is.Not.Null);
		}

		[Test]
		public void MethodGroupConversion_Void_InstanceMethod()
		{
			// delegate void D();
			// D d = M;  with  public void M() {}  (instance method)
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.VoidInstance), "M",
				typeof(MethodGroupConversionTestCases.DVoid));
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
			Assert.That(c.DelegateCapturesFirstArgument);
			Assert.That(c.Method, Is.Not.Null);
		}

		[Test]
		public void MethodGroupConversion_MatchingSignature()
		{
			// delegate object D(int argument);
			// D d = M;  with  public static object M(int argument) {...}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.ReturnObjectFromInt), "M",
				typeof(MethodGroupConversionTestCases.DObjInt));
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_InvalidReturnType()
		{
			// delegate object D(int argument);
			// D d = M;  with  public static int M(int argument) {...}
			// int -> object is a boxing conversion, not an identity/reference conversion,
			// so the method is not delegate-compatible.
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.ReturnIntFromInt), "M",
				typeof(MethodGroupConversionTestCases.DObjInt));
			Assert.That(!c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_CovariantReturnType()
		{
			// delegate object D(int argument);
			// D d = M;  with  public static string M(int argument) {...}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.ReturnStringFromInt), "M",
				typeof(MethodGroupConversionTestCases.DObjInt));
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_RefArgumentTypesEqual()
		{
			// delegate void D(ref object o);
			// D d = M;  with  public static void M(ref object o) {}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.RefObjParam), "M",
				typeof(MethodGroupConversionTestCases.DRefObj));
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_RefArgumentObjectVsDynamic()
		{
			// delegate void D(ref object o);
			// D d = M;  with  public static void M(ref dynamic o) {}
			// ref parameters require an identity conversion, and object <-> dynamic IS an
			// identity conversion, so this is valid (Roslyn accepts it; the original
			// NRefactory test expected invalid, which matched pre-Roslyn csc behavior).
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.RefDynamicParam), "M",
				typeof(MethodGroupConversionTestCases.DRefObj));
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_RefVsOut()
		{
			// delegate void D(ref object o);
			// D d = M;  with  public static void M(out object o) {...}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.OutObjParam), "M",
				typeof(MethodGroupConversionTestCases.DRefObj));
			Assert.That(!c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_RefVsNormal()
		{
			// delegate void D(ref object o);
			// D d = M;  with  public static void M(object o) {}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.ObjParam), "M",
				typeof(MethodGroupConversionTestCases.DRefObj));
			Assert.That(!c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_NormalVsOut()
		{
			// delegate void D(object o);
			// D d = M;  with  public static void M(out object o) {...}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.OutObjParam), "M",
				typeof(MethodGroupConversionTestCases.DObj));
			Assert.That(!c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_MatchingNormalParameter()
		{
			// delegate void D(object o);
			// D d = M;  with  public static void M(object o) {}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.ObjParam), "M",
				typeof(MethodGroupConversionTestCases.DObj));
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_IdentityConversion()
		{
			// delegate void D(object o);
			// D d = M;  with  public static void M(dynamic o) {}
			// object -> dynamic is an identity conversion, so M is delegate-compatible.
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.DynamicParam), "M",
				typeof(MethodGroupConversionTestCases.DObj));
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_Contravariance()
		{
			// delegate void D(string o);
			// D d = M;  with  public static void M(object o) {}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.ObjParam), "M",
				typeof(MethodGroupConversionTestCases.DStr));
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test, Ignore("Not sure if this conversion should be valid or not... NR and mcs both accept it as valid, csc treats it as invalid")]
		public void MethodGroupConversion_NoContravarianceDynamic()
		{
			// delegate void D(string o);
			// D d = M;  with  public static void M(dynamic o) {}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.DynamicParam), "M",
				typeof(MethodGroupConversionTestCases.DStr));
			//Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
		}

		[Test]
		public void MethodGroupConversion_ExactMatchIsBetter()
		{
			// delegate void D(string a);
			// D d = M;  with  static void M(object x) {}  and  static void M(string x = null) {}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.OverloadObjectOrOptionalString), "M",
				typeof(MethodGroupConversionTestCases.DStr));
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
			Assert.That(c.Method.Parameters.Single().Type.FullName, Is.EqualTo("System.String"));
		}

		[Test]
		public void MethodGroupConversion_CannotLeaveOutOptionalParameters()
		{
			// delegate void D(string a);
			// D d = M;  with  static void M(object x) {}  and  static void M(string x, string y = null) {}
			// The two-parameter overload cannot bind to a one-parameter delegate.
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.OverloadObjectOrStringPlusOptional), "M",
				typeof(MethodGroupConversionTestCases.DStr));
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
			Assert.That(c.Method.Parameters.Single().Type.FullName, Is.EqualTo("System.Object"));
		}

		[Test]
		public void MethodGroupConversion_CannotUseExpandedParams()
		{
			// delegate void D(string a);
			// D d = M;  with  static void M(object x) {}  and  static void M(params string[] x) {}
			// The params overload only matches in expanded form, which method group
			// conversions do not use.
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.OverloadObjectOrParamsString), "M",
				typeof(MethodGroupConversionTestCases.DStr));
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
			Assert.That(c.Method.Parameters.Single().Type.FullName, Is.EqualTo("System.Object"));
		}

		[Test]
		public void MethodGroupConversion_ExtensionMethod()
		{
			// static class Ext { public static void M(this string s, int x) {} }
			// delegate void D(int a);
			// string s = ""; D d = s.M;
			IType stringType = compilation.FindType(KnownTypeCode.String);
			IMethod extensionMethod = compilation.FindType(typeof(MethodGroupConversionExt))
				.GetMethods(m => m.Name == "M").Single();
			var c = MethodGroupConversion(typeof(string), "M",
				typeof(MethodGroupConversionTestCases.DInt),
				targetResult: new ResolveResult(stringType),
				extensionMethods: new[] { extensionMethod });
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
			Assert.That(c.DelegateCapturesFirstArgument);
		}

		[Test]
		public void MethodGroupConversion_ExtensionMethodUsedAsStaticMethod()
		{
			// static class Ext { public static void M(this string s, int x) {} }
			// delegate void D(string s, int a);
			// D d = Ext.M;
			var c = MethodGroupConversion(typeof(MethodGroupConversionExt), "M",
				typeof(MethodGroupConversionTestCases.DStrInt));
			Assert.That(c.IsValid);
			Assert.That(c.IsMethodGroupConversion);
			Assert.That(!c.DelegateCapturesFirstArgument);
		}

		[Test]
		public void MethodGroupConversion_ObjectToDynamic()
		{
			// Action<dynamic> x = F;  with  public void F(object o) {}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.ObjParamInstance), "F",
				typeof(Action<dynamic>));
			Assert.That(c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_ObjectToDynamicGenericArgument()
		{
			// Action<List<dynamic>> x = F;  with  public void F(List<object> l) {}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.ObjListParamInstance), "F",
				typeof(Action<List<dynamic>>));
			Assert.That(c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_ObjectToDynamicReturnValue()
		{
			// Func<dynamic> x = F;  with  public object F() {...}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.ObjReturnInstance), "F",
				typeof(Func<dynamic>));
			Assert.That(c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_DynamicToObject()
		{
			// Action<object> x = F;  with  public void F(dynamic o) {}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.DynamicParamInstance), "F",
				typeof(Action<object>));
			Assert.That(c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_DynamicToObjectGenericArgument()
		{
			// Action<List<object>> x = F;  with  public void F(List<dynamic> l) {}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.DynamicListParamInstance), "F",
				typeof(Action<List<object>>));
			Assert.That(c.IsValid);
		}

		[Test]
		public void MethodGroupConversion_DynamicToObjectReturnValue()
		{
			// Func<object> x = F;  with  public dynamic F() {...}
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.DynamicReturnInstance), "F",
				typeof(Func<object>));
			Assert.That(c.IsValid);
		}

		[Test]
		public void UserDefined_IntLiteral_ViaUInt_ToCustomStruct()
		{
			// struct T { public static implicit operator T(uint a) {...} }
			// T t = 1;
			var c = ConstantConversion(1, typeof(UserDefinedConversionTestCases.TFromUInt));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
		}

		[Test]
		public void UserDefined_NullLiteral_ViaString_ToCustomStruct()
		{
			// struct T { public static implicit operator T(string a) {...} }
			// T t = null;
			var c = ImplicitConversion(typeof(Null), typeof(UserDefinedConversionTestCases.TFromString));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
		}

		[Test]
		public void UserDefined_CanUseLiftedEvenIfReturnTypeAlreadyNullable()
		{
			// struct S { public static implicit operator short?(S s) {...} }
			// int? i = s;  with s of type S?
			var c = ImplicitConversion(typeof(UserDefinedConversionTestCases.SToNullableShort?), typeof(int?));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.IsLifted);
		}

		[Test]
		public void UserDefinedImplicitConversion_PicksExactSourceTypeIfPossible()
		{
			// Convertible has operators from int ("i") and short ("s");
			// Convertible a = 33;
			var c = ConstantConversion(33, typeof(UserDefinedConversionTestCases.ConvertibleFromIntOrShort));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("i"));
		}

		[Test]
		public void UserDefinedImplicitConversion_PicksMostEncompassedSourceType()
		{
			// Convertible has operators from long ("l") and uint ("ui");
			// Convertible a = (ushort)33;
			var c = ConstantConversion((ushort)33, typeof(UserDefinedConversionTestCases.ConvertibleFromLongOrUInt));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("ui"));
		}

		[Test]
		public void UserDefinedImplicitConversion_NoMostEncompassedSourceTypeIsInvalid()
		{
			// Convertible has operators from ulong and int; neither source type encompasses
			// the other, so the conversion from ushort is ambiguous.
			var c = ConstantConversion((ushort)33, typeof(UserDefinedConversionTestCases.ConvertibleFromULongOrInt));
			Assert.That(!c.IsValid);
		}

		[Test]
		public void UserDefinedImplicitConversion_PicksExactTargetTypeIfPossible()
		{
			// Convertible has operators to int ("i") and short ("s");
			// int a = new Convertible();
			var c = ImplicitConversion(typeof(UserDefinedConversionTestCases.ConvertibleToIntOrShort), typeof(int));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("i"));
		}

		[Test]
		public void UserDefinedImplicitConversion_PicksMostEncompassingTargetType()
		{
			// Convertible has operators to int ("i") and ushort ("us");
			// ulong a = new Convertible();  -- only ushort converts implicitly to ulong
			var c = ImplicitConversion(typeof(UserDefinedConversionTestCases.ConvertibleToIntOrUShort), typeof(ulong));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("us"));
		}

		[Test]
		public void UserDefinedImplicitConversion_NoMostEncompassingTargetTypeIsInvalid()
		{
			// Convertible has operators to uint and short; neither target type encompasses
			// the other, so the conversion to long is ambiguous.
			var c = ImplicitConversion(typeof(UserDefinedConversionTestCases.ConvertibleToUIntOrShort), typeof(long));
			Assert.That(!c.IsValid);
		}

		[Test]
		public void UserDefinedImplicitConversion_AmbiguousIsInvalid()
		{
			// Both AmbiguousA and AmbiguousB declare implicit operator AmbiguousB(AmbiguousA).
			var c = ImplicitConversion(typeof(UserDefinedConversionTestCases.AmbiguousA), typeof(UserDefinedConversionTestCases.AmbiguousB));
			Assert.That(!c.IsValid);
		}

		[Test]
		public void UserDefinedImplicitConversion_DefinedNullableTakesPrecedenceOverLifted()
		{
			// struct Convertible declares operators from int ("i") and from int? ("ni");
			// Convertible? a = (int?)33;  -- the user-defined nullable operator wins over
			// the lifted form of the int operator.
			var c = ImplicitConversion(typeof(int?), typeof(UserDefinedConversionTestCases.NullableConvertible?));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(!c.IsLifted);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("ni"));
		}

		[Test]
		public void UserDefinedImplicitConversion_UIntConstant()
		{
			// Convertible has operators from long ("l") and uint ("ui");
			// Convertible a = 33;  -- the constant 33 converts to uint, which is more specific
			var c = ConstantConversion(33, typeof(UserDefinedConversionTestCases.ConvertibleFromLongOrUInt));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("ui"));
		}

		[Test]
		public void UserDefinedImplicitConversion_NullableUIntConstant()
		{
			// Convertible has operators from long? ("l") and uint? ("ui");
			// Convertible a = 33;
			var c = ConstantConversion(33, typeof(UserDefinedConversionTestCases.ConvertibleFromNullableLongOrNullableUInt));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("ui"));
		}

		[Test]
		public void UserDefinedImplicitConversion_UseShortResult_BecauseNullableCannotBeUnpacked()
		{
			// operators to int? ("i") and short ("s");
			// int x = new Test();  -- int? cannot be unpacked to int, so short is used
			var c = ImplicitConversion(typeof(UserDefinedConversionTestCases.ToNullableIntOrShort), typeof(int));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.ReturnType.FullName, Is.EqualTo("System.Int16"));
		}

		[Test]
		public void UserDefinedImplicitConversion_Short_Or_NullableByte_Target()
		{
			// operators to short ("s") and byte? ("b");
			// int? x = new Test();
			var c = ImplicitConversion(typeof(UserDefinedConversionTestCases.ToShortOrNullableByte), typeof(int?));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.ReturnType.FullName, Is.EqualTo("System.Int16"));
		}

		[Test]
		public void UserDefinedImplicitConversion_Byte_Or_NullableShort_Target()
		{
			// operators to byte ("b") and short? ("s");
			// int? x = new Test();
			var c = ImplicitConversion(typeof(UserDefinedConversionTestCases.ToByteOrNullableShort), typeof(int?));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("s"));
		}

		[Test]
		public void UserDefinedImplicitConversion_Int_Or_NullableLong_Source()
		{
			// operators from int ("i") and long? ("l"); source is short
			var c = ImplicitConversion(typeof(short), typeof(UserDefinedConversionTestCases.FromIntOrNullableLong));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("i"));
		}

		[Test]
		public void UserDefinedImplicitConversion_NullableInt_Or_Long_Source()
		{
			// operators from int? ("i") and long ("l"); source is short:
			// neither int? nor long is more specific, so the conversion is ambiguous
			var c = ImplicitConversion(typeof(short), typeof(UserDefinedConversionTestCases.FromNullableIntOrLong));
			Assert.That(!c.IsValid);
			Assert.That(c.IsUserDefined);
		}

		[Test]
		public void UserDefinedImplicitConversion_NullableInt_Or_Long_Constant_Source()
		{
			// operators from int? ("i") and long ("l"); source is the constant 1
			var c = ConstantConversion(1, typeof(UserDefinedConversionTestCases.FromNullableIntOrLong));
			Assert.That(!c.IsValid);
			Assert.That(c.IsUserDefined);
		}

		[Test]
		public void UserDefinedImplicitConversion_NullableInt_Or_NullableLong_Source()
		{
			// operators from int? ("i") and long? ("l"); source is short
			var c = ImplicitConversion(typeof(short), typeof(UserDefinedConversionTestCases.FromNullableIntOrNullableLong));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("i"));
		}

		/// <summary>
		/// Creates a fake method named M with a single parameter of the given type.
		/// </summary>
		IMethod MakeUnaryMethod(Type parameterType)
		{
			var m = new FakeMethod(compilation, SymbolKind.Method);
			m.Name = "M";
			m.Parameters = new[] { new DefaultParameter(compilation.FindType(parameterType), "x", owner: m) };
			return m;
		}

		[Test]
		public void PreferUserDefinedConversionOverReferenceConversion()
		{
			// M(new AA()) with overloads M(object) and M(string), where AA has an implicit
			// conversion to string, picks M(string) -- not because user-defined conversions
			// are better, but because string is a better conversion target.
			var or = new OverloadResolution(compilation, new[] {
				new ResolveResult(compilation.FindType(typeof(UserDefinedConversionTestCases.ConvertibleToString)))
			});
			IMethod mObject = MakeUnaryMethod(typeof(object));
			IMethod mString = MakeUnaryMethod(typeof(string));
			Assert.That(or.AddCandidate(mObject), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(or.AddCandidate(mString), Is.EqualTo(OverloadResolutionErrors.None));
			Assert.That(!or.IsAmbiguous);
			Assert.That(or.BestCandidate, Is.SameAs(mString));
		}

		[Test]
		public void PreferAmbiguousConversionOverReferenceConversion()
		{
			// Ambiguous conversions are a compiler error; but they are not
			// preventing the overload from being chosen.

			// The user-defined conversion AmbiguousA -> AmbiguousB is ambiguous (declared
			// in both classes) and therefore invalid...
			var c = ImplicitConversion(typeof(UserDefinedConversionTestCases.AmbiguousA), typeof(UserDefinedConversionTestCases.AmbiguousB));
			Assert.That(c.IsUserDefined);
			Assert.That(!c.IsValid);

			// ...but M(new AmbiguousA()) with overloads M(AmbiguousB) and M(object) still
			// picks M(AmbiguousB), because AmbiguousB is a better conversion target than object.
			var or = new OverloadResolution(compilation, new[] {
				new ResolveResult(compilation.FindType(typeof(UserDefinedConversionTestCases.AmbiguousA)))
			});
			IMethod mAmbiguousB = MakeUnaryMethod(typeof(UserDefinedConversionTestCases.AmbiguousB));
			IMethod mObject = MakeUnaryMethod(typeof(object));
			or.AddCandidate(mAmbiguousB);
			or.AddCandidate(mObject);
			Assert.That(or.BestCandidate, Is.SameAs(mAmbiguousB));
		}

		[Test]
		public void UserDefinedImplicitConversion_ConversionBeforeUserDefinedOperatorIsCorrect()
		{
			// Convertible has an operator from long; converting an int goes int -> long -> Convertible.
			var c = ImplicitConversion(typeof(int), typeof(UserDefinedConversionTestCases.ConvertibleFromLong));
			Assert.That(c.IsValid);
			Assert.That(c.ConversionBeforeUserDefinedOperator.IsImplicit);
			Assert.That(c.ConversionBeforeUserDefinedOperator.IsNumericConversion);
			Assert.That(c.ConversionBeforeUserDefinedOperator.IsValid);
			Assert.That(c.ConversionAfterUserDefinedOperator.IsIdentityConversion);
		}

		[Test]
		public void UserDefinedImplicitConversion_ConversionAfterUserDefinedOperatorIsCorrect()
		{
			// Convertible has an operator to int; converting to long goes Convertible -> int -> long.
			var c = ImplicitConversion(typeof(UserDefinedConversionTestCases.ConvertibleToInt), typeof(long));
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
			var c = ConstantConversion(7, typeof(UserDefinedConversionTestCases.JsNumber));
			Assert.That(c.IsValid);
			Assert.That(c.IsImplicit);
			Assert.That(!c.IsExplicit);
			Assert.That(c.ConversionBeforeUserDefinedOperator, Is.EqualTo(C.IdentityConversion));
			Assert.That(c.ConversionAfterUserDefinedOperator, Is.EqualTo(C.IdentityConversion));
		}

		[Test]
		public void TupleIdentityConversionWithUnderlyingValueTuple()
		{
			// C# standard 10.2.2: identity conversion between a tuple type and the
			// corresponding constructed ValueTuple<...> type
			var intType = compilation.FindType(typeof(int));
			var stringType = compilation.FindType(typeof(string));
			IType tupleType = new TupleType(compilation, ImmutableArray.Create(intType, stringType), ImmutableArray.Create("a", "b"));
			IType valueTupleType = compilation.FindType(typeof(ValueTuple<int, string>));
			Assert.That(conversions.ImplicitConversion(tupleType, valueTupleType), Is.EqualTo(C.IdentityConversion));
			Assert.That(conversions.ImplicitConversion(valueTupleType, tupleType), Is.EqualTo(C.IdentityConversion));
		}

		[Test]
		public void IdentityConversionNullableReferenceType()
		{
			// C# standard 10.2.2: identity conversion between T and T? for any reference type T
			IType stringType = compilation.FindType(KnownTypeCode.String);
			IType nullableStringType = stringType.ChangeNullability(Nullability.Nullable);
			Assert.That(conversions.ImplicitConversion(stringType, nullableStringType), Is.EqualTo(C.IdentityConversion));
			Assert.That(conversions.ImplicitConversion(nullableStringType, stringType), Is.EqualTo(C.IdentityConversion));
		}

		[Test]
		public void NumericConversionMatrix()
		{
			// C# standard 10.2.3 (implicit numeric conversions) and 10.3.2 (explicit numeric
			// conversions). The implicit table below is transcribed from 10.2.3; every other
			// pair of distinct numeric types must be an explicit numeric conversion.
			var allNumericTypes = new[] {
				typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int),
				typeof(uint), typeof(long), typeof(ulong), typeof(char), typeof(float),
				typeof(double), typeof(decimal)
			};
			var implicitConversions = new Dictionary<Type, Type[]> {
				[typeof(sbyte)] = new[] { typeof(short), typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) },
				[typeof(byte)] = new[] { typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) },
				[typeof(short)] = new[] { typeof(int), typeof(long), typeof(float), typeof(double), typeof(decimal) },
				[typeof(ushort)] = new[] { typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) },
				[typeof(int)] = new[] { typeof(long), typeof(float), typeof(double), typeof(decimal) },
				[typeof(uint)] = new[] { typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) },
				[typeof(long)] = new[] { typeof(float), typeof(double), typeof(decimal) },
				[typeof(ulong)] = new[] { typeof(float), typeof(double), typeof(decimal) },
				[typeof(char)] = new[] { typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal) },
				[typeof(float)] = new[] { typeof(double) },
				[typeof(double)] = new Type[0],
				[typeof(decimal)] = new Type[0],
			};
			foreach (Type from in allNumericTypes)
			{
				foreach (Type to in allNumericTypes)
				{
					string pair = from.Name + " -> " + to.Name;
					if (from == to)
					{
						Assert.That(ImplicitConversion(from, to), Is.EqualTo(C.IdentityConversion), pair);
					}
					else if (Array.IndexOf(implicitConversions[from], to) >= 0)
					{
						Assert.That(ImplicitConversion(from, to), Is.EqualTo(C.ImplicitNumericConversion), pair);
						Assert.That(ExplicitConversion(from, to), Is.EqualTo(C.ImplicitNumericConversion), pair);
					}
					else
					{
						Assert.That(ImplicitConversion(from, to), Is.EqualTo(C.None), pair);
						Assert.That(ExplicitConversion(from, to), Is.EqualTo(C.ExplicitNumericConversion), pair);
					}
				}
			}
		}

		[Test]
		public void InterpolatedStringConversion()
		{
			// C# standard 10.2.5: an interpolated string expression converts to
			// System.IFormattable and System.FormattableString
			var interpolated = new InterpolatedStringResolveResult(compilation.FindType(KnownTypeCode.String),
				"{0}", new ResolveResult(compilation.FindType(KnownTypeCode.Int32)));
			Assert.That(conversions.ImplicitConversion(interpolated, compilation.FindType(KnownTypeCode.IFormattable)), Is.EqualTo(C.ImplicitInterpolatedStringConversion));
			Assert.That(conversions.ImplicitConversion(interpolated, compilation.FindType(KnownTypeCode.FormattableString)), Is.EqualTo(C.ImplicitInterpolatedStringConversion));
			Assert.That(conversions.ImplicitConversion(interpolated, compilation.FindType(KnownTypeCode.String)), Is.EqualTo(C.IdentityConversion));
		}

		[Test]
		public void IdentityDerivedNullableConversion()
		{
			// C# standard 10.2.6/10.6.1: nullable conversion derived from the identity conversion
			Assert.That(ImplicitConversion(typeof(int), typeof(int?)), Is.EqualTo(C.ImplicitNullableConversion));
		}

		[Test]
		public void DelegateToSystemDelegateConversions()
		{
			// C# standard 10.2.8: from any delegate_type to System.Delegate and the
			// interfaces it implements
			Assert.That(ImplicitConversion(typeof(Action), typeof(Delegate)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(Action), typeof(MulticastDelegate)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(Action), typeof(ICloneable)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(Delegate), typeof(Action)), Is.EqualTo(C.None));
		}

		[Test]
		public void ClassToBaseClassConversion()
		{
			// C# standard 10.2.8: from any class_type S to any class_type T, provided S is derived from T
			Assert.That(ImplicitConversion(typeof(UserDefinedExplicitConversionTestCases.DerivedClass), typeof(UserDefinedExplicitConversionTestCases.BaseClass)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(UserDefinedExplicitConversionTestCases.BaseClass), typeof(UserDefinedExplicitConversionTestCases.DerivedClass)), Is.EqualTo(C.None));
		}

		[Test]
		public void ArrayToIReadOnlyListConversion()
		{
			// C# standard 10.2.8: from S[] to IReadOnlyList<T> and its base interfaces
			Assert.That(ImplicitConversion(typeof(string[]), typeof(IReadOnlyList<string>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(string[]), typeof(IReadOnlyList<object>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(string[]), typeof(IReadOnlyCollection<string>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(string[,]), typeof(IReadOnlyList<string>)), Is.EqualTo(C.None));
		}

		[Test]
		public void VarianceConversionWithDynamicTypeArgument()
		{
			// C# standard 10.2.8: implicit reference conversion via a variance-convertible
			// type where the type arguments differ by the object/dynamic identity conversion
			Assert.That(ImplicitConversion(typeof(IEnumerable<string>), typeof(IEnumerable<dynamic>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ImplicitConversion(typeof(List<string>), typeof(IEnumerable<dynamic>)), Is.EqualTo(C.ImplicitReferenceConversion));
		}

		[Test]
		public void BoxingConversions()
		{
			// C# standard 10.2.9
			Assert.That(ImplicitConversion(typeof(int), typeof(object)), Is.EqualTo(C.BoxingConversion));
			Assert.That(ImplicitConversion(typeof(int), typeof(ValueType)), Is.EqualTo(C.BoxingConversion));
			Assert.That(ImplicitConversion(typeof(StringComparison), typeof(Enum)), Is.EqualTo(C.BoxingConversion));
			Assert.That(ImplicitConversion(typeof(int), typeof(IFormattable)), Is.EqualTo(C.BoxingConversion));
			Assert.That(ImplicitConversion(typeof(int), typeof(IComparable<int>)), Is.EqualTo(C.BoxingConversion));
			Assert.That(ImplicitConversion(typeof(int), typeof(IComparable<string>)), Is.EqualTo(C.None));
			// nullable value types box to the reference types their underlying type boxes to
			Assert.That(ImplicitConversion(typeof(int?), typeof(object)), Is.EqualTo(C.BoxingConversion));
			Assert.That(ImplicitConversion(typeof(int?), typeof(IFormattable)), Is.EqualTo(C.BoxingConversion));
			Assert.That(ImplicitConversion(typeof(StringComparison?), typeof(Enum)), Is.EqualTo(C.BoxingConversion));
		}

		[Test]
		public void BoxingConversionViaVariance()
		{
			// C# standard 10.2.9: boxing to an interface that the implemented interface is
			// variance-convertible to
			Assert.That(ImplicitConversion(typeof(StructImplementingIEnumerableOfString), typeof(IEnumerable<string>)), Is.EqualTo(C.BoxingConversion));
			Assert.That(ImplicitConversion(typeof(StructImplementingIEnumerableOfString), typeof(IEnumerable<object>)), Is.EqualTo(C.BoxingConversion));
			Assert.That(ImplicitConversion(typeof(StructImplementingIEnumerableOfString), typeof(IEnumerable)), Is.EqualTo(C.BoxingConversion));
			Assert.That(ImplicitConversion(typeof(StructImplementingIEnumerableOfString), typeof(IEnumerable<int>)), Is.EqualTo(C.None));
		}

		[Test]
		public void ImplicitConstantExpressionConversionToUInt64()
		{
			// C# standard 10.2.11: an int constant expression converts to ulong
			// provided its value is non-negative
			Assert.That(IntegerLiteralConversion(0, typeof(ulong)));
			Assert.That(IntegerLiteralConversion(200, typeof(ulong)));
			Assert.That(!IntegerLiteralConversion(-1, typeof(ulong)));
		}

		[Test]
		public void TypeParameterConversionViaVariance()
		{
			// C# standard 10.2.12 (last bullet group): conversions from T via a
			// variance-convertible interface of its effective base class / interface set
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T",
				constraints: new[] { compilation.FindType(typeof(List<string>)) });
			Assert.That(conversions.ImplicitConversion(t, compilation.FindType(typeof(IEnumerable<object>))), Is.EqualTo(C.ImplicitReferenceConversion));

			ITypeParameter t2 = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T",
				constraints: new[] { compilation.FindType(typeof(IEnumerable<string>)) });
			Assert.That(conversions.ImplicitConversion(t2, compilation.FindType(typeof(IEnumerable<object>))), Is.EqualTo(C.BoxingConversion));
		}

		[Test]
		public void TupleLiteralConversions()
		{
			// C# standard 10.2.13: implicit conversion from a tuple literal, using the
			// implicit conversions of the element expressions
			var intType = compilation.FindType(KnownTypeCode.Int32);
			var stringType = compilation.FindType(KnownTypeCode.String);
			var byteType = compilation.FindType(KnownTypeCode.Byte);

			// (2, null) -> (byte, string)
			var literal = new TupleResolveResult(compilation, ImmutableArray.Create<ResolveResult>(
				new ConstantResolveResult(intType, 2),
				new ConstantResolveResult(SpecialType.NullType, null)));
			Assert.That(conversions.ImplicitConversion(literal, new TupleType(compilation, ImmutableArray.Create(byteType, stringType))),
				Is.EqualTo(C.TupleConversion(ImmutableArray.Create(C.ImplicitConstantExpressionConversion, C.NullLiteralConversion))));

			// arity mismatch
			Assert.That(conversions.ImplicitConversion(literal, new TupleType(compilation, ImmutableArray.Create(byteType, stringType, intType))),
				Is.EqualTo(C.None));

			// (300, null) -> (byte, string): 300 is out of range for byte
			var literal2 = new TupleResolveResult(compilation, ImmutableArray.Create<ResolveResult>(
				new ConstantResolveResult(intType, 300),
				new ConstantResolveResult(SpecialType.NullType, null)));
			Assert.That(conversions.ImplicitConversion(literal2, new TupleType(compilation, ImmutableArray.Create(byteType, stringType))),
				Is.EqualTo(C.None));
		}

		[Test, Ignore("Known bug: CSharpConversions does not support nullable conversions derived from tuple conversions, but csc accepts them")]
		public void LiftedTupleConversions()
		{
			// csc accepts the nullable conversions derived from tuple conversions:
			//   (int, string) t = (1, "one");
			//   (long, object)? a = t;
			//   (int, string)? tn = t;
			//   (long, object)? b = tn;
			Assert.That(ImplicitConversion(typeof((int, string)), typeof((long, object)?)).IsValid, "(int, string) -> (long, object)?");
			Assert.That(ImplicitConversion(typeof((int, string)?), typeof((long, object)?)).IsValid, "(int, string)? -> (long, object)?");
		}

		[Test]
		public void UserDefinedImplicitConversion_OperatorDeclaredInBaseClassOfSource()
		{
			// C# standard 10.5.4: the set of considered operator declarations includes the
			// base classes of the source type
			var c = ImplicitConversion(typeof(UserDefinedConversionTestCases.DerivedFromOperatorInBaseClass), typeof(string));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.DeclaringType.Name, Is.EqualTo("OperatorInBaseClass"));
		}

		[Test, Ignore("C# standard 10.2.16 is not implemented: CSharpConversions.ImplicitConversion has a TODO for default literal conversions, and no ResolveResult represents a typeless default literal")]
		public void DefaultLiteralConversions()
		{
			// C# standard 10.2.16: an implicit conversion exists from a default_literal to
			// any type, producing the default value of the inferred type. Once the semantic
			// model gains a typeless default-literal ResolveResult, this test should assert
			// that it converts to int, string, int? and type parameters.
			Assert.Fail("Default literal conversions are not implemented.");
		}

		[Test, Ignore("C# standard 10.2.18 is not implemented: no ResolveResult represents a switch expression; the decompiler converts each arm separately in ILAst")]
		public void SwitchExpressionConversion()
		{
			// C# standard 10.2.18: an implicit conversion exists from a switch_expression to
			// every type T to which all arm expressions implicitly convert. Once the semantic
			// model gains a switch-expression ResolveResult, this test should assert that the
			// conversion exists iff every arm converts to the target type.
			Assert.Fail("Switch expression conversions are not implemented.");
		}

		[Test]
		public void ThrowExpressionConversion()
		{
			// C# standard 10.2.17: throw expressions convert to any type
			Assert.That(conversions.ImplicitConversion(new ThrowResolveResult(), compilation.FindType(KnownTypeCode.String)), Is.EqualTo(C.ThrowExpressionConversion));
			Assert.That(conversions.ImplicitConversion(new ThrowResolveResult(), compilation.FindType(KnownTypeCode.Int32)), Is.EqualTo(C.ThrowExpressionConversion));
		}

		[Test]
		public void StandardImplicitConversions()
		{
			// C# standard 10.4.2: standard implicit conversions exclude user-defined conversions
			Assert.That(conversions.StandardImplicitConversion(compilation.FindType(typeof(int)), compilation.FindType(typeof(long))), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(conversions.StandardImplicitConversion(compilation.FindType(typeof(string)), compilation.FindType(typeof(object))), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(conversions.StandardImplicitConversion(compilation.FindType(typeof(DateTime)), compilation.FindType(typeof(DateTimeOffset))), Is.EqualTo(C.None));
		}

		[Test]
		public void MethodGroupConversion_GenericMethodTypeInference()
		{
			// C# standard 10.8: delegate parameter types are used to infer the type
			// arguments of a generic method group
			// delegate int D(string s, int i);
			// D d = F;  with  static T F<T>(string s, T t)  -- T=int is inferred
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.GenericMethods), "F",
				typeof(MethodGroupConversionTestCases.DStrIntRetInt));
			Assert.That(c.IsValid);
			Assert.That(c.Method.TypeArguments.Single().IsKnownType(KnownTypeCode.Int32));
		}

		[Test]
		public void MethodGroupConversion_GenericMethodExplicitTypeArguments()
		{
			// C# standard 10.8:
			// delegate int E();
			// E e = G<int>;  with  static T G<T>()
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.GenericMethods), "G",
				typeof(MethodGroupConversionTestCases.DRetInt),
				typeArguments: new[] { compilation.FindType(KnownTypeCode.Int32) });
			Assert.That(c.IsValid);
			Assert.That(c.Method.TypeArguments.Single().IsKnownType(KnownTypeCode.Int32));
		}

		[Test]
		public void MethodGroupConversion_CannotInferFromReturnType()
		{
			// C# standard 10.8: the return type of the delegate is not used for inference
			// E e = G;  with  static T G<T>()
			var c = MethodGroupConversion(typeof(MethodGroupConversionTestCases.GenericMethods), "G",
				typeof(MethodGroupConversionTestCases.DRetInt));
			Assert.That(!c.IsValid);
		}

		[Test]
		public void AnonymousFunctionConversions()
		{
			// C# standard 10.7.1: compatibility of an anonymous function with a delegate type.
			// TestLambda stands in for a lambda whose body is an expression of the given type;
			// the signature checks under test here are performed by CSharpConversions itself.
			IType intType = compilation.FindType(KnownTypeCode.Int32);
			var intParam = new IParameter[] { new DefaultParameter(intType, "x") };

			// (int x) => intExpr  is compatible with Func<int, int> and Func<int, double>
			Assert.That(conversions.ImplicitConversion(new TestLambda(intType, intParam), compilation.FindType(typeof(Func<int, int>))).IsValid);
			Assert.That(conversions.ImplicitConversion(new TestLambda(intType, intParam), compilation.FindType(typeof(Func<int, double>))).IsValid);
			// parameter count mismatch
			Assert.That(conversions.ImplicitConversion(new TestLambda(intType, intParam), compilation.FindType(typeof(Func<int>))), Is.EqualTo(C.None));
			// an explicitly typed parameter must have the delegate's parameter type
			Assert.That(conversions.ImplicitConversion(new TestLambda(intType, intParam), compilation.FindType(typeof(Func<double, int>))), Is.EqualTo(C.None));
			// an implicitly typed parameter list is incompatible with ref/out parameters
			Assert.That(conversions.ImplicitConversion(
				new TestLambda(intType, new IParameter[] { new DefaultParameter(SpecialType.UnknownType, "x") }, isImplicitlyTyped: true),
				compilation.FindType(typeof(MethodGroupConversionTestCases.DRefObj))), Is.EqualTo(C.None));
			// an anonymous method without a signature accepts any parameter list without out parameters
			Assert.That(conversions.ImplicitConversion(
				new TestLambda(intType, hasParameterList: false, isAnonymousMethod: true),
				compilation.FindType(typeof(Func<int, int>))).IsValid);
		}

		[Test]
		public void LambdaToExpressionTreeConversion()
		{
			// C# standard 10.7.1: a lambda expression compatible with D is compatible with
			// Expression<D>; anonymous methods are not.
			IType intType = compilation.FindType(KnownTypeCode.Int32);
			var intParam = new IParameter[] { new DefaultParameter(intType, "x") };
			Assert.That(conversions.ImplicitConversion(new TestLambda(intType, intParam),
				compilation.FindType(typeof(System.Linq.Expressions.Expression<Func<int, int>>))).IsValid);
			Assert.That(conversions.ImplicitConversion(
				new TestLambda(intType, hasParameterList: false, isAnonymousMethod: true),
				compilation.FindType(typeof(System.Linq.Expressions.Expression<Func<int, int>>))), Is.EqualTo(C.None));
		}

		/// <summary>
		/// Stands in for a lambda or anonymous method whose body is an expression of a fixed type.
		/// </summary>
		sealed class TestLambda : LambdaResolveResult
		{
			readonly IType bodyReturnType;
			readonly IParameter[] parameters;

			public TestLambda(IType bodyReturnType, IParameter[] parameters = null,
				bool hasParameterList = true, bool isAnonymousMethod = false, bool isImplicitlyTyped = false)
			{
				this.bodyReturnType = bodyReturnType;
				this.parameters = parameters ?? new IParameter[0];
				this.HasParameterList = hasParameterList;
				this.IsAnonymousMethod = isAnonymousMethod;
				this.IsImplicitlyTyped = isImplicitlyTyped;
			}

			public override bool HasParameterList { get; }
			public override bool IsAnonymousMethod { get; }
			public override bool IsImplicitlyTyped { get; }
			public override bool IsAsync => false;
			public override IReadOnlyList<IParameter> Parameters => parameters;
			public override IType ReturnType => bodyReturnType;
			public override ResolveResult Body => null;

			public override IType GetInferredReturnType(IType[] parameterTypes)
			{
				return bodyReturnType;
			}

			public override Conversion IsValid(IType[] parameterTypes, IType returnType, CSharpConversions conversions)
			{
				return conversions.ImplicitConversion(bodyReturnType, returnType);
			}
		}
	}
}
