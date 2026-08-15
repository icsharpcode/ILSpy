using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	internal class NoLambdaOptionalAndParamsParameters
	{
		public delegate void ParamsAction(params int[] xs);

		public delegate int OptionalFunc(int x = 5);

		private int total;

		public ParamsAction ParamsLambda()
		{
			return (int[] xs) => {
				total += xs.Length;
				Console.WriteLine(xs.Length);
			};
		}

		public OptionalFunc OptionalLambda()
		{
			return (int x) => x * 2;
		}
	}
}
