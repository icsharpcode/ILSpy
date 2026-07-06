using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal static class ExtensionOperators
	{
		extension(ExtOpsInParam)
		{
			public static ExtOpsInParam operator +(in ExtOpsInParam a, in ExtOpsInParam b)
			{
				return default(ExtOpsInParam);
			}
		}

		extension(ExtOpsLogical)
		{
			public static ExtOpsLogical operator &(ExtOpsLogical a, ExtOpsLogical b)
			{
				return default(ExtOpsLogical);
			}

			public static ExtOpsLogical operator |(ExtOpsLogical a, ExtOpsLogical b)
			{
				return default(ExtOpsLogical);
			}

			public static bool operator true(ExtOpsLogical a)
			{
				return a.Value != 0;
			}

			public static bool operator false(ExtOpsLogical a)
			{
				return a.Value == 0;
			}
		}

		extension(ExtOpsOwn)
		{
			public static ExtOpsOwn operator -(ExtOpsOwn a, ExtOpsOwn b)
			{
				return default(ExtOpsOwn);
			}
		}

		extension(ExtOpsValue)
		{
			public static ExtOpsValue operator +(ExtOpsValue a, ExtOpsValue b)
			{
				return default(ExtOpsValue);
			}

			public static ExtOpsValue operator -(ExtOpsValue a, ExtOpsValue b)
			{
				return default(ExtOpsValue);
			}

			public static ExtOpsValue operator *(ExtOpsValue a, ExtOpsValue b)
			{
				return default(ExtOpsValue);
			}

			public static ExtOpsValue operator /(ExtOpsValue a, ExtOpsValue b)
			{
				return default(ExtOpsValue);
			}

			public static ExtOpsValue operator %(ExtOpsValue a, ExtOpsValue b)
			{
				return default(ExtOpsValue);
			}

			public static ExtOpsValue operator &(ExtOpsValue a, ExtOpsValue b)
			{
				return default(ExtOpsValue);
			}

			public static ExtOpsValue operator |(ExtOpsValue a, ExtOpsValue b)
			{
				return default(ExtOpsValue);
			}

			public static ExtOpsValue operator ^(ExtOpsValue a, ExtOpsValue b)
			{
				return default(ExtOpsValue);
			}

			public static ExtOpsValue operator <<(ExtOpsValue a, int b)
			{
				return default(ExtOpsValue);
			}

			public static ExtOpsValue operator >>(ExtOpsValue a, int b)
			{
				return default(ExtOpsValue);
			}

			public static ExtOpsValue operator >>>(ExtOpsValue a, int b)
			{
				return default(ExtOpsValue);
			}

			public static bool operator ==(ExtOpsValue a, ExtOpsValue b)
			{
				return true;
			}

			public static bool operator !=(ExtOpsValue a, ExtOpsValue b)
			{
				return false;
			}

			public static bool operator <(ExtOpsValue a, ExtOpsValue b)
			{
				return true;
			}

			public static bool operator >(ExtOpsValue a, ExtOpsValue b)
			{
				return false;
			}

			public static bool operator <=(ExtOpsValue a, ExtOpsValue b)
			{
				return true;
			}

			public static bool operator >=(ExtOpsValue a, ExtOpsValue b)
			{
				return false;
			}

			public static ExtOpsValue operator +(ExtOpsValue a)
			{
				return a;
			}

			public static ExtOpsValue operator -(ExtOpsValue a)
			{
				return default(ExtOpsValue);
			}

			public static ExtOpsValue operator !(ExtOpsValue a)
			{
				return default(ExtOpsValue);
			}

			public static ExtOpsValue operator ~(ExtOpsValue a)
			{
				return default(ExtOpsValue);
			}

			public static ExtOpsValue operator checked +(ExtOpsValue a, ExtOpsValue b)
			{
				return default(ExtOpsValue);
			}

			public static ExtOpsValue operator checked *(ExtOpsValue a, ExtOpsValue b)
			{
				return default(ExtOpsValue);
			}

			public static ExtOpsValue operator checked -(ExtOpsValue a)
			{
				return default(ExtOpsValue);
			}
		}

		extension(string)
		{
			public static string operator -(string a, string b)
			{
				return a;
			}
		}

		extension<T>(List<T>)
		{
			public static List<T> operator +(List<T> a, List<T> b)
			{
				return a;
			}
		}
	}

	internal static class ExtensionOperatorsUseSites
	{
		public static ExtOpsValue Add(ExtOpsValue x, ExtOpsValue y)
		{
			return x + y;
		}

		public static ExtOpsValue Sub(ExtOpsValue x, ExtOpsValue y)
		{
			return x - y;
		}

		public static ExtOpsValue Mul(ExtOpsValue x, ExtOpsValue y)
		{
			return x * y;
		}

		public static ExtOpsValue Div(ExtOpsValue x, ExtOpsValue y)
		{
			return x / y;
		}

		public static ExtOpsValue Rem(ExtOpsValue x, ExtOpsValue y)
		{
			return x % y;
		}

		public static ExtOpsValue BitwiseAnd(ExtOpsValue x, ExtOpsValue y)
		{
			return x & y;
		}

		public static ExtOpsValue BitwiseOr(ExtOpsValue x, ExtOpsValue y)
		{
			return x | y;
		}

		public static ExtOpsValue ExclusiveOr(ExtOpsValue x, ExtOpsValue y)
		{
			return x ^ y;
		}

		public static ExtOpsValue ShiftLeft(ExtOpsValue x)
		{
			return x << 2;
		}

		public static ExtOpsValue ShiftRight(ExtOpsValue x)
		{
			return x >> 3;
		}

		public static ExtOpsValue UnsignedShiftRight(ExtOpsValue x)
		{
			return x >>> 4;
		}

		public static bool Equality(ExtOpsValue x, ExtOpsValue y)
		{
			return x == y;
		}

		public static bool Inequality(ExtOpsValue x, ExtOpsValue y)
		{
			return x != y;
		}

		public static bool LessThan(ExtOpsValue x, ExtOpsValue y)
		{
			return x < y;
		}

		public static bool GreaterThan(ExtOpsValue x, ExtOpsValue y)
		{
			return x > y;
		}

		public static bool LessThanOrEqual(ExtOpsValue x, ExtOpsValue y)
		{
			return x <= y;
		}

		public static bool GreaterThanOrEqual(ExtOpsValue x, ExtOpsValue y)
		{
			return x >= y;
		}

		public static ExtOpsValue UnaryPlus(ExtOpsValue x)
		{
			return +x;
		}

		public static ExtOpsValue UnaryNegation(ExtOpsValue x)
		{
			return -x;
		}

		public static ExtOpsValue LogicalNot(ExtOpsValue x)
		{
			return !x;
		}

		public static ExtOpsValue OnesComplement(ExtOpsValue x)
		{
			return ~x;
		}

		public static ExtOpsValue CheckedAdd(ExtOpsValue x, ExtOpsValue y)
		{
			return checked(x + y);
		}

		public static ExtOpsValue CheckedMul(ExtOpsValue x, ExtOpsValue y)
		{
			return checked(x * y);
		}

		public static ExtOpsValue CheckedNeg(ExtOpsValue x)
		{
			return checked(-x);
		}

		public static void CompoundAdd(ref ExtOpsValue x, ExtOpsValue y)
		{
			x += y;
		}

		public static void CompoundShiftLeft(ref ExtOpsValue x)
		{
			x <<= 1;
		}

		public static bool TrueOperator(ExtOpsLogical x)
		{
			if (x)
			{
				return true;
			}
			return false;
		}

		public static ExtOpsOwn UseOwnOperator(ExtOpsOwn x, ExtOpsOwn y)
		{
			return x + y;
		}

		public static ExtOpsOwn UseExtensionOperator(ExtOpsOwn x, ExtOpsOwn y)
		{
			return x - y;
		}

		public static ExtOpsInParam InParameters(ExtOpsInParam x, ExtOpsInParam y)
		{
			return x + y;
		}

		public static string Strings(string a, string b)
		{
			return a - b;
		}

		public static List<int> Generic(List<int> a, List<int> b)
		{
			return a + b;
		}
	}

	internal struct ExtOpsInParam
	{
		public int Value;
	}

	internal struct ExtOpsLogical
	{
		public int Value;
	}

	internal struct ExtOpsOwn
	{
		public int Value;

		public static ExtOpsOwn operator +(ExtOpsOwn a, ExtOpsOwn b)
		{
			return default(ExtOpsOwn);
		}
	}

	internal struct ExtOpsValue
	{
		public int Value;
	}
}
