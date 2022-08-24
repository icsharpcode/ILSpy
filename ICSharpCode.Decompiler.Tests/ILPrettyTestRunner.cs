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
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Tests.Helpers;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class ILPrettyTestRunner
	{
		static readonly string TestCasePath = Tester.TestCasePath + "/ILPretty";

		[Test]
		public void AllFilesHaveTests()
		{
			var testNames = typeof(ILPrettyTestRunner).GetMethods()
				.Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Any())
				.Select(m => m.Name)
				.ToArray();
			foreach (var file in new DirectoryInfo(TestCasePath).EnumerateFiles())
			{
				if (file.Extension.Equals(".il", StringComparison.OrdinalIgnoreCase))
				{
					var testName = file.Name.Split('.')[0];
					Assert.Contains(testName, testNames);
					Assert.IsTrue(File.Exists(Path.Combine(TestCasePath, testName + ".cs")));
				}
			}
		}

		[Test, Ignore("Need to decide how to represent virtual methods without 'newslot' flag")]
		public async Task Issue379()
		{
			await Run();
		}

		[Test]
		public async Task Issue646()
		{
			await Run();
		}

		[Test]
		public async Task Issue684()
		{
			await Run();
		}

		[Test]
		public async Task Issue959()
		{
			await Run();
		}

		[Test]
		public async Task Issue982()
		{
			await Run();
		}

		[Test]
		public async Task Issue1038()
		{
			await Run();
		}

		[Test]
		public async Task Issue1047()
		{
			await Run();
		}

		[Test]
		public async Task Issue1389()
		{
			await Run();
		}

		[Test]
		public async Task Issue1918()
		{
			await Run();
		}

		[Test]
		public async Task Issue1922()
		{
			await Run();
		}

		[Test]
		public async Task FSharpUsing_Debug()
		{
			await Run(settings: new DecompilerSettings { RemoveDeadStores = true, UseEnhancedUsing = false, FileScopedNamespaces = false });
		}

		[Test]
		public async Task FSharpUsing_Release()
		{
			await Run(settings: new DecompilerSettings { RemoveDeadStores = true, UseEnhancedUsing = false, FileScopedNamespaces = false });
		}

		[Test]
		public async Task DirectCallToExplicitInterfaceImpl()
		{
			await Run();
		}

		[Test]
		public async Task EvalOrder()
		{
			await Run();
		}

		[Test]
		public async Task CS1xSwitch_Debug()
		{
			await Run(settings: new DecompilerSettings { SwitchExpressions = false, FileScopedNamespaces = false });
		}

		[Test]
		public async Task CS1xSwitch_Release()
		{
			await Run(settings: new DecompilerSettings { SwitchExpressions = false, FileScopedNamespaces = false });
		}

		[Test]
		public async Task UnknownTypes()
		{
			await Run();
		}

		[Test]
		public async Task Issue1145()
		{
			await Run();
		}

		[Test]
		public async Task Issue1157()
		{
			await Run();
		}

		[Test]
		public async Task Issue1256()
		{
			await Run();
		}

		[Test]
		public async Task Issue1323()
		{
			await Run();
		}

		[Test]
		public async Task Issue1325()
		{
			await Run();
		}

		[Test]
		public async Task Issue1681()
		{
			await Run();
		}

		[Test]
		public async Task Issue1454()
		{
			await Run();
		}

		[Test]
		public async Task Issue2104()
		{
			await Run();
		}

		[Test]
		public async Task Issue2443()
		{
			await Run();
		}

		[Test]
		public async Task Issue2260SwitchString()
		{
			await Run();
		}

		[Test]
		public async Task ConstantBlobs()
		{
			await Run();
		}

		[Test]
		public async Task SequenceOfNestedIfs()
		{
			await Run();
		}

		[Test]
		public async Task Unsafe()
		{
			await Run(assemblerOptions: AssemblerOptions.Library | AssemblerOptions.UseLegacyAssembler);
		}

		[Test]
		public async Task CallIndirect()
		{
			await Run();
		}

		[Test]
		public async Task FSharpLoops_Debug()
		{
			CopyFSharpCoreDll();
			await Run(settings: new DecompilerSettings { RemoveDeadStores = true, FileScopedNamespaces = false });
		}

		[Test]
		public async Task FSharpLoops_Release()
		{
			CopyFSharpCoreDll();
			await Run(settings: new DecompilerSettings { RemoveDeadStores = true, FileScopedNamespaces = false });
		}

		[Test]
		public async Task WeirdEnums()
		{
			await Run();
		}

		[Test]
		public async Task GuessAccessors()
		{
			await Run();
		}

		async Task Run([CallerMemberName] string testName = null, DecompilerSettings settings = null,
			AssemblerOptions assemblerOptions = AssemblerOptions.Library)
		{
			if (settings == null)
			{
				// never use file-scoped namespaces, unless explicitly specified
				settings = new DecompilerSettings { FileScopedNamespaces = false };
			}
			var ilFile = Path.Combine(TestCasePath, testName + ".il");
			var csFile = Path.Combine(TestCasePath, testName + ".cs");

			var executable = await Tester.AssembleIL(ilFile, assemblerOptions).ConfigureAwait(false);
			var decompiled = await Tester.DecompileCSharp(executable, settings).ConfigureAwait(false);

			CodeAssert.FilesAreEqual(csFile, decompiled);
			Tester.RepeatOnIOError(() => File.Delete(decompiled));
		}

		static readonly object copyLock = new object();

		static void CopyFSharpCoreDll()
		{
			lock (copyLock)
			{
				if (File.Exists(Path.Combine(TestCasePath, "FSharp.Core.dll")))
					return;
				string fsharpCoreDll = Path.Combine(TestCasePath, "..\\..\\..\\ILSpy-tests\\FSharp\\FSharp.Core.dll");
				if (!File.Exists(fsharpCoreDll))
					Assert.Ignore("Ignored because of missing ILSpy-tests repo. Must be checked out separately from https://github.com/icsharpcode/ILSpy-tests!");
				File.Copy(fsharpCoreDll, Path.Combine(TestCasePath, "FSharp.Core.dll"));
			}
		}
	}
}
