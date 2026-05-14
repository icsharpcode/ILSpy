
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

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.FileLoaders;
using ICSharpCode.ILSpyX.PdbProvider;

using ILSpy;
using ILSpy.Languages;

namespace ILSpy.TreeNodes
{
	sealed class AssemblyTreeNode : ILSpyTreeNode
	{
		readonly LoadedAssembly assembly;
		string? loadError;
		MetadataFile? cachedModule;

		public LoadedAssembly LoadedAssembly => assembly;

		/// <summary>
		/// When this node represents a .dll/.exe entry inside a <see cref="LoadedPackage"/>
		/// (zip or .NET bundle), this is the package entry it was created from. Null for
		/// stand-alone assemblies.
		/// </summary>
		public PackageEntry? PackageEntry { get; }

		/// <summary>
		/// If this assembly is itself a bundle/package (nuget .zip or .NET single-file
		/// bundle), returns the package kind. Returns <see langword="null"/> for plain
		/// stand-alone assemblies and for any assembly that hasn't finished loading yet
		/// or failed to load.
		/// </summary>
		public LoadedPackage.PackageKind? PackageKind {
			get {
				if (LoadedAssembly.HasLoadError || !LoadedAssembly.IsLoaded)
					return null;
				var loadResult = LoadedAssembly.GetLoadResultAsync().GetAwaiter().GetResult();
				return loadResult.Package?.Kind;
			}
		}

		public AssemblyTreeNode(LoadedAssembly assembly) : this(assembly, null)
		{
		}

		internal AssemblyTreeNode(LoadedAssembly assembly, PackageEntry? packageEntry)
		{
			ArgumentNullException.ThrowIfNull(assembly);
			this.assembly = assembly;
			this.PackageEntry = packageEntry;
			LazyLoading = true;
			// Observe the load OUTCOME without triggering it — the cooldown sweep and
			// explicit user actions (path-restore, expand) are what start the load.
			// If we called GetLoadResultAsync here we'd flatten the lazy strategy and
			// every assembly in the list would load the moment its tree node is built.
			assembly.Loaded += OnAssemblyLoaded;
			if (assembly.IsLoaded)
				OnAssemblyLoaded(); // already finished before we subscribed — catch up
		}

		void OnAssemblyLoaded()
		{
			// The Loaded event fires from a thread-pool ContinueWith. Marshal to the UI
			// thread before mutating tree-node state and raising change notifications;
			// SharpTreeNode's RaisePropertyChanged is not thread-safe.
			global::Avalonia.Threading.Dispatcher.UIThread.Post(InitFromLoadResult,
				global::Avalonia.Threading.DispatcherPriority.Background);
		}

		void InitFromLoadResult()
		{
			try
			{
				// GetLoadResultAsync is a no-op now — the load is already complete.
				var loadResult = assembly.GetLoadResultAsync().GetAwaiter().GetResult();
				cachedModule = loadResult.MetadataFile;
				if (cachedModule == null && loadResult.Package == null)
				{
					loadError = File.Exists(assembly.FileName)
						? $"Failed to load '{assembly.FileName}'."
						: $"File not found:\n{assembly.FileName}";
				}
			}
			catch (Exception ex)
			{
				loadError = $"Failed to load '{assembly.FileName}':\n{ex.GetBaseException().Message}";
			}
			RaisePropertyChanged(nameof(Text));
			RaisePropertyChanged(nameof(Icon));
			RaisePropertyChanged(nameof(ToolTip));
			RaisePropertyChanged(nameof(ShowExpander));
		}

		// Assemblies nested in NuGet packages can't be unloaded individually — the parent
		// package entry owns them.
		public override bool CanDelete() => PackageEntry == null;

		public override void Delete() => DeleteCore();

		public override void DeleteCore() => assembly.AssemblyList.Unload(assembly);

