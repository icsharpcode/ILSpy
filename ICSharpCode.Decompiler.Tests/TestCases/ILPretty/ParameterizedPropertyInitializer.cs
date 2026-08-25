public class ParameterizedPropertyInitializer
{
	// C# has no syntax for parameterized property 'Foo'.
	public int get_Foo(int x)
	{
		return x;
	}

	public void set_Foo(int x, int value)
	{
	}

	public static void Consume(ParameterizedPropertyInitializer p)
	{
	}

	public static void Use()
	{
		Consume(new ParameterizedPropertyInitializer { [7] = 5 });
	}
}
