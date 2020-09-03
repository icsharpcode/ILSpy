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

		public void GetOutOverloaded(out int value)
		{
			value = 0;
		}

		public void GetOutOverloaded(out string value)
		{
			value = "Hello World";
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
			GetOut(out var _);
		}

		public void DiscardedOutVsLambdaParameter()
		{
			GetOut(out var _);
			MakeValue((@_ _) => 5);
		}

		public void ExplicitlyTypedDiscard()
		{
			GetOutOverloaded(out string _);
			GetOutOverloaded(out int _);
		}
	}
}