		public override bool Save()
		{
			// Intercept the File → Save Code flow for valid managed assemblies whose active
			// language supports project export (e.g. C#). Offers both the .csproj and the
			// single-file filter; the picked extension drives the export mode. Languages
			// without a ProjectFileExtension (the IL disassembler) fall through to the base
			// single-file path.
			if (!assembly.IsLoadedAsValidAssembly)
				return false;
			var languageService = TryGetLanguageService();
			if (languageService == null)
				return false;
			var language = languageService.CurrentLanguage;
			if (string.IsNullOrEmpty(language.ProjectFileExtension))
				return false;
			_ = SaveAsProjectOrSingleFileAsync(language);
			return true;
		}

		async Task SaveAsProjectOrSingleFileAsync(Language language)
		{
			var shortName = CleanSuggestedFileName(assembly.ShortName);
			var filter = $"{language.Name} project (*{language.ProjectFileExtension})|*{language.ProjectFileExtension}"
				+ $"|{language.Name} (*{language.FileExtension})|*{language.FileExtension}"
				+ "|All files (*.*)|*.*";
			var defaultName = shortName + language.ProjectFileExtension;
			var path = await Commands.FilePickers.SaveAsync(filter, defaultName, "Save Code").ConfigureAwait(false);
			if (string.IsNullOrEmpty(path))
				return;

			var ext = Path.GetExtension(path);
			var isProject = string.Equals(ext, language.ProjectFileExtension, StringComparison.OrdinalIgnoreCase);

			await Task.Run(() => {
				var options = new DecompilationOptions {
					FullDecompilation = true,
					EscapeInvalidIdentifiers = true,
				};
				if (isProject)
					options.SaveAsProjectDirectory = Path.GetDirectoryName(path);
				using var writer = new StreamWriter(path);
				var output = new PlainTextOutput(writer);
				try
				{
					language.DecompileAssembly(assembly, output, options);
				}
				catch (Exception ex)
				{
					output.WriteLine();
					output.WriteLine("/* Save failed:");
					output.WriteLine(ex.ToString());
					output.WriteLine("*/");
				}
			}).ConfigureAwait(false);
		}

		static string CleanSuggestedFileName(string name)
		{
			var invalid = Path.GetInvalidFileNameChars();
			var clean = name;
			foreach (var c in invalid)
				clean = clean.Replace(c, '_');
			return clean;
		}

		static LanguageService? TryGetLanguageService()
		{
			try
			{ return AppEnv.AppComposition.Current.GetExport<LanguageService>(); }
			catch { return null; }
		}

		public override object Text => assembly.Text;

		// ToString is the stable identity used by SessionSettings.ActiveTreeViewPath — must not
		// depend on the active language. The full file path uniquely identifies the assembly.
		public override string ToString() => assembly.FileName;

		public override object Icon {
			get {
				if (assembly.HasLoadError || loadError != null)
					return Images.Images.AssemblyWarning;
				if (!assembly.IsLoaded)
					return Images.Images.AssemblyLoading;

				var loadResult = assembly.GetLoadResultAsync().GetAwaiter().GetResult();
				if (loadResult.Package != null)
				{
					return loadResult.Package.Kind switch {
						LoadedPackage.PackageKind.Zip => Images.Images.NuGet,
						_ => Images.Images.Library,
					};
				}
				if (loadResult.MetadataFile != null)
				{
					return loadResult.MetadataFile.Kind switch {
						MetadataFile.MetadataFileKind.PortableExecutable => Images.Images.Assembly,
						MetadataFile.MetadataFileKind.ProgramDebugDatabase => Images.Images.ProgramDebugDatabase,
						MetadataFile.MetadataFileKind.WebCIL => Images.Images.WebAssemblyFile,
						_ => Images.Images.MetadataFile,
					};
				}
				return Images.Images.Assembly;
			}
		}

