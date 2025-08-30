using System;
using System.Collections.Generic;

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
		}
	}
}
