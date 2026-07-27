#pragma warning disable 657
using System;

public interface IParameterized
{
	// C# has no syntax for parameterized property 'IndexedValue'.
	int get_IndexedValue(int index);
	void set_IndexedValue(int index, int Value);
}

public class ParameterizedBase
{
	// C# has no syntax for parameterized property 'Virt'.
	public virtual int get_Virt(int index)
	{
		return index;
	}

	public virtual void set_Virt(int index, int value)
	{
	}
}

public class ParameterizedDerived : ParameterizedBase
{
	// C# has no syntax for parameterized property 'Virt'.
	public override int get_Virt(int index)
	{
		return checked(index + 1);
	}

	public override void set_Virt(int index, int value)
	{
	}
}

public class ParameterizedProperties : IParameterized
{
	private int _field;

	// C# has no syntax for parameterized property 'SharedProp'.
	public static int get_SharedProp(int index)
	{
		return index;
	}

	public static void set_SharedProp(int index, int value)
	{
	}

	// C# has no syntax for parameterized property 'IndexedValue'.
	public int get_IndexedValue(int index)
	{
		return _field;
	}

	int IParameterized.get_IndexedValue(int index)
	{
		//ILSpy generated this explicit interface implementation from .override directive in get_IndexedValue
		return this.get_IndexedValue(index);
	}

	public void set_IndexedValue(int index, int value)
	{
		_field = value;
	}

	void IParameterized.set_IndexedValue(int index, int value)
	{
		//ILSpy generated this explicit interface implementation from .override directive in set_IndexedValue
		this.set_IndexedValue(index, value);
	}

	// C# has no syntax for parameterized property 'ReadOnlyProp'.
	public int get_ReadOnlyProp(int index)
	{
		return index;
	}

	// C# has no syntax for parameterized property 'SetOnly'.
	public void set_SetOnly(int index, int value)
	{
		_field = checked(index + value);
	}

	// C# has no syntax for parameterized property 'Attributed'.
	// Its 'property:' attributes below are ignored by the compiler (CS0657).
	[property: Obsolete("read-write parameterized property")]
	public int get_Attributed(int index)
	{
		return index;
	}

	public void set_Attributed(int index, int value)
	{
	}

	// C# has no syntax for parameterized property 'Overloaded'.
	public int get_Overloaded(int a)
	{
		return a;
	}

	// C# has no syntax for parameterized property 'Overloaded'.
	public int get_Overloaded(int a, int b)
	{
		return checked(a + b);
	}

	public void Use()
	{
		ParameterizedProperties.set_SharedProp(1, 2);
		_field = ParameterizedProperties.get_SharedProp(3);
		this.set_IndexedValue(4, 5);
		_field = this.get_IndexedValue(6);
		((IParameterized)this).set_IndexedValue(7, 8);
		_field = ((IParameterized)this).get_IndexedValue(9);
	}
}

public class RenamedImplementation : IParameterized
{
	private int _value;

	// C# has no syntax for parameterized property 'Renamed'.
	public int get_Renamed(int index)
	{
		return _value;
	}

	int IParameterized.get_IndexedValue(int index)
	{
		//ILSpy generated this explicit interface implementation from .override directive in get_Renamed
		return this.get_Renamed(index);
	}

	public void set_Renamed(int index, int value)
	{
		_value = value;
	}

	void IParameterized.set_IndexedValue(int index, int value)
	{
		//ILSpy generated this explicit interface implementation from .override directive in set_Renamed
		this.set_Renamed(index, value);
	}
}
