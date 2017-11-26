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
	public class ILPrettyTestRunner
	{
		const string TestCasePath = DecompilerTestBase.TestCasePath + "/ILPretty";

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
		public void FSharpUsing_Debug()
		{
			Run(settings: new DecompilerSettings { RemoveDeadCode = true });
		}

		[Test]
		public void FSharpUsing_Release()
		{
			Run(settings: new DecompilerSettings { RemoveDeadCode = true });
		}

		[Test, Ignore]
		public void FSharpLoops_Debug()
		{
			Run(settings: new DecompilerSettings { RemoveDeadCode = true });
		}

		[Test, Ignore]
		public void FSharpLoops_Release()
		{
			Run(settings: new DecompilerSettings { RemoveDeadCode = true });
		}

		void Run([CallerMemberName] string testName = null, DecompilerSettings settings = null)
		{
			var ilFile = Path.Combine(TestCasePath, testName + ".il");
			var csFile = Path.Combine(TestCasePath, testName + ".cs");

			var executable = Tester.AssembleIL(ilFile, AssemblerOptions.Library);
			var decompiled = Tester.DecompileCSharp(executable, settings);

			CodeAssert.FilesAreEqual(csFile, decompiled);
		}
	}
}
