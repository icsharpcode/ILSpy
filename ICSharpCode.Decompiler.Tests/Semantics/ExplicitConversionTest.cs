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
	using dynamic = ICSharpCode.Decompiler.TypeSystem.ReflectionHelper.Dynamic;

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
			IType from2 = compilation.FindType(from);
			IType to2 = compilation.FindType(to);
			return conversions.ExplicitConversion(from2, to2);
		}

		[Test]
		public void PointerConversion()
		{
			Assert.AreEqual(C.ExplicitPointerConversion, ExplicitConversion(typeof(int*), typeof(short)));
			Assert.AreEqual(C.ExplicitPointerConversion, ExplicitConversion(typeof(short), typeof(void*)));

			Assert.AreEqual(C.ExplicitPointerConversion, ExplicitConversion(typeof(void*), typeof(int*)));
			Assert.AreEqual(C.ExplicitPointerConversion, ExplicitConversion(typeof(long*), typeof(byte*)));
		}

		[Test]
		public void ConversionFromDynamic()
		{
			// Explicit dynamic conversion is for resolve results only;
			// otherwise it's an explicit reference / unboxing conversion
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(dynamic), typeof(string)));
			Assert.AreEqual(C.UnboxingConversion, ExplicitConversion(typeof(dynamic), typeof(int)));

			var dynamicRR = new ResolveResult(SpecialType.Dynamic);
			Assert.AreEqual(C.ExplicitDynamicConversion, conversions.ExplicitConversion(dynamicRR, compilation.FindType(typeof(string))));
			Assert.AreEqual(C.ExplicitDynamicConversion, conversions.ExplicitConversion(dynamicRR, compilation.FindType(typeof(int))));
		}

		[Test]
		public void NumericConversions()
		{
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(sbyte), typeof(uint)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(sbyte), typeof(char)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(byte), typeof(char)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(byte), typeof(sbyte)));
			// if an implicit conversion exists, ExplicitConversion() should return that
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(byte), typeof(int)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(double), typeof(float)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(double), typeof(decimal)));
			Assert.AreEqual(C.ExplicitNumericConversion, ExplicitConversion(typeof(decimal), typeof(double)));
			Assert.AreEqual(C.ImplicitNumericConversion, ExplicitConversion(typeof(int), typeof(decimal)));

			Assert.AreEqual(C.None, ExplicitConversion(typeof(bool), typeof(int)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(int), typeof(bool)));
		}

		[Test]
		public void EnumerationConversions()
		{
			var explicitEnumerationConversion = C.EnumerationConversion(false, false);
			Assert.AreEqual(explicitEnumerationConversion, ExplicitConversion(typeof(sbyte), typeof(StringComparison)));
			Assert.AreEqual(explicitEnumerationConversion, ExplicitConversion(typeof(char), typeof(StringComparison)));
			Assert.AreEqual(explicitEnumerationConversion, ExplicitConversion(typeof(int), typeof(StringComparison)));
			Assert.AreEqual(explicitEnumerationConversion, ExplicitConversion(typeof(decimal), typeof(StringComparison)));
			Assert.AreEqual(explicitEnumerationConversion, ExplicitConversion(typeof(StringComparison), typeof(char)));
			Assert.AreEqual(explicitEnumerationConversion, ExplicitConversion(typeof(StringComparison), typeof(int)));
			Assert.AreEqual(explicitEnumerationConversion, ExplicitConversion(typeof(StringComparison), typeof(decimal)));
			Assert.AreEqual(explicitEnumerationConversion, ExplicitConversion(typeof(StringComparison), typeof(StringSplitOptions)));
		}

		[Test]
		public void NullableConversion_BasedOnIdentityConversion()
		{
			Assert.AreEqual(C.IdentityConversion, ExplicitConversion(typeof(ArraySegment<dynamic>?), typeof(ArraySegment<object>?)));
			Assert.AreEqual(C.ImplicitNullableConversion, ExplicitConversion(typeof(ArraySegment<dynamic>), typeof(ArraySegment<object>?)));
			Assert.AreEqual(C.ExplicitNullableConversion, ExplicitConversion(typeof(ArraySegment<dynamic>?), typeof(ArraySegment<object>)));
		}

		[Test]
		public void NullableConversion_BasedOnImplicitNumericConversion()
		{
			Assert.AreEqual(C.ImplicitLiftedNumericConversion, ExplicitConversion(typeof(int?), typeof(long?)));
			Assert.AreEqual(C.ImplicitLiftedNumericConversion, ExplicitConversion(typeof(int), typeof(long?)));
			Assert.AreEqual(C.ExplicitLiftedNumericConversion, ExplicitConversion(typeof(int?), typeof(long)));
		}

		[Test]
		public void NullableConversion_BasedOnImplicitEnumerationConversion()
		{
			ResolveResult zero = new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), 0);
			ResolveResult one = new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), 1);
			Assert.AreEqual(C.EnumerationConversion(true, true), conversions.ExplicitConversion(zero, compilation.FindType(typeof(StringComparison?))));
			Assert.AreEqual(C.EnumerationConversion(false, true), conversions.ExplicitConversion(one, compilation.FindType(typeof(StringComparison?))));
		}

		[Test]
		public void NullableConversion_BasedOnExplicitNumericConversion()
		{
			Assert.AreEqual(C.ExplicitLiftedNumericConversion, ExplicitConversion(typeof(int?), typeof(short?)));
			Assert.AreEqual(C.ExplicitLiftedNumericConversion, ExplicitConversion(typeof(int), typeof(short?)));
			Assert.AreEqual(C.ExplicitLiftedNumericConversion, ExplicitConversion(typeof(int?), typeof(short)));
		}

		[Test]
		public void NullableConversion_BasedOnExplicitEnumerationConversion()
		{
			C c = C.EnumerationConversion(false, true); // c = explicit lifted enumeration conversion
			Assert.AreEqual(c, ExplicitConversion(typeof(int?), typeof(StringComparison?)));
			Assert.AreEqual(c, ExplicitConversion(typeof(int), typeof(StringComparison?)));
			Assert.AreEqual(c, ExplicitConversion(typeof(int?), typeof(StringComparison)));

			Assert.AreEqual(c, ExplicitConversion(typeof(StringComparison?), typeof(int?)));
			Assert.AreEqual(c, ExplicitConversion(typeof(StringComparison), typeof(int?)));
			Assert.AreEqual(c, ExplicitConversion(typeof(StringComparison?), typeof(int)));

			Assert.AreEqual(c, ExplicitConversion(typeof(StringComparison?), typeof(StringSplitOptions?)));
			Assert.AreEqual(c, ExplicitConversion(typeof(StringComparison), typeof(StringSplitOptions?)));
			Assert.AreEqual(c, ExplicitConversion(typeof(StringComparison?), typeof(StringSplitOptions)));
		}

		[Test]
		public void ExplicitReferenceConversion_SealedClass()
		{
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(object), typeof(string)));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(IEnumerable<char>), typeof(string)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(IEnumerable<int>), typeof(string)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(IEnumerable<object>), typeof(string)));
			Assert.AreEqual(C.ImplicitReferenceConversion, ExplicitConversion(typeof(string), typeof(IEnumerable<char>)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(string), typeof(IEnumerable<int>)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(string), typeof(IEnumerable<object>)));
		}

		[Test]
		public void ExplicitReferenceConversion_NonSealedClass()
		{
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(object), typeof(List<string>)));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(IEnumerable<object>), typeof(List<string>)));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(IEnumerable<string>), typeof(List<string>)));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(IEnumerable<int>), typeof(List<string>)));

			Assert.AreEqual(C.ImplicitReferenceConversion, ExplicitConversion(typeof(List<string>), typeof(IEnumerable<object>)));
			Assert.AreEqual(C.ImplicitReferenceConversion, ExplicitConversion(typeof(List<string>), typeof(IEnumerable<string>)));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(List<string>), typeof(IEnumerable<int>)));

			Assert.AreEqual(C.None, ExplicitConversion(typeof(List<string>), typeof(List<object>)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(List<string>), typeof(List<int>)));
		}

		[Test]
		public void ExplicitReferenceConversion_Interfaces()
		{
			Assert.AreEqual(C.ImplicitReferenceConversion, ExplicitConversion(typeof(IEnumerable<string>), typeof(IEnumerable<object>)));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(IEnumerable<int>), typeof(IEnumerable<object>)));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(IEnumerable<object>), typeof(IEnumerable<string>)));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(IEnumerable<object>), typeof(IEnumerable<int>)));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(IEnumerable<object>), typeof(IConvertible)));
		}

		[Test]
		public void ExplicitReferenceConversion_Arrays()
		{
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(object[]), typeof(string[])));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(dynamic[]), typeof(string[])));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(object[]), typeof(object[,])));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(object[]), typeof(int[])));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(short[]), typeof(int[])));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(Array), typeof(int[])));
		}

		[Test]
		public void ExplicitReferenceConversion_InterfaceToArray()
		{
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(ICloneable), typeof(int[])));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(IEnumerable<string>), typeof(string[])));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(IEnumerable<object>), typeof(string[])));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(IEnumerable<string>), typeof(object[])));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(IEnumerable<string>), typeof(dynamic[])));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(IEnumerable<int>), typeof(int[])));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(IEnumerable<string>), typeof(object[,])));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(IEnumerable<short>), typeof(object[])));
		}

		[Test]
		public void ExplicitReferenceConversion_ArrayToInterface()
		{
			Assert.AreEqual(C.ImplicitReferenceConversion, ExplicitConversion(typeof(int[]), typeof(ICloneable)));
			Assert.AreEqual(C.ImplicitReferenceConversion, ExplicitConversion(typeof(string[]), typeof(IEnumerable<string>)));
			Assert.AreEqual(C.ImplicitReferenceConversion, ExplicitConversion(typeof(string[]), typeof(IEnumerable<object>)));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(object[]), typeof(IEnumerable<string>)));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(dynamic[]), typeof(IEnumerable<string>)));
			Assert.AreEqual(C.ImplicitReferenceConversion, ExplicitConversion(typeof(int[]), typeof(IEnumerable<int>)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(object[,]), typeof(IEnumerable<string>)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(object[]), typeof(IEnumerable<short>)));
		}

		[Test]
		public void ExplicitReferenceConversion_Delegates()
		{
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(MulticastDelegate), typeof(Action)));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(Delegate), typeof(Action)));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(ICloneable), typeof(Action)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(System.Threading.ThreadStart), typeof(Action)));
		}

		[Test]
		public void ExplicitReferenceConversion_GenericDelegates()
		{
			Assert.AreEqual(C.ImplicitReferenceConversion, ExplicitConversion(typeof(Action<object>), typeof(Action<string>)));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(Action<string>), typeof(Action<object>)));

			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(Func<object>), typeof(Func<string>)));
			Assert.AreEqual(C.ImplicitReferenceConversion, ExplicitConversion(typeof(Func<string>), typeof(Func<object>)));

			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(Action<IFormattable>), typeof(Action<IConvertible>)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(Action<IFormattable>), typeof(Action<int>)));
			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(Action<string>), typeof(Action<IEnumerable<int>>)));

			Assert.AreEqual(C.ExplicitReferenceConversion, ExplicitConversion(typeof(Func<IFormattable>), typeof(Func<IConvertible>)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(Func<IFormattable>), typeof(Func<int>)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(Func<string>), typeof(Func<IEnumerable<int>>)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(Func<string>), typeof(Func<IEnumerable<int>>)));
		}

		[Test]
		public void UnboxingConversion()
		{
			Assert.AreEqual(C.UnboxingConversion, ExplicitConversion(typeof(object), typeof(int)));
			Assert.AreEqual(C.UnboxingConversion, ExplicitConversion(typeof(object), typeof(decimal)));
			Assert.AreEqual(C.UnboxingConversion, ExplicitConversion(typeof(ValueType), typeof(int)));
			Assert.AreEqual(C.UnboxingConversion, ExplicitConversion(typeof(IFormattable), typeof(int)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(IEnumerable<object>), typeof(int)));
			Assert.AreEqual(C.UnboxingConversion, ExplicitConversion(typeof(Enum), typeof(StringComparison)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(Enum), typeof(int)));
		}

		[Test]
		public void LiftedUnboxingConversion()
		{
			Assert.AreEqual(C.UnboxingConversion, ExplicitConversion(typeof(object), typeof(int?)));
			Assert.AreEqual(C.UnboxingConversion, ExplicitConversion(typeof(object), typeof(decimal?)));
			Assert.AreEqual(C.UnboxingConversion, ExplicitConversion(typeof(ValueType), typeof(int?)));
			Assert.AreEqual(C.UnboxingConversion, ExplicitConversion(typeof(IFormattable), typeof(int?)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(IEnumerable<object>), typeof(int?)));
			Assert.AreEqual(C.UnboxingConversion, ExplicitConversion(typeof(Enum), typeof(StringComparison?)));
			Assert.AreEqual(C.None, ExplicitConversion(typeof(Enum), typeof(int?)));
		}

		/* TODO: we should probably revive these tests somehow
		Conversion ResolveCast(string program)
		{
			return Resolve<ConversionResolveResult>(program).Conversion;
		}

		[Test]
		public void ObjectToTypeParameter()
		{
			string program = @"using System;
class Test {
	public void M<T>(object o) {
		T t = $(T)o$;
	}
}";
			Assert.AreEqual(C.UnboxingConversion, ResolveCast(program));
		}

		[Test]
		public void UnrelatedClassToTypeParameter()
		{
			string program = @"using System;
class Test {
	public void M<T>(string o) {
		T t = $(T)o$;
	}
}";
			Assert.AreEqual(C.None, ResolveCast(program));
		}

		[Test]
		public void IntefaceToTypeParameter()
		{
			string program = @"using System;
class Test {
	public void M<T>(IDisposable o) {
		T t = $(T)o$;
	}
}";
			Assert.AreEqual(C.UnboxingConversion, ResolveCast(program));
		}

		[Test]
		public void TypeParameterToInterface()
		{
			string program = @"using System;
class Test {
	public void M<T>(T t) {
		IDisposable d = $(IDisposable)t$;
	}
}";
			Assert.AreEqual(C.BoxingConversion, ResolveCast(program));
		}

		[Test]
		public void ValueTypeToTypeParameter()
		{
			string program = @"using System;
class Test {
	public void M<T>(ValueType o) where T : struct {
		T t = $(T)o$;
	}
}";
			Assert.AreEqual(C.UnboxingConversion, ResolveCast(program));
		}

		[Test]
		public void InvalidTypeParameterConversion()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T t) {
		U u = $(U)t$;
	}
}";
			Assert.AreEqual(C.None, ResolveCast(program));
		}

		[Test]
		public void TypeParameterConversion1()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T t) where T : U {
		U u = $(U)t$;
	}
}";
			Assert.AreEqual(C.BoxingConversion, ResolveCast(program));
		}

		[Test]
		public void TypeParameterConversion1Array()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T[] t) where T : U {
		U[] u = $(U[])t$;
	}
}";
			Assert.AreEqual(C.None, ResolveCast(program));
		}

		[Test]
		public void TypeParameterConversion2()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T t) where U : T {
		U u = $(U)t$;
	}
}";
			Assert.AreEqual(C.UnboxingConversion, ResolveCast(program));
		}

		[Test]
		public void TypeParameterConversion2Array()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T[] t) where U : T {
		U[] u = $(U[])t$;
	}
}";
			Assert.AreEqual(C.None, ResolveCast(program));
		}

		[Test]
		public void ImplicitTypeParameterConversionWithClassConstraint()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T t) where T : class where U : class, T {
		U u = $(U)t$;
	}
}";
			Assert.AreEqual(C.ExplicitReferenceConversion, ResolveCast(program));
		}

		[Test]
		public void ImplicitTypeParameterArrayConversionWithClassConstraint()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T[] t) where T : class where U : class, T {
		U[] u = $(U[])t$;
	}
}";
			Assert.AreEqual(C.ExplicitReferenceConversion, ResolveCast(program));
		}

		[Test]
		public void ImplicitTypeParameterConversionWithClassConstraintOnlyOnT()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T t) where U : class, T {
		U u = $(U)t$;
	}
}";
			Assert.AreEqual(C.ExplicitReferenceConversion, ResolveCast(program));
		}

		[Test]
		public void ImplicitTypeParameterArrayConversionWithClassConstraintOnlyOnT()
		{
			string program = @"using System;
class Test {
	public void M<T, U>(T[] t) where U : class, T {
		U[] u = $(U[])t$;
	}
}";
			Assert.AreEqual(C.ExplicitReferenceConversion, ResolveCast(program));
		}

		[Test]
		public void SimpleUserDefinedConversion()
		{
			var rr = Resolve<ConversionResolveResult>(@"
class C1 {}
class C2 {
	public static explicit operator C1(C2 c2) {
		return null;
	}
}
class C {
	public void M() {
		var c2 = new C2();
		C1 c1 = $(C1)c2$;
	}
}");
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.AreEqual("op_Explicit", rr.Conversion.Method.Name);
		}

		[Test]
		public void ExplicitReferenceConversionFollowedByUserDefinedConversion()
		{
			var rr = Resolve<ConversionResolveResult>(@"
		class B {}
		class S : B {}
		class T {
			public static explicit operator T(S s) { return null; }
		}
		class Test {
			void Run(B b) {
				T t = $(T)b$;
			}
		}");
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.AreEqual("B", rr.Input.Type.Name);
		}

		[Test]
		public void ImplicitUserDefinedConversionFollowedByExplicitNumericConversion()
		{
			var rr = Resolve<ConversionResolveResult>(@"
		struct T {
			public static implicit operator float(T t) { return 0; }
		}
		class Test {
			void Run(T t) {
				int x = $(int)t$;
			}
		}");
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			// even though the user-defined conversion is implicit, the combined conversion is explicit
			Assert.IsTrue(rr.Conversion.IsExplicit);
		}

		[Test]
		public void BothDirectConversionAndBaseClassConversionAvailable()
		{
			var rr = Resolve<ConversionResolveResult>(@"
		class B {}
		class S : B {}
		class T {
			public static explicit operator T(S s) { return null; }
			public static explicit operator T(B b) { return null; }
		}
		class Test {
			void Run(B b) {
				T t = $(T)b$;
			}
		}");
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.AreEqual("b", rr.Conversion.Method.Parameters.Single().Name);
		}

		[Test]
		public void UserDefinedExplicitConversion_PicksExactSourceTypeIfPossible()
		{
			string program = @"using System;
class Convertible {
	public static explicit operator Convertible(int i) {return new Convertible(); }
	public static explicit operator Convertible(short s) {return new Convertible(); }
}
class Test {
	public void M() {
		var a = $(Convertible)33$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.AreEqual("i", rr.Conversion.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedExplicitConversion_PicksMostEncompassedSourceTypeIfPossible()
		{
			string program = @"using System;
class Convertible {
	public static explicit operator Convertible(long l) {return new Convertible(); }
	public static explicit operator Convertible(uint ui) {return new Convertible(); }
}
class Test {
	public void M() {
		var a = $(Convertible)(ushort)33$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.AreEqual("ui", rr.Conversion.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedExplicitConversion_PicksMostEncompassingSourceType()
		{
			string program = @"using System;
class Convertible {
	public static explicit operator Convertible(int i) {return new Convertible(); }
	public static explicit operator Convertible(ushort us) {return new Convertible(); }
}
class Test {
	public void M() {
		var a = $(Convertible)(long)33$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.AreEqual("i", rr.Conversion.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedExplicitConversion_NoMostEncompassingSourceTypeIsInvalid()
		{
			string program = @"using System;
class Convertible {
	public static explicit operator Convertible(uint i) {return new Convertible(); }
	public static explicit operator Convertible(short us) {return new Convertible(); }
}
class Test {
	public void M() {
		var a = $(Convertible)(long)33$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsFalse(rr.Conversion.IsValid);
		}

		[Test]
		public void UserDefinedExplicitConversion_PicksExactTargetTypeIfPossible()
		{
			string program = @"using System;
class Convertible {
	public static explicit operator int(Convertible i) {return 0; }
	public static explicit operator short(Convertible s) {return 0; }
}
class Test {
	public void M() {
		var a = $(int)new Convertible()$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.AreEqual("i", rr.Conversion.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedExplicitConversion_PicksMostEncompassingTargetTypeIfPossible()
		{
			string program = @"using System;
class Convertible {
	public static explicit operator int(Convertible i) {return 0; }
	public static explicit operator ushort(Convertible us) {return 0; }
}
class Test {
	public void M() {
		var a = $(ulong)new Convertible()$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.AreEqual("us", rr.Conversion.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedExplicitConversion_PicksMostEncompassedTargetType()
		{
			string program = @"using System;
class Convertible {
	public static explicit operator long(Convertible l) { return 0; }
	public static explicit operator uint(Convertible ui) { return 0; }
}
class Test {
	public void M() {
		var a = $(ushort)new Convertible()$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.AreEqual("ui", rr.Conversion.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedExplicitConversion_NoMostEncompassedTargetTypeIsInvalid()
		{
			string program = @"using System;
class Convertible {
	public static explicit operator ulong(Convertible l) { return 0; }
	public static explicit operator int(Convertible ui) { return 0; }
}
class Test {
	public void M() {
		var a = $(ushort)new Convertible()$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsFalse(rr.Conversion.IsValid);
		}

		[Test]
		public void UserDefinedExplicitConversion_AmbiguousIsInvalid()
		{
			string program = @"using System;
class Convertible1 {
	public static explicit operator Convertible2(Convertible1 c) {return 0; }
}
class Convertible2 {
	public static explicit operator Convertible2(Convertible1 c) {return 0; }
}
class Test {
	public void M() {
		var a = $(Convertible2)new Convertible1()$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsFalse(rr.Conversion.IsValid);
		}

		[Test]
		public void UserDefinedExplicitConversion_Lifted()
		{
			string program = @"using System;
struct Convertible {
	public static explicit operator Convertible(int i) {return new Convertible(); }
}
class Test {
	public void M(int? i) {
		 a = $(Convertible?)i$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.IsTrue(rr.Conversion.IsLifted);
		}

		[Test]
		public void UserDefinedExplicitConversionFollowedByImplicitNullableConversion()
		{
			string program = @"using System;
struct Convertible {
	public static explicit operator Convertible(int i) {return new Convertible(); }
}
class Test {
	public void M(int i) {
		 a = $(Convertible?)i$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.IsFalse(rr.Conversion.IsLifted);
		}

		[Test]
		public void UserDefinedExplicitConversion_ExplicitNullable_ThenUserDefined()
		{
			string program = @"using System;
struct Convertible {
	public static explicit operator Convertible(int i) {return new Convertible(); }
	public static explicit operator Convertible?(int? ni) {return new Convertible(); }
}
class Test {
	public void M(int? i) {
		 a = $(Convertible)i$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.IsFalse(rr.Conversion.IsLifted);
			Assert.AreEqual("i", rr.Conversion.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedExplicitConversion_DefinedNullableTakesPrecedenceOverLifted()
		{
			string program = @"using System;
struct Convertible {
	public static explicit operator Convertible(int i) {return new Convertible(); }
	public static explicit operator Convertible?(int? ni) {return new Convertible(); }
}
class Test {
	public void M() {
		 a = $(Convertible?)(int?)33$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.IsFalse(rr.Conversion.IsLifted);
			Assert.AreEqual("ni", rr.Conversion.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedExplicitConversion_UIntConstant()
		{
			string program = @"using System;
class Convertible {
	public static explicit operator Convertible(long l) {return new Convertible(); }
	public static explicit operator Convertible(uint ui) {return new Convertible(); }
}
class Test {
	public void M() {
		var a = $(Convertible)33$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.AreEqual("ui", rr.Conversion.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedExplicitConversion_NullableUIntConstant()
		{
			string program = @"using System;
class Convertible {
	public static explicit operator Convertible(long? l) {return new Convertible(); }
	public static explicit operator Convertible(uint? ui) {return new Convertible(); }
}
class Test {
	public void M() {
		Convertible a = $(Convertible)33$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.AreEqual("ui", rr.Conversion.Method.Parameters[0].Name);
		}

		[Test]
		public void UseDefinedExplicitConversion_Lifted()
		{
			string program = @"
struct Convertible {
	public static explicit operator Convertible(int i) { return new Convertible(); }
}
class Test {
	public void M(int? i) {
		a = $(Convertible?)i$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.IsTrue(rr.Conversion.IsLifted);
			Assert.IsTrue(rr.Input is LocalResolveResult);
		}

		[Test]
		public void UserDefinedExplicitConversion_Short_Or_NullableByte_Target()
		{
			string program = @"using System;
class Test {
	public static explicit operator short(Test s) { return 0; }
	public static explicit operator byte?(Test b) { return 0; }
}
class Program {
	public static void Main(string[] args)
	{
		int? x = $(int?)new Test()$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.AreEqual("System.Int16", rr.Conversion.Method.ReturnType.FullName);
		}

		[Test]
		public void UserDefinedExplicitConversion_Byte_Or_NullableShort_Target()
		{
			string program = @"using System;
class Test {
	public static explicit operator byte(Test b) { return 0; }
	public static explicit operator short?(Test s) { return 0; }
}
class Program {
	public static void Main(string[] args)
	{
		int? x = $(int?)new Test()$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.AreEqual("s", rr.Conversion.Method.Parameters[0].Name);
		}

		[Test]
		public void ExplicitConversionOperatorsCanOverrideApplicableImplicitOnes()
		{
			string program = @"
struct Convertible {
	public static explicit operator int(Convertible ci) {return 0; }
	public static implicit operator short(Convertible cs) {return 0; }
}
class Test {
	static void Main() {
		int i = $(int)new Convertible()$; // csc uses the explicit conversion operator
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.IsUserDefined);
			Assert.AreEqual("ci", rr.Conversion.Method.Parameters[0].Name);
		}

		[Test]
		public void UserDefinedExplicitConversion_ConversionBeforeUserDefinedOperatorIsCorrect()
		{
			string program = @"using System;
class Convertible {
	public static implicit operator Convertible(int l) {return new Convertible(); }
}
class Test {
	public void M() {
		long i = 33;
		Convertible a = $(Convertible)i$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.ConversionBeforeUserDefinedOperator.IsValid);
			Assert.IsTrue(rr.Conversion.ConversionBeforeUserDefinedOperator.IsExplicit);
			Assert.IsTrue(rr.Conversion.ConversionBeforeUserDefinedOperator.IsNumericConversion);
			Assert.IsTrue(rr.Conversion.ConversionAfterUserDefinedOperator.IsIdentityConversion);
		}

		[Test]
		public void UserDefinedExplicitConversion_ConversionAfterUserDefinedOperatorIsCorrect()
		{
			string program = @"using System;
class Convertible {
	public static implicit operator long(Convertible i) {return 0; }
}
class Test {
	public void M() {
		int a = $(int)new Convertible()$;
	}
}";
			var rr = Resolve<ConversionResolveResult>(program);
			Assert.IsTrue(rr.Conversion.IsValid);
			Assert.IsTrue(rr.Conversion.ConversionBeforeUserDefinedOperator.IsIdentityConversion);
			Assert.IsTrue(rr.Conversion.ConversionAfterUserDefinedOperator.IsValid);
			Assert.IsTrue(rr.Conversion.ConversionAfterUserDefinedOperator.IsExplicit);
			Assert.IsTrue(rr.Conversion.ConversionAfterUserDefinedOperator.IsNumericConversion);
		}*/
	}
}
