using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class SpanConversionOperatorMismatch
	{
		public static implicit operator ReadOnlySpan<char>(object o)
		{
			return default(ReadOnlySpan<char>);
		}

		public static ReadOnlySpan<char> ConvertString(string s)
		{
			return (ReadOnlySpan<char>)(object)s;
		}
	}
}
