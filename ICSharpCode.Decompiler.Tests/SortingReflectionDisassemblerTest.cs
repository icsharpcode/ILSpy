// Copyright (c) 2022 tom-englert.de for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class SortingReflectionDisassemblerTest
	{
		[Test]
		public void MethodsAreSortedByName()
		{
			var il = GetIL((peFile, rd) => {
				var type = peFile.Metadata.TypeDefinitions.Single(handle => handle.GetFullTypeName(peFile.Metadata).Name == nameof(SortingReflectionDisassemblerTest));
				rd.DisassembleType(peFile, type);
			});

			var lines = il.Split('\n');
			var zippedLines = lines.Zip(lines.Skip(1)).Select(pair => pair.First.TrimEnd() + " " + pair.Second.Trim());

			AssertMembersAreSorted(zippedLines, "\t.method", GetMethodName);
		}

		[Test]
		public void ClassesAreSortedByName()
		{
			var il = GetIL((peFile, rd) => rd.WriteModuleContents(peFile));

			AssertMembersAreSorted(il, ".class");
		}

		private string GetMethodName(string line)
		{
			var index = line.IndexOf(" (", StringComparison.Ordinal);

			return line[..index].Split(' ').Last();
		}

		private static string GetMemberName(string il)
		{
			return string.Join(" ", il.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries).SkipWhile(token => Keywords.Contains(token)));
		}

		private void AssertMembersAreSorted(string il, string memberPrefix)
		{
			AssertMembersAreSorted(il.Split('\n'), memberPrefix);
		}

		private void AssertMembersAreSorted(IEnumerable<string> il, string memberPrefix, Func<string, string>? parseName = null)
		{
			parseName ??= a => a;

			var members = il
				.Where(line => line.StartsWith(memberPrefix))
				.Select(GetMemberName)
				.Select(parseName)
				.ToArray();

			Assert.True(members.Any());

			var sorted = members.OrderBy(i => i).ToArray();

			Assert.AreEqual(sorted, members);
		}

		private string GetIL(Action<PEFile, ReflectionDisassembler> writeIL)
		{
			string sourceFileName = GetType().Assembly.Location;

			using var peFileStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read);
			using var peFile = new PEFile(sourceFileName, peFileStream);
			using var writer = new StringWriter();

			var output = new PlainTextOutput(writer);
			var rd = new ReflectionDisassembler(output, CancellationToken.None) {
				AssemblyResolver = new UniversalAssemblyResolver(sourceFileName, true, null),
				DetectControlStructure = false,
				Filter = new SortByNameFilter()
			};

			writeIL(peFile, rd);

			return writer.ToString();
		}

		private static readonly HashSet<string> Keywords = new() {
			".class",
			".method",
			".field",
			".property",
			".event",
			"interface",
			"public",
			"private",
			"nested",
			"assembly",
			"family",
			"famandassem",
			"famorassem",
			"auto",
			"sequential",
			"explicit",
			"auto",
			"ansi",
			"unicode",
			"abstract",
			"sealed",
			"specialname",
			"import",
			"serializable",
			"windowsruntime",
			"beforefieldinit",
			"final",
			"hidebysig",
			"specialname",
			"export",
			"rtspecialname",
			"reqsecobj",
			"newslot",
			"strict",
			"abstract",
			"virtual",
			"static",
			"private",
			"famandassem",
			"assembly",
			"family",
			"famorassem",
			"public",
			"unmanaged",
			"cdecl",
			"stdcall",
			"thiscall",
			"fastcall",
			"vararg",
			"cil",
			"native",
			"optil",
			"runtime",
			"synchronized",
			"noinlining",
			"nooptimization",
			"preservesig",
			"internalcall",
			"forwardref",
			"aggressiveinlining",
			"instance"
		};

		private void DummyMethod<T>(T p1, string p2)
		{

		}
	}
}