		public override object? ToolTip {
			get {
				if (assembly.HasLoadError || loadError != null)
					return loadError ?? "Assembly could not be loaded.";
				if (!assembly.IsLoaded || cachedModule == null)
					return assembly.FileName;

				var tb = new TextBlock { TextWrapping = TextWrapping.Wrap };

				var metadata = cachedModule.Metadata;
				if (metadata?.IsAssembly == true && metadata.TryGetFullAssemblyName(out var assemblyName))
				{
					tb.Inlines!.Add(BoldRun("Name: "));
					tb.Inlines.Add(new Run(assemblyName));
					tb.Inlines.Add(new LineBreak());
				}

				tb.Inlines!.Add(BoldRun("Location: "));
				tb.Inlines.Add(new Run(assembly.FileName));

				if (cachedModule is PEFile peFile)
				{
					tb.Inlines.Add(new LineBreak());
					tb.Inlines.Add(BoldRun("Architecture: "));
					tb.Inlines.Add(new Run(GetPlatformDisplayName(peFile)));
				}

				string? runtime = GetRuntimeDisplayName(cachedModule);
				if (runtime != null)
				{
					tb.Inlines.Add(new LineBreak());
					tb.Inlines.Add(BoldRun("Runtime: "));
					tb.Inlines.Add(new Run(runtime));
				}

				return tb;
			}
		}

		public override bool ShowExpander => !assembly.HasLoadError && loadError == null && base.ShowExpander;

		public override bool IsAutoLoaded => assembly.IsAutoLoaded;

		/// <summary>
		/// Finds the <see cref="NamespaceTreeNode"/> for the given namespace string, or
		/// <c>null</c> if no children are loaded yet or the namespace has no top-level types
		/// in this assembly.
		/// </summary>
		public NamespaceTreeNode? FindNamespaceNode(string namespaceName)
		{
			ArgumentNullException.ThrowIfNull(namespaceName);
			EnsureLazyChildren();
			return Children.OfType<NamespaceTreeNode>().FirstOrDefault(ns => ns.Name == namespaceName);
		}

		/// <summary>
		/// Finds the <see cref="TypeTreeNode"/> for the given top-level type definition.
		/// Walks the assembly's namespaces (loading them as needed) and matches by
		/// <see cref="TypeTreeNode.Handle"/>.
		/// </summary>
		public TypeTreeNode? FindTypeNode(ITypeDefinition type)
		{
			ArgumentNullException.ThrowIfNull(type);
			var ns = FindNamespaceNode(type.Namespace);
			if (ns == null)
				return null;
			ns.EnsureLazyChildren();
			var handle = (System.Reflection.Metadata.TypeDefinitionHandle)type.MetadataToken;
			return ns.Children.OfType<TypeTreeNode>().FirstOrDefault(n => n.Handle == handle);
		}

		protected override void LoadChildren()
		{
			LoadResult loadResult;
			try
			{
				loadResult = assembly.GetLoadResultAsync().GetAwaiter().GetResult();
			}
			catch
			{
				return;
			}

			// Zip / .NET-bundle packages: surface their folder structure instead of trying to
			// decompile the package itself.
			if (loadResult.Package != null)
			{
				foreach (var child in PackageFolderTreeNode.LoadChildrenForFolder(loadResult.Package.RootFolder))
					Children.Add(child);
				return;
			}

			var module = loadResult.MetadataFile ?? cachedModule;
			if (module == null)
				return;

			if (module.Kind != MetadataFile.MetadataFileKind.PortableExecutable
				&& module.Kind != MetadataFile.MetadataFileKind.WebCIL)
			{
				return;
			}

			Children.Add(new ILSpy.Metadata.MetadataTreeNode(module, ICSharpCode.ILSpy.Properties.Resources.Metadata));

			// Surface portable PDB metadata (embedded or side-by-side) as a top-level sibling
			// of the host Metadata folder so PDB browsing is one click away instead of buried
			// under Metadata → Debug Directory. Reuses the cached PdbProvider that the rest of
			// the app already opened for decompilation — no second parse of the PDB blob.
			var debugInfo = assembly.GetDebugInfoOrNull();
			if (debugInfo is PortableDebugInfoProvider ppdb
				&& ppdb.GetMetadataReader() is not null)
			{
				var label = $"Debug Metadata ({(ppdb.IsEmbedded ? "Embedded" : "From portable PDB")})";
				Children.Add(new ILSpy.Metadata.MetadataTreeNode(ppdb.ToMetadataFile(), label));
			}

			Children.Add(new ReferenceFolderTreeNode(module, this));

			if (module.Resources.Any())
				Children.Add(new ResourceListTreeNode(module));

			var metadata = module.Metadata;
			// Every top-level namespace string in the module — INCLUDING the empty string for
			// types declared at module scope. The empty namespace becomes a NamespaceTreeNode
			// whose Text renders as "-"; without that path, global-namespace types (every PE's
			// <Module> pseudo-type plus any user-declared ones) would have no parent node to
			// live under, and the long-standing tree shape would break.
			var namespaces = metadata.TypeDefinitions
				.Where(t => metadata.GetTypeDefinition(t).GetDeclaringType().IsNil)
				.Select(t => metadata.GetString(metadata.GetTypeDefinition(t).Namespace))
				.Distinct()
				.OrderBy(ns => ns, NaturalStringComparer.Instance);

			foreach (var ns in namespaces)
				Children.Add(new NamespaceTreeNode(ns, module));
		}

