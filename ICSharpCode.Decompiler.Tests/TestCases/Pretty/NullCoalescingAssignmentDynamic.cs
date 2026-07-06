namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class NullCoalescingAssignmentDynamic
	{
		public static void Use(object x)
		{
		}

		// Null-coalescing assignment on dynamic targets stays in lowered form
		// on purpose; see https://github.com/icsharpcode/ILSpy/issues/2552.
		public void DynamicStatementString(dynamic d, string b)
		{
#if EXPECTED_OUTPUT
			if ((object)d.X == null)
			{
				d.X = b;
			}
#else
			d.X ??= b;
#endif
		}

		public void DynamicStatementInt(dynamic d)
		{
#if EXPECTED_OUTPUT
			if ((object)d.Y == null)
			{
				d.Y = 42;
			}
#else
			d.Y ??= 42;
#endif
		}

		public void DynamicExpressionString(dynamic d, string b)
		{
#if EXPECTED_OUTPUT
			Use(d.X ?? (d.X = b));
#else
			Use(d.X ??= b);
#endif
		}

		public void DynamicExpressionInt(dynamic d)
		{
#if EXPECTED_OUTPUT
			Use(d.Y ?? (d.Y = 42));
#else
			Use(d.Y ??= 42);
#endif
		}
	}
}
