using System;
using System.IO;

internal class UsingDeclaration
{
	public static void Run()
	{
		using StringWriter stringWriter = new StringWriter();
		stringWriter.Write("x");
		Console.WriteLine(stringWriter.ToString());
	}
}
