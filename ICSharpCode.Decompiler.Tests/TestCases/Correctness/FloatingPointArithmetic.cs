using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class FloatingPointArithmetic
	{
		public static int Main(string[] args)
		{
			Issue999();
			Issue1656();
			return 0;
		}

		static void Issue999()
		{
			for (float i = -10f; i <= 10f; i += 0.01f) 
				Console.WriteLine("{1:R}: {0:R}", M(i), i);
		}

		static float M(float v)
		{
			return 0.99f * v + 0.01f;
		}

		static void Issue1656()
		{
			double primary = 'B';
			CxAssert((++primary) == 'C');
			CxAssert((--primary) == 'B');
			CxAssert((primary++) == 'B');
			CxAssert((primary--) == 'C');
		}

		static void CxAssert(bool v)
		{
			if (!v) {
				throw new InvalidOperationException();
			}
		}
	}
}
