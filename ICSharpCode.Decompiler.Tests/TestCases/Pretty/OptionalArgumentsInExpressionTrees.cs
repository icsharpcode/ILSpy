using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class OptionalArgumentsInExpressionTrees
	{
		public delegate int OptionalDelegate(int x = 21);

		public interface IWithOptional
		{
			int IfaceM(int x = 9);
		}

		public enum MyEnum
		{
			A = 0,
			B = 42
		}

		public class WithOptional
		{
			public int this[int x, int y = 10] => x + y;

			public WithOptional(int x = 5, string s = "ctor")
			{
			}

			public static int StaticM(int a, int b = 7, string s = "hello", decimal d = 1.5m, MyEnum e = MyEnum.B, CancellationToken ct = default(CancellationToken))
			{
				return a + b;
			}

			public int InstanceM(int a = 1, double d = 2.5)
			{
				return a;
			}

			public static int TwoArgs(int a, int b)
			{
				return a - b;
			}
		}

		public class Overloaded
		{
			public static int Ambig(int x)
			{
				return x;
			}

			public static int Ambig(int x, int y = 0)
			{
				return x + y;
			}

			public static T GenericM<T>(T value, int count = 3)
			{
				return value;
			}

			public static int NonTrailing(int a = 1, int b = 2)
			{
				return a + b;
			}
		}

		public class CornerDefaults
		{
			public static int StringNull(int a, string s = null)
			{
				return a;
			}

			public static int NullableNull(int a, int? n = null)
			{
				return a;
			}

			public static int NullableInt(int a, int? n = 5)
			{
				return a;
			}

			public static int DoubleNaN(int a, double d = double.NaN)
			{
				return a;
			}

			public static int DateTimeDefault(int a, DateTime dt = default(DateTime))
			{
				return a;
			}

			public static int ObjectNull(int a, object o = null)
			{
				return a;
			}

			public static int FloatValue(int a, float f = 2.5f)
			{
				return a;
			}

			public static int LongMax(int a, long l = long.MaxValue)
			{
				return a;
			}
		}

		public Expression<Func<int>> OmitAllOptional = () => WithOptional.StaticM(1);

		public Expression<Func<int>> SupplySomeOptional = () => WithOptional.StaticM(1, 2);

#if EXPECTED_OUTPUT
		public Expression<Func<int>> NamedInOrder = () => WithOptional.StaticM(1, 3);
#else
		public Expression<Func<int>> NamedInOrder = () => WithOptional.StaticM(1, b: 3);
#endif

#if EXPECTED_OUTPUT
		public Expression<Func<int>> NamedThenOmitted = () => WithOptional.StaticM(1, 2, "world");
#else
		public Expression<Func<int>> NamedThenOmitted = () => WithOptional.StaticM(1, 2, s: "world");
#endif

#if EXPECTED_OUTPUT
		public Expression<Func<int>> AllNamedInPosition = () => WithOptional.TwoArgs(2, 1);
#else
		public Expression<Func<int>> AllNamedInPosition = () => WithOptional.TwoArgs(a: 2, b: 1);
#endif

		public Expression<Func<WithOptional, int>> InstanceOmitAllOptional = (WithOptional w) => w.InstanceM();

#if EXPECTED_OUTPUT
		public Expression<Func<WithOptional, int>> NamedIndexerArgument = (WithOptional w) => w[1, 5];
#else
		public Expression<Func<WithOptional, int>> NamedIndexerArgument = (WithOptional w) => w[1, y: 5];
#endif

		public Expression<Func<WithOptional>> CtorOmitAllOptional = () => new WithOptional();

#if EXPECTED_OUTPUT
		public Expression<Func<WithOptional>> CtorNamedArgument = () => new WithOptional(5, "x");
#else
		public Expression<Func<WithOptional>> CtorNamedArgument = () => new WithOptional(5, s: "x");
#endif

		public Expression<Func<string, int>> ExtensionOmitAllOptional = (string s) => s.ExtM();

#if EXPECTED_OUTPUT
		public Expression<Func<List<int>, int, bool>> ContainsNamedComparer = (List<int> a, int i) => a.Contains(i, null);
#else
		public Expression<Func<List<int>, int, bool>> ContainsNamedComparer = (List<int> a, int i) => a.Contains(i, comparer: null);
#endif

		public Expression<Func<int>> OverloadHazardKeepsDefault = () => Overloaded.Ambig(5, 0);

#if EXPECTED_OUTPUT
		public Expression<Func<int>> OverloadHazardNamed = () => Overloaded.Ambig(5, 0);
#else
		public Expression<Func<int>> OverloadHazardNamed = () => Overloaded.Ambig(5, y: 0);
#endif

		public Expression<Func<string>> GenericInferred = () => Overloaded.GenericM("x");

#if EXPECTED_OUTPUT
		public Expression<Func<int>> GenericExplicitTypeArgument = () => Overloaded.GenericM(1);
#else
		public Expression<Func<int>> GenericExplicitTypeArgument = () => Overloaded.GenericM<int>(1);
#endif

		public Expression<Func<int>> OmitTrailingOnly = () => Overloaded.NonTrailing(10);

		public Expression<Func<int>> ExplicitDefaultKeptForTrailingValue = () => Overloaded.NonTrailing(1, 20);

		public Expression<Func<OptionalDelegate, int>> DelegateInvokeOmitOptional = (OptionalDelegate d) => d();

		public Expression<Func<IWithOptional, int>> InterfaceOmitOptional = (IWithOptional w) => w.IfaceM();

		public Expression<Func<Func<int>>> NestedLambdaOmitOptional = () => () => Overloaded.GenericM(7);

		public Expression<Func<int>> OmitStringNull = () => CornerDefaults.StringNull(1);

		public Expression<Func<int>> OmitNullableNull = () => CornerDefaults.NullableNull(1);

#if EXPECTED_OUTPUT
		public Expression<Func<int>> OmitNullableInt = () => CornerDefaults.NullableInt(1, 5);
#else
		public Expression<Func<int>> OmitNullableInt = () => CornerDefaults.NullableInt(1);
#endif

		public Expression<Func<int>> OmitDoubleNaN = () => CornerDefaults.DoubleNaN(1);

		public Expression<Func<int>> OmitDateTimeDefault = () => CornerDefaults.DateTimeDefault(1);

		public Expression<Func<int>> OmitObjectNull = () => CornerDefaults.ObjectNull(1);

		public Expression<Func<int>> OmitFloatValue = () => CornerDefaults.FloatValue(1);

		public Expression<Func<int>> OmitLongMax = () => CornerDefaults.LongMax(1);
	}
	public static class OptionalArgumentsInExpressionTreesExtensions
	{
		public static int ExtM(this string s, int start = 0, int len = -1)
		{
			return s.Length;
		}
	}
}
