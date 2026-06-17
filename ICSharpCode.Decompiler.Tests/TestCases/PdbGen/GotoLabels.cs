using System;

internal class GotoLabels
{
	public static void Run(int n)
	{
		if (n == 0)
		{
			goto IL_0008;
		}
		if (n != 1)
		{
			return;
		}
		goto IL_0012;
		IL_0008:
		Console.WriteLine("zero");
		goto IL_0012;
		IL_0012:
		Console.WriteLine("one");
		n--;
		if (n <= 0)
		{
			return;
		}
		goto IL_0008;
	}
}
