using System;

internal class MemberInitializerSingleCtor
{
	private int field = Compute(1);

	private int Property { get; } = Compute(2);

	public MemberInitializerSingleCtor()
	{
		Console.WriteLine(field + Property);
	}

	private static int Compute(int value)
	{
		return value;
	}

	private static void Main()
	{
		new MemberInitializerSingleCtor();
	}
}
