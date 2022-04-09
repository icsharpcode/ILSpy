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
			foreach (var file in new DirectoryInfo(TestCasePath).EnumerateFiles())
			{
				if (file.Extension.Equals(".il", StringComparison.OrdinalIgnoreCase)
					|| file.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase))
				{
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

		static readonly CompilerOptions[] roslynOnlyWithNet40Options =
		{
			CompilerOptions.UseRoslyn1_3_2 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn1_3_2 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn1_3_2,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn1_3_2,
			CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		static readonly CompilerOptions[] roslynOnlyOptions =
		{
			CompilerOptions.UseRoslyn1_3_2,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn1_3_2,
			CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		static readonly CompilerOptions[] roslyn2OrNewerWithNet40Options =
		{
			CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		static readonly CompilerOptions[] roslyn2OrNewerOptions =
		{
			CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		static readonly CompilerOptions[] roslyn3OrNewerWithNet40Options =
		{
			CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		static readonly CompilerOptions[] roslyn3OrNewerOptions =
		{
			CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		static readonly CompilerOptions[] roslynLatestOnlyWithNet40Options =
		{
			CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		static readonly CompilerOptions[] roslynLatestOnlyOptions =
		{
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		static readonly CompilerOptions[] defaultOptions =
		{
			CompilerOptions.None,
			CompilerOptions.Optimize,
			CompilerOptions.UseRoslyn1_3_2 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn1_3_2 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn1_3_2,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn1_3_2,
			CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		static readonly CompilerOptions[] defaultOptionsWithMcs =
		{
			CompilerOptions.None,
			CompilerOptions.Optimize,
			CompilerOptions.UseRoslyn1_3_2 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn1_3_2 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn1_3_2,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn1_3_2,
			CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
			CompilerOptions.UseMcs2_6_4,
			CompilerOptions.Optimize | CompilerOptions.UseMcs2_6_4,
			CompilerOptions.UseMcs5_23,
			CompilerOptions.Optimize | CompilerOptions.UseMcs5_23
		};

		[Test]
		public async Task HelloWorld()
		{
			await RunForLibrary();
			await RunForLibrary(asmOptions: AssemblerOptions.UseDebug);
		}

		[Test]
		public async Task IndexRangeTest([ValueSource(nameof(roslyn3OrNewerOptions))] CompilerOptions cscOptions)
		{
			if (cscOptions.HasFlag(CompilerOptions.UseRoslynLatest))
			{
				Assert.Ignore("See https://github.com/icsharpcode/ILSpy/issues/2540");
			}
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task InlineAssignmentTest([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task CompoundAssignmentTest([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task ShortCircuit([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task CustomShortCircuitOperators([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task ExceptionHandling([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions, decompilerSettings: new DecompilerSettings {
				NullPropagation = false,
				// legacy csc generates a dead store in debug builds
				RemoveDeadStores = (cscOptions == CompilerOptions.None),
				FileScopedNamespaces = false,
			});
		}

		[Test]
		public async Task Switch([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions, decompilerSettings: new DecompilerSettings {
				// legacy csc generates a dead store in debug builds
				RemoveDeadStores = (cscOptions == CompilerOptions.None),
				SwitchExpressions = false,
				FileScopedNamespaces = false,
			});
		}

		[Test]
		public async Task SwitchExpressions([ValueSource(nameof(roslynLatestOnlyOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task ReduceNesting([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task DelegateConstruction([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions | CompilerOptions.Preview);
		}

		[Test]
		public async Task AnonymousTypes([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Async([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Lock([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Using([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(
				cscOptions: cscOptions,
				decompilerSettings: new DecompilerSettings {
					UseEnhancedUsing = false,
					FileScopedNamespaces = false,
				}
			);
		}

		[Test]
		public async Task UsingVariables([ValueSource(nameof(roslyn3OrNewerWithNet40Options))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task LiftedOperators([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Generics([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Loops([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions, decompilerSettings: new DecompilerSettings {
				// legacy csc generates a dead store in debug builds
				RemoveDeadStores = (cscOptions == CompilerOptions.None),
				UseExpressionBodyForCalculatedGetterOnlyProperties = false,
				FileScopedNamespaces = false,
			});
		}

		[Test]
		public async Task LocalFunctions([ValueSource(nameof(roslyn2OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions | CompilerOptions.Preview);
		}

		[Test]
		public async Task PropertiesAndEvents([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions | CompilerOptions.NullableEnable);
		}

		[Test]
		public async Task AutoProperties([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task QueryExpressions([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task TypeAnalysisTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task CheckedUnchecked([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task UnsafeCode([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task ConstructorInitializers([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task PInvoke([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			// This tests needs our own disassembler; ildasm has a bug with marshalinfo.
			await RunForLibrary(cscOptions: cscOptions, asmOptions: AssemblerOptions.UseOwnDisassembler);
		}

		[Test]
		public async Task OutVariables([ValueSource(nameof(roslyn2OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task PatternMatching([ValueSource(nameof(roslyn2OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task InitializerTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions | CompilerOptions.NullableEnable);
		}

		[Test]
		public async Task DynamicTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task ExpressionTrees([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task FixProxyCalls([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task ValueTypes([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task VariableNaming([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions | CompilerOptions.GeneratePdb);
		}

		[Test]
		public async Task VariableNamingWithoutSymbols([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			var settings = Tester.GetSettings(cscOptions);
			settings.UseDebugSymbols = false;
			await RunForLibrary(cscOptions: cscOptions, decompilerSettings: settings);
		}

		[Test]
		public async Task CS72_PrivateProtected([ValueSource(nameof(roslyn2OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task AsyncForeach([ValueSource(nameof(roslyn3OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task AsyncMain([ValueSource(nameof(roslyn2OrNewerOptions))] CompilerOptions cscOptions)
		{
			await Run(cscOptions: cscOptions);
		}

		[Test]
		public async Task AsyncStreams([ValueSource(nameof(roslyn3OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task AsyncUsing([ValueSource(nameof(roslyn3OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(
				cscOptions: cscOptions,
				decompilerSettings: new DecompilerSettings { UseEnhancedUsing = false, FileScopedNamespaces = false }
			);
		}

		[Test]
		public async Task CustomTaskType([ValueSource(nameof(roslyn2OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task NullableRefTypes([ValueSource(nameof(roslyn3OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions | CompilerOptions.NullableEnable);
		}

		[Test]
		public async Task NativeInts([ValueSource(nameof(roslynLatestOnlyOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions | CompilerOptions.Preview);
		}

		[Test]
		public async Task FileScopedNamespaces([ValueSource(nameof(roslynLatestOnlyOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions, decompilerSettings: new DecompilerSettings());
		}

		[Test]
		public async Task Structs([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task FunctionPointers([ValueSource(nameof(roslynLatestOnlyOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions | CompilerOptions.Preview);
		}

		[Test]
		public async Task Records([ValueSource(nameof(roslynLatestOnlyOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions | CompilerOptions.NullableEnable);
		}

		[Test]
		public async Task NullPropagation([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task StringInterpolation([ValueSource(nameof(roslynOnlyWithNet40Options))] CompilerOptions cscOptions)
		{
			if (!cscOptions.HasFlag(CompilerOptions.TargetNet40) && cscOptions.HasFlag(CompilerOptions.UseRoslynLatest))
			{
				Assert.Ignore("DefaultInterpolatedStringHandler is not yet supported!");
				return;
			}

			await Run(cscOptions: cscOptions);
		}

		[Test]
		public async Task CS73_StackAllocInitializers([ValueSource(nameof(roslyn3OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task RefLocalsAndReturns([ValueSource(nameof(roslyn2OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task ThrowExpressions([ValueSource(nameof(roslyn2OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task WellKnownConstants([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task QualifierTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task TupleTests([ValueSource(nameof(roslyn2OrNewerOptions))] CompilerOptions cscOptions)
		{
			if (cscOptions.HasFlag(CompilerOptions.UseRoslynLatest))
			{
				Assert.Ignore("DefaultInterpolatedStringHandler is not yet supported!");
				return;
			}

			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task NamedArguments([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task OptionalArguments([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions | CompilerOptions.Preview);
		}

		[Test]
		public async Task ConstantsTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Issue1080([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions, decompilerSettings: new DecompilerSettings(CSharp.LanguageVersion.CSharp6));
		}

		[Test]
		public async Task AssemblyCustomAttributes([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task CustomAttributes([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			if (cscOptions.HasFlag(CompilerOptions.UseRoslynLatest))
			{
				// Test C# 11 generic attributes
				cscOptions |= CompilerOptions.Preview;
			}
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task CustomAttributes2([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task CustomAttributeConflicts([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task CustomAttributeSamples([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task MemberTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task MultidimensionalArray([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task EnumTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task InterfaceTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task TypeMemberTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task YieldReturn([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task UserDefinedConversions([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Discards([ValueSource(nameof(roslyn2OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task DeconstructionTests([ValueSource(nameof(roslyn2OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task CS9_ExtensionGetEnumerator([ValueSource(nameof(roslyn3OrNewerWithNet40Options))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task CovariantReturns([ValueSource(nameof(roslyn3OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task StaticAbstractInterfaceMembers([ValueSource(nameof(roslynLatestOnlyOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions | CompilerOptions.Preview);
		}

		async Task RunForLibrary([CallerMemberName] string testName = null, AssemblerOptions asmOptions = AssemblerOptions.None, CompilerOptions cscOptions = CompilerOptions.None, DecompilerSettings decompilerSettings = null)
		{
			await Run(testName, asmOptions | AssemblerOptions.Library, cscOptions | CompilerOptions.Library, decompilerSettings);
		}

		async Task Run([CallerMemberName] string testName = null, AssemblerOptions asmOptions = AssemblerOptions.None, CompilerOptions cscOptions = CompilerOptions.None, DecompilerSettings decompilerSettings = null)
		{
			var csFile = Path.Combine(TestCasePath, testName + ".cs");
			var exeFile = Path.Combine(TestCasePath, testName) + Tester.GetSuffix(cscOptions) + ".exe";
			if (cscOptions.HasFlag(CompilerOptions.Library))
			{
				exeFile = Path.ChangeExtension(exeFile, ".dll");
			}

			// 1. Compile
			CompilerResults output = null;
			try
			{
				output = await Tester.CompileCSharp(csFile, cscOptions, exeFile).ConfigureAwait(false);
			}
			finally
			{
				if (output != null)
					output.DeleteTempFiles();
			}

			// 2. Decompile
			var decompiled = await Tester.DecompileCSharp(exeFile, decompilerSettings ?? Tester.GetSettings(cscOptions)).ConfigureAwait(false);

			// 3. Compile
			CodeAssert.FilesAreEqual(csFile, decompiled, Tester.GetPreprocessorSymbols(cscOptions).ToArray());
		}
	}
}
