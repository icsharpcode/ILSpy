using System;

internal class TryCatchFinally
{
	public static void Run(int n)
	{
		try
		{
			Console.WriteLine(n);
		}
		catch (InvalidOperationException ex)
		{
			Console.WriteLine(ex.Message);
		}
		catch (Exception) when (n > 0)
		{
			Console.WriteLine("filtered");
		}
		finally
		{
			Console.WriteLine("done");
		}
	}
}
