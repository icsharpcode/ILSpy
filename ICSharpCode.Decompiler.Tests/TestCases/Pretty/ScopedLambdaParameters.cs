using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class ScopedLambdaParameters
	{
		public delegate Span<int> ScopedRefFunc(scoped ref int x);

		public ScopedRefFunc UnusedScopedRefParameter()
		{
			// The parameter list must not be dropped here: an anonymous function
			// without a parameter list has unscoped implicit parameters, which do
			// not match the delegate's "scoped ref" parameter (CS8986).
			return delegate (scoped ref int x) {
				return default(Span<int>);
			};
		}
	}
}
