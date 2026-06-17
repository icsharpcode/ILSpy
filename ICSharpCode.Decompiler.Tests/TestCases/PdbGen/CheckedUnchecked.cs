using System;

internal class CheckedUnchecked
{
	public static void Run(int a, int b)
	{
		Console.WriteLine(checked(a + b));
		Console.WriteLine(a * b);
	}
}
