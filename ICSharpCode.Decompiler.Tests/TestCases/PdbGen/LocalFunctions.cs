using System;

internal class LocalFunctions
{
	public static void Run(int n)
	{
		Console.WriteLine(Square(n));
		static int Square(int x)
		{
			return x * x;
		}
	}
}
