using System;
using System.Threading.Tasks;

internal class AsyncAwait
{
	public static async Task<int> SumAsync(int a, int b)
	{
		await Task.Yield();
		int num = a + b;
		Console.WriteLine(num);
		return num;
	}
}
