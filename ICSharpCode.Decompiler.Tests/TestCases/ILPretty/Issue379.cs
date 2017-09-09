using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
    internal class Issue379
    {
		public virtual void Test<T>() where T : new()
		{
		}
	}
}
