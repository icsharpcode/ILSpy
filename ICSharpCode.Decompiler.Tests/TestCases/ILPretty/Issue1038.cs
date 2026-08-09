using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class Issue1038<TK, TR> where TR : class, new()
	{
		public event Action<TK, TR> TestEvent = (TK A_0, TR A_1) => {
		};
	}
}
