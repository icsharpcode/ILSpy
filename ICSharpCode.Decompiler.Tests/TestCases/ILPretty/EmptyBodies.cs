internal class EmptyBodies
{
	public static void RetVoid()
	{
	}
	public static int RetInt()
	{
		return (int)/*Error near IL_0001: Stack underflow*/;
	}
	public static void Nop()
	{
		/*Error: End of method reached without returning.*/;
	}
}