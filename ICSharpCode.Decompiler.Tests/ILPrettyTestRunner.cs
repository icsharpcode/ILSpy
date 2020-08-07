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
			foreach (var file in new DirectoryInfo(TestCasePath).EnumerateFiles()) {
				if (file.Extension.Equals(".il", StringComparison.OrdinalIgnoreCase)) {
					var testName = file.Name.Split('.')[0];
					Assert.Contains(testName, testNames);
					Assert.IsTrue(File.Exists(Path.Combine(TestCasePath, testName + ".cs")));
				}
			}
		}

		[Test, Ignore("Need to decide how to represent virtual methods without 'newslot' flag")]
		public void Issue379()
		{
			Run();
		}

		[Test]
		public void Issue646()
		{
			Run();
		}

		[Test]
		public void Issue684()
		{
			Run();
		}

		[Test]
		public void Issue959()
		{
			Run();
		}

		[Test]
		public void Issue982()
		{
			Run();
		}

		[Test]
		public void Issue1038()
		{
			Run();
		}

		[Test]
		public void Issue1047()
		{
			Run();
		}

		[Test]
		public void Issue1389()
		{
			Run();
		}

		[Test]
		public void Issue1918()
		{
			Run();
		}

		[Test]
		public void Issue1922()
		{
			Run();
		}

		[Test]
		public void FSharpUsing_Debug()
		{
			Run(settings: new DecompilerSettings { RemoveDeadStores = true });
		}

		[Test]
		public void FSharpUsing_Release()
		{
			Run(settings: new DecompilerSettings { RemoveDeadStores = true });
		}

		[Test]
		public void DirectCallToExplicitInterfaceImpl()
		{
			Run();
		}

		[Test]
		public void CS1xSwitch_Debug()
		{
			Run(settings: new DecompilerSettings { SwitchExpressions = false });
		}

		[Test]
		public void CS1xSwitch_Release()
		{
			Run(settings: new DecompilerSettings { SwitchExpressions = false });
		}

		[Test]
		public void Issue1145()
		{
			Run();
		}

		[Test]
		public void Issue1157()
		{
			Run();
		}

		[Test]
		public void Issue1256()
		{
			Run();
		}

		[Test]
		public void Issue1323()
		{
			Run();
		}

		[Test]
		public void Issue1325()
		{
			Run();
		}

		[Test]
		public void Issue1681()
		{
			Run();
		}

		[Test]
		public void Issue1454()
		{
			Run();
		}

		[Test]
		public void ConstantBlobs()
		{
			Run();
		}

		[Test]
		public void SequenceOfNestedIfs()
		{
			Run();
		}

		[Test]
		public void Unsafe()
		{
			Run();
		}

		[Test]
		public void CallIndirect()
		{
			Run();
		}

		[Test]
		public void FSharpLoops_Debug()
		{
			CopyFSharpCoreDll();
			Run(settings: new DecompilerSettings { RemoveDeadStores = true });
		}

		[Test]
		public void FSharpLoops_Release()
		{
			CopyFSharpCoreDll();
			Run(settings: new DecompilerSettings { RemoveDeadStores = true });
		}

		[Test]
		public void WeirdEnums()
		{
			Run();
		}

		void Run([CallerMemberName] string testName = null, DecompilerSettings settings = null)
		{
			var ilFile = Path.Combine(TestCasePath, testName + ".il");
			var csFile = Path.Combine(TestCasePath, testName + ".cs");

			var executable = Tester.AssembleIL(ilFile, AssemblerOptions.Library);
			var decompiled = Tester.DecompileCSharp(executable, settings);

			CodeAssert.FilesAreEqual(csFile, decompiled);
		}

		static readonly object copyLock = new object();

		static void CopyFSharpCoreDll()
		{
			lock (copyLock) {
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
