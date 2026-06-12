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
using System.Xml.Linq;

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class ApiVisibilityFilterTests
{
	[AvaloniaTest]
	public async Task IsPublicAPI_Reflects_Method_Accessibility()
	{
		// IsPublicAPI is the gate the Filter() override consults when ShowApiLevel == PublicOnly.
		// Public / Protected / ProtectedOrInternal members count as "public API"; Internal /
		// ProtectedAndInternal / Private do not.

		// Arrange — boot, expand Enumerable, capture two methods of differing accessibility.
		var (vm, enumerableNode) = await BootAndExpandEnumerableAsync();
		var publicMethod = enumerableNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "Empty");
		TestCapture.Step("before-public-api-checks");

		// Act + Assert — Empty<T> is public static.
		publicMethod.IsPublicAPI.Should().BeTrue("Enumerable.Empty<T> is public");

		// Find a private helper somewhere in CoreLib.
		var (privateNode, privateMethod) = FindNonPublicMethodInLoadedAssemblies(vm);
		privateNode.IsPublicAPI.Should().BeFalse(
			$"{privateMethod.DeclaringType?.Name}.{privateMethod.Name} is non-public");

		// Assembly + root nodes inherit ILSpyTreeNode's true default — no accessibility to
		// weigh, so they never pick up the gray non-public styling. Without this assertion,
		// flipping the base default to false silently passes everything else even though
		// every assembly row would render gray (mutation-test hole, May 2026).
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(
			typeof(System.Linq.Enumerable).Assembly.GetName().Name!);
		assemblyNode.IsPublicAPI.Should().BeTrue(
			"AssemblyTreeNode must inherit the true default");

		// Namespaces aggregate from their children — System.Linq holds public types so its
		// namespace node reports IsPublicAPI=true.
		var publicNamespace = vm.AssemblyTreeModel.FindNode<NamespaceTreeNode>(
			"System.Linq", "System.Linq");
		publicNamespace.IsPublicAPI.Should().BeTrue(
			"a namespace whose children include public types is itself public-API");
	}

	[AvaloniaTest]
	public async Task Filter_Hides_NonPublic_Method_When_ShowApiLevel_Is_PublicOnly()
	{
		// At PublicOnly, every member tree node whose IsPublicAPI is false reports
		// FilterResult.Hidden so the assembly tree stops rendering it.

		// Arrange — boot, find a known non-public method.
		var (vm, _) = await BootAndExpandEnumerableAsync();
		var (privateNode, _) = FindNonPublicMethodInLoadedAssemblies(vm);
		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings.LanguageSettings;

		// Act + Assert — flip ApiLevel on the singleton settings between calls (they alias the
		// same instance, so we must re-read after each set rather than capture both up front).
		settings.ShowApiLevel = ApiVisibility.PublicOnly;
		privateNode.Filter(settings).Should().Be(FilterResult.Hidden);
		settings.ShowApiLevel = ApiVisibility.All;
		privateNode.Filter(settings).Should().Be(FilterResult.Match);
	}

	[AvaloniaTest]
	public async Task Filter_PublicAndInternal_Shows_NonCompilerGenerated_Private_Members()
	{
		// "Show public, private and internal" (PublicAndInternal) keeps every member that isn't
		// compiler-generated — including private + internal helpers. Only the All level loosens
		// the compiler-generated cut.

		// Arrange — boot, find a private regular method.
		var (vm, _) = await BootAndExpandEnumerableAsync();
		var (privateNode, _) = FindNonPublicMethodInLoadedAssemblies(vm);
		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings.LanguageSettings;

		// Act + Assert — visible at PublicAndInternal (Language.ShowMember returns true for
		// normal private methods). Hidden at PublicOnly. Visible at All.
		settings.ShowApiLevel = ApiVisibility.PublicAndInternal;
		privateNode.Filter(settings).Should().Be(FilterResult.Match);
		settings.ShowApiLevel = ApiVisibility.PublicOnly;
		privateNode.Filter(settings).Should().Be(FilterResult.Hidden);
		settings.ShowApiLevel = ApiVisibility.All;
		privateNode.Filter(settings).Should().Be(FilterResult.Match);
	}

	[AvaloniaTest]
	public async Task CSharpLanguage_ShowMember_Hides_Compiler_Generated_Members()
	{
		// At ShowApiLevel != All, the language-level ShowMember filter is consulted on top of
		// IsPublicAPI to drop compiler-generated members (anonymous-type backing fields, lambda
		// closure classes, async-state-machine fields, …). MemberIsHidden in the decompiler
		// already knows how to spot these.

		// Arrange — boot.
		var (vm, _) = await BootAndExpandEnumerableAsync();
		var typeSystem = GetCoreLibTypeSystem(vm);
		// Find a compiler-generated entity — every modern CoreLib type has at least one.
		var compilerGenerated = FindCompilerGeneratedMember(typeSystem);
		var languageService = AppComposition.Current.GetExport<LanguageService>();
		var csharp = languageService.Languages.Single(l => l.Name == "C#");

		// Act + Assert — language reports the member as hidden.
		csharp.ShowMember(compilerGenerated).Should().BeFalse(
			$"compiler-generated entity '{compilerGenerated.Name}' should be hidden by C#'s ShowMember");
	}

	[AvaloniaTest]
	public async Task Switching_ApiVis_PublicOnly_Reduces_Visible_Method_Count_On_Type()
	{
		// End-to-end: a type with a mix of public/non-public methods shows fewer methods after
		// flipping ApiVisPublicOnly. Drives the AssemblyListPane's children-filter pipeline that
		// re-evaluates visibility when ShowApiLevel changes.

		// Arrange — boot, find a CoreLib type with mixed accessibility (String has many).
		var (_, vm) = await TestHarness.BootAsync(3);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var stringType = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.String");
		stringType.IsExpanded = true;
		await Waiters.WaitForAsync(() => stringType.Children.OfType<MethodTreeNode>().Any());
		TestCapture.Step("string-type-expanded");

		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings.LanguageSettings;
		settings.ShowApiLevel = ApiVisibility.All;
		var allCount = CountVisibleMethods(stringType, settings);

		// Act — switch to PublicOnly.
		settings.ShowApiLevel = ApiVisibility.PublicOnly;
		var publicCount = CountVisibleMethods(stringType, settings);
		TestCapture.Step("api-visibility-public-only");

		// Assert — strictly fewer methods at PublicOnly than at All (String has internal/private
		// helpers we expect to disappear).
		publicCount.Should().BeLessThan(allCount,
			"flipping ApiVisPublicOnly should hide non-public String methods");
		publicCount.Should().BeGreaterThan(0, "public methods on String must still be visible");
	}

	[AvaloniaTest]
	public async Task AssemblyListPane_Refilters_The_Tree_In_Place_When_ShowApiLevel_Changes()
	{
		// The pane subscribes to LanguageSettings.PropertyChanged and re-applies the API-level
		// filter to the already-realised tree (ILSpyTreeNode.RefreshRealizedFilter). Without that
		// wire-up the menu radio would flip the setting but already-expanded members would keep
		// their old visibility. Asserts non-public methods become hidden in place after the flip.

		var (window, vm) = await TestHarness.BootAsync();
		await Waiters.WaitForAsync(
			() => window.GetVisualDescendants().OfType<AssemblyListPane>().Any());
		await window.WaitForComponent<AssemblyListPane>();

		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings.LanguageSettings;
		settings.ShowApiLevel = ApiVisibility.All;

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var stringType = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.String");
		stringType.IsExpanded = true;
		await Waiters.WaitForAsync(() => stringType.Children.OfType<MethodTreeNode>().Any());
		int allVisible = stringType.Children.OfType<MethodTreeNode>().Count(m => !m.IsHidden);
		allVisible.Should().BeGreaterThan(0);

		// Act — flip to PublicOnly; the pane must re-filter the already-expanded members in place.
		settings.ShowApiLevel = ApiVisibility.PublicOnly;
		TestCapture.Step("api-visibility-flipped");

		await Waiters.WaitForAsync(
			() => stringType.Children.OfType<MethodTreeNode>().Count(m => !m.IsHidden) < allVisible,
			description: "flipping ShowApiLevel must hide non-public methods in the already-realised tree");
	}

	[AvaloniaTest]
	public async Task NonPublic_Member_Rows_Render_With_NonPublicAPI_Class()
	{
		// Tree-node labels for non-public members carry a `nonPublicAPI` class so the styling
		// pipeline can paint them gray. Public rows do not. Asserting on the class (rather
		// than Foreground brush) avoids depending on theme brush resolution.

		// Arrange — boot, ensure ShowApiLevel=All so non-public methods are visible at all.
		var (window, vm) = await TestHarness.BootAsync();
		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings.LanguageSettings;
		settings.ShowApiLevel = ApiVisibility.All;

		// Pick a CoreLib type with mixed accessibility (String has plenty).
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var stringType = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(coreLibName, "System", "System.String");
		stringType.IsExpanded = true;
		await Waiters.WaitForAsync(() => stringType.Children.OfType<MethodTreeNode>().Any());
		TestCapture.Step("string-type-expanded");

		var publicMethod = stringType.Children.OfType<MethodTreeNode>().First(m => m.IsPublicAPI);
		var nonPublicMethod = stringType.Children.OfType<MethodTreeNode>().First(m => !m.IsPublicAPI);

		await Waiters.WaitForAsync(() => window.GetVisualDescendants().OfType<AssemblyListPane>().Any());
		var pane = await window.WaitForComponent<AssemblyListPane>();
		var grid = await pane.WaitForComponent<ICSharpCode.ILSpy.Controls.TreeView.SharpTreeView>();

		// Selecting each node scrolls it into view; that's how the row's TextBlock realises.
		vm.AssemblyTreeModel.SelectNode(publicMethod);
		await Waiters.WaitForAsync(() => FindRowTextBlock(grid, (string)publicMethod.Text!) != null);
		var publicLabel = FindRowTextBlock(grid, (string)publicMethod.Text!);

		vm.AssemblyTreeModel.SelectNode(nonPublicMethod);
		await Waiters.WaitForAsync(() => FindRowTextBlock(grid, (string)nonPublicMethod.Text!) != null);
		var nonPublicLabel = FindRowTextBlock(grid, (string)nonPublicMethod.Text!);
		TestCapture.Step("non-public-method-selected");

		// Assert — non-public row's TextBlock carries the class; public row does not.
		publicLabel.Should().NotBeNull();
		nonPublicLabel.Should().NotBeNull();
		nonPublicLabel!.Classes.Should().Contain("nonPublicAPI",
			"non-public members should carry the gray-text styling class");
		publicLabel!.Classes.Should().NotContain("nonPublicAPI",
			"public members should keep the default brush");

		// Cleanup — restore the default visibility level so following tests aren't tainted.
		// SettingsService is [Shared] across the composition host; the type stays expanded
		// only at the model level which is harmless, but ShowApiLevel mutation bleeds.
		settings.ShowApiLevel = ApiVisibility.PublicAndInternal;
	}

	static TextBlock? FindRowTextBlock(Control grid, string label)
		=> grid.GetVisualDescendants().OfType<TextBlock>()
			.FirstOrDefault(tb => string.Equals(tb.Text, label, System.StringComparison.Ordinal));

	[AvaloniaTest]
	public async Task Toolbar_Has_Three_ApiVisibility_Toggle_Buttons_Bound_To_LanguageSettings()
	{
		// Mirrors the View-menu radios as toolbar ToggleButtons. Each button two-way binds to a
		// mutually-exclusive bool projection on LanguageSettings, so checking one auto-unchecks
		// the others.

		// Arrange — boot.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		TestCapture.Step("booted");
		var toggles = window.GetVisualDescendants().OfType<ToggleButton>()
			.Where(t => t.Name is "ShowPublicOnlyButton" or "ShowPrivateInternalButton" or "ShowAllButton")
			.ToDictionary(t => t.Name!);
		var settings = AppComposition.Current.GetExport<SettingsService>().SessionSettings.LanguageSettings;

		// Assert — all three toolbar buttons exist.
		toggles.Should().HaveCount(3,
			"toolbar should expose ShowPublicOnly, ShowPrivateInternal, and ShowAll ToggleButtons");

		// Act + Assert — click ShowPublicOnly. The setting flips, the other two unckeck.
		settings.ShowApiLevel = ApiVisibility.PublicAndInternal;
		toggles["ShowPrivateInternalButton"].IsChecked.Should().BeTrue("baseline before click");

		toggles["ShowPublicOnlyButton"].IsChecked = true;
		TestCapture.Step("show-public-only-checked");
		settings.ShowApiLevel.Should().Be(ApiVisibility.PublicOnly);
		toggles["ShowPrivateInternalButton"].IsChecked.Should().BeFalse(
			"flipping ShowPublicOnly must propagate-cancel ShowPrivateInternal via OnPropertyChanged");
		toggles["ShowAllButton"].IsChecked.Should().BeFalse();

		// Act + Assert — same but flipping the All button.
		toggles["ShowAllButton"].IsChecked = true;
		TestCapture.Step("show-all-checked");
		settings.ShowApiLevel.Should().Be(ApiVisibility.All);
		toggles["ShowPublicOnlyButton"].IsChecked.Should().BeFalse();
		toggles["ShowPrivateInternalButton"].IsChecked.Should().BeFalse();
	}

	static int CountVisibleMethods(TypeTreeNode type, LanguageSettings settings)
		=> type.Children.OfType<MethodTreeNode>()
			.Count(m => m.Filter(settings) != FilterResult.Hidden);

	static async Task<(MainWindowViewModel vm, TypeTreeNode enumerableNode)> BootAndExpandEnumerableAsync()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var node = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		node.IsExpanded = true;
		await Waiters.WaitForAsync(() => node.Children.OfType<MethodTreeNode>().Any());
		return (vm, node);
	}

	static ICompilation GetCoreLibTypeSystem(MainWindowViewModel vm)
	{
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assembly = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Single(a => string.Equals(System.IO.Path.GetFileNameWithoutExtension(a.FileName), coreLibName, System.StringComparison.OrdinalIgnoreCase));
		return assembly.GetMetadataFileOrNull()!.GetTypeSystemOrNull()!;
	}

	static (MethodTreeNode node, IMethod method) FindNonPublicMethodInLoadedAssemblies(MainWindowViewModel vm)
	{
		var typeSystem = GetCoreLibTypeSystem(vm);
		var method = FindMethodWithAccessibility(typeSystem, Accessibility.Private);
		return (new MethodTreeNode(method), method);
	}

	static IMethod FindMethodWithAccessibility(ICompilation typeSystem, Accessibility accessibility)
		=> typeSystem.MainModule.TypeDefinitions
			.SelectMany(t => t.Methods)
			.First(m => m.Accessibility == accessibility);

	static IEntity FindCompilerGeneratedMember(ICompilation typeSystem)
		=> typeSystem.MainModule.TypeDefinitions
			.SelectMany(t => t.NestedTypes.Cast<IEntity>().Concat(t.Methods).Concat(t.Fields))
			.First(e => e.HasAttribute(KnownAttribute.CompilerGenerated));
}
