public class ParamsPropertySetter
{
	private int[] values;

	public int[] Values {
		get {
			return values;
		}
		set {
			values = value;
		}
	}

	public void Use()
	{
		Values = new int[2] { 1, 2 };
	}
}
