using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class CS6_StringInterpolation
	{
		public static void General(string[] args)
		{
			Console.WriteLine($"{args.Length}");
			Console.WriteLine($"a{{0{args.Length}");
			Console.WriteLine($"{args.Length:x}");
			Console.WriteLine($"\ta{args.Length}b");
			Console.WriteLine($"\ta{args.Length}ba{args[0]}a{args[args.Length]}a{args.Length}");
		}

		public static void Invalid(string[] args)
		{
			Console.WriteLine(string.Format("", args.Length));
			Console.WriteLine(string.Format("a", args.Length));
			Console.WriteLine(string.Format("}", args.Length));
			Console.WriteLine(string.Format("{", args.Length));
			Console.WriteLine(string.Format(":", args.Length));
			Console.WriteLine(string.Format("\t", args.Length));
			Console.WriteLine(string.Format("\\", args.Length));
			Console.WriteLine(string.Format("\"", args.Length));
			Console.WriteLine(string.Format("aa", args.Length));
			Console.WriteLine(string.Format("a}", args.Length));
			Console.WriteLine(string.Format("a{", args.Length));
			Console.WriteLine(string.Format("a:", args.Length));
			Console.WriteLine(string.Format("a\t", args.Length));
			Console.WriteLine(string.Format("a\\", args.Length));
			Console.WriteLine(string.Format("a\"", args.Length));
			Console.WriteLine(string.Format("a{:", args.Length));
			Console.WriteLine(string.Format("a{0", args.Length));
			Console.WriteLine(string.Format("a{{0", args.Length));
			Console.WriteLine(string.Format("}a{{0", args.Length));
			Console.WriteLine(string.Format("}{", args.Length));
			Console.WriteLine(string.Format("{}", args.Length));
			Console.WriteLine(string.Format("{0:}", args.Length));
			Console.WriteLine(string.Format("{0{a}0}", args.Length));
			Console.WriteLine(string.Format("test: {0}", string.Join(",", args)));
		}
	}
}
