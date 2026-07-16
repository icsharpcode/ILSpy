// Copyright (c) 2026 Siegfried Pammer
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

using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// The tree resolves each type once, when its assembly's namespace band is built, and holds the
/// resulting entity. Those entities belong to one compilation, and only one is cached per module --
/// keyed on the effective decompiler settings. Anything that changes those settings drops the cached
/// compilation, so the tree has to re-resolve or it is left holding entities from a compilation that
/// no longer exists.
/// </summary>
[TestFixture]
public class TypeSystemStalenessTests
{
	static TypeTreeNode FirstTypeNode(AssemblyTreeNode assembly)
		=> assembly.Children.OfType<NamespaceTreeNode>()
			.SelectMany(ns => ns.Children)
			.OfType<TypeTreeNode>()
			.First();

	[AvaloniaTest]
	public async Task Changing_The_Language_Version_Re_Resolves_The_Tree_Against_The_New_Type_System()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var languageService = AppComposition.Current.GetExport<LanguageService>();
		var originalVersion = languageService.CurrentVersion;

		try
		{
			var assembly = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(TreeNavigation.CoreLibName);
			assembly.IsExpanded = true;
			assembly.EnsureLazyChildren();

			var before = FirstTypeNode(assembly).TypeDefinition.Compilation;

			// C# 1 switches off the language features that map onto TypeSystemOptions (dynamic,
			// tuples, nullable annotations, ...), so the module's cached compilation is rebuilt.
			var oldest = languageService.CurrentLanguage.LanguageVersions.First();
			Assert.That(oldest, Is.Not.EqualTo(originalVersion),
				"the fixture language must offer a version other than the active one, or this test "
				+ "changes nothing");
			languageService.CurrentVersion = oldest;

			var after = FirstTypeNode(assembly).TypeDefinition.Compilation;
			Assert.That(after, Is.Not.SameAs(before),
				"the tree must re-resolve its entities after the language version changes the effective "
				+ "decompiler settings; holding the old compilation's entities leaves every label, icon "
				+ "and filter rendering against a type system the rest of the app has already dropped");
		}
		finally
		{
			languageService.CurrentVersion = originalVersion;
		}
	}

	[AvaloniaTest]
	public async Task Rebuilding_After_A_Language_Version_Change_Restores_The_Selected_Node()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var languageService = AppComposition.Current.GetExport<LanguageService>();
		var originalVersion = languageService.CurrentVersion;

		try
		{
			var model = vm.AssemblyTreeModel;
			var assembly = model.FindNode<AssemblyTreeNode>(TreeNavigation.CoreLibName);
			assembly.IsExpanded = true;
			assembly.EnsureLazyChildren();

			var selected = FirstTypeNode(assembly);
			model.SelectedItem = selected;
			var pathBefore = AssemblyTreeModel.GetPathForNode(selected);

			languageService.CurrentVersion = languageService.CurrentLanguage.LanguageVersions.First();

			// The rebuild replaces the node objects, so identity cannot survive -- the path must.
			Assert.That(model.SelectedItem, Is.Not.Null,
				"the rebuild must not drop the selection: the node the user was looking at has to come back");
			Assert.That(AssemblyTreeModel.GetPathForNode(model.SelectedItem), Is.EqualTo(pathBefore),
				"the same node, identified by path, must be selected again after the tree is rebuilt");
			Assert.That(model.SelectedItem, Is.Not.SameAs(selected),
				"and it must be the freshly resolved node, not the one holding the dropped compilation's entity");
		}
		finally
		{
			languageService.CurrentVersion = originalVersion;
		}
	}
}
