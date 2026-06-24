internal class TailCall
{
	public static int Callee(int x)
	{
		return x;
	}
	public static int Caller(int x)
	{
		return /*tail.*/Callee(x);
	}
}
