using System;
using System.Runtime.CompilerServices;

[assembly: Extension]

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	[Extension]
	internal static class NoExtensionMethods
	{
		[Extension]
		internal unsafe static Func<T> AsFunc<T>(T value) where T : class
		{
			return new Func<T>(value, (nint)(delegate*<T, T>)(&Return));
		}

		[Extension]
		private static T Return<T>(T value)
		{
			return value;
		}

		internal static Func<int, int> ExtensionMethodAsStaticFunc()
		{
			return Return;
		}

		internal unsafe static Func<object> ExtensionMethodBoundToNull()
		{
			return new Func<object>(null, (nint)(delegate*<object, object>)(&Return));
		}
	}
}
