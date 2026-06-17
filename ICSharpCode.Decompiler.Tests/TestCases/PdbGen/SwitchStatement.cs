using System;

internal class SwitchStatement
{
	public static string Classify(int n)
	{
		switch (n)
		{
		case 0:
			return "zero";
		case 1:
		case 2:
			return "small";
		default:
			Console.WriteLine(n);
			return "other";
		}
	}
}
