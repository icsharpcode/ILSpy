using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	// Pins that out variables flowing through short-circuit operators, conditional access,
	// loops and exception filters decompile to recompilable code. The C# 10 "improved
	// definite assignment" rules (https://github.com/dotnet/csharplang/issues/4465) made
	// several of these flow shapes legal without dummy initializers; the decompiler must
	// never produce output where an out variable's assignment is unprovable (CS0165).
	public class OutVariableFlows
	{
		public class Container
		{
			private readonly Dictionary<string, int> map = new Dictionary<string, int>();

			public bool TryGet(string key, out int value)
			{
				return map.TryGetValue(key, out value);
			}
		}

		public class GenericSource
		{
			public bool TryGet<T>(string key, out T value)
			{
				value = default(T);
				return key != null;
			}
		}

		public struct StructValue
		{
			public int A;

			public string B;
		}

		public class Provider
		{
			public bool TryGetStruct(out StructValue value)
			{
				value = new StructValue {
					A = 1,
					B = "x"
				};
				return true;
			}
		}

		public static int GuardedUse(Container c, string key)
		{
			if (c != null && c.TryGet(key, out var value))
			{
				return value;
			}
			return -1;
		}

		public static int NegatedGuardEarlyReturn(Container c, string key)
		{
			if (c == null || !c.TryGet(key, out var value))
			{
				return -1;
			}
			return value;
		}

		public static int TernaryUse(Container c, string key)
		{
#if OPT
			if (c == null || !c.TryGet(key, out var value))
			{
				return -1;
			}
			return value;
#else
			int value;
			return (c != null && c.TryGet(key, out value)) ? value : (-1);
#endif
		}

		public static int WhileCondition(Container c)
		{
			int num = 0;
			int num2 = 0;
			int value;
			while (c.TryGet(num2.ToString(), out value))
			{
				num += value;
				num2++;
			}
			return num;
		}

		public static int ForCondition(Dictionary<int, int> d)
		{
			int num = 0;
			int value;
			for (int i = 0; d.TryGetValue(i, out value); i++)
			{
				if (value <= 0)
				{
					break;
				}
				num += value;
			}
			return num;
		}

		public static int NestedShortCircuit(Container a, Container b, string key)
		{
			if (a.TryGet(key, out var value) && b.TryGet(key, out var value2))
			{
				return value + value2;
			}
			return 0;
		}

		public static int MixedAndOr(Container a, Container b, string key)
		{
			if ((a != null && a.TryGet(key, out var value)) || (b != null && b.TryGet(key, out value)))
			{
				return value;
			}
			return -1;
		}

		public static string StructGuardedUse(Provider p)
		{
			if (p != null && p.TryGetStruct(out var value))
			{
				return value.B + value.A;
			}
			return null;
		}

		public static int TryParseChain(string s1, string s2)
		{
			if (int.TryParse(s1, out var result) && int.TryParse(s2, out var result2))
			{
				return result + result2;
			}
#if OPT
			if (!int.TryParse(s1, out var result3))
			{
				return 0;
			}
			return result3;
#else
			int result3;
			return int.TryParse(s1, out result3) ? result3 : 0;
#endif
		}

		public static string CoalesceAssignTernary(Dictionary<string, string> d, string key)
		{
			string text = null;
			if (text == null)
			{
				text = (d.TryGetValue(key, out var value) ? value : "missing");
			}
			return text;
		}

		public static int LiftedNotCoalesce(Container c, string key)
		{
			// The dummy initializer is required in the decompiled output: with this expression
			// shape the C# 10 improved definite assignment rules do not cover '(!(x)) ?? true',
			// so dropping the initializer would make this method uncompilable (CS0165).
			// The input uses 'is not true' because recompiling the lifted-negation form does
			// not reproduce the nullable-lifted IL this output shape comes from.
#if EXPECTED_OUTPUT
			int value = default(int);
			if ((!(c?.TryGet(key, out value))) ?? true)
#else
			if (c?.TryGet(key, out var value) is not true)
#endif
			{
				return -1;
			}
			return value;
		}

		public static int CatchFilter(Action a)
		{
			int result;
			try
			{
				a();
				return 0;
			}
			catch (Exception ex) when (int.TryParse(ex.Message, out result))
			{
				return result;
			}
		}

		public static int TryCatchEarlyReturn(string s)
		{
			try
			{
				if (!int.TryParse(s, out var result))
				{
					return -1;
				}
				return result;
			}
			catch (Exception)
			{
				return -2;
			}
		}

		public static T GenericGuardedUse<T>(GenericSource s, string key)
		{
			if (s != null && s.TryGet<T>(key, out var value))
			{
				return value;
			}
			return default(T);
		}

		public static int IsPatternCombined(object o, string key)
		{
			if (o is Container container && container.TryGet(key, out var value))
			{
				return value;
			}
			return -1;
		}

		public static Func<int> CapturedInClosure(Container c, string key)
		{
			if (c != null && c.TryGet(key, out var value))
			{
				return () => value;
			}
			return null;
		}

		public static Func<int> LambdaConditional(Dictionary<int, int> d, int key)
		{
#if OPT
#if EXPECTED_OUTPUT
			return () => (d != null && d.TryGetValue(key, out var value)) ? value : (-1);
#else
			return () => (d == null || !d.TryGetValue(key, out var value)) ? (-1) : value;
#endif
#else
			return delegate {
				Dictionary<int, int> dictionary = d;
				int value;
				return (dictionary != null && dictionary.TryGetValue(key, out value)) ? value : (-1);
			};
#endif
		}
	}
}
