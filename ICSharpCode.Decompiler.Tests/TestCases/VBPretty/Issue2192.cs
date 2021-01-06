using System;
using System.Linq;

public class Issue2192
{
	public static void M()
	{
		string[] source = new string[3] { "abc", "defgh", "ijklm" };
		string text = "test";
		Console.WriteLine(source.Count((string w) => w.Length > text.Length));
	}
}