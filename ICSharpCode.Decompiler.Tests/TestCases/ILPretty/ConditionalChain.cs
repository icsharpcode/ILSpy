using System;

public class ConditionalChain
{
	public int Ladder(int n, int[] a, int[] b)
	{
		int num;
		if (n > 40)
		{
			num = a[0];
		}
		else if (n > 30)
		{
			num = b[0];
		}
		else if (n > 20)
		{
			num = a[1];
		}
		else
		{
			num = ((n > 10) ? b[1] : a[2]);
		}
		Console.WriteLine(num);
		return num;
	}
}
