using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class LambdaOptionalAndParamsParameters
	{
		public delegate void ParamsAction(params int[] xs);

		public delegate int OptionalFunc(int x = 5);

		private int total;

		// Roslyn 4.14 puts no ParamArrayAttribute on the lambda's own method, so for a named
		// delegate type the 'params' cannot be recovered there; newer compilers record it.
		public ParamsAction ParamsStatementBody()
		{
#if ROSLYN5 || !EXPECTED_OUTPUT
			return (params int[] xs) => {
#else
			return (int[] xs) => {
#endif
				total += xs.Length;
				Console.WriteLine(xs.Length);
			};
		}

		public ParamsAction ParamsExpressionBody()
		{
#if ROSLYN5 || !EXPECTED_OUTPUT
			return (params int[] xs) => Console.WriteLine(xs.Length);
#else
			return (int[] xs) => Console.WriteLine(xs.Length);
#endif
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
