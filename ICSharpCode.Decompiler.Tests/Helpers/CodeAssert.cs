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
			if (!CodeComparer.Compare(input1, input2, diff, CodeComparer.NormalizeLine, definedSymbols))
			{
				Assert.Fail(diff.ToString());
			}
		}
	}

	public static class CodeComparer
	{
		public static bool Compare(string input1, string input2, StringWriter diff, Func<string, string> normalizeLine, string[] definedSymbols = null)
		{
			var collection1 = NormalizeAndSplitCode(input1, definedSymbols ?? new string[0]);
			var collection2 = NormalizeAndSplitCode(input2, definedSymbols ?? new string[0]);
			var diffSections = DiffLib.Diff.CalculateSections(
				collection1, collection2, new CodeLineEqualityComparer(normalizeLine)
			);
			var alignedDiff = Diff.AlignElements(collection1, collection2, diffSections, new StringSimilarityDiffElementAligner());

			bool result = true;
			int line1 = 0, line2 = 0;
			const int contextSize = 10;
			int consecutiveMatches = contextSize;
			var hiddenMatches = new List<string>();

			foreach (var change in alignedDiff)
			{
				switch (change.Operation)
				{
					case DiffOperation.Match:
						AppendMatch($"{++line1,4} {++line2,4} ", change.ElementFromCollection1.Value);
						break;
					case DiffOperation.Insert:
						string pos = $"     {++line2,4} ";
						if (ShouldIgnoreChange(change.ElementFromCollection2.Value))
						{
							AppendMatch(pos, change.ElementFromCollection2.Value);
						}
						else
						{
							AppendDelta(pos, " + ", change.ElementFromCollection2.Value);
							result = false;
						}
						break;
					case DiffOperation.Delete:
						pos = $"{++line1,4}      ";
						if (ShouldIgnoreChange(change.ElementFromCollection1.Value))
						{
							AppendMatch(pos, change.ElementFromCollection1.Value);
						}
						else
						{
							AppendDelta(pos, " - ", change.ElementFromCollection1.Value);
							result = false;
						}
						break;
					case DiffOperation.Modify:
					case DiffOperation.Replace:
						AppendDelta($"{++line1,4}      ", "(-)", change.ElementFromCollection1.Value);
						AppendDelta($"     {++line2,4} ", "(+)", change.ElementFromCollection2.Value);
						result = false;
						break;
				}
			}
			if (hiddenMatches.Count > 0)
			{
				diff.WriteLine("  ...");
			}

			return result;

			void AppendMatch(string pos, string code)
			{
				consecutiveMatches++;
				if (consecutiveMatches > contextSize)
				{
					// hide this match
					hiddenMatches.Add(pos + "    " + code);
				}
				else
				{
					diff.WriteLine(pos + "    " + code);
				}
			}

			void AppendDelta(string pos, string changeType, string code)
			{
				consecutiveMatches = 0;
				if (hiddenMatches.Count > contextSize)
				{
					diff.WriteLine("  ...");
				}
				for (int i = Math.Max(0, hiddenMatches.Count - contextSize); i < hiddenMatches.Count; i++)
				{
					diff.WriteLine(hiddenMatches[i]);
				}
				hiddenMatches.Clear();
				diff.WriteLine(pos + changeType + " " + code);
			}
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
			if (index >= 0)
			{
				return line.Substring(0, index);
			}
			else if (line.StartsWith("#", StringComparison.Ordinal))
			{
				return string.Empty;
			}
			else
			{
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
				if (trivia.IsKind(SyntaxKind.DisabledTextTrivia))
				{
					return default(SyntaxTrivia); // delete
				}
				if (trivia.IsKind(SyntaxKind.PragmaWarningDirectiveTrivia))
				{
					return default(SyntaxTrivia); // delete
				}
				return base.VisitTrivia(trivia);
			}
		}

		private static IList<string> NormalizeAndSplitCode(string input, IEnumerable<string> definedSymbols)
		{
			var syntaxTree = CSharpSyntaxTree.ParseText(input, new CSharpParseOptions(preprocessorSymbols: definedSymbols));
			var result = new DeleteDisabledTextRewriter().Visit(syntaxTree.GetRoot());
			input = result.ToFullString();
			return input.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}
