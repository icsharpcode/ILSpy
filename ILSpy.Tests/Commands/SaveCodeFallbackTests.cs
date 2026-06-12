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

using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Right-click "Save Code" must work for nodes that don't override <see cref="ILSpyTreeNode.Save"/>
/// (types, namespaces, members): those need the same decompile-to-file fallback that the Ctrl+S
/// command has, applied to the clicked node. Previously the context entry only called Save() and
/// did nothing when it returned false.
/// </summary>
[TestFixture]
public class SaveCodeFallbackTests
{
	[AvaloniaTest]
	public async Task A_TypeTreeNode_Has_No_Custom_Save_So_The_Fallback_Is_Required()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		Assert.That(typeNode, Is.Not.Null);

		typeNode!.Save().Should().BeFalse(
			"a TypeTreeNode has no Save() override, so Save Code must fall through to decompile-to-file");
	}

	[AvaloniaTest]
	public async Task SaveCode_Writes_A_Clicked_Type_Node_To_A_File()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var fixture = await vm.OpenFixtureAsync();
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			fixture.ShortName, fixture.ShortName, $"{fixture.ShortName}.{FixtureAssembly.TypeName}");
		Assert.That(typeNode, Is.Not.Null);

		var language = AppComposition.Current.GetExport<LanguageService>()
			.Languages.OfType<CSharpLanguage>().First();

		var path = Path.Combine(Path.GetTempPath(), "ILSpySaveType_" + System.Guid.NewGuid().ToString("N") + ".cs");
		try
		{
			await SaveCodeHelper.WriteNodeToFileAsync(typeNode!, language, path);

			File.Exists(path).Should().BeTrue();
			var contents = await File.ReadAllTextAsync(path);
			contents.Should().Contain(FixtureAssembly.TypeName, "the decompiled type's name must be present");
			contents.Should().Contain(FixtureAssembly.MemberName, "the type's members must be decompiled (FullDecompilation)");
		}
		finally
		{
			if (File.Exists(path))
				File.Delete(path);
		}
	}
}
