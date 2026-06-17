using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.PdbGen;

public class ForLoopTests
{
	public static void SimplePrintLoop(string[] args)
	{
		for (int i = 0; i < args.Length; i++)
		{
			Console.WriteLine(args[i]);
		}
	}

	public static void SimplePrintLoopWithCondition(string[] args)
	{
		for (int i = 0; i < args.Length; i++)
		{
			if (i % 2 != 0)
			{
				Console.WriteLine(args[i]);
			}
		}
	}
}
