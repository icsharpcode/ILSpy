using System;

internal class MemberInitializers
{
	private int instanceField = Compute(1);

	private int InstanceProperty { get; } = Compute(2);

	public MemberInitializers()
	{
		Console.WriteLine(instanceField + InstanceProperty);
	}

	public MemberInitializers(int value)
	{
		Console.WriteLine(instanceField + InstanceProperty + value);
	}

	private static int Compute(int value)
	{
		return value;
	}

	private static void Main()
	{
		_ = new MemberInitializers();
		_ = new MemberInitializers(3);
	}
}
