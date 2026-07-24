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
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.Tests.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Semantics
{
	using C = Conversion;
	using dynamic = ConversionTest.Dynamic;

	[TestFixture, Parallelizable(ParallelScope.All)]
	public class ExplicitConversionsTest
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

		Conversion ExplicitConversion(Type from, Type to)
		{
			IType from2 = compilation.FindType(from).AcceptVisitor(new ConversionTest.ReplaceSpecialTypesVisitor());
			IType to2 = compilation.FindType(to).AcceptVisitor(new ConversionTest.ReplaceSpecialTypesVisitor());
			return conversions.ExplicitConversion(from2, to2);
		}

		[Test]
		public void PointerConversion()
		{
			Assert.That(ExplicitConversion(typeof(int*), typeof(short)), Is.EqualTo(C.ExplicitPointerConversion));
			Assert.That(ExplicitConversion(typeof(short), typeof(void*)), Is.EqualTo(C.ExplicitPointerConversion));

			Assert.That(ExplicitConversion(typeof(void*), typeof(int*)), Is.EqualTo(C.ExplicitPointerConversion));
			Assert.That(ExplicitConversion(typeof(long*), typeof(byte*)), Is.EqualTo(C.ExplicitPointerConversion));
		}

		[Test]
		public void ConversionFromDynamic()
		{
			// Explicit dynamic conversion is for resolve results only;
			// otherwise it's an explicit reference / unboxing conversion
			Assert.That(ExplicitConversion(typeof(dynamic), typeof(string)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(dynamic), typeof(int)), Is.EqualTo(C.UnboxingConversion));

			var dynamicRR = new ResolveResult(SpecialType.Dynamic);
			Assert.That(conversions.ExplicitConversion(dynamicRR, compilation.FindType(typeof(string))), Is.EqualTo(C.ExplicitDynamicConversion));
			Assert.That(conversions.ExplicitConversion(dynamicRR, compilation.FindType(typeof(int))), Is.EqualTo(C.ExplicitDynamicConversion));
		}

		[Test]
		public void NumericConversions()
		{
			Assert.That(ExplicitConversion(typeof(sbyte), typeof(uint)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(sbyte), typeof(char)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(byte), typeof(char)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(byte), typeof(sbyte)), Is.EqualTo(C.ExplicitNumericConversion));
			// if an implicit conversion exists, ExplicitConversion() should return that
			Assert.That(ExplicitConversion(typeof(byte), typeof(int)), Is.EqualTo(C.ImplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(double), typeof(float)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(double), typeof(decimal)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(decimal), typeof(double)), Is.EqualTo(C.ExplicitNumericConversion));
			Assert.That(ExplicitConversion(typeof(int), typeof(decimal)), Is.EqualTo(C.ImplicitNumericConversion));

			Assert.That(ExplicitConversion(typeof(bool), typeof(int)), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(int), typeof(bool)), Is.EqualTo(C.None));
		}

		[Test]
		public void EnumerationConversions()
		{
			var explicitEnumerationConversion = C.EnumerationConversion(false, false);
			Assert.That(ExplicitConversion(typeof(sbyte), typeof(StringComparison)), Is.EqualTo(explicitEnumerationConversion));
			Assert.That(ExplicitConversion(typeof(char), typeof(StringComparison)), Is.EqualTo(explicitEnumerationConversion));
			Assert.That(ExplicitConversion(typeof(int), typeof(StringComparison)), Is.EqualTo(explicitEnumerationConversion));
			Assert.That(ExplicitConversion(typeof(decimal), typeof(StringComparison)), Is.EqualTo(explicitEnumerationConversion));
			Assert.That(ExplicitConversion(typeof(StringComparison), typeof(char)), Is.EqualTo(explicitEnumerationConversion));
			Assert.That(ExplicitConversion(typeof(StringComparison), typeof(int)), Is.EqualTo(explicitEnumerationConversion));
			Assert.That(ExplicitConversion(typeof(StringComparison), typeof(decimal)), Is.EqualTo(explicitEnumerationConversion));
			Assert.That(ExplicitConversion(typeof(StringComparison), typeof(StringSplitOptions)), Is.EqualTo(explicitEnumerationConversion));
		}

		[Test]
		public void NullableConversion_BasedOnIdentityConversion()
		{
			Assert.That(ExplicitConversion(typeof(ArraySegment<dynamic>?), typeof(ArraySegment<object>?)), Is.EqualTo(C.IdentityConversion));
			Assert.That(ExplicitConversion(typeof(ArraySegment<dynamic>), typeof(ArraySegment<object>?)), Is.EqualTo(C.ImplicitNullableConversion));
			Assert.That(ExplicitConversion(typeof(ArraySegment<dynamic>?), typeof(ArraySegment<object>)), Is.EqualTo(C.ExplicitNullableConversion));
		}

		[Test]
		public void NullableConversion_BasedOnImplicitNumericConversion()
		{
			Assert.That(ExplicitConversion(typeof(int?), typeof(long?)), Is.EqualTo(C.ImplicitLiftedNumericConversion));
			Assert.That(ExplicitConversion(typeof(int), typeof(long?)), Is.EqualTo(C.ImplicitLiftedNumericConversion));
			Assert.That(ExplicitConversion(typeof(int?), typeof(long)), Is.EqualTo(C.ExplicitLiftedNumericConversion));
		}

		[Test]
		public void NullableConversion_BasedOnImplicitEnumerationConversion()
		{
			ResolveResult zero = new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), 0);
			ResolveResult one = new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), 1);
			Assert.That(conversions.ExplicitConversion(zero, compilation.FindType(typeof(StringComparison?))), Is.EqualTo(C.EnumerationConversion(true, true)));
			Assert.That(conversions.ExplicitConversion(one, compilation.FindType(typeof(StringComparison?))), Is.EqualTo(C.EnumerationConversion(false, true)));
		}

		[Test]
		public void NullableConversion_BasedOnExplicitNumericConversion()
		{
			Assert.That(ExplicitConversion(typeof(int?), typeof(short?)), Is.EqualTo(C.ExplicitLiftedNumericConversion));
			Assert.That(ExplicitConversion(typeof(int), typeof(short?)), Is.EqualTo(C.ExplicitLiftedNumericConversion));
			Assert.That(ExplicitConversion(typeof(int?), typeof(short)), Is.EqualTo(C.ExplicitLiftedNumericConversion));
		}

		[Test]
		public void NullableConversion_BasedOnExplicitEnumerationConversion()
		{
			C c = C.EnumerationConversion(false, true); // c = explicit lifted enumeration conversion
			Assert.That(ExplicitConversion(typeof(int?), typeof(StringComparison?)), Is.EqualTo(c));
			Assert.That(ExplicitConversion(typeof(int), typeof(StringComparison?)), Is.EqualTo(c));
			Assert.That(ExplicitConversion(typeof(int?), typeof(StringComparison)), Is.EqualTo(c));

			Assert.That(ExplicitConversion(typeof(StringComparison?), typeof(int?)), Is.EqualTo(c));
			Assert.That(ExplicitConversion(typeof(StringComparison), typeof(int?)), Is.EqualTo(c));
			Assert.That(ExplicitConversion(typeof(StringComparison?), typeof(int)), Is.EqualTo(c));

			Assert.That(ExplicitConversion(typeof(StringComparison?), typeof(StringSplitOptions?)), Is.EqualTo(c));
			Assert.That(ExplicitConversion(typeof(StringComparison), typeof(StringSplitOptions?)), Is.EqualTo(c));
			Assert.That(ExplicitConversion(typeof(StringComparison?), typeof(StringSplitOptions)), Is.EqualTo(c));
		}

		[Test]
		public void ExplicitReferenceConversion_SealedClass()
		{
			Assert.That(ExplicitConversion(typeof(object), typeof(string)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<char>), typeof(string)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<int>), typeof(string)), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(IEnumerable<object>), typeof(string)), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(string), typeof(IEnumerable<char>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(string), typeof(IEnumerable<int>)), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(string), typeof(IEnumerable<object>)), Is.EqualTo(C.None));
		}

		[Test]
		public void ExplicitReferenceConversion_NonSealedClass()
		{
			Assert.That(ExplicitConversion(typeof(object), typeof(List<string>)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<object>), typeof(List<string>)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<string>), typeof(List<string>)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<int>), typeof(List<string>)), Is.EqualTo(C.ExplicitReferenceConversion));

			Assert.That(ExplicitConversion(typeof(List<string>), typeof(IEnumerable<object>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(List<string>), typeof(IEnumerable<string>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(List<string>), typeof(IEnumerable<int>)), Is.EqualTo(C.ExplicitReferenceConversion));

			Assert.That(ExplicitConversion(typeof(List<string>), typeof(List<object>)), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(List<string>), typeof(List<int>)), Is.EqualTo(C.None));
		}

		[Test]
		public void ExplicitReferenceConversion_Interfaces()
		{
			Assert.That(ExplicitConversion(typeof(IEnumerable<string>), typeof(IEnumerable<object>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<int>), typeof(IEnumerable<object>)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<object>), typeof(IEnumerable<string>)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<object>), typeof(IEnumerable<int>)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<object>), typeof(IConvertible)), Is.EqualTo(C.ExplicitReferenceConversion));
		}

		[Test]
		public void ExplicitReferenceConversion_Arrays()
		{
			Assert.That(ExplicitConversion(typeof(object[]), typeof(string[])), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(dynamic[]), typeof(string[])), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(object[]), typeof(object[,])), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(object[]), typeof(int[])), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(short[]), typeof(int[])), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(Array), typeof(int[])), Is.EqualTo(C.ExplicitReferenceConversion));
		}

		[Test]
		public void ExplicitReferenceConversion_InterfaceToArray()
		{
			Assert.That(ExplicitConversion(typeof(ICloneable), typeof(int[])), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<string>), typeof(string[])), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<object>), typeof(string[])), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<string>), typeof(object[])), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<string>), typeof(dynamic[])), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<int>), typeof(int[])), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<string>), typeof(object[,])), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(IEnumerable<short>), typeof(object[])), Is.EqualTo(C.None));
		}

		[Test]
		public void ExplicitReferenceConversion_ArrayToInterface()
		{
			Assert.That(ExplicitConversion(typeof(int[]), typeof(ICloneable)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(string[]), typeof(IEnumerable<string>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(string[]), typeof(IEnumerable<object>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(object[]), typeof(IEnumerable<string>)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(dynamic[]), typeof(IEnumerable<string>)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(int[]), typeof(IEnumerable<int>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(object[,]), typeof(IEnumerable<string>)), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(object[]), typeof(IEnumerable<short>)), Is.EqualTo(C.None));
		}

		[Test]
		public void ExplicitReferenceConversion_Delegates()
		{
			Assert.That(ExplicitConversion(typeof(MulticastDelegate), typeof(Action)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(Delegate), typeof(Action)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(ICloneable), typeof(Action)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(System.Threading.ThreadStart), typeof(Action)), Is.EqualTo(C.None));
		}

		[Test]
		public void ExplicitReferenceConversion_GenericDelegates()
		{
			Assert.That(ExplicitConversion(typeof(Action<object>), typeof(Action<string>)), Is.EqualTo(C.ImplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(Action<string>), typeof(Action<object>)), Is.EqualTo(C.ExplicitReferenceConversion));

			Assert.That(ExplicitConversion(typeof(Func<object>), typeof(Func<string>)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(Func<string>), typeof(Func<object>)), Is.EqualTo(C.ImplicitReferenceConversion));

			Assert.That(ExplicitConversion(typeof(Action<IFormattable>), typeof(Action<IConvertible>)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(Action<IFormattable>), typeof(Action<int>)), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(Action<string>), typeof(Action<IEnumerable<int>>)), Is.EqualTo(C.ExplicitReferenceConversion));

			Assert.That(ExplicitConversion(typeof(Func<IFormattable>), typeof(Func<IConvertible>)), Is.EqualTo(C.ExplicitReferenceConversion));
			Assert.That(ExplicitConversion(typeof(Func<IFormattable>), typeof(Func<int>)), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(Func<string>), typeof(Func<IEnumerable<int>>)), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(Func<string>), typeof(Func<IEnumerable<int>>)), Is.EqualTo(C.None));
		}

		[Test]
		public void UnboxingConversion()
		{
			Assert.That(ExplicitConversion(typeof(object), typeof(int)), Is.EqualTo(C.UnboxingConversion));
			Assert.That(ExplicitConversion(typeof(object), typeof(decimal)), Is.EqualTo(C.UnboxingConversion));
			Assert.That(ExplicitConversion(typeof(ValueType), typeof(int)), Is.EqualTo(C.UnboxingConversion));
			Assert.That(ExplicitConversion(typeof(IFormattable), typeof(int)), Is.EqualTo(C.UnboxingConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<object>), typeof(int)), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(Enum), typeof(StringComparison)), Is.EqualTo(C.UnboxingConversion));
			Assert.That(ExplicitConversion(typeof(Enum), typeof(int)), Is.EqualTo(C.None));
		}

		[Test]
		public void LiftedUnboxingConversion()
		{
			Assert.That(ExplicitConversion(typeof(object), typeof(int?)), Is.EqualTo(C.UnboxingConversion));
			Assert.That(ExplicitConversion(typeof(object), typeof(decimal?)), Is.EqualTo(C.UnboxingConversion));
			Assert.That(ExplicitConversion(typeof(ValueType), typeof(int?)), Is.EqualTo(C.UnboxingConversion));
			Assert.That(ExplicitConversion(typeof(IFormattable), typeof(int?)), Is.EqualTo(C.UnboxingConversion));
			Assert.That(ExplicitConversion(typeof(IEnumerable<object>), typeof(int?)), Is.EqualTo(C.None));
			Assert.That(ExplicitConversion(typeof(Enum), typeof(StringComparison?)), Is.EqualTo(C.UnboxingConversion));
			Assert.That(ExplicitConversion(typeof(Enum), typeof(int?)), Is.EqualTo(C.None));
		}

		/// <summary>
		/// Converts a constant expression (e.g. an integer literal) to the target type.
		/// </summary>
		Conversion ExplicitConstantConversion(object value, Type to)
		{
			IType fromType = compilation.FindType(value.GetType());
			IType to2 = compilation.FindType(to).AcceptVisitor(new ConversionTest.ReplaceSpecialTypesVisitor());
			return conversions.ExplicitConversion(new ConstantResolveResult(fromType, value), to2);
		}

		[Test]
		public void ObjectToTypeParameter()
		{
			// void M<T>(object o) { T t = (T)o; }
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			Assert.That(conversions.ExplicitConversion(compilation.FindType(KnownTypeCode.Object), t), Is.EqualTo(C.UnboxingConversion));
		}

		[Test]
		public void UnrelatedClassToTypeParameter()
		{
			// void M<T>(string o) { T t = (T)o; }
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			Assert.That(conversions.ExplicitConversion(compilation.FindType(KnownTypeCode.String), t), Is.EqualTo(C.None));
		}

		[Test]
		public void IntefaceToTypeParameter()
		{
			// void M<T>(IDisposable o) { T t = (T)o; }
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			Assert.That(conversions.ExplicitConversion(compilation.FindType(typeof(IDisposable)), t), Is.EqualTo(C.UnboxingConversion));
		}

		[Test]
		public void TypeParameterToInterface()
		{
			// void M<T>(T t) { IDisposable d = (IDisposable)t; }
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			Assert.That(conversions.ExplicitConversion(t, compilation.FindType(typeof(IDisposable))), Is.EqualTo(C.BoxingConversion));
		}

		[Test]
		public void ValueTypeToTypeParameter()
		{
			// void M<T>(ValueType o) where T : struct { T t = (T)o; }
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T", hasValueTypeConstraint: true);
			Assert.That(conversions.ExplicitConversion(compilation.FindType(typeof(ValueType)), t), Is.EqualTo(C.UnboxingConversion));
		}

		[Test]
		public void InvalidTypeParameterConversion()
		{
			// void M<T, U>(T t) { U u = (U)t; }
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U");
			Assert.That(conversions.ExplicitConversion(t, u), Is.EqualTo(C.None));
		}

		[Test]
		public void TypeParameterConversion1()
		{
			// void M<T, U>(T t) where T : U { U u = (U)t; }
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U");
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T", constraints: new[] { u });
			Assert.That(conversions.ExplicitConversion(t, u), Is.EqualTo(C.BoxingConversion));
		}

		[Test]
		public void TypeParameterConversion1Array()
		{
			// void M<T, U>(T[] t) where T : U { U[] u = (U[])t; }
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U");
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T", constraints: new[] { u });
			Assert.That(conversions.ExplicitConversion(new ArrayType(compilation, t), new ArrayType(compilation, u)), Is.EqualTo(C.None));
		}

		[Test]
		public void TypeParameterConversion2()
		{
			// void M<T, U>(T t) where U : T { U u = (U)t; }
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U", constraints: new[] { t });
			Assert.That(conversions.ExplicitConversion(t, u), Is.EqualTo(C.UnboxingConversion));
		}

		[Test]
		public void TypeParameterConversion2Array()
		{
			// void M<T, U>(T[] t) where U : T { U[] u = (U[])t; }
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U", constraints: new[] { t });
			Assert.That(conversions.ExplicitConversion(new ArrayType(compilation, t), new ArrayType(compilation, u)), Is.EqualTo(C.None));
		}

		[Test]
		public void ImplicitTypeParameterConversionWithClassConstraint()
		{
			// void M<T, U>(T t) where T : class where U : class, T { U u = (U)t; }
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T", hasReferenceTypeConstraint: true);
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U", hasReferenceTypeConstraint: true, constraints: new[] { t });
			Assert.That(conversions.ExplicitConversion(t, u), Is.EqualTo(C.ExplicitReferenceConversion));
		}

		[Test]
		public void ImplicitTypeParameterArrayConversionWithClassConstraint()
		{
			// void M<T, U>(T[] t) where T : class where U : class, T { U[] u = (U[])t; }
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T", hasReferenceTypeConstraint: true);
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U", hasReferenceTypeConstraint: true, constraints: new[] { t });
			Assert.That(conversions.ExplicitConversion(new ArrayType(compilation, t), new ArrayType(compilation, u)), Is.EqualTo(C.ExplicitReferenceConversion));
		}

		[Test]
		public void ImplicitTypeParameterConversionWithClassConstraintOnlyOnT()
		{
			// void M<T, U>(T t) where U : class, T { U u = (U)t; }
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U", hasReferenceTypeConstraint: true, constraints: new[] { t });
			Assert.That(conversions.ExplicitConversion(t, u), Is.EqualTo(C.ExplicitReferenceConversion));
		}

		[Test]
		public void ImplicitTypeParameterArrayConversionWithClassConstraintOnlyOnT()
		{
			// void M<T, U>(T[] t) where U : class, T { U[] u = (U[])t; }
			ITypeParameter t = new DefaultTypeParameter(compilation, SymbolKind.Method, 0, "T");
			ITypeParameter u = new DefaultTypeParameter(compilation, SymbolKind.Method, 1, "U", hasReferenceTypeConstraint: true, constraints: new[] { t });
			Assert.That(conversions.ExplicitConversion(new ArrayType(compilation, t), new ArrayType(compilation, u)), Is.EqualTo(C.ExplicitReferenceConversion));
		}

		[Test]
		public void SimpleUserDefinedConversion()
		{
			// C1 c1 = (C1)c2;  with  explicit operator C1(C2 c2)
			var c = ExplicitConversion(typeof(UserDefinedExplicitConversionTestCases.SimpleSource),
				typeof(UserDefinedExplicitConversionTestCases.SimpleTarget));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Name, Is.EqualTo("op_Explicit"));
		}

		[Test]
		public void ExplicitReferenceConversionFollowedByUserDefinedConversion()
		{
			// class S : B, explicit operator T(S s);
			// T t = (T)b;  with b of type B needs the explicit reference conversion B -> S first
			var c = ExplicitConversion(typeof(UserDefinedExplicitConversionTestCases.BaseClass),
				typeof(UserDefinedExplicitConversionTestCases.TFromDerived));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
		}

		[Test]
		public void ImplicitUserDefinedConversionFollowedByExplicitNumericConversion()
		{
			// struct T { implicit operator float(T t) }
			// int x = (int)t;
			var c = ExplicitConversion(typeof(UserDefinedExplicitConversionTestCases.TImplicitToFloat), typeof(int));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			// even though the user-defined conversion is implicit, the combined conversion is explicit
			Assert.That(c.IsExplicit);
		}

		[Test]
		public void BothDirectConversionAndBaseClassConversionAvailable()
		{
			// class S : B, T with explicit operators from S ("s") and from B ("b");
			// T t = (T)b;  picks the operator from B
			var c = ExplicitConversion(typeof(UserDefinedExplicitConversionTestCases.BaseClass),
				typeof(UserDefinedExplicitConversionTestCases.TFromDerivedOrBase));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters.Single().Name, Is.EqualTo("b"));
		}

		[Test]
		public void UserDefinedExplicitConversion_PicksExactSourceTypeIfPossible()
		{
			// explicit operators from int ("i") and short ("s");
			// (Convertible)33
			var c = ExplicitConstantConversion(33, typeof(UserDefinedExplicitConversionTestCases.ExplicitFromIntOrShort));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("i"));
		}

		[Test]
		public void UserDefinedExplicitConversion_PicksMostEncompassedSourceTypeIfPossible()
		{
			// explicit operators from long ("l") and uint ("ui");
			// (Convertible)(ushort)33
			var c = ExplicitConstantConversion((ushort)33, typeof(UserDefinedExplicitConversionTestCases.ExplicitFromLongOrUInt));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("ui"));
		}

		[Test]
		public void UserDefinedExplicitConversion_PicksMostEncompassingSourceType()
		{
			// explicit operators from int ("i") and ushort ("us");
			// (Convertible)(long)33
			var c = ExplicitConstantConversion((long)33, typeof(UserDefinedExplicitConversionTestCases.ExplicitFromIntOrUShort));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("i"));
		}

		[Test]
		public void UserDefinedExplicitConversion_NoMostEncompassingSourceTypeIsInvalid()
		{
			// explicit operators from uint and short; neither source type encompasses the
			// other, so the conversion from long is ambiguous.
			var c = ExplicitConstantConversion((long)33, typeof(UserDefinedExplicitConversionTestCases.ExplicitFromUIntOrShort));
			Assert.That(!c.IsValid);
		}

		[Test]
		public void UserDefinedExplicitConversion_PicksExactTargetTypeIfPossible()
		{
			// explicit operators to int ("i") and short ("s");
			// (int)new Convertible()
			var c = ExplicitConversion(typeof(UserDefinedExplicitConversionTestCases.ExplicitToIntOrShort), typeof(int));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("i"));
		}

		[Test]
		public void UserDefinedExplicitConversion_PicksMostEncompassingTargetTypeIfPossible()
		{
			// explicit operators to int ("i") and ushort ("us");
			// (ulong)new Convertible()
			var c = ExplicitConversion(typeof(UserDefinedExplicitConversionTestCases.ExplicitToIntOrUShort), typeof(ulong));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("us"));
		}

		[Test]
		public void UserDefinedExplicitConversion_PicksMostEncompassedTargetType()
		{
			// explicit operators to long ("l") and uint ("ui");
			// (ushort)new Convertible()
			var c = ExplicitConversion(typeof(UserDefinedExplicitConversionTestCases.ExplicitToLongOrUInt), typeof(ushort));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("ui"));
		}

		[Test]
		public void UserDefinedExplicitConversion_NoMostEncompassedTargetTypeIsInvalid()
		{
			// explicit operators to ulong and int; neither target type encompasses the
			// other, so the conversion to ushort is ambiguous.
			var c = ExplicitConversion(typeof(UserDefinedExplicitConversionTestCases.ExplicitToULongOrInt), typeof(ushort));
			Assert.That(!c.IsValid);
		}

		[Test]
		public void UserDefinedExplicitConversion_AmbiguousIsInvalid()
		{
			// Both ExplicitAmbiguousA and ExplicitAmbiguousB declare
			// explicit operator ExplicitAmbiguousB(ExplicitAmbiguousA).
			var c = ExplicitConversion(typeof(UserDefinedExplicitConversionTestCases.ExplicitAmbiguousA),
				typeof(UserDefinedExplicitConversionTestCases.ExplicitAmbiguousB));
			Assert.That(!c.IsValid);
		}

		[Test]
		public void UserDefinedExplicitConversion_Lifted()
		{
			// struct Convertible { explicit operator Convertible(int i) }
			// (Convertible?)i  with i of type int?
			var c = ExplicitConversion(typeof(int?), typeof(UserDefinedExplicitConversionTestCases.ExplicitFromInt?));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.IsLifted);
		}

		[Test]
		public void UserDefinedExplicitConversionFollowedByImplicitNullableConversion()
		{
			// struct Convertible { explicit operator Convertible(int i) }
			// (Convertible?)i  with i of type int
			var c = ExplicitConversion(typeof(int), typeof(UserDefinedExplicitConversionTestCases.ExplicitFromInt?));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(!c.IsLifted);
		}

		[Test]
		public void UserDefinedExplicitConversion_ExplicitNullable_ThenUserDefined()
		{
			// struct Convertible with explicit operators from int ("i") and from int? ("ni");
			// (Convertible)i  with i of type int? unwraps the nullable and uses the int operator
			var c = ExplicitConversion(typeof(int?), typeof(UserDefinedExplicitConversionTestCases.ExplicitNullableConvertible));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(!c.IsLifted);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("i"));
		}

		[Test]
		public void UserDefinedExplicitConversion_DefinedNullableTakesPrecedenceOverLifted()
		{
			// struct Convertible with explicit operators from int ("i") and from int? ("ni");
			// (Convertible?)(int?)33  -- the user-defined nullable operator wins over the
			// lifted form of the int operator.
			var c = ExplicitConversion(typeof(int?), typeof(UserDefinedExplicitConversionTestCases.ExplicitNullableConvertible?));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(!c.IsLifted);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("ni"));
		}

		[Test]
		public void UserDefinedExplicitConversion_UIntConstant()
		{
			// explicit operators from long ("l") and uint ("ui");
			// (Convertible)33  -- the constant 33 converts to uint, which is more specific
			var c = ExplicitConstantConversion(33, typeof(UserDefinedExplicitConversionTestCases.ExplicitFromLongOrUInt));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("ui"));
		}

		[Test]
		public void UserDefinedExplicitConversion_NullableUIntConstant()
		{
			// explicit operators from long? ("l") and uint? ("ui");
			// (Convertible)33
			var c = ExplicitConstantConversion(33, typeof(UserDefinedExplicitConversionTestCases.ExplicitFromNullableLongOrNullableUInt));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("ui"));
		}

		[Test]
		public void UseDefinedExplicitConversion_Lifted()
		{
			// Same conversion as UserDefinedExplicitConversion_Lifted, but through the
			// ResolveResult-based entry point (the original test cast a local variable).
			var c = conversions.ExplicitConversion(
				new ResolveResult(compilation.FindType(typeof(int?))),
				compilation.FindType(typeof(UserDefinedExplicitConversionTestCases.ExplicitFromInt?)));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.IsLifted);
		}

		[Test]
		public void UserDefinedExplicitConversion_Short_Or_NullableByte_Target()
		{
			// explicit operators to short ("s") and byte? ("b");
			// (int?)new Test()
			var c = ExplicitConversion(typeof(UserDefinedExplicitConversionTestCases.ExplicitToShortOrNullableByte), typeof(int?));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.ReturnType.FullName, Is.EqualTo("System.Int16"));
		}

		[Test]
		public void UserDefinedExplicitConversion_Byte_Or_NullableShort_Target()
		{
			// explicit operators to byte ("b") and short? ("s");
			// (int?)new Test()
			var c = ExplicitConversion(typeof(UserDefinedExplicitConversionTestCases.ExplicitToByteOrNullableShort), typeof(int?));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("s"));
		}

		[Test]
		public void ExplicitConversionOperatorsCanOverrideApplicableImplicitOnes()
		{
			// struct Convertible { explicit operator int(Convertible ci); implicit operator short(Convertible cs); }
			// int i = (int)new Convertible();  -- csc uses the explicit conversion operator
			var c = ExplicitConversion(typeof(UserDefinedExplicitConversionTestCases.ExplicitIntImplicitShort), typeof(int));
			Assert.That(c.IsValid);
			Assert.That(c.IsUserDefined);
			Assert.That(c.Method.Parameters[0].Name, Is.EqualTo("ci"));
		}

		[Test]
		public void UserDefinedExplicitConversion_ConversionBeforeUserDefinedOperatorIsCorrect()
		{
			// implicit operator Convertible(int l); casting a long goes long -> int -> Convertible,
			// with an explicit numeric conversion before the operator.
			var c = ExplicitConversion(typeof(long), typeof(UserDefinedExplicitConversionTestCases.ImplicitFromInt));
			Assert.That(c.IsValid);
			Assert.That(c.ConversionBeforeUserDefinedOperator.IsValid);
			Assert.That(c.ConversionBeforeUserDefinedOperator.IsExplicit);
			Assert.That(c.ConversionBeforeUserDefinedOperator.IsNumericConversion);
			Assert.That(c.ConversionAfterUserDefinedOperator.IsIdentityConversion);
		}

		[Test]
		public void UserDefinedExplicitConversion_ConversionAfterUserDefinedOperatorIsCorrect()
		{
			// implicit operator long(Convertible i); casting to int goes Convertible -> long -> int,
			// with an explicit numeric conversion after the operator.
			var c = ExplicitConversion(typeof(UserDefinedExplicitConversionTestCases.ImplicitToLong), typeof(int));
			Assert.That(c.IsValid);
			Assert.That(c.ConversionBeforeUserDefinedOperator.IsIdentityConversion);
			Assert.That(c.ConversionAfterUserDefinedOperator.IsValid);
			Assert.That(c.ConversionAfterUserDefinedOperator.IsExplicit);
			Assert.That(c.ConversionAfterUserDefinedOperator.IsNumericConversion);
		}
	}
}
