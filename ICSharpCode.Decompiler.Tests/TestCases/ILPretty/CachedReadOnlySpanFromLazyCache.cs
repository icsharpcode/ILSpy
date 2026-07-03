using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public static class CachedReadOnlySpanFromLazyCache
	{
		public static ReadOnlySpan<char> NewLine {
			get {
				return new ReadOnlySpan<char>(new char[2] { '\r', '\n' });
			}
		}
	}
}
