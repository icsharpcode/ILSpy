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

using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.Symbols;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX.Symbols;
using ICSharpCode.ILSpyX.TreeView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Symbols;

[TestFixture]
public class SymbolServiceTests
{
	[Test]
	public void Settings_default_to_the_public_servers_without_auto_download_or_hosting()
	{
		var settings = new SymbolSettings();
		settings.LoadFromXml(new XElement("SymbolSettings"));

		settings.SymbolPath.Should().Be(SymbolPath.DefaultSymbolPath);
		settings.UseEnvironmentSymbolPath.Should().BeTrue();
		settings.AutoDownload.Should().BeFalse("network lookups while loading must be opted into");
		settings.StartSymbolServer.Should().BeFalse();
		settings.SymbolServerPort.Should().Be(SymbolSettings.DefaultPort);
	}

	[Test]
	public void Settings_round_trip_through_xml()
	{
		var settings = new SymbolSettings {
			SymbolPath = @"C:\syms", UseEnvironmentSymbolPath = false, CacheDirectory = @"D:\cache",
			AutoDownload = true, StartSymbolServer = true, SymbolServerPort = 4242
		};

		var copy = new SymbolSettings();
		copy.LoadFromXml(settings.SaveToXml());

		copy.SymbolPath.Should().Be(@"C:\syms");
		copy.UseEnvironmentSymbolPath.Should().BeFalse();
		copy.CacheDirectory.Should().Be(@"D:\cache");
		copy.AutoDownload.Should().BeTrue();
		copy.StartSymbolServer.Should().BeTrue();
		copy.SymbolServerPort.Should().Be(4242);
	}

	[Test]
	public void Environment_symbol_path_is_searched_first_when_enabled()
	{
		var settings = new SymbolSettings { SymbolPath = @"C:\mine" };

		SymbolService.ComposeSymbolPath(settings, @"srv*https://env.example").Should().Be(@"srv*https://env.example;C:\mine");
		SymbolService.ComposeSymbolPath(settings, null).Should().Be(@"C:\mine");
		settings.UseEnvironmentSymbolPath = false;
		SymbolService.ComposeSymbolPath(settings, @"srv*https://env.example").Should().Be(@"C:\mine");
	}

	[AvaloniaTest]
	public void Locator_follows_the_settings_and_reaches_the_assembly_list_manager()
	{
		var service = AppComposition.Current.GetExport<SymbolService>();
		var manager = AppComposition.Current.GetExport<SettingsService>().AssemblyListManager;
		manager.SymbolLocator.Should().BeSameAs(service.Locator);

		service.Settings.UseEnvironmentSymbolPath = false;
		service.Settings.SymbolPath = @"C:\a;C:\b";
		service.Settings.AutoDownload = true;

		service.Locator.SymbolPath.Elements.Select(e => e.Location).Should().Equal(@"C:\a", @"C:\b");
		service.Locator.AutoDownload.Should().BeTrue();
		manager.SymbolLocator.Should().BeSameAs(service.Locator);
	}

	[AvaloniaTest]
	public async Task Options_dialog_has_a_symbols_page()
	{
		var (_, vm) = await TestHarness.BootAsync();
		AppComposition.Current.GetExport<MainMenuCommandRegistry>().GetCommand(nameof(Resources._Options)).Execute(null);
		var model = vm.DockWorkspace.Documents!.VisibleDockables!.OfType<ContentTabPage>()
			.Select(t => t.Content).OfType<OptionsPageModel>().Single();

		var page = model.Pages.OfType<SymbolSettingsViewModel>().Should().ContainSingle().Subject;
		page.Title.Should().Be(Resources.Symbols);
		page.Settings.Should().BeSameAs(AppComposition.Current.GetExport<SymbolService>().Settings);
	}

