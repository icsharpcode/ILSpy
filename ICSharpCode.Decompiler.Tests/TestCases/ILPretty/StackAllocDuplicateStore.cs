using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public static class StackAllocDuplicateStore
	{
		public unsafe static int Seq(int a, int b, int c)
		{
			byte* ptr = stackalloc byte[12];
			*(int*)ptr = a;
			*(int*)ptr = 99;
			((int*)ptr)[1] = b;
			((int*)ptr)[2] = c;
			Span<int> span = new Span<int>(ptr, 3);
			return span[0] + span[1] + span[2];
		}
	}
}
