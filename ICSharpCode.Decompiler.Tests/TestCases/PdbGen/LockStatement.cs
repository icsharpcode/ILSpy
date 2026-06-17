using System;

internal class LockStatement
{
	private static readonly object sync = new object();

	public static void Run()
	{
		lock (sync)
		{
			Console.WriteLine("locked");
		}
	}
}
