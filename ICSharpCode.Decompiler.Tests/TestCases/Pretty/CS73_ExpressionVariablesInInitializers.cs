using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class CS73_ExpressionVariablesInInitializers
	{
		public class FieldInits
		{
			public static string S = "42";

			public static object Box = 42;

			public int F = (int.TryParse(S, out var result) ? result : (-1));

			public static int SF = (int.TryParse(S, out var result) ? result : (-1));

			public object O = ((Box is int num) ? num : 0);

			public int Multi = ((int.TryParse(S, out var result2) && int.TryParse(S, out var result3)) ? (result2 + result3) : (-1));

			public int P { get; set; } = int.TryParse(S, out var result4) ? result4 : (-1);
		}

		public class MultipleCtors
		{
			public static string S = "42";

			public int F = (int.TryParse(S, out var result) ? result : (-1));

			public MultipleCtors()
			{
				Console.WriteLine("a");
			}

			public MultipleCtors(int x)
			{
				Console.WriteLine(x);
			}
		}

		public class GenericFieldInit<T> where T : class
		{
			public static Dictionary<string, T> Map = new Dictionary<string, T>();

			public T Value = (Map.TryGetValue("k", out var value) ? value : null);
		}

		public class CapturedInFieldInit
		{
			public static string S = "42";

			public Func<int> FL = (int.TryParse(S, out var f) ? ((Func<int>)(() => f)) : null);
		}

		public class NestedOutVars
		{
			public static string S = "42";

			public int N = ((int.TryParse(S, out var result) && TryTransform(result, out var output)) ? output : result);

			public static bool TryTransform(int input, out int output)
			{
				output = input * 2;
				return true;
			}
		}

		public class SingleDiscards
		{
			public static string S = "42";

			public bool Ok = int.TryParse(S, out var _);

			public bool Ok2 = double.TryParse(S, out var _);

			public static bool StaticOk = int.TryParse(S, out var _);

			public static bool StaticOk2 = long.TryParse(S, out var _);
		}

		public class BaseClass
		{
			public BaseClass(int a)
			{
			}

			public BaseClass(bool b)
			{
			}

			public BaseClass(int a, int b)
			{
			}

			public BaseClass(int a, Func<int> b)
			{
			}
		}

		public class CtorInits : BaseClass
		{
			public static int M(out int x)
			{
				x = 42;
				return 1;
			}

			public CtorInits(string s)
				: base(M(out var x), x)
			{
			}

			public CtorInits(double d)
				: base(int.TryParse(d.ToString(), out var result) ? result : 0, result)
			{
			}

			public CtorInits(object o)
				: base((o is int num) ? num : 0, (o is string text) ? text.Length : (-1))
			{
			}

			public CtorInits(char c)
				: base(int.TryParse(c.ToString(), out var _))
			{
			}

			public CtorInits(byte b)
				: base(M(out var x), () => x)
			{
			}
		}

		public class UsedInBody : BaseClass
		{
			public int Y;

			public UsedInBody(string s)
				: base(int.TryParse(s, out var result) ? result : 0)
			{
				Y = result;
			}
		}

		public class PatternUsedInBody : BaseClass
		{
			public int Y;

			public PatternUsedInBody(object o)
				: base((o is string text) ? text.Length : 0)
			{
				Y = ((o is int num) ? num : (-1));
			}
		}

		public struct StructThisChain
		{
			public int A;

			public StructThisChain(string s)
				: this(int.TryParse(s, out var result) ? result : 0)
			{
			}

			public StructThisChain(int a)
			{
				A = a;
			}
		}

		public class Queries
		{
			public IEnumerable<int> LetClause(IEnumerable<string> xs)
			{
				return from x in xs
					   let val = int.TryParse(x, out var result) ? result : 0
					   select val + 1;
			}

			public IEnumerable<int> WhereClause(IEnumerable<string> xs)
			{
				return from x in xs
					   where int.TryParse(x, out var _)
					   select x.Length;
			}

			public IEnumerable<int> FromClause(IEnumerable<string> xs)
			{
#if OPT
				return from x in xs
					   from c in int.TryParse(x, out var result) ? new int[1] { result } : new int[0]
					   select c;
#else
				return from x in xs
					   from c in (!int.TryParse(x, out var result)) ? new int[0] : new int[1] { result }
					   select c;
#endif
			}

			public IEnumerable<int> GroupByClause(IEnumerable<string> xs)
			{
				return from x in xs
					   group x by int.TryParse(x, out var result) ? result : 0 into g
					   select g.Key;
			}

			public IEnumerable<int> JoinClause(IEnumerable<string> xs, IEnumerable<int> ys)
			{
				return from x in xs
					   join y in ys on int.TryParse(x, out var result) ? result : 0 equals y
					   select y;
			}

			public IEnumerable<int> PatternInWhere(IEnumerable<object> xs)
			{
				return from x in xs
					   where x is string text && text.Length > 2
					   select ((string)x).Length;
			}

			public IEnumerable<int> LetPattern(IEnumerable<object> xs)
			{
				return from x in xs
					   let len = (x is string text) ? text.Length : (-1)
					   select len;
			}

			public IEnumerable<int> SelectMethodChain(IEnumerable<string> xs)
			{
				return xs.Select((string x) => int.TryParse(x, out var result) ? result : 0);
			}
		}
	}
}
