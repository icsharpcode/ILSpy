#pragma warning disable CS9099, CS9100
using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class LambdaOptionalAndParamsParameters
	{
		public delegate void ParamsAction(params int[] xs);

		public delegate int OptionalFunc(int x = 5);

		public delegate int PlainFunc(int x);

		// A lambda states 'params' and defaults on its own account: what the delegate declares
		// is the delegate's, and reflection over the lambda's method reports only the lambda's.
		// Roslyn records the modifier on the anonymous function's method from version 5 on, so
		// the cases that need it back are guarded on ROSLYN5.

		private int total;

#if ROSLYN5
		public ParamsAction ParamsStatementBody()
		{
			return (params int[] xs) => {
				total += xs.Length;
				Console.WriteLine(xs.Length);
			};
		}
#endif

#if ROSLYN5
		public ParamsAction ParamsSingleStatementBody()
		{
			return (params int[] xs) => {
				Console.WriteLine(xs.Length);
			};
		}
#endif

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

		public ParamsAction ParamsWithoutParameterList()
		{
			return delegate {
			};
		}

		public OptionalFunc OptionalWithoutParameterList()
		{
			return delegate {
				return 1;
			};
		}

		// The lambda's own defaults and params modifier can differ from the delegate's; the metadata
		// records the lambda's, so that is what must round-trip.
		public OptionalFunc OptionalDifferentDefault()
		{
			return (int x = 7) => x * 2;
		}

		public OptionalFunc OptionalOnlyInDelegate()
		{
			return (int x) => x * 2;
		}

		public PlainFunc OptionalOnlyInLambda()
		{
			return (int x = 3) => x * 2;
		}

		public ParamsAction ParamsOnlyInDelegate()
		{
			return (int[] xs) => {
				Console.WriteLine(xs.Length);
			};
		}

#if ROSLYN5
		public Action<int[]> ParamsOnlyInLambda()
		{
			return (params int[] xs) => {
				Console.WriteLine(xs.Length);
			};
		}
#endif
	}
}
