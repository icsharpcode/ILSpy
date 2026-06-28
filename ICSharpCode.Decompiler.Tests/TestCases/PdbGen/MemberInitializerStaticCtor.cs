using System;

internal class MemberInitializerStaticCtor
{
	private static int field = Compute(1);

	private static int Property { get; } = Compute(2);

	private static int Compute(int value)
	{
		return value;
	}

	private static void Main()
	{
		Console.WriteLine(field + Property);
	}
}
