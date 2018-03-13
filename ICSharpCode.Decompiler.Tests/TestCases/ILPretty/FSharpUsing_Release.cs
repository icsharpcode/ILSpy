using System;
using System.IO;

public static class FSharpUsingPatterns
{
	public static void sample1()
	{
		using (var fileStream = File.Create("x.txt")) {
			fileStream.WriteByte(1);
		}
	}

	public static void sample2()
	{
		Console.WriteLine("some text");
		using (var fileStream = File.Create("x.txt")) {
			fileStream.WriteByte(2);
			Console.WriteLine("some text");
		}
	}

	public static void sample3()
	{
		Console.WriteLine("some text");
		using (var fileStream = File.Create("x.txt")) {
			fileStream.WriteByte(3);
		}
		Console.WriteLine("some text");
	}

	public static void sample4()
	{
		Console.WriteLine("some text");
		var num = default(int);
		using (var fileStream = File.OpenRead("x.txt")) {
			num = fileStream.ReadByte();
		}
		var num2 = num;
		Console.WriteLine("read:" + num2.ToString());
	}

	public static void sample5()
	{
		Console.WriteLine("some text");
		var num = default(int);
		using (var fileStream = File.OpenRead("x.txt")) {
			num = fileStream.ReadByte();
		}
		var num2 = num;
		var num3 = default(int);
		using (var fileStream = File.OpenRead("x.txt")) {
			fileStream.ReadByte();
			num3 = fileStream.ReadByte();
		}
		num = num3;
		Console.WriteLine("read: {0}, {1}", num2, num);
	}
}
