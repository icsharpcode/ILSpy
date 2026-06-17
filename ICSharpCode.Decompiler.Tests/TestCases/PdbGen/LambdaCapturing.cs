using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.PdbGen;

public class LambdaCapturing
{
	public static void Main(string[] args)
	{
		int num = 4;
		int captured = Environment.TickCount + num;
		Test((int a, int b) => a + b + captured);
	}

	private static void Test(Func<int, int, int> p)
	{
		p(1, 2);
	}
}
