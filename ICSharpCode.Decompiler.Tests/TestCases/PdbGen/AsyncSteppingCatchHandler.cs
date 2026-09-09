using System;
using System.Threading.Tasks;

internal class AsyncSteppingCatchHandler
{
	public static async Task RunAsync()
	{
		await Task.Yield();
		Console.WriteLine("run");
	}

	public static async Task<int> SumAsync(int a, int b)
	{
		try
		{
			await Task.Yield();
			return a + b;
		}
		catch (InvalidOperationException)
		{
			return 0;
		}
	}

	public static async void FireAndForget()
	{
		await Task.Yield();
		Console.WriteLine("done");
	}
}
