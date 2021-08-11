using System;
using System.Reflection;
using ClassLibrary1;

[assembly: AssemblyCompany("ConsoleApp8")]
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0")]
[assembly: AssemblyProduct("ConsoleApp8")]
[assembly: AssemblyTitle("ConsoleApp8")]

namespace ConsoleApp8
{
	internal class Program : Class1
	{
		public Program(string _)
			: base(_)
		{
			Console.WriteLine("Class1(string)<-Program(string)");
		}

		private static void Main(string[] args)
		{
			new Program("");
			Console.WriteLine("Hello World!");
		}
	}
}