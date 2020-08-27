// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using System.Runtime.InteropServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public static class T00_LiftedOperators
	{
		// C# uses 4 different patterns of IL for lifted operators: bool, other primitive types, decimal, other structs.
		// Different patterns are used depending on whether both of the operands are nullable or only the left/right operand is nullable.
		// Negation must not be pushed through such comparisons because it would change the semantics (except for equality/inequality).
		// A comparison used in a condition differs somewhat from a comparison used as a simple value.

		public static void BoolBasic(bool? a, bool? b)
		{
			if (a == b)
			{
				Console.WriteLine();
			}
			if (a != b)
			{
				Console.WriteLine();
			}
		}

		public static void BoolComplex(bool? a, Func<bool> x)
		{
			if (a == x())
			{
				Console.WriteLine();
			}
			if (a != x())
			{
				Console.WriteLine();
			}

			if (x() == a)
			{
				Console.WriteLine();
			}
			if (x() != a)
			{
				Console.WriteLine();
			}
			if (a ?? x())
			{
				Console.WriteLine();
			}
		}

		public static void BoolConst(bool? a)
		{
			if (a == true)
			{
				Console.WriteLine();
			}
			if (a != true)
			{
				Console.WriteLine();
			}
			if (a == false)
			{
				Console.WriteLine();
			}
			if (a != false)
			{
				Console.WriteLine();
			}
			if (a ?? true)
			{
				Console.WriteLine();
			}
#if !ROSLYN
			// Roslyn 3 (VS2019) started optimizing this to "a.GetValueOrDefault()"
			if (a ?? false)
			{
				Console.WriteLine();
			}
#endif
		}

		public static void BoolValueBasic(bool? a, bool? b)
		{
			Console.WriteLine(a == b);
			Console.WriteLine(a != b);

			Console.WriteLine(a & b);
			Console.WriteLine(a | b);
			Console.WriteLine(a ^ b);
			Console.WriteLine(a ?? b);
			Console.WriteLine(!a);
			a &= b;
			a |= b;
			a ^= b;
		}

		public static void BoolValueComplex(bool? a, Func<bool> x)
		{
			Console.WriteLine(a == x());
			Console.WriteLine(a != x());

			Console.WriteLine(x() == a);
			Console.WriteLine(x() != a);

			//Console.WriteLine(a & x()); // we currently can't tell the order
			//Console.WriteLine(a | x()); // of the operands in bool [&|] bool?
			Console.WriteLine(a ^ x());
			Console.WriteLine(a ?? x());
			//a &= x(); -- also affected by order of operand problem
			//a |= x();
			a ^= x();

			Console.WriteLine(x() & a);
			Console.WriteLine(x() | a);
			Console.WriteLine(x() ^ a);
			(new bool?[0])[0] ^= x();
			(new bool?[0])[0] ^= a;
		}

		public static void BoolValueConst(bool? a)
		{
			Console.WriteLine(a == true);
			Console.WriteLine(a != true);
			Console.WriteLine(a == false);
			Console.WriteLine(a != false);
			Console.WriteLine(a ?? true);
#if !ROSLYN
			// Roslyn 3 (VS2019) started optimizing this to "a.GetValueOrDefault()"
			Console.WriteLine(a ?? false);
#endif
		}

		public static void IntBasic(int? a, int? b)
		{
			if (a == b)
			{
				Console.WriteLine();
			}
			if (a != b)
			{
				Console.WriteLine();
			}
			if (a > b)
			{
				Console.WriteLine();
			}
			if (a < b)
			{
				Console.WriteLine();
			}
			if (a >= b)
			{
				Console.WriteLine();
			}
			if (a <= b)
			{
				Console.WriteLine();
			}

			if (!(a > b))
			{
				Console.WriteLine();
			}
			if (!(a <= b))
			{
				Console.WriteLine();
			}
		}

		public static void IntComplex(int? a, Func<int> x)
		{
			if (a == x())
			{
				Console.WriteLine();
			}
			if (a != x())
			{
				Console.WriteLine();
			}
			if (a > x())
			{
				Console.WriteLine();
			}

			if (x() == a)
			{
				Console.WriteLine();
			}
			if (x() != a)
			{
				Console.WriteLine();
			}
			if (x() > a)
			{
				Console.WriteLine();
			}

			if (!(a > x()))
			{
				Console.WriteLine();
			}
			if (!(a <= x()))
			{
				Console.WriteLine();
			}
		}

		public static void IntConst(int? a)
		{
			if (a == 2)
			{
				Console.WriteLine();
			}
			if (a != 2)
			{
				Console.WriteLine();
			}
			if (a > 2)
			{
				Console.WriteLine();
			}

			if (2 == a)
			{
				Console.WriteLine();
			}
			if (2 != a)
			{
				Console.WriteLine();
			}
			if (2 > a)
			{
				Console.WriteLine();
			}
		}

		public static void IntValueBasic(int? a, int? b)
		{
			Console.WriteLine(a == b);
			Console.WriteLine(a != b);
			Console.WriteLine(a > b);

			Console.WriteLine(!(a > b));
			Console.WriteLine(!(a >= b));

			Console.WriteLine(a + b);
			Console.WriteLine(a - b);
			Console.WriteLine(a * b);
			Console.WriteLine(a / b);
			Console.WriteLine(a % b);
			Console.WriteLine(a & b);
			Console.WriteLine(a | b);
			Console.WriteLine(a ^ b);
			Console.WriteLine(a << b);
			Console.WriteLine(a >> b);
			Console.WriteLine(a ?? b);
			Console.WriteLine(-a);
			Console.WriteLine(~a);
			// TODO:
			//Console.WriteLine(a++);
			//Console.WriteLine(a--);
			Console.WriteLine(++a);
			Console.WriteLine(--a);
			a += b;
			a -= b;
			a *= b;
			a /= b;
			a %= b;
			a &= b;
			a |= b;
			a ^= b;
			a <<= b;
			a >>= b;
		}

		public static void IntValueComplex(int? a, Func<int> x)
		{
			Console.WriteLine(a == x());
			Console.WriteLine(a != x());
			Console.WriteLine(a > x());

			Console.WriteLine(x() == a);
			Console.WriteLine(x() != a);
			Console.WriteLine(x() > a);

			Console.WriteLine(a + x());
			Console.WriteLine(a - x());
			Console.WriteLine(a * x());
			Console.WriteLine(a / x());
			Console.WriteLine(a % x());
			Console.WriteLine(a & x());
			Console.WriteLine(a | x());
			Console.WriteLine(a ^ x());
			Console.WriteLine(a << x());
			Console.WriteLine(a >> x());
			Console.WriteLine(a ?? x());
			a += x();
			a -= x();
			a *= x();
			a /= x();
			a %= x();
			a &= x();
			a |= x();
			a ^= x();
			a <<= x();
			a >>= x();

			Console.WriteLine(x() + a);
			(new int?[0])[0] += x();
		}

		public static void IntValueConst(int? a)
		{
			Console.WriteLine(a == 2);
			Console.WriteLine(a != 2);
			Console.WriteLine(a > 2);

			Console.WriteLine(2 == a);
			Console.WriteLine(2 != a);
			Console.WriteLine(2 > a);

			Console.WriteLine(a + 2);
			Console.WriteLine(a - 2);
			Console.WriteLine(a * 2);
			Console.WriteLine(a / 2);
			Console.WriteLine(a % 2);
			Console.WriteLine(a & 2);
			Console.WriteLine(a | 2);
			Console.WriteLine(a ^ 2);
			Console.WriteLine(a << 2);
			Console.WriteLine(a >> 2);
			Console.WriteLine(a ?? 2);
			a += 2;
			a -= 2;
			a *= 2;
			a /= 2;
			a %= 2;
			a &= 2;
			a |= 2;
			a ^= 2;
			a <<= 2;
			a >>= 2;

			Console.WriteLine(2 + a);
		}

		public static void NumberBasic(decimal? a, decimal? b)
		{
			if (a == b)
			{
				Console.WriteLine();
			}
#if ROSLYN
			// Roslyn 2.9 started invoking op_Equality even if the source code says 'a != b'
			if (!(a == b))
			{
				Console.WriteLine();
			}
#else
			if (a != b)
			{
				Console.WriteLine();
			}
#endif
			if (a > b)
			{
				Console.WriteLine();
			}
			if (a < b)
			{
				Console.WriteLine();
			}
			if (a >= b)
			{
				Console.WriteLine();
			}
			if (a <= b)
			{
				Console.WriteLine();
			}

			if (!(a > b))
			{
				Console.WriteLine();
			}
			if (!(a < b))
			{
				Console.WriteLine();
			}
		}

		public static void NumberComplex(decimal? a, Func<decimal> x)
		{
			// Tests deactivated because we insert redundant casts;
			// TODO: revisit after decision has been made regarding the type system.
			//if (a == x()) {
			//	Console.WriteLine();
			//}
			//if (a != x()) {
			//	Console.WriteLine();
			//}
			//if (a > x()) {
			//	Console.WriteLine();
			//}

			//if (x() == a) {
			//	Console.WriteLine();
			//}
			//if (x() != a) {
			//	Console.WriteLine();
			//}
			//if (x() > a) {
			//	Console.WriteLine();
			//}
		}

		public static void NumberConst(decimal? a)
		{
			// Tests deactivated because we insert redundant casts;
			// TODO: revisit after decision has been made regarding the type system.
			//if (a == 2m) {
			//	Console.WriteLine();
			//}
			//if (a != 2m) {
			//	Console.WriteLine();
			//}
			//if (a > 2m) {
			//	Console.WriteLine();
			//}

			//if (2m == a) {
			//	Console.WriteLine();
			//}
			//if (2m != a) {
			//	Console.WriteLine();
			//}
			//if (2m > a) {
			//	Console.WriteLine();
			//}
		}

		public static void NumberValueBasic(decimal? a, decimal? b)
		{
			Console.WriteLine(a == b);
#if ROSLYN
			// Roslyn 2.9 started invoking op_Equality even if the source code says 'a != b'
			Console.WriteLine(!(a == b));
#else
			Console.WriteLine(a != b);
#endif
			Console.WriteLine(a > b);

			Console.WriteLine(!(a > b));
			Console.WriteLine(!(a <= b));

			Console.WriteLine(a + b);
			Console.WriteLine(a - b);
			Console.WriteLine(a * b);
			Console.WriteLine(a / b);
			Console.WriteLine(a % b);
			Console.WriteLine(a ?? b);
			Console.WriteLine(-a);
			// TODO:
			//Console.WriteLine(a++);
			//Console.WriteLine(a--);
			//Console.WriteLine(++a);
			//Console.WriteLine(--a);
			a += b;
			a -= b;
			a *= b;
			a /= b;
			a %= b;
		}

		public static void NumberValueComplex(decimal? a, Func<decimal> x)
		{
			// Tests deactivated because we insert redundant casts;
			// TODO: revisit after decision has been made regarding the type system.
			//Console.WriteLine(a == x());
			//Console.WriteLine(a != x());
			//Console.WriteLine(a > x());

			//Console.WriteLine(x() == a);
			//Console.WriteLine(x() != a);
			//Console.WriteLine(x() > a);

			//Console.WriteLine(a + x());
			//Console.WriteLine(a - x());
			//Console.WriteLine(a * x());
			//Console.WriteLine(a / x());
			//Console.WriteLine(a % x());
			//Console.WriteLine(a ?? x());
			//a += x();
			//a -= x();
			//a *= x();
			//a /= x();
			//a %= x();

			//Console.WriteLine(x() + a);
			//(new decimal?[0])[0] += x();
		}

		public static void NumberValueConst(decimal? a)
		{
			// Tests deactivated because we insert redundant casts;
			// TODO: revisit after decision has been made regarding the type system.
			//Console.WriteLine(a == 2m);
			//Console.WriteLine(a != 2m);
			//Console.WriteLine(a > 2m);

			//Console.WriteLine(2m == a);
			//Console.WriteLine(2m != a);
			//Console.WriteLine(2m > a);

			//Console.WriteLine(a + 2m);
			//Console.WriteLine(a - 2m);
			//Console.WriteLine(a * 2m);
			//Console.WriteLine(a / 2m);
			//Console.WriteLine(a % 2m);
			//Console.WriteLine(a ?? 2m);
			//a += 2m;
			//a -= 2m;
			//a *= 2m;
			//a /= 2m;
			//a %= 2m;

			//Console.WriteLine(2m + a);
		}

		public static void CompareWithImplictCast(int? a, long? b)
		{
			if (a < b)
			{
				Console.WriteLine();
			}
			if (a == b)
			{
				Console.WriteLine();
			}
			// TODO: unnecessary cast
			//if (a < 10L) {
			//	Console.WriteLine();
			//}
			//if (a == 10L) {
			//	Console.WriteLine();
			//}
		}

		public static void CompareWithSignChange(int? a, int? b)
		{
			if ((uint?)a < (uint?)b)
			{
				Console.WriteLine();
			}
			// TODO: unnecessary cast
			//if ((uint?)a < 10) {
			//	Console.WriteLine();
			//}
		}

		public static void StructBasic(TS? a, TS? b)
		{
			if (a == b)
			{
				Console.WriteLine();
			}
			if (a != b)
			{
				Console.WriteLine();
			}
			if (a > b)
			{
				Console.WriteLine();
			}
			if (a < b)
			{
				Console.WriteLine();
			}
			if (a >= b)
			{
				Console.WriteLine();
			}
			if (a <= b)
			{
				Console.WriteLine();
			}

			if (!(a == b))
			{
				Console.WriteLine();
			}
			if (!(a != b))
			{
				Console.WriteLine();
			}
			if (!(a > b))
			{
				Console.WriteLine();
			}
		}

		public static void StructComplex(TS? a, Func<TS> x)
		{
			// Tests deactivated because we insert redundant casts;
			// TODO: revisit after decision has been made regarding the type system.
			//if (a == x()) {
			//	Console.WriteLine();
			//}
			//if (a != x()) {
			//	Console.WriteLine();
			//}
			//if (a > x()) {
			//	Console.WriteLine();
			//}

			//if (x() == a) {
			//	Console.WriteLine();
			//}
			//if (x() != a) {
			//	Console.WriteLine();
			//}
			//if (x() > a) {
			//	Console.WriteLine();
			//}
		}

		public static void StructValueBasic(TS? a, TS? b, int? i)
		{
			Console.WriteLine(a == b);
			Console.WriteLine(a != b);
			Console.WriteLine(a > b);

			Console.WriteLine(!(a == b));
			Console.WriteLine(!(a != b));
			Console.WriteLine(!(a > b));

			Console.WriteLine(a + b);
			Console.WriteLine(a - b);
			Console.WriteLine(a * b);
			Console.WriteLine(a / b);
			Console.WriteLine(a % b);
			Console.WriteLine(a & b);
			Console.WriteLine(a | b);
			Console.WriteLine(a ^ b);
			Console.WriteLine(a << i);
			Console.WriteLine(a >> i);
			Console.WriteLine(a ?? b);
			Console.WriteLine(+a);
			Console.WriteLine(-a);
			Console.WriteLine(!a);
			Console.WriteLine(~a);
			// TODO:
			//Console.WriteLine(a++);
			//Console.WriteLine(a--);
			//Console.WriteLine(++a);
			//Console.WriteLine(--a);
			//Console.WriteLine((int?)a);
			a += b;
			a -= b;
			a *= b;
			a /= b;
			a %= b;
			a &= b;
			a |= b;
			a ^= b;
			a <<= i;
			a >>= i;
		}

		public static void StructValueComplex(TS? a, Func<TS> x, Func<int> i)
		{
			// Tests deactivated because we insert redundant casts;
			// TODO: revisit after decision has been made regarding the type system.
			//Console.WriteLine(a == x());
			//Console.WriteLine(a != x());
			//Console.WriteLine(a > x());

			//Console.WriteLine(x() == a);
			//Console.WriteLine(x() != a);
			//Console.WriteLine(x() > a);

			//Console.WriteLine(a + x());
			//Console.WriteLine(a - x());
			//Console.WriteLine(a * x());
			//Console.WriteLine(a / x());
			//Console.WriteLine(a % x());
			//Console.WriteLine(a & x());
			//Console.WriteLine(a | x());
			//Console.WriteLine(a ^ x());
			//Console.WriteLine(a << i());
			//Console.WriteLine(a >> i());
			//Console.WriteLine(a ?? x());
			//a += x();
			//a -= x();
			//a *= x();
			//a /= x();
			//a %= x();
			//a &= x();
			//a |= x();
			//a ^= x();
			//a <<= i();
			//a >>= i();

			//Console.WriteLine(x() + a);
			//(new TS?[0])[0] += x();
		}

		public static bool RetEq(int? a, int? b)
		{
			return a == b;
		}

		public static bool RetEqConv(long? a, int? b)
		{
			return a == b;
		}

		public static bool RetEqConst(long? a)
		{
			return a == 10;
		}

		public static bool RetIneqConst(long? a)
		{
			return a != 10;
		}

		public static bool RetLt(int? a, int? b)
		{
			return a < b;
		}

		public static bool RetLtConst(int? a)
		{
			return a < 10;
		}

		public static bool RetLtConv(long? a, int? b)
		{
			return a < b;
		}

		public static bool RetNotLt(int? a, int? b)
		{
			return !(a < b);
		}
	}

	internal class T01_LiftedImplicitConversions
	{
		public int? ExtendI4(byte? b)
		{
			return b;
		}

		public int? ExtendToI4(sbyte? b)
		{
			return b;
		}

		public long? ExtendI8(byte? b)
		{
			return b;
		}

		public long? ExtendToI8(sbyte? b)
		{
			return b;
		}

		public long? ExtendI8(int? b)
		{
			return b;
		}

		public long? ExtendToI8(uint? b)
		{
			return b;
		}

		// TODO: unnecessary cast
		//public double? ToFloat(int? b)
		//{
		//	return b;
		//}

		//public long? InArithmetic(uint? b)
		//{
		//	return 100L + b;
		//}

		public long? AfterArithmetic(uint? b)
		{
			return 100 + b;
		}

		// TODO: unnecessary cast
		//public static double? InArithmetic2(float? nf, double? nd, float f)
		//{
		//	return nf + nd + f;
		//}

		public static long? InArithmetic3(int? a, long? b, int? c, long d)
		{
			return a + b + c + d;
		}
	}

	internal class T02_LiftedExplicitConversions
	{
		private static void Print<T>(T? x) where T : struct
		{
			Console.WriteLine(x);
		}

		public static void UncheckedCasts(int? i4, long? i8, float? f)
		{
			Print((byte?)i4);
			Print((short?)i4);
			Print((uint?)i4);
			Print((uint?)i8);
			Print((uint?)f);
		}

		public static void CheckedCasts(int? i4, long? i8, float? f)
		{
			checked
			{
				Print((byte?)i4);
				Print((short?)i4);
				Print((uint?)i4);
				Print((uint?)i8);
				//Print((uint?)f); TODO
			}
		}
	}

	internal class T03_NullCoalescingTests
	{
		private static void Print<T>(T x)
		{
			Console.WriteLine(x);
		}

		public static void Objects(object a, object b)
		{
			Print(a ?? b);
		}

		public static void Nullables(int? a, int? b)
		{
			Print(a ?? b);
		}

		public static void NullableWithNonNullableFallback(int? a, int b)
		{
			Print(a ?? b);
		}

		public static void NullableWithImplicitConversion(short? a, int? b)
		{
			Print(a ?? b);
		}

		public static void NullableWithImplicitConversionAndNonNullableFallback(short? a, int b)
		{
			// TODO: unnecessary cast
			//Print(a ?? b);
		}

		public static void Chain(int? a, int? b, int? c, int d)
		{
			Print(a ?? b ?? c ?? d);
		}

		public static void ChainWithImplicitConversions(int? a, short? b, long? c, byte d)
		{
			// TODO: unnecessary casts
			//Print(a ?? b ?? c ?? d);
		}

		public static void ChainWithComputation(int? a, short? b, long? c, byte d)
		{
			// TODO: unnecessary casts
			//Print((a + 1) ?? (b + 2) ?? (c + 3) ?? (d + 4));
		}

		public static object ReturnObjects(object a, object b)
		{
			return a ?? b;
		}

		public static int? ReturnNullables(int? a, int? b)
		{
			return a ?? b;
		}

		public static int ReturnNullableWithNonNullableFallback(int? a, int b)
		{
			return a ?? b;
		}

		public static int ReturnChain(int? a, int? b, int? c, int d)
		{
			return a ?? b ?? c ?? d;
		}

		public static long ReturnChainWithImplicitConversions(int? a, short? b, long? c, byte d)
		{
			//TODO: unnecessary casts
			//return a ?? b ?? c ?? d;
			return 0L;
		}

		public static long ReturnChainWithComputation(int? a, short? b, long? c, byte d)
		{
			//TODO: unnecessary casts
			//return (a + 1) ?? (b + 2) ?? (c + 3) ?? (d + 4);
			return 0L;
		}
	}

	// dummy structure for testing custom operators
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct TS
	{
		// unary
		public static TS operator +(TS a)
		{
			throw null;
		}
		public static TS operator -(TS a)
		{
			throw null;
		}
		public static TS operator !(TS a)
		{
			throw null;
		}
		public static TS operator ~(TS a)
		{
			throw null;
		}
		public static TS operator ++(TS a)
		{
			throw null;
		}
		public static TS operator --(TS a)
		{
			throw null;
		}

		public static explicit operator int(TS a)
		{
			throw null;
		}

		// binary
		public static TS operator +(TS a, TS b)
		{
			throw null;
		}
		public static TS operator -(TS a, TS b)
		{
			throw null;
		}
		public static TS operator *(TS a, TS b)
		{
			throw null;
		}
		public static TS operator /(TS a, TS b)
		{
			throw null;
		}
		public static TS operator %(TS a, TS b)
		{
			throw null;
		}
		public static TS operator &(TS a, TS b)
		{
			throw null;
		}
		public static TS operator |(TS a, TS b)
		{
			throw null;
		}
		public static TS operator ^(TS a, TS b)
		{
			throw null;
		}
		public static TS operator <<(TS a, int b)
		{
			throw null;
		}
		public static TS operator >>(TS a, int b)
		{
			throw null;
		}

		// comparisons
		public static bool operator ==(TS a, TS b)
		{
			throw null;
		}
		public static bool operator !=(TS a, TS b)
		{
			throw null;
		}
		public static bool operator <(TS a, TS b)
		{
			throw null;
		}
		public static bool operator <=(TS a, TS b)
		{
			throw null;
		}
		public static bool operator >(TS a, TS b)
		{
			throw null;
		}
		public static bool operator >=(TS a, TS b)
		{
			throw null;
		}

		public override bool Equals(object obj)
		{
			throw null;
		}
		public override int GetHashCode()
		{
			throw null;
		}
	}
}
