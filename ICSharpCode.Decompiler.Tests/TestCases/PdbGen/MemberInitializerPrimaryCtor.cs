using System;

internal class MemberInitializerPrimaryCtor(int factor)
{
	private int field = Compute(factor);

	public override string ToString()
	{
		return field.ToString();
	}

	private static int Compute(int value)
	{
		return value;
	}

	private static void Main()
	{
		Console.WriteLine(new MemberInitializerPrimaryCtor(3));
	}
}
