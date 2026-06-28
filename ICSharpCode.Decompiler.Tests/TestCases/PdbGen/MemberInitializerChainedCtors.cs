using System;

internal class MemberInitializerChainedCtors
{
	private int field = Compute(1);

	private int Property { get; } = Compute(2);

	public MemberInitializerChainedCtors()
	{
		Console.WriteLine(field + Property);
	}

	public MemberInitializerChainedCtors(int value)
		: this()
	{
		Console.WriteLine(value);
	}

	private static int Compute(int value)
	{
		return value;
	}

	private static void Main()
	{
		new MemberInitializerChainedCtors();
		new MemberInitializerChainedCtors(3);
	}
}
