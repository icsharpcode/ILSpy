// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Analyzers;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

/// <summary>
/// Regression: analysing the "Used By" of a constructor must not throw. The reported
/// user-visible failure was an InvalidCastException in the live app; this test exercises
/// the same path (analyzer enumeration + the search-node WrapResult switch) but bypasses
/// the search node's catch so any exception bubbles up to NUnit with its stack trace.
/// </summary>
[TestFixture]
public class AnalyzerConstructorUsesTests
{
	[AvaloniaTest]
	public async Task Used_By_Analyzer_Runs_Over_A_Constructor_Without_Throwing()
	{
		var (_, vm) = await TestHarness.BootAsync();

		// Pick any concrete type with at least one public instance ctor; the type itself
		// doesn't matter — what matters is that the symbol fed to the analyzer is a method
		// whose IsConstructor is true.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Lookup`2");
		typeNode.EnsureLazyChildren();
		var ctorTreeNode = typeNode.Children.OfType<MethodTreeNode>()
			.FirstOrDefault(m => m.MethodDefinition.IsConstructor);
		(ctorTreeNode is null).Should().BeFalse(
			"Lookup<TKey,TElement> defines at least one constructor — the test depends on it");
		var ctor = (IMethod)ctorTreeNode!.Member!;
		ctor.IsConstructor.Should().BeTrue();

		// Resolve the "Used By" analyzer that actually applies to this method.
		var analyzer = AnalyzerTreeNode.Analyzers
			.Where(a => a.Metadata?.Header == "Used By")
			.Select(a => a.CreateExport().Value)
			.First(a => a.Show(ctor));

		// Drive Analyze directly (no AnalyzerSearchTreeNode → no swallowing catch). Take
		// a small slice so we don't pay for a full scan; the bug repros on the first call
		// site if it's going to throw at all.
		var assemblyList = vm.AssemblyTreeModel.AssemblyList!;
		var context = new AnalyzerContext {
			CancellationToken = CancellationToken.None,
			Language = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Languages.LanguageService>().CurrentLanguage,
			AssemblyList = assemblyList,
		};
		var results = analyzer.Analyze(ctor, context).Take(20).ToList();
		TestCapture.Step("before-used-by-results");
		results.Should().NotBeEmpty("constructors of public LINQ types must have at least one in-tree caller");
	}

	[AvaloniaTest]
	public async Task Uses_Analyzer_Runs_Over_A_Constructor_Without_Throwing()
	{
		// "Uses" is the forward direction (what does the body of this method touch?).
		// MethodUsesAnalyzer can return type/method/field/property/event ISymbols — the
		// path most likely to surface a switch-arm hole for some result shape we missed.

		var (_, vm) = await TestHarness.BootAsync();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Lookup`2");
		typeNode.EnsureLazyChildren();
		var ctor = (IMethod)typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.IsConstructor).Member!;

		var analyzer = AnalyzerTreeNode.Analyzers
			.Where(a => a.Metadata?.Header == "Uses")
			.Select(a => a.CreateExport().Value)
			.First(a => a.Show(ctor));

		var assemblyList = vm.AssemblyTreeModel.AssemblyList!;
		var context = new AnalyzerContext {
			CancellationToken = CancellationToken.None,
			Language = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Languages.LanguageService>().CurrentLanguage,
			AssemblyList = assemblyList,
		};
		var results = analyzer.Analyze(ctor, context).Take(20).ToList();
		TestCapture.Step("before-uses-results");
		results.Should().NotBeEmpty();
	}

	[AvaloniaTest]
	public async Task Analyze_Pushes_A_Constructor_Onto_The_Pane_Without_Throwing()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Lookup`2");
		typeNode.EnsureLazyChildren();
		var ctor = (IMethod)typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.IsConstructor).Member!;

		var analyzerVm = AppComposition.Current.GetExport<AnalyzerTreeViewModel>();
		var beforeCount = analyzerVm.Root.Children.Count;

		// This was the user-reported throw point.
		analyzerVm.Analyze(ctor);
		TestCapture.Step("constructor-pushed-to-pane");

		analyzerVm.Root.Children.Count.Should().Be(beforeCount + 1);
	}
}
