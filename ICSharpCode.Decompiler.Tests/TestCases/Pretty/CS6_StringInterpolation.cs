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
			Console.WriteLine($"\ta{((args.Length != 0) ? 5 : 0)}");
			Console.WriteLine($"\ta{(object)(args ?? args)}");
			Console.WriteLine($"\ta{args[0][0] == 'a'}");
			// This is legal, but we cannot create a pretty test for it, as it would require us
			// to convert string literals to string interpolation literals even without a string.Format call.
			// I do not think, this is worth the effort.
			//Console.WriteLine($"\ta{$"a" == args[0]}");
		}

		public static void InvalidFormatString(string[] args)
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
