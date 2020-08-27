using System;
using System.IO;
using System.Threading.Tasks;

namespace EquivalentCSharpConsoleApp
{
	static class Program
	{
		public static void Main(string[] args)
		{
			var task = new Task(ProcessDataAsync);
			task.Start();
			task.Wait();
			Console.ReadLine();
		}

		public async static void ProcessDataAsync()
		{
			Task<int> task = HandleFileAsync("C:\\enable1.txt");
			Console.WriteLine("Please wait, processing");
			int result = await task;
			Console.WriteLine("Count: " + result.ToString());
		}

		public async static Task<int> HandleFileAsync(string file)
		{
			Console.WriteLine("HandleFile enter");
			int count = 0;
			using (StreamReader reader = new StreamReader(file))
			{
				string value = await reader.ReadToEndAsync();
				count += value.Length;
				for (var i = 0; i <= 10000; i += 1)
				{
					var x = value.GetHashCode();
					if (x == 0)
						count -= 1;
				}
			}

			Console.WriteLine("HandleFile exit");
			return count;
		}
	}
}