// Copyright (c) 2026 Piero Viano
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Navigation;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Navigation;

/// <summary>Interface member whose rename must reach the implementations and every call site.</summary>
public interface IRenameTarget
{
	int Compute(int value);
}

/// <summary>Implements <see cref="IRenameTarget"/> with a virtual member that is overridden below.</summary>
public class RenameTargetBase : IRenameTarget
{
	public virtual int Compute(int value) => value;
}

/// <summary>Overrides the virtual implementation.</summary>
public class RenameTargetDerived : RenameTargetBase
{
	public override int Compute(int value) => value + 1;
}

/// <summary>Calls the member through the interface and through the derived class.</summary>
public static class RenameTargetCaller
{
	public static int ViaInterface(IRenameTarget target) => target.Compute(1);

	public static int ViaDerived() => new RenameTargetDerived().Compute(2);

	public static int Unrelated() => 3;
}

/// <summary>
/// Read-only "Find Usages for Rename": for a symbol, everything a rename would have to touch --
/// its declaration, related declarations (implementations / overrides / base members, type
/// constructors) and every usage across the loaded assemblies -- without editing anything.
/// </summary>
[TestFixture]
public class FindUsagesForRenameTests
{
	static async Task<(MainWindow Window, MainWindowViewModel Vm, ITypeDefinition Base)> SetupAsync()
	{
		var (window, vm) = await TestHarness.BootAsync();
		await vm.OpenAssemblyAsync(typeof(RenameTargetBase).Assembly.Location);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"ILSpy.Tests",
			"ICSharpCode.ILSpy.Tests.Navigation",
			"ICSharpCode.ILSpy.Tests.Navigation.RenameTargetBase");
		return (window, vm, typeNode.TypeDefinition);
	}

	static string Names(System.Collections.Generic.IEnumerable<IEntity> entities)
		=> string.Join(", ", entities.Select(e => e.DeclaringTypeDefinition?.Name + "." + e.Name));

	[AvaloniaTest]
	public async Task Virtual_Member_Collects_Base_Interface_Override_And_All_Call_Sites()
	{
		var (_, vm, baseType) = await SetupAsync();
		var compute = baseType.Methods.Single(m => m.Name == nameof(RenameTargetBase.Compute));

		var result = await RenameUsageFinder.FindAsync(compute, vm.AssemblyTreeModel.AssemblyList!,
			vm.DockWorkspace.ActiveDecompilerTab?.Language ?? AppComposition.Current.GetExport<ICSharpCode.ILSpy.Languages.LanguageService>().CurrentLanguage,
			CancellationToken.None);

		Names(result.Declarations).Should().Be("RenameTargetBase.Compute");
		Names(result.RelatedDeclarations).Should().Contain("IRenameTarget.Compute")
			.And.Contain("RenameTargetDerived.Compute");
		var usageNames = result.Usages.Select(u => u.DeclaringTypeDefinition?.Name + "." + u.Name).ToList();
		usageNames.Should().Contain("RenameTargetCaller.ViaInterface", "the interface call site must change too");
		usageNames.Should().Contain("RenameTargetCaller.ViaDerived", "the call through the override must change too");
		usageNames.Should().NotContain("RenameTargetCaller.Unrelated");
	}

	[AvaloniaTest]
	public async Task Type_Collects_Its_Constructors_And_Usages()
	{
		var (_, vm, baseType) = await SetupAsync();
		var derived = baseType.ParentModule!.TopLevelTypeDefinitions.Single(t => t.Name == nameof(RenameTargetDerived));

		var result = await RenameUsageFinder.FindAsync(derived, vm.AssemblyTreeModel.AssemblyList!,
			AppComposition.Current.GetExport<ICSharpCode.ILSpy.Languages.LanguageService>().CurrentLanguage, CancellationToken.None);

		result.Declarations.Should().ContainSingle().Which.Name.Should().Be(nameof(RenameTargetDerived));
		result.RelatedDeclarations.OfType<IMethod>().Should().Contain(m => m.IsConstructor,
			"a type's constructors carry its name");
		result.Usages.Select(u => u.Name).Should().Contain(nameof(RenameTargetCaller.ViaDerived));
	}

	[AvaloniaTest]
	public async Task Command_Opens_A_Navigable_Report_For_The_Selected_Member()
	{
		var (_, vm, _) = await SetupAsync();
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"ILSpy.Tests",
			"ICSharpCode.ILSpy.Tests.Navigation",
			"ICSharpCode.ILSpy.Tests.Navigation.RenameTargetBase");
		typeNode.EnsureLazyChildren();
		var methodNode = typeNode.Children.OfType<MethodTreeNode>().Single(m => m.MethodDefinition.Name == nameof(RenameTargetBase.Compute));
		vm.AssemblyTreeModel.SelectNode(methodNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>().GetCommand(nameof(Resources.FindUsagesForRename));
		command.CanExecute(null).Should().BeTrue();
		command.Execute(null);

		DecompilerTabPageModel? report = null;
		await Waiters.WaitForAsync(() => {
			report = vm.DockWorkspace.Documents!.VisibleDockables!.OfType<ContentTabPage>()
				.Select(t => t.Content).OfType<DecompilerTabPageModel>()
				.FirstOrDefault(t => t.Title?.StartsWith(Resources.FindUsagesForRename, StringComparison.Ordinal) == true
					&& !t.IsDecompiling && t.Text.Contains("ViaInterface"));
			return report != null;
		}, null, "the Find Usages for Rename report tab");

		report!.Text.Should().Contain("ILSpy.Tests", "results are grouped by assembly");
		report.Text.Should().Contain("RenameTargetCaller", "and by declaring type");
		var link = report.References!.FirstOrDefault(r => r.Reference is IMethod { Name: nameof(RenameTargetCaller.ViaInterface) });
		link.Should().NotBeNull("every row links to its entity");

		report.RaiseNavigateRequested(link!);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var selected = ((object?)vm.AssemblyTreeModel.SelectedItem).Should().BeOfType<MethodTreeNode>().Subject;
		selected.MethodDefinition.Name.Should().Be(nameof(RenameTargetCaller.ViaInterface));
	}

	[AvaloniaTest]
	public async Task Context_Menu_Entry_Is_In_The_Navigation_Category_And_Visible_For_Code_References()
	{
		var (_, _, baseType) = await SetupAsync();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var export = registry.Entries.Single(e => e.Metadata.Header == nameof(Resources.FindUsagesForRename));
		export.Metadata.Category.Should().Be("Navigation");

		var compute = baseType.Methods.Single(m => m.Name == nameof(RenameTargetBase.Compute));
		var segment = new ReferenceSegment { Reference = compute, StartOffset = 0, Length = 1 };
		export.Value.IsVisible(new TextViewContext { Reference = segment }).Should().BeTrue();
		export.Value.IsVisible(new TextViewContext()).Should().BeFalse();
	}
}
