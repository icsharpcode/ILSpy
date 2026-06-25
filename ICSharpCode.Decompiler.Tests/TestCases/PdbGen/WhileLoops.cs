using System;

internal class WhileLoops
{
	public static void PrintWhile(string[] args)
	{
		int i = 0;
		while (i < args.Length)
		{
			Console.WriteLine(args[i]);
			i++;
		}
	}

	public static void PrintDoWhile(int count)
	{
		int i = 0;
		do
		{
			Console.WriteLine(i);
			i++;
		}
		while (i < count);
	}
}
