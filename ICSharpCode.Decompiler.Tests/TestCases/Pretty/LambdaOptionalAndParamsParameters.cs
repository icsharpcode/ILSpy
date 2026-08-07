using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class LambdaOptionalAndParamsParameters
	{
		public delegate void ParamsAction(params int[] xs);

		public delegate int OptionalFunc(int x = 5);

		private int total;

		public ParamsAction ParamsStatementBody()
		{
			return (params int[] xs) => {
				total += xs.Length;
				Console.WriteLine(xs.Length);
			};
		}

		public ParamsAction ParamsExpressionBody()
		{
			return (params int[] xs) => Console.WriteLine(xs.Length);
		}

		public OptionalFunc OptionalStatementBody()
		{
			return (int x = 5) => {
				total += x;
				return x * 2;
			};
		}

		public OptionalFunc OptionalExpressionBody()
		{
			return (int x = 5) => x * 2;
		}
	}
}
