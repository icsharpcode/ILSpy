using System;
using System.IO;

public static class FSharpUsingPatterns
{
	public static void sample1()
	{
		using (FileStream fileStream = File.Create("x.txt")) {
			fileStream.WriteByte(1);
		}
	}

	public static void sample2()
	{
		Console.WriteLine("some text");
		using (FileStream fileStream = File.Create("x.txt")) {
			fileStream.WriteByte(2);
			Console.WriteLine("some text");
		}
	}

	public static void sample3()
	{
		Console.WriteLine("some text");
		using (FileStream fileStream = File.Create("x.txt")) {
			fileStream.WriteByte(3);
		}
		Console.WriteLine("some text");
	}

	public static void sample4()
	{
		Console.WriteLine("some text");
		int num;
		using (FileStream fileStream = File.OpenRead("x.txt")) {
			num = fileStream.ReadByte();
		}
		int num2 = num;
		Console.WriteLine("read:" + num2);
	}

	public static void sample5()
	{
		Console.WriteLine("some text");
		int num;
		using (FileStream fileStream = File.OpenRead("x.txt")) {
			num = fileStream.ReadByte();
		}
		int num2 = num;
		int num3;
		using (FileStream fileStream = File.OpenRead("x.txt")) {
			fileStream.ReadByte();
			num3 = fileStream.ReadByte();
		}
		num = num3;
		Console.WriteLine("read: {0}, {1}", num2, num);
	}
}
