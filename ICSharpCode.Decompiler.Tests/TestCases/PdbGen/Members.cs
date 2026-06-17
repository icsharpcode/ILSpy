using System;

internal class C : IDisposable
{
	private int ExpressionProperty => 42;

	private int Property
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	private C this[int index] => null;

	private C this[string s]
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public event Action Event
	{
		add
		{
		}
		remove
		{
		}
	}

	public static implicit operator C(int i)
	{
		return null;
	}

	static C()
	{
	}

	public C()
	{
		Console.WriteLine();
	}

	~C()
	{
	}

	void IDisposable.Dispose()
	{
	}

	private static void Main()
	{
		C c = new C();
		c.Event += delegate
		{
		};
		_ = c.Property;
		_ = c.ExpressionProperty;
		_ = c[0];
		_ = c[""];
		_ = (C)1;
		Local();
		static void Local()
		{
			Console.WriteLine();
		}
	}
}
