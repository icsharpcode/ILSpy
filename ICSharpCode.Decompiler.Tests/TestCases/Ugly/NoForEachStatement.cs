using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	internal class NoForEachStatement
	{
		public static void SimpleNonGenericForeach(IEnumerable enumerable)
		{
			foreach (object item in enumerable)
			{
				Console.WriteLine(item);
			}
		}

		public static void SimpleForeachOverInts(IEnumerable<int> enumerable)
		{
			foreach (int item in enumerable)
			{
				Console.WriteLine(item);
			}
		}
	}
}
