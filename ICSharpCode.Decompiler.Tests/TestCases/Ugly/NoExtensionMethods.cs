using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	internal static class NoExtensionMethods
	{
		internal static Func<T> AsFunc<T>(this T value) where T : class
		{
			return new Func<T>(value.Return);
		}

		private static T Return<T>(this T value)
		{
			return value;
		}
	}
}
