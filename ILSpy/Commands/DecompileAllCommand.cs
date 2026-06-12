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

#if DEBUG
using System;
using System.Collections.Concurrent;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// DEBUG-only File-menu stress test: decompile every loaded assembly to
	/// <c>c:\temp\decompiled\&lt;ShortName&gt;.cs</c> in parallel and report per-assembly
	/// timing in a results tab. Hard-coded output path matches WPF — the command's
	/// CanExecute gates on that directory existing so it's effectively opt-in.
	/// </summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.DEBUGDecompile), MenuCategory = "Debug", MenuOrder = 30)]
	[Shared]
	sealed class DecompileAllCommand : SimpleCommand
	{
		const string OutputDir = @"c:\temp\decompiled";

		readonly AssemblyTreeModel assemblyTreeModel;
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public DecompileAllCommand(AssemblyTreeModel assemblyTreeModel, DockWorkspace dockWorkspace)
		{
			this.assemblyTreeModel = assemblyTreeModel;
			this.dockWorkspace = dockWorkspace;
		}

		public override bool CanExecute(object? parameter) => Directory.Exists(OutputDir);

		public override void Execute(object? parameter) => ExecuteAsync().HandleExceptions();

		async Task ExecuteAsync()
		{
			var settings = AppEnv.AppComposition.TryGetExport<SettingsService>()?.CreateEffectiveDecompilerSettings()
				?? new ICSharpCode.Decompiler.DecompilerSettings();
			// Run in a dedicated frozen tab so navigation cannot cancel this long run.
			await dockWorkspace.RunInNewTabAsync("Decompiling all assemblies…", token => Task.Run(() => {
				var output = new AvaloniaEditTextOutput { Title = "Decompile All" };
				var bag = new ConcurrentBag<string>();
				Parallel.ForEach(
					Partitioner.Create(assemblyTreeModel.AssemblyList!.GetAssemblies(), loadBalance: true),
					new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount, CancellationToken = token },
					asm => {
						if (asm.HasLoadError)
							return;
						var watch = Stopwatch.StartNew();
						Exception? ex = null;
						var path = Path.Combine(OutputDir, WholeProjectDecompiler.CleanUpFileName(asm.ShortName, ".cs"));
						try
						{
							using var writer = new StreamWriter(path);
							var options = new DecompilationOptions(settings) {
								CancellationToken = token,
								FullDecompilation = true,
							};
							new CSharpLanguage().DecompileAssembly(asm, new PlainTextOutput(writer), options);
						}
						catch (Exception caught)
						{
							ex = caught;
						}
						watch.Stop();
						bag.Add(asm.ShortName + " - " + watch.Elapsed + (ex != null ? " - " + ex.GetType().Name : ""));
					});
				foreach (var line in bag.OrderBy(s => s, StringComparer.OrdinalIgnoreCase))
				{
					output.Write(line);
					output.WriteLine();
				}
				return output;
			}, token));
		}
	}

	/// <summary>
	/// DEBUG-only File-menu stress test: disassemble every loaded assembly to
	/// <c>c:\temp\disassembled\&lt;Name&gt;.il</c> in parallel. Same shape as
	/// <see cref="DecompileAllCommand"/> but routes through <see cref="ILLanguage"/>.
	/// CanExecute gates on the output directory existing.
	/// </summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.DEBUGDisassemble), MenuCategory = "Debug", MenuOrder = 31)]
	[Shared]
	sealed class DisassembleAllCommand : SimpleCommand
	{
		const string OutputDir = @"c:\temp\disassembled";

		readonly AssemblyTreeModel assemblyTreeModel;
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public DisassembleAllCommand(AssemblyTreeModel assemblyTreeModel, DockWorkspace dockWorkspace)
		{
			this.assemblyTreeModel = assemblyTreeModel;
			this.dockWorkspace = dockWorkspace;
		}

		public override bool CanExecute(object? parameter) => Directory.Exists(OutputDir);

		public override void Execute(object? parameter) => ExecuteAsync().HandleExceptions();

		async Task ExecuteAsync()
		{
			var settings = AppEnv.AppComposition.TryGetExport<SettingsService>()?.CreateEffectiveDecompilerSettings()
				?? new ICSharpCode.Decompiler.DecompilerSettings();
			// Run in a dedicated frozen tab so navigation cannot cancel this long run.
			await dockWorkspace.RunInNewTabAsync("Disassembling all assemblies…", token => Task.Run(() => {
				var output = new AvaloniaEditTextOutput { Title = "Disassemble All" };
				var bag = new ConcurrentBag<string>();
				Parallel.ForEach(
					Partitioner.Create(assemblyTreeModel.AssemblyList!.GetAssemblies(), loadBalance: true),
					new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount, CancellationToken = token },
					asm => {
						if (asm.HasLoadError)
							return;
						var watch = Stopwatch.StartNew();
						Exception? ex = null;
						var safeName = WholeProjectDecompiler.CleanUpFileName(asm.Text?.ToString() ?? asm.ShortName, ".il");
						var path = Path.Combine(OutputDir, safeName);
						try
						{
							using var writer = new StreamWriter(path);
							var options = new DecompilationOptions(settings) {
								CancellationToken = token,
								FullDecompilation = true,
							};
							new ILLanguage().DecompileAssembly(asm, new PlainTextOutput(writer), options);
						}
						catch (Exception caught)
						{
							ex = caught;
						}
						watch.Stop();
						bag.Add(asm.ShortName + " - " + watch.Elapsed + (ex != null ? " - " + ex.GetType().Name : ""));
					});
				foreach (var line in bag.OrderBy(s => s, StringComparer.OrdinalIgnoreCase))
				{
					output.Write(line);
					output.WriteLine();
				}
				return output;
			}, token));
		}
	}

	/// <summary>
	/// DEBUG-only stress test: re-decompile the current selection 100 times in series
	/// and report average wall-clock time. Used for tracking decompilation perf
	/// regressions across changes.
	/// </summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.DEBUGDecompile100x), MenuCategory = "Debug", MenuOrder = 32)]
	[Shared]
	sealed class Decompile100TimesCommand : SimpleCommand
	{
		const int NumRuns = 100;

		readonly AssemblyTreeModel assemblyTreeModel;
		readonly LanguageService languageService;
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public Decompile100TimesCommand(AssemblyTreeModel assemblyTreeModel, LanguageService languageService, DockWorkspace dockWorkspace)
		{
			this.assemblyTreeModel = assemblyTreeModel;
			this.languageService = languageService;
			this.dockWorkspace = dockWorkspace;
		}

		public override void Execute(object? parameter) => ExecuteAsync().HandleExceptions();

		async Task ExecuteAsync()
		{
			var language = languageService.CurrentLanguage;
			var nodes = assemblyTreeModel.SelectedItems.OfType<ILSpyTreeNode>().ToArray();
			if (nodes.Length == 0)
				return;
			var settings = AppEnv.AppComposition.TryGetExport<SettingsService>()?.CreateEffectiveDecompilerSettings()
				?? new ICSharpCode.Decompiler.DecompilerSettings();
			// Run in a dedicated frozen tab so navigation cannot cancel this long run.
			await dockWorkspace.RunInNewTabAsync("Decompiling 100×…", token => Task.Run(() => {
				var watch = Stopwatch.StartNew();
				var options = new DecompilationOptions(settings) { CancellationToken = token };
				for (int i = 0; i < NumRuns; i++)
				{
					foreach (var node in nodes)
						node.Decompile(language, new PlainTextOutput(), options);
				}
				watch.Stop();
				var output = new AvaloniaEditTextOutput { Title = "Decompile 100×" };
				var msPerRun = watch.Elapsed.TotalMilliseconds / NumRuns;
				output.Write($"Average time: {msPerRun:f1}ms");
				output.WriteLine();
				return output;
			}, token));
		}
	}
}
#endif
