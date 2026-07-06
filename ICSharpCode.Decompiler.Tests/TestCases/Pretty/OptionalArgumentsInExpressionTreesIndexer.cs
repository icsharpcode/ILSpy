using System;
using System.Linq.Expressions;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class OptionalArgumentsInExpressionTreesIndexer
	{
		public class WithIndexer
		{
			public int this[int x, int y = 10] => x + y;
		}

		public Expression<Func<WithIndexer, int>> OmitOptionalIndexerArgument = (WithIndexer w) => w[1];

#if EXPECTED_OUTPUT
		public Expression<Func<WithIndexer, int>> ExplicitDefaultIndexerArgument = (WithIndexer w) => w[1];
#else
		public Expression<Func<WithIndexer, int>> ExplicitDefaultIndexerArgument = (WithIndexer w) => w[1, 10];
#endif
	}
}
