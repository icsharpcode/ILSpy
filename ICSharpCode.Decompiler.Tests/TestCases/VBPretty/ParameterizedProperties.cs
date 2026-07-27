#pragma warning disable 657
using System;

public interface IParameterized
{
	// C# has no syntax for parameterized properties; the accessors of
	// property 'IndexedValue' are emitted as ordinary methods.
	int get_IndexedValue(int index);
	void set_IndexedValue(int index, int Value);
}

public class ParameterizedProperties : IParameterized
{
	private int _field;

	// C# has no syntax for parameterized properties; the accessors of
	// property 'SharedProp' are emitted as ordinary methods.
	public static int get_SharedProp(int index)
	{
		return index;
	}

	public static void set_SharedProp(int index, int value)
	{
	}

	// C# has no syntax for parameterized properties; the accessors of
	// property 'IndexedValue' are emitted as ordinary methods.
	public int get_IndexedValue(int index)
	{
		return _field;
	}

	public void set_IndexedValue(int index, int value)
	{
		_field = value;
	}

	// C# has no syntax for parameterized properties; the accessors of
	// property 'ReadOnlyProp' are emitted as ordinary methods.
	public int get_ReadOnlyProp(int index)
	{
		return index;
	}

	// C# has no syntax for parameterized properties; the accessors of
	// property 'Attributed' are emitted as ordinary methods. The property's
	// attributes are kept below under the inert 'property:' target (CS0657).
	[property: Obsolete("read-write parameterized property")]
	public int get_Attributed(int index)
	{
		return index;
	}

	public void set_Attributed(int index, int value)
	{
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
