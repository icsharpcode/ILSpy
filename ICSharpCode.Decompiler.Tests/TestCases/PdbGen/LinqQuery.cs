using System;
using System.Collections.Generic;
using System.Linq;

internal class LinqQuery
{
	public static void Run(IEnumerable<int> numbers)
	{
		foreach (int item in from n in numbers
			where n > 0
			select n * n)
		{
			Console.WriteLine(item);
		}
	}
}