		public override FilterResult Filter(LanguageSettings settings)
		{
			if (settings.SearchTermMatches(LoadedAssembly.ShortName))
				return FilterResult.Match;
			else
				return FilterResult.Recurse;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			void HandleException(Exception ex, string message)
			{
				language.WriteCommentLine(output, message);
				output.WriteLine();
				output.MarkFoldStart("Exception details", true);
				output.Write(ex.ToString());
				output.MarkFoldEnd();
			}

			try
			{
				var loadResult = assembly.GetLoadResultAsync().GetAwaiter().GetResult();
				if (loadResult.MetadataFile != null)
				{
					switch (loadResult.MetadataFile.Kind)
					{
						case MetadataFile.MetadataFileKind.ProgramDebugDatabase:
						case MetadataFile.MetadataFileKind.Metadata:
							output.WriteLine("// " + assembly.FileName);
							break;
						default:
							language.DecompileAssembly(assembly, output, options);
							break;
					}
				}
				else if (loadResult.Package != null)
				{
					output.WriteLine("// " + assembly.FileName);
					DecompilePackage(loadResult.Package, output);
				}
				else if (loadResult.FileLoadException != null)
				{
					HandleException(loadResult.FileLoadException, loadResult.FileLoadException.Message);
				}
			}
			catch (BadImageFormatException badImage)
			{
				HandleException(badImage, "This file does not contain a managed assembly.");
			}
			catch (FileNotFoundException fileNotFound) when (options.SaveAsProjectDirectory == null)
			{
				HandleException(fileNotFound, "The file was not found.");
			}
			catch (DirectoryNotFoundException dirNotFound) when (options.SaveAsProjectDirectory == null)
			{
				HandleException(dirNotFound, "The directory was not found.");
			}
			catch (MetadataFileNotSupportedException notSupported)
			{
				HandleException(notSupported, notSupported.Message);
			}
		}

		static void DecompilePackage(LoadedPackage package, ITextOutput output)
		{
			switch (package.Kind)
			{
				case LoadedPackage.PackageKind.Zip:
					output.WriteLine("// File format: .zip file");
					break;
				case LoadedPackage.PackageKind.Bundle:
					var header = package.BundleHeader;
					output.WriteLine($"// File format: .NET bundle {header.MajorVersion}.{header.MinorVersion}");
					break;
			}
			output.WriteLine();
			output.WriteLine("Entries:");
			foreach (var entry in package.Entries)
			{
				output.WriteLine($" {entry.Name} ({entry.TryGetLength()} bytes)");
			}
		}

		static Run BoldRun(string text) => new(text) { FontWeight = FontWeight.Bold };

