using System.Runtime.InteropServices;

public class ParameterizedPropertySetterCall
{
	// C# has no syntax for parameterized property 'P'.
	public int get_P(int i)
	{
		return i;
	}

	public void set_P([Optional][DefaultParameterValue(0)] int i, int value)
	{
	}

	public void Use()
	{
		this.set_P(0, 5);
	}
}
