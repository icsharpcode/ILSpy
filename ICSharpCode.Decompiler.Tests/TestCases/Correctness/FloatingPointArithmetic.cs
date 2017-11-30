using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class FloatingPointArithmetic
	{
		public static int Main(string[] args)
		{
			Issue999();
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
	}
}
