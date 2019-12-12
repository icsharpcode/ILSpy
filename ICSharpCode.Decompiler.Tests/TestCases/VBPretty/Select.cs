using Microsoft.VisualBasic.CompilerServices;
using System;

[StandardModule]
internal sealed class Program
{
	public static void SelectOnString()
	{
		switch (Environment.CommandLine) {
			case "123":
				Console.WriteLine("a");
				break;
			case "444":
				Console.WriteLine("b");
				break;
			case "222":
				Console.WriteLine("c");
				break;
			case "11":
				Console.WriteLine("d");
				break;
			case "dd":
				Console.WriteLine("e");
				break;
			case "sss":
				Console.WriteLine("f");
				break;
			case "aa":
				Console.WriteLine("g");
				break;
			case null:
			case "":
				Console.WriteLine("empty");
				break;
		}
	}
}
