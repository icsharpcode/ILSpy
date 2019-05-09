using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class Discards
	{
		public class @_
		{

		}

		public void GetOut(out int value)
		{
			value = 0;
		}

		public void MakeValue(Func<object, string, int> func)
		{

		}

		public void MakeValue(Func<@_, int> func)
		{

		}

		public void SimpleParameter(@_ _)
		{
		}

		public void ParameterHiddenByLocal(@_ _)
		{
			GetOut(out int _);
		}

		public void DiscardedOutVsLambdaParameter()
		{
			GetOut(out int _);
			MakeValue((@_ _) => 5);
		}
	}
}
