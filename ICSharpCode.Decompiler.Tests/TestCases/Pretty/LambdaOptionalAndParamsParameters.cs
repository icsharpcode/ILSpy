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
#if CS140
			return (params int[] xs) => {
				total += xs.Length;
				Console.WriteLine(xs.Length);
			};
#elif EXPECTED_OUTPUT
			// Roslyn 4.14 does not emit ParamArrayAttribute on the lambda's method, so the
			// 'params' modifier cannot be recovered from metadata.
			return delegate (int[] xs) {
				total += xs.Length;
				Console.WriteLine(xs.Length);
			};
#else
			return (params int[] xs) => {
				total += xs.Length;
				Console.WriteLine(xs.Length);
			};
#endif
		}

		public ParamsAction ParamsExpressionBody()
		{
#if CS140
			return (params int[] xs) => {
				Console.WriteLine(xs.Length);
			};
#elif EXPECTED_OUTPUT
			return delegate (int[] xs) {
				Console.WriteLine(xs.Length);
			};
#else
			return (params int[] xs) => Console.WriteLine(xs.Length);
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
