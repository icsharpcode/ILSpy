using System;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class AsyncMain
	{
		public static async Task Main(string[] args)
		{
			await Task.Delay(1000);
			Console.WriteLine("Hello Wolrd!");
		}
	}
}
