using System;
using System.Linq;
using System.Runtime.CompilerServices;

public class Issue2192
{
	public static void M()
	{
		string[] source = new string[3] { "abc", "defgh", "ijklm" };
		string text = "test";
		Console.WriteLine(source.Count([SpecialName] (string w) => w.Length > text.Length));
	}
}