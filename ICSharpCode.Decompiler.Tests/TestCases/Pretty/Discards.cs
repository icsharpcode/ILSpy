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

		// out _ is currently not supported: the test cases below are not useful
		//public void ParameterHiddenByLocal(@_ _)
		//{
		//	GetOut(out int value);
		//}

		//public void DiscardedOutVsLambdaParameter()
		//{
		//	GetOut(out int value);
		//	MakeValue((@_ _) => 5);
		//}
	}
}
