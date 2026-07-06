using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	// Desired output for out variables whose assignment is only provable via the C# 10
	// "improved definite assignment" rules (https://github.com/dotnet/csharplang/issues/4465):
	// a conditional-access invocation compared with a bool constant ('== true') or coalesced
	// with 'false' guarantees assignment on the true branch, so no dummy initializer and no
	// hoisted declaration are needed. The decompiler currently emits
	// 'T value = default(T);' before each of these statements and passes 'out value' instead
	// of declaring the variable inline. The EXPECTED_OUTPUT branches exist where the input
	// must use a different (but IL-equivalent) expression form to produce the nullable-lifted
	// IL that keeps the conditional access in the decompiled output.
	public class ImprovedDefiniteAssignment
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

		public class Wrapper
		{
			public Container Inner;

			public Dictionary<int, int> Data;
		}

		public static int CoalesceOutVar(Container c, string key)
		{
#if EXPECTED_OUTPUT
			if (c?.TryGet(key, out var value) ?? false)
#else
			if (c?.TryGet(key, out var value) is true)
#endif
			{
				return value;
			}
			return -1;
		}

		public static T GenericCoalesceOutVar<T>(GenericSource s, string key)
		{
#if EXPECTED_OUTPUT
			if (s?.TryGet<T>(key, out var value) ?? false)
#else
			if (s?.TryGet<T>(key, out var value) is true)
#endif
			{
				return value;
			}
			return default(T);
		}

		public static string StructCoalesceOutVar(Provider p)
		{
#if EXPECTED_OUTPUT
			if (p?.TryGetStruct(out var value) ?? false)
#else
			if (p?.TryGetStruct(out var value) is true)
#endif
			{
				return value.B + value.A;
			}
			return null;
		}

		public static int ChainedConditionalOutVar(Wrapper w, string key)
		{
#if EXPECTED_OUTPUT
			if (w != null && w.Inner?.TryGet(key, out var value) == true)
#else
			if (w?.Inner?.TryGet(key, out var value) == true)
#endif
			{
				return value;
			}
			return -1;
		}

		public static int WhileChainedOutVar(Wrapper w)
		{
			int num = 0;
			int num2 = 0;
#if EXPECTED_OUTPUT
			while (w != null && w.Data?.TryGetValue(num2, out var value) == true)
#else
			while (w?.Data?.TryGetValue(num2, out var value) == true)
#endif
			{
				num += value;
				num2++;
			}
			return num;
		}
	}
}
