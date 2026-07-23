using System;
using System.Runtime.InteropServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	internal class NoLambdaOptionalAndParamsParameters
	{
		public delegate void ParamsAction(params int[] xs);

		public delegate int OptionalFunc(int x = 5);

		private int total;

		public ParamsAction ParamsLambda()
		{
			return ([ParamArray] int[] xs) => {
				total += xs.Length;
				Console.WriteLine(xs.Length);
			};
		}

		public OptionalFunc OptionalLambda()
		{
			return ([Optional][DefaultParameterValue(5)] int x) => x * 2;
		}
	}
}
