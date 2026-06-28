using System;

internal class MemberInitializerImplicitCtor
{
	private int field = Compute(1);

	private int Property { get; } = Compute(2);

	private static int Compute(int value)
	{
		return value;
	}

	private static void Main()
	{
		MemberInitializerImplicitCtor memberInitializerImplicitCtor = new MemberInitializerImplicitCtor();
		Console.WriteLine(memberInitializerImplicitCtor.field + memberInitializerImplicitCtor.Property);
	}
}
