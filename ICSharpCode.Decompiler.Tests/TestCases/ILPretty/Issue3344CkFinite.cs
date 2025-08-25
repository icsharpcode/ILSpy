using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class Issue3344
	{
		private static float GetFloat()
		{
			return 3.5f;
		}

		private static float CkFinite()
		{
			float num = GetFloat();
			if (!float.IsFinite(num))
			{
				throw new ArithmeticException();
			}
			return num;
		}
	}
}