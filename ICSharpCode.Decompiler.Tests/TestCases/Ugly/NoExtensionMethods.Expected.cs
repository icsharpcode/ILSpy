using System;
using System.Runtime.CompilerServices;

[assembly: Extension]

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	[Extension]
	internal static class NoExtensionMethods
	{
		[Extension]
		internal static Func<T> AsFunc<T>(T value) where T : class
		{
			return new Func<T>(value, __ldftn(Return));
		}

		[Extension]
		private static T Return<T>(T value)
		{
			return value;
		}
	}
}
