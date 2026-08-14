public class CallSite
{
	public static bool CompareVirtual<T>(Source<T> s, T a, T b)
	{
		return a < b;
	}

	public static bool CompareDirect(Source<int> s, int a, int b)
	{
		return a > b;
	}
}
public class Source<T>
{
	public virtual bool operator <(T a, T b)
	{
		return false;
	}

	public bool operator >(T a, T b)
	{
		return false;
	}
}
