using System;
using System.Collections.Generic;
using System.IO;

internal class ForeachUsing
{
	public static void Run(IEnumerable<int> items)
	{
		using (StringWriter stringWriter = new StringWriter())
		{
			stringWriter.Write("x");
		}
		foreach (int item in items)
		{
			Console.WriteLine(item);
		}
	}
}
