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

using Avalonia.Threading;

using AvaloniaEdit.Highlighting;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Single entry in the references folder. Minimal port: shows the reference name
	/// using ILAmbience escaping plus a generic assembly icon. Children (the
	/// per-reference type list and transitive references) are not yet implemented.
	/// </summary>
	public sealed class AssemblyReferenceTreeNode : ILSpyTreeNode
	{
		enum LoadState { Unloaded, Loading, Loaded, Failed }

		readonly AssemblyReference reference;
		readonly AssemblyTreeNode parentAssembly;
		LoadState state;

		public AssemblyReferenceTreeNode(AssemblyReference reference, AssemblyTreeNode parentAssembly)
		{
			this.reference = reference ?? throw new ArgumentNullException(nameof(reference));
			this.parentAssembly = parentAssembly ?? throw new ArgumentNullException(nameof(parentAssembly));
			LazyLoading = true;
		}

		public AssemblyReference AssemblyReference => reference;

		public override object Text => ILAmbience.EscapeName(reference.Name);

		public override object? NavigationText => $"{Text} ({ICSharpCode.ILSpy.Properties.Resources.References})";

		public override object Icon {
			get {
				if (state == LoadState.Unloaded)
				{
					state = LoadState.Loading;
					Dispatcher.UIThread.Post(() => {
						var resolver = parentAssembly.LoadedAssembly.GetAssemblyResolver();
						var resolved = resolver.Resolve(reference);
						state = resolved == null ? LoadState.Failed : LoadState.Loaded;
						RaisePropertyChanged(nameof(Icon));
					}, DispatcherPriority.Background);
				}
				return state switch {
					LoadState.Loaded => Images.Assembly,
					LoadState.Failed => Images.AssemblyWarning,
					_ => Images.AssemblyLoading,
				};
			}
		}

		protected override void LoadChildren()
		{
			// Always emit the "Referenced Types (N)" subnode keyed on the *parent* module's
			// metadata — this lists what the parent uses, regardless of whether the referenced
			// assembly itself resolves. Then, if the reference resolves, surface its own
			// AssemblyRefs as transitive children.
			var parentModule = parentAssembly.LoadedAssembly.GetMetadataFileOrNull();
			if (parentModule != null)
			{
				var parentTypeSystem = (MetadataModule?)parentModule.GetTypeSystemWithCurrentOptionsOrNull()?.MainModule;
				if (parentTypeSystem != null)
					Children.Add(new AssemblyReferenceReferencedTypesTreeNode(parentTypeSystem, reference));
			}

			var resolver = parentAssembly.LoadedAssembly.GetAssemblyResolver();
			var resolved = resolver.Resolve(reference);
			if (resolved == null)
				return;
			foreach (var childRef in resolved.AssemblyReferences)
				Children.Add(new AssemblyReferenceTreeNode(childRef, parentAssembly));
		}

		public override void ActivateItem(IPlatformRoutedEventArgs e)
		{
			if (parentAssembly.Parent is not AssemblyListTreeNode listNode)
				return;
			var resolver = parentAssembly.LoadedAssembly.GetAssemblyResolver();
			var resolved = resolver.Resolve(reference);
			if (resolved == null)
				return;
			var loaded = listNode.FindAssemblyNode(resolved);
			if (loaded == null)
				return;
			AppComposition.Current.GetExport<AssemblyTreeModel>().SelectedItem = loaded;
			e.Handled = true;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			var loaded = parentAssembly.LoadedAssembly.LoadedAssemblyReferencesInfo.TryGetInfo(reference.FullName, out var info);
			if (reference.IsWindowsRuntime)
				output.WriteLine(reference.FullName + " [WinRT]" + (!loaded ? " (unresolved)" : ""));
			else
				output.WriteLine(reference.FullName + (!loaded ? " (unresolved)" : ""));
			if (loaded)
			{
				output.Indent();
				output.WriteLine("Assembly reference loading information:");
				if (info!.HasErrors)
					output.WriteLine("There were some problems during assembly reference load, see below for more information!");
				PrintAssemblyLoadLogMessages(output, info);
				output.Unindent();
				output.WriteLine();
			}
		}

		internal static void PrintAssemblyLoadLogMessages(ITextOutput output, UnresolvedAssemblyNameReference asm)
		{
			var smartOutput = output as ISmartTextOutput;

			foreach (var item in asm.Messages)
			{
				switch (item.Item1)
				{
					case MessageKind.Error:
						smartOutput?.BeginSpan(ErrorColor);
						output.Write("Error: ");
						smartOutput?.EndSpan();
						break;
					case MessageKind.Warning:
						smartOutput?.BeginSpan(WarningColor);
						output.Write("Warning: ");
						smartOutput?.EndSpan();
						break;
					default:
						output.Write(item.Item1 + ": ");
						break;
				}
				output.WriteLine(item.Item2);
			}
		}

		static readonly HighlightingColor ErrorColor = new() {
			Foreground = new SimpleHighlightingBrush(global::Avalonia.Media.Colors.Red),
			FontWeight = global::Avalonia.Media.FontWeight.Bold,
		};

		static readonly HighlightingColor WarningColor = new() {
			Foreground = new SimpleHighlightingBrush(global::Avalonia.Media.Colors.Goldenrod),
			FontWeight = global::Avalonia.Media.FontWeight.Bold,
		};
	}
}
