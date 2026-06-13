using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.LocationsInAst
{
	// A small method body gives the location-setting output path several statements and tokens
	// to assign positions to.
	internal class LocationSample
	{
		public int Add(int a, int b)
		{
			int sum = a + b;
			return sum;
		}
	}

	// Exercises the statement headers whose sequence-point coordinates SequencePointBuilder derives
	// from token positions ('{'/'}', if/while/do-while/foreach/switch/lock '(...)', catch/when).
	internal class SequencePointSample
	{
		public int Headers(int n, int[] items)
		{
			int sum = 0;
			if (n > 0)
			{
				sum++;
			}
			else
			{
				sum--;
			}
			while (sum < n)
			{
				sum += 2;
			}
			do
			{
				sum--;
			}
			while (sum > 0);
			foreach (int item in items)
			{
				sum += item;
			}
			switch (n)
			{
				case 1:
					sum = 1;
					break;
				default:
					sum = 0;
					break;
			}
			lock (items)
			{
				sum++;
			}
			try
			{
				sum += n;
			}
			catch (Exception ex) when (ex.Message.Length > 0)
			{
				sum = -1;
			}
			catch (Exception)
			{
				sum = -2;
			}
			return sum;
		}
	}
}
