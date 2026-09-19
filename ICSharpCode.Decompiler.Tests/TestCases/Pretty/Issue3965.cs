using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class Issue3965
	{
		private static void M(int x)
		{
		}

		private static int GetValue()
		{
			return 42;
		}

		private static bool GetCondition()
		{
			return true;
		}

		private static void Use(Action<int> action)
		{
		}

		private static void Use(Func<int> func)
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

		public static void DiscardedMethodGroupConversionBeforeUse()
		{
#if EXPECTED_OUTPUT
			new Action<int>(M);
			Use(M);
#else
			_ = (Action<int>)M;
			Use(M);
#endif
		}

		public static void DiscardedMethodGroupConversionAfterUse()
		{
#if EXPECTED_OUTPUT
			Use(M);
			new Action<int>(M);
#else
			Use(M);
			_ = (Action<int>)M;
#endif
		}

		public static void MultipleDiscardedMethodGroupConversions()
		{
#if EXPECTED_OUTPUT
			new Action<int>(M);
			new Func<int>(GetValue);
#else
			_ = (Action<int>)M;
			_ = (Func<int>)GetValue;
#endif
		}

		public static void DiscardUseDiscardMethodGroupConversion()
		{
#if EXPECTED_OUTPUT
			new Action<int>(M);
			Use(M);
			new Action<int>(M);
#else
			_ = (Action<int>)M;
			Use(M);
			_ = (Action<int>)M;
#endif
		}

		public static void TwoDiscardedMethodGroupConversionsToSameMethod()
		{
#if EXPECTED_OUTPUT
			new Action<int>(M);
			new Action<int>(M);
#else
			_ = (Action<int>)M;
			_ = (Action<int>)M;
#endif
		}

		public static void DiscardedMethodGroupConversionInCondition()
		{
#if EXPECTED_OUTPUT
			new Action<int>(M);
			if (GetCondition())
			{
				new Action<int>(M);
			}
#else
			_ = (Action<int>)M;
			if (GetCondition())
			{
				_ = (Action<int>)M;
			}
#endif
		}

		public static void DiscardedMethodGroupConversionsInForLoop()
		{
			for (int i = 0; i < 2; i++)
			{
#if EXPECTED_OUTPUT
				new Action<int>(M);
				new Action<int>(M);
#else
				_ = (Action<int>)M;
				_ = (Action<int>)M;
#endif
			}
		}
	}
}
