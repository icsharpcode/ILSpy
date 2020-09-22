using System;

public static class Issue684
{
	static int Main(string[] A_0)
	{
		int[] array = new int[1000];
		int num = int.Parse(Console.ReadLine());
		// Point of this test was to ensure the stack slot here uses an appropriate type,
		// (bool instead of int). Unfortunately our type fixup runs too late to affect variable names.
		bool num2 = num >= 1000;
		if (!num2)
		{
			num2 = num < 2;
		}
		if (num2)
		{
			Console.WriteLine(-1);
		}
		else
		{
			int i = 2;
			for (int num3 = 2; num3 <= num; num3 = i)
			{
				Console.WriteLine(num3);
				for (; i <= num; i += num3)
				{
					int num4 = (array[i] = 1);
				}
				i = num3;
				while (true)
				{
					bool num5 = i <= num;
					if (num5)
					{
						num5 = array[i] != 0;
					}
					if (!num5)
					{
						break;
					}
					i++;
				}
			}
		}
		return 0;
	}
}
