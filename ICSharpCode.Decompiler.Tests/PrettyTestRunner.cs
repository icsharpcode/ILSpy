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

		static readonly CompilerOptions[] noRoslynOptions =
		{
			CompilerOptions.None,
			CompilerOptions.Optimize
		};

		static readonly CompilerOptions[] roslynOnlyOptions =
		{
			CompilerOptions.UseRoslyn,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn
		};

		static readonly CompilerOptions[] dotnetCoreOnlyOptions =
		{
			CompilerOptions.UseRoslyn | CompilerOptions.ReferenceCore,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn | CompilerOptions.ReferenceCore
		};

		static readonly CompilerOptions[] defaultOptions =
		{
			CompilerOptions.None,
			CompilerOptions.Optimize,
			CompilerOptions.UseRoslyn,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn
		};

		static readonly CompilerOptions[] defaultOptionsWithMcs =
		{
			CompilerOptions.None,
			CompilerOptions.Optimize,
			CompilerOptions.UseRoslyn,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn,
			CompilerOptions.UseMcs,
			CompilerOptions.Optimize | CompilerOptions.UseMcs
		};

		[Test]
		public void HelloWorld()
		{
			RunForLibrary();
			RunForLibrary(asmOptions: AssemblerOptions.UseDebug);
		}

		[Test]
		public void IndexRangeTest([ValueSource(nameof(dotnetCoreOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void InlineAssignmentTest([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void CompoundAssignmentTest([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void ShortCircuit([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void CustomShortCircuitOperators([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void ExceptionHandling([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions, decompilerSettings: new DecompilerSettings {
				NullPropagation = false,
				// legacy csc generates a dead store in debug builds
				RemoveDeadStores = (cscOptions == CompilerOptions.None)
			});
		}

		[Test]
		public void Switch([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions, decompilerSettings: new DecompilerSettings {
				// legacy csc generates a dead store in debug builds
				RemoveDeadStores = (cscOptions == CompilerOptions.None),
				SwitchExpressions = false,
			});
		}

		[Test]
		public void SwitchExpressions([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void ReduceNesting([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void DelegateConstruction([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions | CompilerOptions.Preview);
		}

		[Test]
		public void AnonymousTypes([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void Async([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void Lock([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void Using([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(
				cscOptions: cscOptions,
				decompilerSettings: new DecompilerSettings { UseEnhancedUsing = false }
			);
		}

		[Test]
		public void UsingVariables([ValueSource(nameof(dotnetCoreOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void LiftedOperators([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void Generics([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void Loops([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions, decompilerSettings: new DecompilerSettings {
				// legacy csc generates a dead store in debug builds
				RemoveDeadStores = (cscOptions == CompilerOptions.None)
			});
		}

		[Test]
		public void LocalFunctions([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions | CompilerOptions.Preview);
		}

		[Test]
		public void PropertiesAndEvents([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void AutoProperties([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void QueryExpressions([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void TypeAnalysisTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void CheckedUnchecked([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void UnsafeCode([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void ConstructorInitializers([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void PInvoke([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			// This tests needs our own disassembler; ildasm has a bug with marshalinfo.
			RunForLibrary(cscOptions: cscOptions, asmOptions: AssemblerOptions.UseOwnDisassembler);
		}

		[Test]
		public void OutVariables([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void InitializerTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void DynamicTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void ExpressionTrees([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void FixProxyCalls([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void ValueTypes([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void VariableNaming([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions | CompilerOptions.GeneratePdb);
		}

		[Test]
		public void VariableNamingWithoutSymbols([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			var settings = Tester.GetSettings(cscOptions);
			settings.UseDebugSymbols = false;
			RunForLibrary(cscOptions: cscOptions, decompilerSettings: settings);
		}

		[Test]
		public void CS72_PrivateProtected([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void AsyncForeach([ValueSource(nameof(dotnetCoreOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void AsyncMain([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test]
		public void AsyncStreams([ValueSource(nameof(dotnetCoreOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void AsyncUsing([ValueSource(nameof(dotnetCoreOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(
				cscOptions: cscOptions,
				decompilerSettings: new DecompilerSettings { UseEnhancedUsing = false }
			);
		}

		[Test]
		public void CustomTaskType([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void NullableRefTypes([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void NativeInts([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions | CompilerOptions.Preview);
		}

		[Test]
		public void NullPropagation([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void CS6_StringInterpolation([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test]
		public void CS73_StackAllocInitializers([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void RefLocalsAndReturns([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void ThrowExpressions([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void WellKnownConstants([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void QualifierTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void TupleTests([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void NamedArguments([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void OptionalArguments([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void ConstantsTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void Issue1080([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void AssemblyCustomAttributes([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void CustomAttributes([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void CustomAttributes2([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void CustomAttributeConflicts([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void CustomAttributeSamples([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void MemberTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void MultidimensionalArray([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void EnumTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void InterfaceTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions | CompilerOptions.ReferenceCore);
		}

		[Test]
		public void TypeMemberTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void YieldReturn([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void UserDefinedConversions([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void Discards([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public void DeconstructionTests([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			RunForLibrary(cscOptions: cscOptions);
		}

		void RunForLibrary([CallerMemberName] string testName = null, AssemblerOptions asmOptions = AssemblerOptions.None, CompilerOptions cscOptions = CompilerOptions.None, DecompilerSettings decompilerSettings = null)
		{
			Run(testName, asmOptions | AssemblerOptions.Library, cscOptions | CompilerOptions.Library, decompilerSettings);
		}

		void Run([CallerMemberName] string testName = null, AssemblerOptions asmOptions = AssemblerOptions.None, CompilerOptions cscOptions = CompilerOptions.None, DecompilerSettings decompilerSettings = null)
		{
			var csFile = Path.Combine(TestCasePath, testName + ".cs");
			var exeFile = Path.Combine(TestCasePath, testName) + Tester.GetSuffix(cscOptions) + ".exe";
			if (cscOptions.HasFlag(CompilerOptions.Library)) {
				exeFile = Path.ChangeExtension(exeFile, ".dll");
			}

			// 1. Compile
			CompilerResults output = null;
			try {
				output = Tester.CompileCSharp(csFile, cscOptions, exeFile);
			} finally {
				if (output != null)
					output.TempFiles.Delete();
			}

			// 2. Decompile
			var decompiled = Tester.DecompileCSharp(exeFile, decompilerSettings ?? Tester.GetSettings(cscOptions));
			
			// 3. Compile
			CodeAssert.FilesAreEqual(csFile, decompiled, Tester.GetPreprocessorSymbols(cscOptions).ToArray());
		}
	}
}
