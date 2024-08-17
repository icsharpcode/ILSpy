using System;

public class MonoFixed
{
	public unsafe void FixMultipleStrings(string text)
	{
		fixed (char* ptr = text)
		{
			fixed (char* userName = Environment.UserName)
			{
				fixed (char* ptr2 = text)
				{
					*ptr = 'c';
					*userName = 'd';
					*ptr2 = 'e';
				}
			}
		}
	}
}
