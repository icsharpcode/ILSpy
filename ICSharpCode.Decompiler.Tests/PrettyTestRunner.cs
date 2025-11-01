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
					Assert.That(testNames, Has.Member(testName));
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

		static readonly CompilerOptions[] roslyn4OrNewerWithNet40Options =
		{
			CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		static readonly CompilerOptions[] roslyn4OrNewerOptions =
		{
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		static readonly CompilerOptions[] defaultOptions =
		{
			CompilerOptions.None,
			CompilerOptions.Optimize,
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
			await RunForLibrary(cscOptions: cscOptions, configureDecompiler: settings => {
				settings.NullPropagation = false;
				// legacy csc generates a dead store in debug builds
				settings.RemoveDeadStores = (cscOptions == CompilerOptions.None);
				settings.FileScopedNamespaces = false;
			});
		}

		[Test]
		public async Task Switch([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions, configureDecompiler: settings => {
				// legacy csc generates a dead store in debug builds
				settings.RemoveDeadStores = (cscOptions == CompilerOptions.None);
				settings.SwitchExpressions = false;
			});
		}

		[Test]
		public async Task SwitchExpressions([ValueSource(nameof(roslyn3OrNewerOptions))] CompilerOptions cscOptions)
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
			await RunForLibrary(cscOptions: cscOptions, configureDecompiler: settings => {
				settings.QueryExpressions = false;
				settings.NullableReferenceTypes = false;
			});
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
				configureDecompiler: settings => {
					settings.UseEnhancedUsing = false;
					settings.FileScopedNamespaces = false;
				}
			);
		}

		[Test]
		public async Task UsingVariables([ValueSource(nameof(roslyn3OrNewerWithNet40Options))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task GloballyQualifiedTypeInStringInterpolation([ValueSource(nameof(roslynOnlyWithNet40Options))] CompilerOptions cscOptions)
		{
			// https://github.com/icsharpcode/ILSpy/issues/3447
			await RunForLibrary(
				cscOptions: cscOptions,
				configureDecompiler: settings => {
					settings.UsingDeclarations = false;
					settings.AlwaysUseGlobal = true;
				}
			);
		}

		[Test]
		public async Task LiftedOperators([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Operators([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
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
			await RunForLibrary(cscOptions: cscOptions, configureDecompiler: settings => {
				// legacy csc generates a dead store in debug builds
				settings.RemoveDeadStores = (cscOptions == CompilerOptions.None);
				settings.UseExpressionBodyForCalculatedGetterOnlyProperties = false;
			});
		}

		[Test]
		public async Task LocalFunctions([ValueSource(nameof(roslyn2OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
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
			await RunForLibrary(cscOptions: cscOptions | CompilerOptions.ReferenceUnsafe);
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
			await RunForLibrary(cscOptions: cscOptions, configureDecompiler: settings => settings.UseDebugSymbols = false);
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
				configureDecompiler: settings => { settings.UseEnhancedUsing = false; }
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
		public async Task NativeInts([ValueSource(nameof(roslyn3OrNewerWithNet40Options))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task FileScopedNamespaces([ValueSource(nameof(roslyn4OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions, configureDecompiler: settings => settings.FileScopedNamespaces = true);
		}

		[Test]
		public async Task Structs([ValueSource(nameof(defaultOptionsWithMcs))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task FunctionPointers([ValueSource(nameof(roslyn3OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Records([ValueSource(nameof(roslyn3OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions | CompilerOptions.NullableEnable);
		}

		[Test]
		public async Task ExtensionProperties([ValueSource(nameof(roslyn4OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions | CompilerOptions.Preview);
		}

		[Test]
		public async Task NullPropagation([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task StringInterpolation([ValueSource(nameof(roslynOnlyWithNet40Options))] CompilerOptions cscOptions)
		{
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
		public async Task RefFields([ValueSource(nameof(roslyn4OrNewerOptions))] CompilerOptions cscOptions)
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
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task OptionalArgumentsDisabled([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(
				cscOptions: cscOptions,
				configureDecompiler: settings => {
					settings.OptionalArguments = false;
				}
			);
		}

		[Test]
		public async Task Comparisons([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task ConstantsTests([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task ParamsCollections([ValueSource(nameof(roslyn4OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task ExpandParamsArgumentsDisabled([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions, configureDecompiler: settings => settings.ExpandParamsArguments = false);
		}

		[Test]
		public async Task Issue1080([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions, configureDecompiler: settings => settings.SetLanguageVersion(CSharp.LanguageVersion.CSharp6));
		}

		[Test]
		public async Task Issue3439([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Issue3406([ValueSource(nameof(roslyn4OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Issue3442([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Issue3483([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions | CompilerOptions.CheckForOverflowUnderflow, configureDecompiler: settings => settings.CheckForOverflowUnderflow = true);
		}

		[Test]
		public async Task Issue3541([ValueSource(nameof(roslyn4OrNewerWithNet40Options))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Issue3571_A([ValueSource(nameof(roslyn2OrNewerWithNet40Options))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Issue3571_B([ValueSource(nameof(roslyn2OrNewerWithNet40Options))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Issue3571_C([ValueSource(nameof(roslyn2OrNewerWithNet40Options))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task Issue3576([ValueSource(nameof(roslyn2OrNewerWithNet40Options))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task AssemblyCustomAttributes([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task CustomAttributes([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
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
		public async Task StaticAbstractInterfaceMembers([ValueSource(nameof(roslyn4OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task MetadataAttributes([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task PointerArithmetic([ValueSource(nameof(defaultOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		[Test]
		public async Task InlineArrayTests([ValueSource(nameof(roslyn4OrNewerOptions))] CompilerOptions cscOptions)
		{
			await RunForLibrary(cscOptions: cscOptions);
		}

		async Task RunForLibrary([CallerMemberName] string testName = null, AssemblerOptions asmOptions = AssemblerOptions.None, CompilerOptions cscOptions = CompilerOptions.None, Action<DecompilerSettings> configureDecompiler = null)
		{
			await Run(testName, asmOptions | AssemblerOptions.Library, cscOptions | CompilerOptions.Library, configureDecompiler);
		}

		async Task Run([CallerMemberName] string testName = null, AssemblerOptions asmOptions = AssemblerOptions.None, CompilerOptions cscOptions = CompilerOptions.None, Action<DecompilerSettings> configureDecompiler = null)
		{
			var csFile = Path.Combine(TestCasePath, testName + ".cs");
			var exeFile = TestsAssemblyOutput.GetFilePath(TestCasePath, testName, Tester.GetSuffix(cscOptions) + ".exe");
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
			var settings = Tester.GetSettings(cscOptions);
			configureDecompiler?.Invoke(settings);
			var decompiled = await Tester.DecompileCSharp(exeFile, settings).ConfigureAwait(false);

			// First try textual comparison: if equal, treat as pass and clean up
			string csCompareDiff = null;
			try
			{
				var diff = new StringWriter();
				if (!CodeComparer.Compare(File.ReadAllText(csFile), File.ReadAllText(decompiled), diff, CodeComparer.NormalizeLine, Tester.GetPreprocessorSymbols(cscOptions).Append("EXPECTED_OUTPUT").ToArray()))
				{
					csCompareDiff = diff.ToString();
					throw new AssertionException(null);
				}
				// textual match: cleanup decompiled and return
				Tester.RepeatOnIOError(() => File.Delete(decompiled));
				return;
			}
			catch (AssertionException)
			{
				// textual compare failed -> fall back to IL roundtrip
			}

			// 3. Compare by disassembling original and recompiled decompiled output to IL
			// create a per-test temp directory named by test + options to aid debugging; append ' (2)' etc only if needed
			string baseTempName = testName + Tester.GetSuffix(cscOptions);
			string tempDir = Path.Combine(Path.GetTempPath(), "ICSharpCode.Decompiler.Tests", baseTempName);
			if (Directory.Exists(tempDir))
			{
				void PrepareNewDirName()
				{
					int idx = 2;
					string newDir;
					while (true)
					{
						newDir = Path.Combine(Path.GetTempPath(), "ICSharpCode.Decompiler.Tests", baseTempName + " (" + idx + ")");
						if (!Directory.Exists(newDir))
						{
							tempDir = newDir;
							break;
						}
						idx++;
					}
				}
				const bool preferOverwrite = true;
				if (preferOverwrite)
				{
					try
					{
						Tester.RepeatOnIOError(() => Directory.Delete(tempDir, true));
					}
					catch
					{
						PrepareNewDirName();
					}
				}
				else
				{
					PrepareNewDirName();
				}
			}
			Directory.CreateDirectory(tempDir);

			string originalIl = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(exeFile) + ".original.il");
			// Use the same file name for the recompiled assembly so the logical assembly name in the IL matches.
			string recompiledAssembly = Path.Combine(tempDir, Path.GetFileName(exeFile));
			string recompiledIl = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(exeFile) + ".decompiled.il");

			// move the decompiled C# file into the temp dir so all artifacts are together
			var decompiledNewFileName = Path.GetFileNameWithoutExtension(csFile) + ".decompiled" + Path.GetExtension(csFile);
			string decompiledInTemp = Path.Combine(tempDir, decompiledNewFileName);
			if (!string.Equals(decompiled, decompiledInTemp, StringComparison.OrdinalIgnoreCase))
			{
				File.Move(decompiled, decompiledInTemp);
				decompiled = decompiledInTemp;
			}

			bool success = false;
			try
			{
				await Tester.Disassemble(exeFile, originalIl, asmOptions | AssemblerOptions.SortedOutput).ConfigureAwait(false);

				CompilerResults recompiled = null;
				try
				{
					recompiled = await Tester.CompileCSharp(decompiled, cscOptions, recompiledAssembly).ConfigureAwait(false);
				}
				finally
				{
					if (recompiled != null)
						recompiled.DeleteTempFiles();
				}

				await Tester.Disassemble(recompiledAssembly, recompiledIl, asmOptions | AssemblerOptions.SortedOutput).ConfigureAwait(false);

				CodeAssert.AreEqualIL(File.ReadAllText(originalIl), File.ReadAllText(recompiledIl));
				// IL roundtrip matched: treat test as warning
				var csCompareWarning = "The writing style does not conform to the latest recommendations:\r\n" + csCompareDiff + "\r\n\r\n";
				Assert.Warn(csCompareWarning + "Textual comparison failed but IL roundtrip matched.");
				success = true; // only set true if comparison didn't throw
			}
			finally
			{
				if (success)
				{
					// cleanup entire temp directory when comparison succeeded
					try
					{
						Tester.RepeatOnIOError(() => Directory.Delete(tempDir, true));
					}
					catch { /* ignore cleanup failures */ }
				}
				else
				{
					// leave files for debugging; print location for quick access
					var csCompareResult = "Differences between the decompiled C# source code and the original:\r\n" + csCompareDiff + "\r\n\r\n";
					try
					{ TestContext.WriteLine(csCompareResult + "Decompilation roundtrip failed. Temporary files kept in: " + tempDir); }
					catch { Console.WriteLine(csCompareResult + "Decompilation roundtrip failed. Temporary files kept in: " + tempDir); }
					// copy the original input C# file into the temp dir for convenient comparison
					try
					{ string destCs = Path.Combine(tempDir, Path.GetFileName(csFile)); Tester.RepeatOnIOError(() => File.Copy(csFile, destCs, overwrite: true)); }
					catch { }
					// copy the original compiled assembly and pdb into the temp dir for full comparison
					try
					{
						var destAsm = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(exeFile) + ".original" + Path.GetExtension(exeFile));
						Tester.RepeatOnIOError(() => File.Copy(exeFile, destAsm, overwrite: true));
						var pdb = Path.ChangeExtension(exeFile, ".pdb");
						if (File.Exists(pdb))
						{ var destPdb = Path.Combine(tempDir, Path.GetFileNameWithoutExtension(pdb) + ".original" + Path.GetExtension(pdb)); Tester.RepeatOnIOError(() => File.Copy(pdb, destPdb, overwrite: true)); }
					}
					catch { }
				}
			}
		}
	}
}
