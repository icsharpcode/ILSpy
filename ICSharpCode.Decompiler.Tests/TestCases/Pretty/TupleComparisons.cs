using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class TupleComparisons
	{
		public class CustomEquality
		{
			public static bool operator ==(CustomEquality a, CustomEquality b)
			{
				return true;
			}

			public static bool operator !=(CustomEquality a, CustomEquality b)
			{
				return false;
			}

			public override bool Equals(object obj)
			{
				return false;
			}

			public override int GetHashCode()
			{
				return 0;
			}
		}

		public bool TupleEquality((int, int) t1, (int, int) t2)
		{
			return t1 == t2;
		}

		public bool TupleInequality((int, int) t1, (int, int) t2)
		{
			return t1 != t2;
		}

		public bool ComparisonWithTupleLiteral((int a, int b) t1, (int a, int b) t2)
		{
			return t1 != t2 && t2 == (t1.a + t1.b, 0);
		}

		public bool NestedTuples((int, int) ab, int c, (int, int) de, int f)
		{
			return (ab, c) == (de, f);
		}

		public bool NullableTupleEquality((int, int)? t1, (int, int)? t2)
		{
			return t1 == t2;
		}

		public bool NullableTupleInequality((int, int)? t1, (int, int)? t2)
		{
			return t1 != t2;
		}

		public bool NullableTupleComparedToLiteral((int, int)? t1, int a, int b)
		{
			return t1 == (a, b);
		}

		public bool ImplicitConversionsFromLiteral((int, long) t)
		{
			return t == (1, 2);
		}

		public bool ImplicitElementConversions((byte a, int b) t1, (int a, long b) t2)
		{
			return t1 == t2;
		}

		public bool StringElements((string, int) t1, (string, int) t2)
		{
			return t1 == t2;
		}

		public bool CustomOperatorElements((CustomEquality, int) t1, (CustomEquality, int) t2)
		{
			return t1 == t2;
		}

		public bool DynamicElements((dynamic, int) t1, (object, int) t2)
		{
			return t1 == t2;
		}

		public bool ElementNamesDoNotMatter((int a, int b) t1, (int x, int y) t2)
		{
			return t1 == t2;
		}

		public bool NullableElements((int?, int) t1, (int?, int) t2)
		{
			return t1 == t2;
		}

		public bool EightElements((int, int, int, int, int, int, int, int) t1, (int, int, int, int, int, int, int, int) t2)
		{
			return t1 == t2;
		}

		public bool OperandsEvaluatedOnlyOnce(Func<(int, int)> f1, Func<(int, int)> f2)
		{
			return f1() == f2();
		}

		public void UsedInCondition((int, int) t1, (int, int) t2)
		{
			if (t1 == t2)
			{
				Console.WriteLine("equal");
			}
		}
	}
}
