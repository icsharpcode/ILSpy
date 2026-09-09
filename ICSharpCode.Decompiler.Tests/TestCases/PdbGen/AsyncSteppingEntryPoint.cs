using System;
using System.Threading.Tasks;

internal class AsyncSteppingEntryPoint
{
	public static async Task Main()
	{
		try
		{
			await Task.Yield();
			Console.WriteLine("main");
		}
		catch (InvalidOperationException e)
		{
			Console.WriteLine(e.Message);
		}
	}

	public static async Task Main(int notTheEntryPoint)
	{
		try
		{
			await Task.Yield();
			Console.WriteLine(notTheEntryPoint);
		}
		catch (InvalidOperationException e)
		{
			Console.WriteLine(e.Message);
		}
	}

	public static async Task NotTheEntryPointAsync()
	{
		try
		{
			await Task.Yield();
			Console.WriteLine("other");
		}
		catch (InvalidOperationException e)
		{
			Console.WriteLine(e.Message);
		}
	}

	public static async void FireAndForget()
	{
		try
		{
			await Task.Yield();
			Console.WriteLine("done");
		}
		catch (InvalidOperationException e)
		{
			Console.WriteLine(e.Message);
		}
	}
}
