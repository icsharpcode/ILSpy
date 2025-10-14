using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public static class ParamsCollections
	{
		public static void ParamsEnumerable(params IEnumerable<int> values)
		{
		}
		public static void ParamsList(params List<int> values)
		{
		}
		public static void ParamsReadOnlySpan(params ReadOnlySpan<int> values)
		{
		}
		public static void ParamsSpan(params Span<int> values)
		{
			// note: implicitly "scoped", "params scoped Span<int> values" is allowed
			// but "scoped" is always redundant for params.
		}
		public static void ParamUnscopedSpan([UnscopedRef] params Span<int> values)
		{
		}
	}
}
