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
		int firstByte = num;
		Console.WriteLine("read:" + firstByte.ToString());
	}

	public static void sample5()
	{
		Console.WriteLine("some text");
		int secondByte;
		using (FileStream fileStream = File.OpenRead("x.txt")) {
			secondByte = fileStream.ReadByte();
		}
		int firstByte = secondByte;
		int num2;
		using (FileStream fileStream = File.OpenRead("x.txt")) {
			int num = fileStream.ReadByte();
			num2 = fileStream.ReadByte();
		}
		secondByte = num2;
		Console.WriteLine("read: {0}, {1}", firstByte, secondByte);
	}
}
