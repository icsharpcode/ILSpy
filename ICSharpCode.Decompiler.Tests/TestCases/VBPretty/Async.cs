using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.VisualBasic.CompilerServices;

[StandardModule]
internal sealed class AsyncProgram
{
	[STAThread]
	public static void Main(string[] args)
	{
		Task task = new Task(ProcessDataAsync);
		task.Start();
		task.Wait();
		Console.ReadLine();
	}

	public static async void ProcessDataAsync()
	{
		Task<int> task = HandleFileAsync("C:\\enable1.txt");
		Console.WriteLine("Please wait, processing");
		Console.WriteLine("Count: " + await task);
	}

	public static async Task<int> HandleFileAsync(string file)
	{
		Console.WriteLine("HandleFile enter");
		int num = 0;
		checked
		{
			using (StreamReader streamReader = new StreamReader(file))
			{
				string text = await streamReader.ReadToEndAsync();
				num += text.Length;
				int num2 = 0;
				do
				{
					if (text.GetHashCode() == 0)
					{
						num--;
					}
					num2++;
				} while (num2 <= 10000);
			}

			Console.WriteLine("HandleFile exit");
			return num;
		}
	}
}
