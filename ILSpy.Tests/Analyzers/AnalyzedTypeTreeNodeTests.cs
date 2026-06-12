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
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Analyzers.TreeNodes;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

[TestFixture]
public class AnalyzedTypeTreeNodeTests
{
	[AvaloniaTest]
	public async Task Wrapping_A_TypeDefinition_Produces_The_Languages_Stringification()
	{
		// AnalyzedTypeTreeNode renders the analysed type using the same Language.TypeToString
		// the rest of the assembly tree uses — so users see "System.Object" exactly the way
		// it appears under the assembly node, not a raw ReflectionName.

		var (_, vm) = await TestHarness.BootAsync();

		var assemblyNode = vm.AssemblyTreeModel.FindCoreLib();
		var module = assemblyNode.LoadedAssembly.GetMetadataFileOrNull();
		module.Should().NotBeNull();
		var typeSystem = module!.GetTypeSystemOrNull();
		typeSystem.Should().NotBeNull();

		var objectType = typeSystem!.MainModule.TopLevelTypeDefinitions
			.Single(t => t.FullName == "System.Object");

		var node = new AnalyzedTypeTreeNode(objectType, source: null);
		TestCapture.Step("before-type-text");
		node.Text.ToString().Should().Contain("Object");
		node.Member.Should().BeSameAs(objectType);
		node.Icon.Should().NotBeNull("every entity node must surface a non-null icon");
	}

	[AvaloniaTest]
	public async Task LoadChildren_On_A_Type_Materialises_All_Applicable_Analyzer_Headers()
	{
		// Expanding an analysed type lazily fills its Children with one AnalyzerSearchTreeNode
		// per applicable analyzer (the analyzers whose Show(symbol) returns true). For a public
		// non-static reference type like System.Object, "Used By", "Instantiated By", "Exposed
		// By", and "Extension Methods" all match.

		var (_, vm) = await TestHarness.BootAsync();

		var assemblyNode = vm.AssemblyTreeModel.FindCoreLib();
		var module = assemblyNode.LoadedAssembly.GetMetadataFileOrNull();
		module.Should().NotBeNull();
		var typeSystem = module!.GetTypeSystemOrNull();
		typeSystem.Should().NotBeNull();

		var objectType = typeSystem!.MainModule.TopLevelTypeDefinitions
			.Single(t => t.FullName == "System.Object");

		var node = new AnalyzedTypeTreeNode(objectType, source: null);
		node.EnsureLazyChildren();

		var headers = node.Children.OfType<AnalyzerSearchTreeNode>()
			.Select(c => c.AnalyzerHeader)
			.ToArray();

		TestCapture.Step("before-type-analyzer-headers");
		headers.Should().Contain("Used By", "TypeUsedByAnalyzer applies to every TypeDefinition");
		headers.Should().Contain("Exposed By", "TypeExposedByAnalyzer applies to public types");
		headers.Should().Contain("Extension Methods",
			"TypeExtensionMethodsAnalyzer applies to every TypeDefinition that could be a target");
	}
}
