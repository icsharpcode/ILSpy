﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.FileLoaders;
using ICSharpCode.ILSpyX.PdbProvider;
using ICSharpCode.ILSpy.Controls.TreeView;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

using Microsoft.Win32;

using TypeDefinitionHandle = System.Reflection.Metadata.TypeDefinitionHandle;
using ICSharpCode.ILSpyX.TreeView;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Tree node representing an assembly.
	/// This class is responsible for loading both namespace and type nodes.
	/// </summary>
	public sealed class AssemblyTreeNode : ILSpyTreeNode
	{
		readonly Dictionary<string, NamespaceTreeNode> namespaces = new Dictionary<string, NamespaceTreeNode>();
		readonly Dictionary<TypeDefinitionHandle, TypeTreeNode> typeDict = new Dictionary<TypeDefinitionHandle, TypeTreeNode>();
		ICompilation typeSystem;

		public AssemblyTreeNode(LoadedAssembly assembly) : this(assembly, null)
		{
		}

		internal AssemblyTreeNode(LoadedAssembly assembly, PackageEntry packageEntry)
		{
			this.LoadedAssembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
			this.LazyLoading = true;
			this.PackageEntry = packageEntry;
			Init();
		}

		public AssemblyList AssemblyList {
			get { return LoadedAssembly.AssemblyList; }
		}

		public LoadedAssembly LoadedAssembly { get; }

		/// <summary>
		/// If this assembly was loaded from a bundle; this property returns the bundle entry that the
		/// assembly was loaded from.
		/// </summary>
		public PackageEntry PackageEntry { get; }

		public override bool IsAutoLoaded {
			get {
				return LoadedAssembly.IsAutoLoaded;
			}
		}

		public override object Text => LoadedAssembly.Text;

		public override object Icon {
			get {
				if (LoadedAssembly.IsLoaded)
				{
					if (LoadedAssembly.HasLoadError)
						return Images.AssemblyWarning;
					var loadResult = LoadedAssembly.GetLoadResultAsync().GetAwaiter().GetResult();
					if (loadResult.Package != null)
					{
						return loadResult.Package.Kind switch {
							LoadedPackage.PackageKind.Zip => Images.NuGet,
							_ => Images.Library,
						};
					}
					if (loadResult.MetadataFile != null)
					{
						return loadResult.MetadataFile.Kind switch {
							MetadataFile.MetadataFileKind.PortableExecutable => Images.Assembly,
							MetadataFile.MetadataFileKind.ProgramDebugDatabase => Images.ProgramDebugDatabase,
							MetadataFile.MetadataFileKind.WebCIL => Images.WebAssemblyFile,
							_ => Images.MetadataFile,
						};
					}
					else
					{
						return Images.Assembly;
					}
				}
				else
				{
					return Images.FindAssembly;
				}
			}
		}

		TextBlock tooltip;

		public override object ToolTip {
			get {
				if (LoadedAssembly.HasLoadError)
					return "Assembly could not be loaded. Click here for details.";

				if (tooltip == null && LoadedAssembly.IsLoaded)
				{
					tooltip = new TextBlock();
					var module = LoadedAssembly.GetMetadataFileOrNull();
					var metadata = module?.Metadata;
					if (metadata?.IsAssembly == true && metadata.TryGetFullAssemblyName(out var assemblyName))
					{
						tooltip.Inlines.Add(new Bold(new Run("Name: ")));
						tooltip.Inlines.Add(new Run(assemblyName));
						tooltip.Inlines.Add(new LineBreak());
					}
					tooltip.Inlines.Add(new Bold(new Run("Location: ")));
					tooltip.Inlines.Add(new Run(LoadedAssembly.FileName));
					if (module != null)
					{
						if (module is PEFile peFile)
						{
							tooltip.Inlines.Add(new LineBreak());
							tooltip.Inlines.Add(new Bold(new Run("Architecture: ")));
							tooltip.Inlines.Add(new Run(Language.GetPlatformDisplayName(peFile)));
						}
						string runtimeName = Language.GetRuntimeDisplayName(module);
						if (runtimeName != null)
						{
							tooltip.Inlines.Add(new LineBreak());
							tooltip.Inlines.Add(new Bold(new Run("Runtime: ")));
							tooltip.Inlines.Add(new Run(runtimeName));
						}
						var debugInfo = LoadedAssembly.GetDebugInfoOrNull();
						tooltip.Inlines.Add(new LineBreak());
						tooltip.Inlines.Add(new Bold(new Run("Debug info: ")));
						tooltip.Inlines.Add(new Run(debugInfo?.Description ?? "none"));
					}
				}

				return tooltip;
			}
		}

		public override bool ShowExpander {
			get { return !LoadedAssembly.HasLoadError; }
		}

		async void Init()
		{
			try
			{
				await this.LoadedAssembly.GetLoadResultAsync();
				RaisePropertyChanged(nameof(Text)); // shortname might have changed
			}
			catch
			{
				RaisePropertyChanged(nameof(ShowExpander)); // cannot expand assemblies with load error
			}
			// change from "Loading" icon to final icon
			RaisePropertyChanged(nameof(Icon));
			RaisePropertyChanged(nameof(ExpandedIcon));
			RaisePropertyChanged(nameof(ToolTip));
		}

		protected override void LoadChildren()
		{
			LoadResult loadResult;
			try
			{
				loadResult = LoadedAssembly.GetLoadResultAsync().GetAwaiter().GetResult();
			}
			catch
			{
				// if we crashed on loading, then we don't have any children
				return;
			}
			try
			{
				if (loadResult.MetadataFile != null)
				{
					switch (loadResult.MetadataFile.Kind)
					{
						case MetadataFile.MetadataFileKind.PortableExecutable:
							LoadChildrenForPEFile(loadResult.MetadataFile);
							break;
						case MetadataFile.MetadataFileKind.WebCIL:
							LoadChildrenForWebCilFile((WebCilFile)loadResult.MetadataFile);
							break;
						default:
							var metadata = loadResult.MetadataFile;
							this.Children.Add(new MetadataTablesTreeNode(metadata));
							this.Children.Add(new StringHeapTreeNode(metadata));
							this.Children.Add(new UserStringHeapTreeNode(metadata));
							this.Children.Add(new GuidHeapTreeNode(metadata));
							this.Children.Add(new BlobHeapTreeNode(metadata));
							break;
					}
				}
				else if (loadResult.Package != null)
				{
					var package = loadResult.Package;
					this.Children.AddRange(PackageFolderTreeNode.LoadChildrenForFolder(package.RootFolder));
				}
			}
			catch (Exception ex)
			{
				App.UnhandledException(ex);
			}
		}

		void LoadChildrenForPEFile(MetadataFile module)
		{
			typeSystem = LoadedAssembly.GetTypeSystemOrNull();
			var assembly = (MetadataModule)typeSystem.MainModule;
			this.Children.Add(new MetadataTreeNode(module, Resources.Metadata));
			Decompiler.DebugInfo.IDebugInfoProvider debugInfo = LoadedAssembly.GetDebugInfoOrNull();
			if (debugInfo is PortableDebugInfoProvider ppdb
				&& ppdb.GetMetadataReader() is System.Reflection.Metadata.MetadataReader reader)
			{
				this.Children.Add(new MetadataTreeNode(ppdb.ToMetadataFile(), $"Debug Metadata ({(ppdb.IsEmbedded ? "Embedded" : "From portable PDB")})"));
			}
			this.Children.Add(new ReferenceFolderTreeNode(module, this));
			if (module.Resources.Any())
				this.Children.Add(new ResourceListTreeNode(module));
			foreach (NamespaceTreeNode ns in namespaces.Values)
			{
				ns.Children.Clear();
			}
			namespaces.Clear();
			bool useNestedStructure = MainWindow.Instance.CurrentDisplaySettings.UseNestedNamespaceNodes;
			foreach (var type in assembly.TopLevelTypeDefinitions.OrderBy(t => t.ReflectionName, NaturalStringComparer.Instance))
			{
				var ns = GetOrCreateNamespaceTreeNode(type.Namespace);
				TypeTreeNode node = new TypeTreeNode(type, this);
				typeDict[(TypeDefinitionHandle)type.MetadataToken] = node;
				ns.Children.Add(node);
			}
			foreach (NamespaceTreeNode ns in namespaces.Values
				.Where(ns => ns.Children.Count > 0 && ns.Parent == null)
				.OrderBy(n => n.Name, NaturalStringComparer.Instance))
			{
				this.Children.Add(ns);
				SetPublicAPI(ns);
			}

			NamespaceTreeNode GetOrCreateNamespaceTreeNode(string @namespace)
			{
				if (!namespaces.TryGetValue(@namespace, out NamespaceTreeNode ns))
				{
					if (useNestedStructure)
					{
						int decimalIndex = @namespace.LastIndexOf('.');
						if (decimalIndex < 0)
						{
							var escapedNamespace = Language.EscapeName(@namespace);
							ns = new NamespaceTreeNode(escapedNamespace);
						}
						else
						{
							var parentNamespaceTreeNode = GetOrCreateNamespaceTreeNode(@namespace.Substring(0, decimalIndex));
							var escapedInnerNamespace = Language.EscapeName(@namespace.Substring(decimalIndex + 1));
							ns = new NamespaceTreeNode(escapedInnerNamespace);
							parentNamespaceTreeNode.Children.Add(ns);
						}
					}
					else
					{
						var escapedNamespace = Language.EscapeName(@namespace);
						ns = new NamespaceTreeNode(escapedNamespace);
					}
					namespaces.Add(@namespace, ns);
				}
				return ns;
			}
		}

		void LoadChildrenForWebCilFile(WebCilFile module)
		{
			typeSystem = LoadedAssembly.GetTypeSystemOrNull();
			var assembly = (MetadataModule)typeSystem.MainModule;
			this.Children.Add(new MetadataTreeNode(module, Resources.Metadata));
			Decompiler.DebugInfo.IDebugInfoProvider debugInfo = LoadedAssembly.GetDebugInfoOrNull();
			if (debugInfo is PortableDebugInfoProvider ppdb
				&& ppdb.GetMetadataReader() is System.Reflection.Metadata.MetadataReader reader)
			{
				this.Children.Add(new MetadataTreeNode(ppdb.ToMetadataFile(), $"Debug Metadata ({(ppdb.IsEmbedded ? "Embedded" : "From portable PDB")})"));
			}
			this.Children.Add(new ReferenceFolderTreeNode(module, this));
			if (module.Resources.Any())
				this.Children.Add(new ResourceListTreeNode(module));
			foreach (NamespaceTreeNode ns in namespaces.Values)
			{
				ns.Children.Clear();
			}
			namespaces.Clear();
			bool useNestedStructure = MainWindow.Instance.CurrentDisplaySettings.UseNestedNamespaceNodes;
			foreach (var type in assembly.TopLevelTypeDefinitions.OrderBy(t => t.ReflectionName, NaturalStringComparer.Instance))
			{
				var ns = GetOrCreateNamespaceTreeNode(type.Namespace);
				TypeTreeNode node = new TypeTreeNode(type, this);
				typeDict[(TypeDefinitionHandle)type.MetadataToken] = node;
				ns.Children.Add(node);
			}
			foreach (NamespaceTreeNode ns in namespaces.Values
				.Where(ns => ns.Children.Count > 0 && ns.Parent == null)
				.OrderBy(n => n.Name, NaturalStringComparer.Instance))
			{
				this.Children.Add(ns);
				SetPublicAPI(ns);
			}

			NamespaceTreeNode GetOrCreateNamespaceTreeNode(string @namespace)
			{
				if (!namespaces.TryGetValue(@namespace, out NamespaceTreeNode ns))
				{
					if (useNestedStructure)
					{
						int decimalIndex = @namespace.LastIndexOf('.');
						if (decimalIndex < 0)
						{
							var escapedNamespace = Language.EscapeName(@namespace);
							ns = new NamespaceTreeNode(escapedNamespace);
						}
						else
						{
							var parentNamespaceTreeNode = GetOrCreateNamespaceTreeNode(@namespace.Substring(0, decimalIndex));
							var escapedInnerNamespace = Language.EscapeName(@namespace.Substring(decimalIndex + 1));
							ns = new NamespaceTreeNode(escapedInnerNamespace);
							parentNamespaceTreeNode.Children.Add(ns);
						}
					}
					else
					{
						var escapedNamespace = Language.EscapeName(@namespace);
						ns = new NamespaceTreeNode(escapedNamespace);
					}
					namespaces.Add(@namespace, ns);
				}
				return ns;
			}
		}

		private static void SetPublicAPI(NamespaceTreeNode ns)
		{
			foreach (NamespaceTreeNode innerNamespace in ns.Children.OfType<NamespaceTreeNode>())
			{
				SetPublicAPI(innerNamespace);
			}
			ns.SetPublicAPI(ns.Children.OfType<ILSpyTreeNode>().Any(n => n.IsPublicAPI));
		}

		/// <summary>
		/// Finds the node for a top-level type.
		/// </summary>
		public TypeTreeNode FindTypeNode(ITypeDefinition type)
		{
			if (type == null)
				return null;
			EnsureLazyChildren();
			TypeTreeNode node;
			if (typeDict.TryGetValue((TypeDefinitionHandle)type.MetadataToken, out node))
				return node;
			else
				return null;
		}

		/// <summary>
		/// Finds the node for a namespace.
		/// </summary>
		public NamespaceTreeNode FindNamespaceNode(string namespaceName)
		{
			if (string.IsNullOrEmpty(namespaceName))
				return null;
			EnsureLazyChildren();
			NamespaceTreeNode node;
			if (namespaces.TryGetValue(namespaceName, out node))
				return node;
			else
				return null;
		}

		public override bool CanDrag(SharpTreeNode[] nodes)
		{
			// prohibit dragging assemblies nested in nuget packages
			return nodes.All(n => n is AssemblyTreeNode { PackageEntry: null });
		}

		public override void StartDrag(object dragSource, SharpTreeNode[] nodes, IPlatformDragDrop dragdropManager)
		{
			dragdropManager.DoDragDrop(dragSource, Copy(nodes), XPlatDragDropEffects.All);
		}

		public override bool CanDelete()
		{
			// prohibit deleting assemblies nested in nuget packages
			return PackageEntry == null;
		}

		public override void Delete()
		{
			DeleteCore();
		}

		public override void DeleteCore()
		{
			LoadedAssembly.AssemblyList.Unload(LoadedAssembly);
		}

		internal const string DataFormat = "ILSpyAssemblies";

		public override IPlatformDataObject Copy(SharpTreeNode[] nodes)
		{
			var dataObject = new WpfWindowsDataObject(new DataObject());
			dataObject.SetData(DataFormat, nodes.OfType<AssemblyTreeNode>().Select(n => n.LoadedAssembly.FileName).ToArray());
			return dataObject;
		}

		public override FilterResult Filter(FilterSettings settings)
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
				var loadResult = LoadedAssembly.GetLoadResultAsync().GetAwaiter().GetResult();
				if (loadResult.MetadataFile != null)
				{
					language.DecompileAssembly(LoadedAssembly, output, options);
				}
				else if (loadResult.Package != null)
				{
					output.WriteLine("// " + LoadedAssembly.FileName);
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

		private void DecompilePackage(LoadedPackage package, ITextOutput output)
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

		public override bool Save(TabPageModel tabPage)
		{
			if (!LoadedAssembly.IsLoadedAsValidAssembly)
				return false;
			Language language = this.Language;
			if (string.IsNullOrEmpty(language.ProjectFileExtension))
				return false;
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.FileName = WholeProjectDecompiler.CleanUpFileName(LoadedAssembly.ShortName) + language.ProjectFileExtension;
			dlg.Filter = language.Name + " project|*" + language.ProjectFileExtension + "|" + language.Name + " single file|*" + language.FileExtension + "|All files|*.*";
			if (dlg.ShowDialog() == true)
			{
				DecompilationOptions options = MainWindow.Instance.CreateDecompilationOptions();
				options.FullDecompilation = true;
				if (dlg.FilterIndex == 1)
				{
					options.SaveAsProjectDirectory = Path.GetDirectoryName(dlg.FileName);
					foreach (string entry in Directory.GetFileSystemEntries(options.SaveAsProjectDirectory))
					{
						if (!string.Equals(entry, dlg.FileName, StringComparison.OrdinalIgnoreCase))
						{
							var result = MessageBox.Show(
								Resources.AssemblySaveCodeDirectoryNotEmpty,
								Resources.AssemblySaveCodeDirectoryNotEmptyTitle,
								MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
							if (result == MessageBoxResult.No)
								return true; // don't save, but mark the Save operation as handled
							break;
						}
					}
				}
				tabPage.ShowTextView(textView => textView.SaveToDisk(language, new[] { this }, options, dlg.FileName));
			}
			return true;
		}

		public override string ToString()
		{
			// ToString is used by FindNodeByPath/GetPathForNode
			// Fixes #821 - Reload All Assemblies Should Point to the Correct Assembly
			return LoadedAssembly.FileName;
		}
	}

	[ExportContextMenuEntry(Header = nameof(Resources._Remove), Icon = "images/Delete")]
	sealed class RemoveAssembly : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return false;
			return context.SelectedTreeNodes.All(n => n is AssemblyTreeNode);
		}

		public bool IsEnabled(TextViewContext context)
		{
			return true;
		}

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return;
			foreach (var node in context.SelectedTreeNodes)
			{
				node.Delete();
			}
		}
	}

	[ExportContextMenuEntry(Header = nameof(Resources._Reload), Icon = "images/Refresh")]
	sealed class ReloadAssembly : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return false;
			return context.SelectedTreeNodes.All(n => n is AssemblyTreeNode);
		}

		public bool IsEnabled(TextViewContext context)
		{
			return true;
		}

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return;
			var paths = new List<string[]>();
			using (context.TreeView.LockUpdates())
			{
				foreach (var node in context.SelectedTreeNodes)
				{
					paths.Add(MainWindow.GetPathForNode(node));
					var la = ((AssemblyTreeNode)node).LoadedAssembly;
					la.AssemblyList.ReloadAssembly(la.FileName);
				}
			}
			MainWindow.Instance.SelectNodes(paths.Select(p => MainWindow.Instance.FindNodeByPath(p, true)).ToArray());
			MainWindow.Instance.RefreshDecompiledView();
		}
	}

	[ExportContextMenuEntry(Header = nameof(Resources._LoadDependencies), Category = nameof(Resources.Dependencies))]
	sealed class LoadDependencies : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return false;
			return context.SelectedTreeNodes.All(n => n is AssemblyTreeNode asm && asm.LoadedAssembly.IsLoadedAsValidAssembly);
		}

		public bool IsEnabled(TextViewContext context)
		{
			return true;
		}

		public async void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return;
			var tasks = new List<Task>();
			foreach (var node in context.SelectedTreeNodes)
			{
				var la = ((AssemblyTreeNode)node).LoadedAssembly;
				var resolver = la.GetAssemblyResolver();
				var module = la.GetMetadataFileOrNull();
				if (module != null)
				{
					var metadata = module.Metadata;
					foreach (var assyRef in metadata.AssemblyReferences)
					{
						tasks.Add(resolver.ResolveAsync(new AssemblyReference(module, assyRef)));
					}
				}
			}
			await Task.WhenAll(tasks);
			MainWindow.Instance.RefreshDecompiledView();
		}
	}

	[ExportContextMenuEntry(Header = nameof(Resources._AddMainList), Category = nameof(Resources.Dependencies))]
	sealed class AddToMainList : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return false;
			return context.SelectedTreeNodes.Where(n => n is AssemblyTreeNode).Any(n => ((AssemblyTreeNode)n).IsAutoLoaded);
		}

		public bool IsEnabled(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return false;
			return context.SelectedTreeNodes.Any(n => n is AssemblyTreeNode);
		}

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return;
			foreach (var node in context.SelectedTreeNodes)
			{
				var loadedAssm = ((AssemblyTreeNode)node).LoadedAssembly;
				if (!loadedAssm.HasLoadError)
				{
					loadedAssm.IsAutoLoaded = false;
					node.RaisePropertyChanged(nameof(ILSpyTreeNode.IsAutoLoaded));
				}
			}
			MainWindow.Instance.CurrentAssemblyList.RefreshSave();
		}
	}

	[ExportContextMenuEntry(Header = nameof(Resources._OpenContainingFolder), Category = nameof(Resources.Shell))]
	sealed class OpenContainingFolder : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return false;
			return context.SelectedTreeNodes
				.All(n => {
					var a = GetAssemblyTreeNode(n);
					return a != null && File.Exists(a.LoadedAssembly.FileName);
				});
		}

		internal static AssemblyTreeNode GetAssemblyTreeNode(SharpTreeNode node)
		{
			while (node != null)
			{
				if (node is AssemblyTreeNode a)
					return a;
				node = node.Parent;
			}
			return null;
		}

		public bool IsEnabled(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return false;
			return context.SelectedTreeNodes
				.All(n => {
					var a = GetAssemblyTreeNode(n);
					return a != null && File.Exists(a.LoadedAssembly.FileName);
				});
		}

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return;
			foreach (var n in context.SelectedTreeNodes)
			{
				var node = GetAssemblyTreeNode(n);
				var path = node.LoadedAssembly.FileName;
				if (File.Exists(path))
				{
					MainWindow.ExecuteCommand("explorer.exe", $"/select,\"{path}\"");
				}
			}
		}
	}

	[ExportContextMenuEntry(Header = nameof(Resources._OpenCommandLineHere), Category = nameof(Resources.Shell))]
	sealed class OpenCmdHere : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return false;
			return context.SelectedTreeNodes
				.All(n => {
					var a = OpenContainingFolder.GetAssemblyTreeNode(n);
					return a != null && File.Exists(a.LoadedAssembly.FileName);
				});
		}

		public bool IsEnabled(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return false;
			return context.SelectedTreeNodes
				.All(n => {
					var a = OpenContainingFolder.GetAssemblyTreeNode(n);
					return a != null && File.Exists(a.LoadedAssembly.FileName);
				});
		}

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return;
			foreach (var n in context.SelectedTreeNodes)
			{
				var node = OpenContainingFolder.GetAssemblyTreeNode(n);
				var path = Path.GetDirectoryName(node.LoadedAssembly.FileName);
				if (Directory.Exists(path))
				{
					MainWindow.ExecuteCommand("cmd.exe", $"/k \"cd {path}\"");
				}
			}
		}
	}

}
