using System;

public static class Issue684
{
	static int Main(string[] A_0)
	{
		int[] array = new int[1000];
		int num = int.Parse(Console.ReadLine());
		// Point of this test was to ensure the stack slot here uses an appropriate type,
		// (bool instead of int). Unfortunately our type fixup runs too late to affect variable names.
		bool flag = num >= 1000;
		if (!flag)
		{
			flag = num < 2;
		}
		if (flag)
		{
			Console.WriteLine(-1);
		}
		else
		{
			int i = 2;
			for (int num2 = 2; num2 <= num; num2 = i)
			{
				Console.WriteLine(num2);
				for (; i <= num; i += num2)
				{
					int num3 = 1;
					array[i] = num3;
				}
				i = num2;
				while (true)
				{
					bool flag2 = i <= num;
					if (flag2)
					{
						flag2 = array[i] != 0;
					}
					if (!flag2)
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
