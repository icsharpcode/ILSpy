internal class EmptyBodies
{
	public static void RetVoid()
	{
	}
	public static int RetInt()
	{
		/*Error: Method body consists only of 'ret', but nothing is being returned. Decompiled assembly might be a reference assembly.*/;
	}
	public static void Nop()
	{
		/*Error: End of method reached without returning.*/;
	}
}