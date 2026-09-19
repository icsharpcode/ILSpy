public class IndexerAccessorParameterNames
{
	public int this[int x, int y] {
		get {
			return x;
		}
		set {
		}
	}

	private int Get(int i)
	{
		return i;
	}

	public void Use()
	{
		this[y: Get(1), x: Get(2)] = 3;
	}
}
