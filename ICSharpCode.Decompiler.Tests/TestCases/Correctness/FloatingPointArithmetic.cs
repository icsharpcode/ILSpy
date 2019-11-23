using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class FloatingPointArithmetic
	{
		public static int Main(string[] args)
		{
			Issue999();
			Issue1656();
			Issue1794();
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

		static void Issue1794()
		{
			Console.WriteLine("CastUnsignedToFloat:");
			Console.WriteLine(CastUnsignedToFloat(9007199791611905).ToString("r"));
			Console.WriteLine("CastUnsignedToDouble:");
			Console.WriteLine(CastUnsignedToDouble(9007199791611905).ToString("r"));
			Console.WriteLine("CastUnsignedToFloatViaDouble:");
			Console.WriteLine(CastUnsignedToFloatViaDouble(9007199791611905).ToString("r"));

			Console.WriteLine("CastSignedToFloat:");
			Console.WriteLine(CastSignedToFloat(9007199791611905).ToString("r"));
			Console.WriteLine("ImplicitCastSignedToFloat:");
			Console.WriteLine(ImplicitCastSignedToFloat(9007199791611905).ToString("r"));
			Console.WriteLine("CastSignedToDouble:");
			Console.WriteLine(CastSignedToDouble(9007199791611905).ToString("r"));
			Console.WriteLine("CastSignedToFloatViaDouble:");
			Console.WriteLine(CastSignedToFloatViaDouble(9007199791611905).ToString("r"));
		}

		static float CastUnsignedToFloat(ulong val)
		{
			return (float)val;
		}

		static double CastUnsignedToDouble(ulong val)
		{
			return (double)val;
		}

		static float CastUnsignedToFloatViaDouble(ulong val)
		{
			// The double-rounding can increase the rounding error
			return (float)(double)val;
		}

		static float CastSignedToFloat(long val)
		{
			return (float)val;
		}


		static double CastSignedToDouble(long val)
		{
			return (double)val;
		}

		static float CastSignedToFloatViaDouble(long val)
		{
			// The double-rounding can increase the rounding error
			return (float)(double)val;
		}

		static float ImplicitCastSignedToFloat(long val)
		{
			return val;
		}

		static void CxAssert(bool v)
		{
			if (!v) {
				throw new InvalidOperationException();
			}
		}
	}
}
