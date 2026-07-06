using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	// Pins the current decompiled form of C# 7.3 tuple comparisons: the element-wise
	// expansion with evaluation-order temporaries. Once the decompiler learns to
	// reconstruct tuple comparisons (see TupleComparisons.cs), the EXPECTED_OUTPUT
	// blocks in this file are expected to collapse to the tuple `==`/`!=` form.
	public class TupleComparisonsExpanded
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
#if EXPECTED_OUTPUT && OPT
			(int, int) tuple = t1;
			(int, int) tuple2 = t2;
			if (tuple.Item1 == tuple2.Item1)
			{
				return tuple.Item2 == tuple2.Item2;
			}
			return false;
#elif EXPECTED_OUTPUT
			(int, int) tuple = t1;
			(int, int) tuple2 = t2;
			return tuple.Item1 == tuple2.Item1 && tuple.Item2 == tuple2.Item2;
#else
			return t1 == t2;
#endif
		}

		public bool TupleInequality((int, int) t1, (int, int) t2)
		{
#if EXPECTED_OUTPUT && OPT
			(int, int) tuple = t1;
			(int, int) tuple2 = t2;
			if (tuple.Item1 == tuple2.Item1)
			{
				return tuple.Item2 != tuple2.Item2;
			}
			return true;
#elif EXPECTED_OUTPUT
			(int, int) tuple = t1;
			(int, int) tuple2 = t2;
			return tuple.Item1 != tuple2.Item1 || tuple.Item2 != tuple2.Item2;
#else
			return t1 != t2;
#endif
		}

		public bool TupleLiteralsDecayToElementComparisons(int a, int b, int c, int d)
		{
#if EXPECTED_OUTPUT && OPT
			if (a == c)
			{
				return b == d;
			}
			return false;
#elif EXPECTED_OUTPUT
			return a == c && b == d;
#else
			return (a, b) == (c, d);
#endif
		}

		public bool NestedTuples((int, int) ab, int c, (int, int) de, int f)
		{
#if EXPECTED_OUTPUT && OPT
			(int, int) tuple = ab;
			(int, int) tuple2 = de;
			if (tuple.Item1 == tuple2.Item1 && tuple.Item2 == tuple2.Item2)
			{
				return c == f;
			}
			return false;
#elif EXPECTED_OUTPUT
			(int, int) tuple = ab;
			(int, int) tuple2 = de;
			return tuple.Item1 == tuple2.Item1 && tuple.Item2 == tuple2.Item2 && c == f;
#else
			return (ab, c) == (de, f);
#endif
		}

		public bool MixedElementTypes((string, CustomEquality, int?, double, float) t1, (string, CustomEquality, int?, double, float) t2)
		{
#if EXPECTED_OUTPUT && OPT
			(string, CustomEquality, int?, double, float) tuple = t1;
			(string, CustomEquality, int?, double, float) tuple2 = t2;
			if (tuple.Item1 == tuple2.Item1 && tuple.Item2 == tuple2.Item2 && tuple.Item3 == tuple2.Item3 && tuple.Item4 == tuple2.Item4)
			{
				return tuple.Item5 == tuple2.Item5;
			}
			return false;
#elif EXPECTED_OUTPUT
			(string, CustomEquality, int?, double, float) tuple = t1;
			(string, CustomEquality, int?, double, float) tuple2 = t2;
			return tuple.Item1 == tuple2.Item1 && tuple.Item2 == tuple2.Item2 && tuple.Item3 == tuple2.Item3 && tuple.Item4 == tuple2.Item4 && tuple.Item5 == tuple2.Item5;
#else
			return t1 == t2;
#endif
		}

		public bool EightElements((int, int, int, int, int, int, int, int) t1, (int, int, int, int, int, int, int, int) t2)
		{
#if EXPECTED_OUTPUT && OPT
			(int, int, int, int, int, int, int, int) tuple = t1;
			(int, int, int, int, int, int, int, int) tuple2 = t2;
			if (tuple.Item1 == tuple2.Item1 && tuple.Item2 == tuple2.Item2 && tuple.Item3 == tuple2.Item3 && tuple.Item4 == tuple2.Item4 && tuple.Item5 == tuple2.Item5 && tuple.Item6 == tuple2.Item6 && tuple.Item7 == tuple2.Item7)
			{
				return tuple.Rest.Item1 == tuple2.Rest.Item1;
			}
			return false;
#elif EXPECTED_OUTPUT
			(int, int, int, int, int, int, int, int) tuple = t1;
			(int, int, int, int, int, int, int, int) tuple2 = t2;
			return tuple.Item1 == tuple2.Item1 && tuple.Item2 == tuple2.Item2 && tuple.Item3 == tuple2.Item3 && tuple.Item4 == tuple2.Item4 && tuple.Item5 == tuple2.Item5 && tuple.Item6 == tuple2.Item6 && tuple.Item7 == tuple2.Item7 && tuple.Rest.Item1 == tuple2.Rest.Item1;
#else
			return t1 == t2;
#endif
		}

		public bool OperandsEvaluatedOnlyOnce(Func<(int, int, int)> f1, Func<(int, int, int)> f2)
		{
#if EXPECTED_OUTPUT && OPT
			(int, int, int) tuple = f1();
			(int, int, int) tuple2 = f2();
			if (tuple.Item1 == tuple2.Item1 && tuple.Item2 == tuple2.Item2)
			{
				return tuple.Item3 == tuple2.Item3;
			}
			return false;
#elif EXPECTED_OUTPUT
			(int, int, int) tuple = f1();
			(int, int, int) tuple2 = f2();
			return tuple.Item1 == tuple2.Item1 && tuple.Item2 == tuple2.Item2 && tuple.Item3 == tuple2.Item3;
#else
			return f1() == f2();
#endif
		}

		public bool NullableTupleComparedToNull((int, int)? t1)
		{
#if EXPECTED_OUTPUT
			return !t1.HasValue;
#else
			return t1 == null;
#endif
		}

		public bool NullComparedToNullableTuple((int, int)? t1)
		{
#if EXPECTED_OUTPUT
			return t1.HasValue;
#else
			return null != t1;
#endif
		}

		public void UsedInCondition((int, int) t1, (int, int) t2)
		{
#if EXPECTED_OUTPUT
			(int, int) tuple = t1;
			(int, int) tuple2 = t2;
			if (tuple.Item1 == tuple2.Item1 && tuple.Item2 == tuple2.Item2)
			{
				Console.WriteLine("equal");
			}
#else
			if (t1 == t2)
			{
				Console.WriteLine("equal");
			}
#endif
		}
	}
}
