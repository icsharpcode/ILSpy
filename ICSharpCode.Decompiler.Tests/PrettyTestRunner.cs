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
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ICSharpCode.Decompiler.Tests.Helpers;
using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class PrettyTestRunner
	{
		static readonly string TestCasePath = Tester.TestCasePath + "/Pretty";

		[Test]
		public void AllFilesHaveTests()
		{
			var testNames = typeof(PrettyTestRunner).GetMethods()
				.Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Any())
				.Select(m => m.Name)
				.ToArray();
			foreach (var file in new DirectoryInfo(TestCasePath).EnumerateFiles()) {
				if (file.Extension.Equals(".il", StringComparison.OrdinalIgnoreCase)
					|| file.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase)) {
					var testName = file.Name.Split('.')[0];
					Assert.Contains(testName, testNames);
				}
			}
		}

		static readonly CSharpCompilerOptions[] noRoslynOptions =
		{
			CSharpCompilerOptions.None,
			CSharpCompilerOptions.Optimize
		};

		static readonly CSharpCompilerOptions[] roslynOnlyOptions =
		{
			CSharpCompilerOptions.UseRoslyn,
			CSharpCompilerOptions.Optimize | CSharpCompilerOptions.UseRoslyn
		};

		static readonly CSharpCompilerOptions[] defaultOptions =
		{
			CSharpCompilerOptions.None,
			CSharpCompilerOptions.Optimize,
			CSharpCompilerOptions.UseRoslyn,
			CSharpCompilerOptions.Optimize | CSharpCompilerOptions.UseRoslyn
		};

		static readonly CSharpCompilerOptions[] defaultOptionsWithMcs =
		{
			CSharpCompilerOptions.None,
			CSharpCompilerOptions.Optimize,
			CSharpCompilerOptions.UseRoslyn,
			CSharpCompilerOptions.Optimize | CSharpCompilerOptions.UseRoslyn,
			CSharpCompilerOptions.UseMcs,
			CSharpCompilerOptions.Optimize | CSharpCompilerOptions.UseMcs
		};

		[Test]
		public void HelloWorld()
		{
			RunForLibrary();
			RunForLibrary(asmOptions: AssemblerOptions.UseDebug);
		}
		
		[Test]
		public void InlineAssignmentTest([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void CompoundAssignmentTest([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void ShortCircuit([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void CustomShortCircuitOperators([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void ExceptionHandling([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions, decompilerSettings: new DecompilerSettings {
				NullPropagation = false,
				// legacy csc generates a dead store in debug builds
				RemoveDeadCode = (cscOptions == CSharpCompilerOptions.None)
			});
		}

		[Test]
		public void Switch([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void DelegateConstruction([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void AnonymousTypes([ValueSource("defaultOptionsWithMcs")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void Async([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void Lock([ValueSource("defaultOptionsWithMcs")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void Using([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void LiftedOperators([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void Generics([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void Loops([ValueSource("defaultOptionsWithMcs")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions, decompilerSettings: new DecompilerSettings {
				// legacy csc generates a dead store in debug builds
				RemoveDeadCode = (cscOptions == CSharpCompilerOptions.None)
			});
		}

		[Test]
		public void PropertiesAndEvents([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void AutoProperties([ValueSource("roslynOnlyOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void QueryExpressions([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void TypeAnalysisTests([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void CheckedUnchecked([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void UnsafeCode([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void PInvoke([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			// This tests needs our own disassembler; ildasm has a bug with marshalinfo.
			RunForLibrary(cscOptions: cscOptions, asmOptions: AssemblerOptions.UseOwnDisassembler);
		}

		[Test]
		public void InitializerTests([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void DynamicTests([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void ExpressionTrees([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void FixProxyCalls([Values(CSharpCompilerOptions.None, CSharpCompilerOptions.Optimize, CSharpCompilerOptions.UseRoslyn)] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void VariableNaming([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void VariableNamingWithoutSymbols([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions, decompilerSettings: new DecompilerSettings { UseDebugSymbols = false });
		}

		[Test]
		public void CS72_PrivateProtected([ValueSource("roslynOnlyOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void AsyncMain([ValueSource("roslynOnlyOptions")] CSharpCompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test]
		public void NullPropagation([ValueSource("roslynOnlyOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void CS6_StringInterpolation([ValueSource("roslynOnlyOptions")] CSharpCompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test]
		public void RefLocalsAndReturns([ValueSource("roslynOnlyOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void WellKnownConstants([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void QualifierTests([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void TupleTests([ValueSource("roslynOnlyOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void NamedArguments([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void OptionalArguments([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void Issue1080([ValueSource(nameof(roslynOnlyOptions))] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void AssemblyCustomAttributes([ValueSource(nameof(defaultOptions))] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void CustomAttributes([ValueSource(nameof(defaultOptions))] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void CustomAttributeConflicts([ValueSource(nameof(defaultOptions))] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void CustomAttributeSamples([ValueSource(nameof(defaultOptions))] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void MemberTests([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void TypeTests([ValueSource("defaultOptions")] CSharpCompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		void RunForLibrary([CallerMemberName] string testName = null, AssemblerOptions asmOptions = AssemblerOptions.None, CSharpCompilerOptions cscOptions = CSharpCompilerOptions.None, DecompilerSettings decompilerSettings = null)
		{
			Run(testName, asmOptions | AssemblerOptions.Library, cscOptions | CSharpCompilerOptions.Library, decompilerSettings);
		}

		void Run([CallerMemberName] string testName = null, AssemblerOptions asmOptions = AssemblerOptions.None, CSharpCompilerOptions cscOptions = CSharpCompilerOptions.None, DecompilerSettings decompilerSettings = null)
		{
			var ilFile = Path.Combine(TestCasePath, testName) + Tester.GetSuffix(cscOptions) + ".il";
			var csFile = Path.Combine(TestCasePath, testName + ".cs");

			if (!File.Exists(ilFile)) {
				// re-create .il file if necessary
				CompilerResults output = null;
				try {
					string outputFile = Path.ChangeExtension(ilFile,
						cscOptions.HasFlag(CSharpCompilerOptions.Library) ? ".dll" : ".exe");
					output = Tester.CompileCSharp(csFile, cscOptions, outputFile);
					Tester.Disassemble(output.PathToAssembly, ilFile, asmOptions);
				} finally {
					if (output != null)
						output.TempFiles.Delete();
				}
			}

			var executable = Tester.AssembleIL(ilFile, asmOptions);
			var decompiled = Tester.DecompileCSharp(executable, decompilerSettings ?? Tester.GetSettings(cscOptions));
			
			CodeAssert.FilesAreEqual(csFile, decompiled, Tester.GetPreprocessorSymbols(cscOptions).ToArray());
		}
	}
}
