// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Tests.Helpers;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class DisassemblerPrettyTestRunner
	{
		static readonly string TestCasePath = Tester.TestCasePath + "/Disassembler/Pretty";

		[Test]
		public void AllFilesHaveTests()
		{
			var testNames = typeof(DisassemblerPrettyTestRunner).GetMethods()
				.Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Any())
				.Select(m => m.Name)
				.ToArray();
			foreach (var file in new DirectoryInfo(TestCasePath).EnumerateFiles())
			{
				if (file.Extension.Equals(".il", StringComparison.OrdinalIgnoreCase))
				{
					var testName = file.Name.Split('.')[0];
					Assert.Contains(testName, testNames);
				}
			}
		}

		[Test]
		public async Task SecurityDeclarations()
		{
			await Run();
		}

		[Test]
		public async Task SortMembers()
		{
			await Run(ilExpectedFile: Path.Combine(TestCasePath, "SortMembers.expected.il"), asmOptions: AssemblerOptions.SortedOutput);
		}

		async Task Run([CallerMemberName] string testName = null, string ilExpectedFile = null, AssemblerOptions asmOptions = AssemblerOptions.None)
		{
			var ilInputFile = Path.Combine(TestCasePath, testName + ".il");
			ilExpectedFile ??= ilInputFile;
			var ilResultFile = Path.Combine(TestCasePath, testName + ".result.il");

			var executable = await Tester.AssembleIL(ilInputFile, AssemblerOptions.Library).ConfigureAwait(false);
			var disassembled = await Tester.Disassemble(executable, ilResultFile, AssemblerOptions.UseOwnDisassembler | asmOptions).ConfigureAwait(false);

			CodeAssert.FilesAreEqual(ilExpectedFile, disassembled);
		}
	}
}