		static string GetPlatformDisplayName(PEFile file)
		{
			return file.Reader.PEHeaders.CoffHeader.Machine switch {
				System.Reflection.PortableExecutable.Machine.I386 => "x86",
				System.Reflection.PortableExecutable.Machine.Amd64 => "x64",
				System.Reflection.PortableExecutable.Machine.IA64 => "Itanium",
				System.Reflection.PortableExecutable.Machine.Arm => "ARM",
				System.Reflection.PortableExecutable.Machine.Arm64 => "ARM64",
				_ => file.Reader.PEHeaders.CoffHeader.Machine.ToString(),
			};
		}

		static string? GetRuntimeDisplayName(MetadataFile module)
		{
			return module.Metadata.MetadataKind switch {
				System.Reflection.Metadata.MetadataKind.Ecma335 => module.Metadata.MetadataVersion,
				System.Reflection.Metadata.MetadataKind.WindowsMetadata => "WinRT",
				System.Reflection.Metadata.MetadataKind.ManagedWindowsMetadata => "Managed WinRT",
				_ => null,
			};
		}

		// Right-click → "Remove" — unloads every selected assembly from the active list. Only
		// visible when the entire selection is assembly nodes, so it doesn't pollute right-click
		// menus opened on member / namespace / resource rows. MEF discovers this via the
		// [ExportContextMenuEntry] attribute and surfaces it through ContextMenuEntryRegistry.
		[ExportContextMenuEntry(Header = nameof(ICSharpCode.ILSpy.Properties.Resources._Remove))]
		[System.Composition.Shared]
		sealed class RemoveAssembly : IContextMenuEntry
		{
			public bool IsVisible(TextViewContext context)
			{
				var nodes = context.SelectedTreeNodes;
				return nodes is { Length: > 0 } && nodes.All(n => n is AssemblyTreeNode);
			}

			public bool IsEnabled(TextViewContext context) => true;

			public void Execute(TextViewContext context)
			{
				if (context.SelectedTreeNodes == null)
					return;
				// Snapshot before mutation — Unload reshapes the tree and the live selection.
				foreach (var node in context.SelectedTreeNodes.OfType<AssemblyTreeNode>().ToArray())
					node.Delete();
			}
		}

		// Right-click → "Reload" — re-reads each selected assembly from disk so the user can
		// pick up edits made by an external build without restarting ILSpy. Same visibility
		// shape as Remove (assembly nodes only). The model's existing path-restoration plumbing
		// handles re-selecting equivalent nodes after the reload churns the tree.
		[ExportContextMenuEntry(Header = nameof(ICSharpCode.ILSpy.Properties.Resources._Reload))]
		[System.Composition.Shared]
		[method: System.Composition.ImportingConstructor]
		sealed class ReloadAssembly(AssemblyTree.AssemblyTreeModel assemblyTreeModel) : IContextMenuEntry
		{
			public bool IsVisible(TextViewContext context)
			{
				var nodes = context.SelectedTreeNodes;
				return nodes is { Length: > 0 } && nodes.All(n => n is AssemblyTreeNode);
			}

			public bool IsEnabled(TextViewContext context) => true;

			public void Execute(TextViewContext context)
			{
				if (context.SelectedTreeNodes == null)
					return;
				// Snapshot before mutation — ReloadAssembly mutates the AssemblyList and the
				// live tree selection.
				var nodes = context.SelectedTreeNodes.OfType<AssemblyTreeNode>().ToArray();
				var paths = nodes.Select(n => AssemblyTree.AssemblyTreeModel.GetPathForNode(n)).ToArray();
				foreach (var node in nodes)
				{
					var loaded = node.LoadedAssembly;
					loaded.AssemblyList.ReloadAssembly(loaded.FileName);
				}
				// Re-select using the saved paths so the user keeps their position after the
				// LoadedAssembly instances are swapped out.
				if (paths.Length > 0)
				{
					var restored = paths
						.Select(p => assemblyTreeModel.FindNodeByPath(p, returnBestMatch: true))
						.OfType<ICSharpCode.ILSpyX.TreeView.SharpTreeNode>()
						.LastOrDefault();
					if (restored != null)
						assemblyTreeModel.SelectNode(restored);
				}
			}
		}
	}
}
