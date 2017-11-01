using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DiffLib;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	public static class CodeAssert
	{
		public static void FilesAreEqual(string fileName1, string fileName2, string[] definedSymbols = null)
		{
			AreEqual(File.ReadAllText(fileName1), File.ReadAllText(fileName2), definedSymbols);
		}

		public static void AreEqual(string input1, string input2, string[] definedSymbols = null)
		{
			var diff = new StringWriter();
			if (!CodeComparer.Compare(input1, input2, diff, CodeComparer.NormalizeLine, definedSymbols)) {
				Assert.Fail(diff.ToString());
			}
		}
	}

	public static class CodeComparer
	{
		public static bool Compare(string input1, string input2, StringWriter diff, Func<string, string> normalizeLine, string[] definedSymbols = null)
		{
			var differ = new AlignedDiff<string>(
				NormalizeAndSplitCode(input1, definedSymbols ?? new string[0]),
				NormalizeAndSplitCode(input2, definedSymbols ?? new string[0]),
				new CodeLineEqualityComparer(normalizeLine),
				new StringSimilarityComparer(),
				new StringAlignmentFilter());

			bool result = true, ignoreChange;

			int line1 = 0, line2 = 0;

			foreach (var change in differ.Generate()) {
				switch (change.Change) {
					case ChangeType.Same:
						diff.Write("{0,4} {1,4} ", ++line1, ++line2);
						diff.Write("  ");
						diff.WriteLine(change.Element1);
						break;
					case ChangeType.Added:
						diff.Write("     {1,4} ", line1, ++line2);
						result &= ignoreChange = ShouldIgnoreChange(change.Element2);
						diff.Write(ignoreChange ? "    " : " +  ");
						diff.WriteLine(change.Element2);
						break;
					case ChangeType.Deleted:
						diff.Write("{0,4}      ", ++line1, line2);
						result &= ignoreChange = ShouldIgnoreChange(change.Element1);
						diff.Write(ignoreChange ? "    " : " -  ");
						diff.WriteLine(change.Element1);
						break;
					case ChangeType.Changed:
						diff.Write("{0,4}      ", ++line1, line2);
						result = false;
						diff.Write("(-) ");
						diff.WriteLine(change.Element1);
						diff.Write("     {1,4} ", line1, ++line2);
						diff.Write("(+) ");
						diff.WriteLine(change.Element2);
						break;
				}
			}

			return result;
		}

		class CodeLineEqualityComparer : IEqualityComparer<string>
		{
			private IEqualityComparer<string> baseComparer = EqualityComparer<string>.Default;
			private Func<string, string> normalizeLine;

			public CodeLineEqualityComparer(Func<string, string> normalizeLine)
			{
				this.normalizeLine = normalizeLine;
			}

			public bool Equals(string x, string y)
			{
				return baseComparer.Equals(
					normalizeLine(x),
					normalizeLine(y)
				);
			}

			public int GetHashCode(string obj)
			{
				return baseComparer.GetHashCode(NormalizeLine(obj));
			}
		}

		public static string NormalizeLine(string line)
		{
			line = line.Trim();
			var index = line.IndexOf("//", StringComparison.Ordinal);
			if (index >= 0) {
				return line.Substring(0, index);
			} else if (line.StartsWith("#", StringComparison.Ordinal)) {
				return string.Empty;
			} else {
				return line;
			}
		}

		private static bool ShouldIgnoreChange(string line)
		{
			// for the result, we should ignore blank lines and added comments
			return NormalizeLine(line) == string.Empty;
		}

		class DeleteDisabledTextRewriter : CSharpSyntaxRewriter
		{
			public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
			{
				if (trivia.IsKind(SyntaxKind.DisabledTextTrivia)) {
					return default(SyntaxTrivia); // delete
				}
				return base.VisitTrivia(trivia);
			}
		}

		private static IEnumerable<string> NormalizeAndSplitCode(string input, IEnumerable<string> definedSymbols)
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(input, new CSharpParseOptions(preprocessorSymbols: definedSymbols));
			var result = new DeleteDisabledTextRewriter().Visit(syntaxTree.GetRoot());
			input = result.ToFullString();
			return input.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}
