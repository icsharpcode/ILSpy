using System;
using System.Collections.Generic;

internal class YieldReturn
{
	public static IEnumerable<int> Range(int start, int count)
	{
		for (int i = 0; i < count; i++)
		{
			Console.WriteLine(i);
			yield return start + i;
		}
	}
}
