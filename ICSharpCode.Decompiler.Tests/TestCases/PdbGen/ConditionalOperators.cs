using System;

internal class ConditionalOperators
{
	public static void Run(string value, int n)
	{
		Console.WriteLine(value ?? "default");
		Console.WriteLine((n > 0) ? "positive" : "non-positive");
	}
}
