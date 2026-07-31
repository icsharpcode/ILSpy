using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.IsolatedDecompilation
{
	// A static class whose only extension members use the classic "this" parameter syntax:
	// the type carries [Extension], but contains no C# 14 extension blocks.
	internal static class ClassicExtensions
	{
		public static int Twice(this int x)
		{
			return x * 2;
		}

		private sealed class Nested
		{
			public int Get()
			{
				return 42;
			}
		}
	}

	// A static class that mixes a real C# 14 extension block with an ordinary nested type.
	internal static class BlockExtensions
	{
		extension(List<int> list)
		{
			public int DoubledCount => list.Count * 2;
		}

		private sealed class Nested
		{
			public int Get()
			{
				return 42;
			}
		}
	}
}
