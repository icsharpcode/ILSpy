using System;

public class MonoFixed
{
	public unsafe void FixMultipleStrings(string text)
	{
		fixed (char* ptr = text)
		{
			fixed (char* ptr2 = Environment.UserName)
			{
				fixed (char* ptr3 = text)
				{
					*ptr = 'c';
					*ptr2 = 'd';
					*ptr3 = 'e';
				}
			}
		}
	}
}
