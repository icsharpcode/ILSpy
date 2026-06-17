using System;

internal class NestedLoops
{
	public static void Print(int rows, int columns)
	{
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < columns; j++)
			{
				Console.WriteLine(i * j);
			}
		}
	}
}
