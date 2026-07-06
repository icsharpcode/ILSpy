namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal struct CallSiteLogical
	{
		public int Value;
	}

	internal struct CallSiteValue
	{
		public int Value;
	}

	internal static class ExtensionOperatorCallSites
	{
		extension(CallSiteLogical)
		{
			public static CallSiteLogical operator &(CallSiteLogical a, CallSiteLogical b)
			{
				return default(CallSiteLogical);
			}

			public static CallSiteLogical operator |(CallSiteLogical a, CallSiteLogical b)
			{
				return default(CallSiteLogical);
			}

			public static bool operator true(CallSiteLogical a)
			{
				return a.Value != 0;
			}

			public static bool operator false(CallSiteLogical a)
			{
				return a.Value == 0;
			}
		}

		extension(CallSiteValue)
		{
			public static CallSiteValue operator +(CallSiteValue a, CallSiteValue b)
			{
				return default(CallSiteValue);
			}

			public static CallSiteValue operator ++(CallSiteValue a)
			{
				return default(CallSiteValue);
			}

			public static CallSiteValue operator --(CallSiteValue a)
			{
				return default(CallSiteValue);
			}

			public static CallSiteValue operator checked ++(CallSiteValue a)
			{
				return default(CallSiteValue);
			}
		}
	}

	internal static class ExtensionOperatorCallSitesUseSites
	{
		public static CallSiteLogical AndAlso(CallSiteLogical x, CallSiteLogical y)
		{
			return x && y;
		}

		public static CallSiteLogical OrElse(CallSiteLogical x, CallSiteLogical y)
		{
			return x || y;
		}

		public static void Increment(ref CallSiteValue x)
		{
			x++;
		}

		public static void Decrement(ref CallSiteValue x)
		{
			x--;
		}

		public static void IncrementChecked(ref CallSiteValue x)
		{
			checked
			{
				x++;
			}
		}

		public static CallSiteValue? LiftedAdd(CallSiteValue? x, CallSiteValue? y)
		{
			return x + y;
		}
	}
}
