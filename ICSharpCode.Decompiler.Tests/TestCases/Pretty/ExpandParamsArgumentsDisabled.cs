using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class ExpandParamsArgumentsDisabled
	{
		public void Test()
		{
			MethodWithParams(Array.Empty<int>());
			MethodWithParams(new int[1] { 5 });
		}

		public void MethodWithParams(params int[] b)
		{
		}
	}
}