	[AvaloniaTest]
	public async Task Load_symbols_entry_finds_the_pdb_on_the_symbol_path()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var fixture = SymbolFixture.Create();
		var pdb = fixture.DetachPdb();
		var service = AppComposition.Current.GetExport<SymbolService>();
		service.Settings.UseEnvironmentSymbolPath = false;
		service.Settings.SymbolPath = Path.GetDirectoryName(pdb)!;
		var assembly = await vm.OpenAssemblyAsync(fixture.AssemblyPath);
		assembly.GetDebugInfoOrNull().Should().BeNull("precondition: the PDB is not next to the assembly");
		var node = vm.AssemblyTreeModel.FindAssemblyNode(assembly)!;

		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().GetEntry(nameof(Resources.LoadSymbolsFromSymbolServer));
		var context = new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { node } };
		entry.IsVisible(context).Should().BeTrue();
		entry.Execute(context);

		await Waiters.WaitForAsync(() => assembly.GetDebugInfoOrNull() != null, description: "symbols to load from the symbol path");
		assembly.PdbFileName.Should().Be(pdb);
	}

	[AvaloniaTest]
	public async Task View_original_source_opens_the_source_link_document_in_a_new_tab()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var fixture = SymbolFixture.Create();
		var handler = new FakeHttpHandler();
		handler.Add(SymbolFixture.SourceLinkUri, Encoding.UTF8.GetBytes("// original\nclass Greeter {}\n"));
		var service = AppComposition.Current.GetExport<SymbolService>();
		service.HttpClient = new HttpClient(handler);
		var assembly = await vm.OpenAssemblyAsync(fixture.AssemblyPath);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(fixture.Name, fixture.Name, fixture.Name + "." + SymbolFixture.TypeName);

		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().GetEntry(nameof(Resources.ViewOriginalSource));
		var context = new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { typeNode } };
		entry.IsVisible(context).Should().BeTrue("the type has a portable PDB with a Source Link map");
		entry.Execute(context);

		DecompilerTabPageModel? Tab() => vm.DockWorkspace.Documents?.VisibleDockables?.OfType<ContentTabPage>()
			.Select(t => t.Content).OfType<DecompilerTabPageModel>().FirstOrDefault(m => m.Title == "Greeter.cs");
		await Waiters.WaitForAsync(() => Tab()?.Text.Contains("// original") == true, description: "the original source tab");
		Tab()!.SyntaxExtension.Should().Be(".cs");
		Tab()!.Text.Should().Contain(SymbolFixture.SourceLinkUri, "the header names where the source came from");
	}

	[AvaloniaTest]
	public async Task View_original_source_is_hidden_without_debug_info()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var fixture = SymbolFixture.Create();
		fixture.DetachPdb();
		await vm.OpenAssemblyAsync(fixture.AssemblyPath);
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(fixture.Name, fixture.Name, fixture.Name + "." + SymbolFixture.TypeName);

		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().GetEntry(nameof(Resources.ViewOriginalSource));

		entry.IsVisible(new TextViewContext { SelectedTreeNodes = new SharpTreeNode[] { typeNode } }).Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Symbol_server_serves_pdbs_for_the_open_assemblies()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var fixture = SymbolFixture.Create();
		fixture.DetachPdb();
		await vm.OpenAssemblyAsync(fixture.AssemblyPath);
		var service = AppComposition.Current.GetExport<SymbolService>();
		service.Settings.SymbolServerPort = 0;
		service.Settings.CacheDirectory = SymbolFixture.NewTempDirectory();

		service.StartServer();
		try
		{
			service.IsServerRunning.Should().BeTrue();
			service.ServerAddress.Should().StartWith("http://localhost:");
			using var reader = new PEReader(File.OpenRead(fixture.AssemblyPath));
			var key = SymbolKey.GetPdbKeys(reader).Single();
			using var client = new HttpClient { BaseAddress = new System.Uri(service.ServerAddress!) };

			var response = await client.GetAsync(key.Key);

			response.StatusCode.Should().Be(HttpStatusCode.OK);
		}
		finally
		{
			await service.StopServerAsync();
		}
		service.IsServerRunning.Should().BeFalse();
		service.ServerAddress.Should().BeNull();
	}
}
