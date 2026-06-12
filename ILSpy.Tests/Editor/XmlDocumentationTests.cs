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

using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// Diagnoses whether the XML-documentation lookup that backs the decompiler-view hover
/// tooltip actually surfaces non-empty docs for a well-documented system method. The
/// renderer + wiring (<c>DocumentationRenderer</c>,
/// <c>DecompilerTextView.BuildHoverContent</c>, <c>AppendXmlDocumentation</c>) are
/// already in place; this test verifies the underlying
/// <see cref="ICSharpCode.Decompiler.Documentation.XmlDocLoader.LoadDocumentation"/>
/// path produces a real doc string for at least one ubiquitous CoreLib method.
/// </summary>
[TestFixture]
public class XmlDocumentationTests
{
	[AvaloniaTest]
	public async Task XmlDocLoader_Surfaces_Documentation_For_CoreLib_String_Concat()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var stringNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.String");
		stringNode.IsExpanded = true;
		var concatNode = stringNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Concat");

		var concat = concatNode.MethodDefinition;
		Assert.That(concat, Is.Not.Null);
		Assert.That(concat!.ParentModule, Is.Not.Null);
		Assert.That(concat.ParentModule!.MetadataFile, Is.Not.Null);

		// XmlDocLoader's modern-.NET fallback (added in the shared decompiler library) walks
		// the parallel ref pack — <dotnet>/packs/Microsoft.NETCore.App.Ref/<version>/ref/<tfm>/*.xml
		// — and aggregates every XML there into a single provider, since each entity's
		// metadata-token-bearing assembly (System.Private.CoreLib.dll) differs from the one
		// whose XML carries its docs (System.Runtime.xml).
		var provider = XmlDocLoader.LoadDocumentation(concat.ParentModule.MetadataFile!);
		((object?)provider).Should().NotBeNull(
			"XmlDocLoader's modern-.NET ref-pack fallback must locate XMLs for the test-host runtime layout");

		var documentation = provider!.GetDocumentation(concat.GetIdString());
		documentation.Should().NotBeNullOrEmpty(
			"System.String.Concat is one of the most-documented methods in CoreLib — the hover tooltip would be empty without this");
		documentation.Should().Contain("<summary",
			"the raw documentation string must include the <summary> tag the renderer parses");
	}
}
