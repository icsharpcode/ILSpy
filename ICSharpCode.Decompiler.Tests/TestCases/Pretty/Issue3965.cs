using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class Issue3965
	{
		private static void M(int x)
		{
		}

		private static void Use(Action<int> action)
		{
		}

		public static void DiscardedMethodGroupConversion()
		{
#if EXPECTED_OUTPUT
			new Action<int>(M);
#else
			_ = (Action<int>)M;
#endif
		}

		public static void UsedMethodGroupConversion()
		{
			Use(M);
		}
	}
}
