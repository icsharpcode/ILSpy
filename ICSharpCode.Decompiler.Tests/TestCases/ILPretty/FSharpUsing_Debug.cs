using System;
using System.IO;

public static class FSharpUsingPatterns
{
	public static void sample1()
	{
		using (FileStream fileStream = File.Create("x.txt")) {
			fileStream.WriteByte((byte)1);
		}
	}

	public static void sample2()
	{
		Console.WriteLine("some text");
		using (FileStream fileStream = File.Create("x.txt")) {
			fileStream.WriteByte((byte)2);
			Console.WriteLine("some text");
		}
	}

	public static void sample3()
	{
		Console.WriteLine("some text");
		using (FileStream fileStream = File.Create("x.txt")) {
			fileStream.WriteByte((byte)3);
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
		int num;
		using (FileStream fileStream = File.OpenRead("x.txt")) {
			num = fileStream.ReadByte();
		}
		int firstByte = num;
		int num3;
		using (FileStream fileStream = File.OpenRead("x.txt")) {
			int num2 = fileStream.ReadByte();
			num3 = fileStream.ReadByte();
		}
		int secondByte = num3;
		Console.WriteLine("read: {0}, {1}", firstByte, secondByte);
	}
}
