using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class StringInterpolation
	{
		public static void Main(string[] args)
		{
		}

		public static void General(string[] args)
		{
			Console.WriteLine($"{args.Length}");
			Console.WriteLine($"a{{0{args.Length}");
			Console.WriteLine($"{args.Length:x}");
			Console.WriteLine($"\ta{args.Length}b");
			Console.WriteLine($"\ta{args.Length}ba{args[0]}a{args[args.Length]}a{args.Length}");
			Console.WriteLine($"\ta{((args.Length != 0) ? 5 : 0)}");
			Console.WriteLine($"\ta{args ?? args}");
			Console.WriteLine($"\ta{args[0][0] == 'a'}");
			Console.WriteLine($"\ta{$"a{args.Length}" == args[0]}");
			Console.WriteLine($"\ta{args.Length}}}");
			Console.WriteLine($"{args.Length,5:x}");
			Console.WriteLine($"{args.Length,5}");
		}

		public static void ArrayExpansionSpecialCases(object[] args)
		{
			Console.WriteLine($"args: {args}");
			Console.WriteLine(string.Format("args: {0}", args));
		}

		public static void InvalidFormatString(string[] args)
		{
#pragma warning disable IDE0043
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
			Console.WriteLine(string.Format("test: {0}}", args.Length));
#pragma warning restore
		}

		public void FormattableStrings(FormattableString s, string[] args)
		{
			s = $"{args.Length}";
			s = $"a{{0{args.Length}";
			s = $"{args.Length:x}";
			s = $"\ta{args.Length}b";
			s = $"\ta{args.Length}ba{args[0]}a{args[args.Length]}a{args.Length}";
			s = $"\ta{((args.Length != 0) ? 5 : 0)}";
			s = $"\ta{args ?? args}";
			s = $"\ta{args[0][0] == 'a'}";
			s = $"\ta{$"a{args.Length}" == args[0]}";
			RequiresCast($"{args.Length}");
			RequiresCast($"a{{0{args.Length}");
			RequiresCast($"{args.Length:x}");
			RequiresCast($"\ta{args.Length}b");
			RequiresCast($"\ta{args.Length}ba{args[0]}a{args[args.Length]}a{args.Length}");
			RequiresCast($"\ta{((args.Length != 0) ? 5 : 0)}");
			RequiresCast($"\ta{args ?? args}");
			RequiresCast($"\ta{args[0][0] == 'a'}");
			RequiresCast($"\ta{$"a{args.Length}" == args[0]}");
			RequiresCast((FormattableString)$"{args.Length}");
			RequiresCast((FormattableString)$"a{{0{args.Length}");
			RequiresCast((FormattableString)$"{args.Length:x}");
			RequiresCast((FormattableString)$"\ta{args.Length}b");
			RequiresCast((FormattableString)$"\ta{args.Length}ba{args[0]}a{args[args.Length]}a{args.Length}");
			RequiresCast((FormattableString)$"\ta{((args.Length != 0) ? 5 : 0)}");
			RequiresCast((FormattableString)$"\ta{args ?? args}");
			RequiresCast((FormattableString)$"\ta{args[0][0] == 'a'}");
			RequiresCast((FormattableString)$"\ta{$"a{args.Length}" == args[0]}");
			RequiresCast((IFormattable)$"{args.Length}");
			RequiresCast((IFormattable)$"a{{0{args.Length}");
			RequiresCast((IFormattable)$"{args.Length:x}");
			RequiresCast((IFormattable)$"\ta{args.Length}b");
			RequiresCast((IFormattable)$"\ta{args.Length}ba{args[0]}a{args[args.Length]}a{args.Length}");
			RequiresCast((IFormattable)$"\ta{((args.Length != 0) ? 5 : 0)}");
			RequiresCast((IFormattable)$"\ta{args ?? args}");
			RequiresCast((IFormattable)$"\ta{args[0][0] == 'a'}");
			RequiresCast((IFormattable)$"\ta{$"a{args.Length}" == args[0]}");
		}

		public void Issue1497(string[] args)
		{
			Console.WriteLine($"args[0]: {args[0].Trim(':').Trim('&').Trim(':').Trim('&')} asdf {args.Length:x} test");
		}

		public void RequiresCast(string value)
		{
		}

		public void RequiresCast(FormattableString value)
		{
		}

		public void RequiresCast(IFormattable value)
		{
		}
	}
}
