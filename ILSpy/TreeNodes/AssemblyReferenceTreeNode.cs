// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Themes;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Node within assembly reference list.
	/// </summary>
	public sealed class AssemblyReferenceTreeNode : ILSpyTreeNode
	{
		readonly MetadataModule module;
		readonly AssemblyReference r;
		readonly AssemblyTreeNode parentAssembly;

		public AssemblyReferenceTreeNode(MetadataModule module, AssemblyReference r, AssemblyTreeNode parentAssembly)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			this.r = r ?? throw new ArgumentNullException(nameof(r));
			this.parentAssembly = parentAssembly ?? throw new ArgumentNullException(nameof(parentAssembly));
			this.LazyLoading = true;
		}

		public AssemblyReference AssemblyReference => r;

		public override object Text {
			get { return Language.EscapeName(r.Name) + GetSuffixString(r.Handle); }
		}

		public override object Icon => ImagesProvider.Assembly;

		public override bool ShowExpander {
			get {
				// Special case for mscorlib: It likely doesn't have any children so call EnsureLazyChildren to
				// remove the expander from the node.
				if (r.Name == "mscorlib")
				{
					// See https://github.com/icsharpcode/ILSpy/issues/2548: Adding assemblies to the tree view
					// while the list of references is updated causes problems with WPF's ListView rendering.
					// Moving the assembly resolving out of the "add assembly reference"-loop by using the
					// dispatcher fixes the issue.
					Dispatcher.CurrentDispatcher.BeginInvoke((Action)EnsureLazyChildren, DispatcherPriority.Normal);
				}
				return base.ShowExpander;
			}
		}

		public override void ActivateItem(IPlatformRoutedEventArgs e)
		{
			if (parentAssembly.Parent is AssemblyListTreeNode assemblyListNode)
			{
				var resolver = parentAssembly.LoadedAssembly.GetAssemblyResolver();
				assemblyListNode.Select(assemblyListNode.FindAssemblyNode(resolver.Resolve(r)));
				e.Handled = true;
			}
		}

		protected override void LoadChildren()
		{
			this.Children.Add(new AssemblyReferenceReferencedTypesTreeNode(module, r));

			var resolver = parentAssembly.LoadedAssembly.GetAssemblyResolver(SettingsService.DecompilerSettings.AutoLoadAssemblyReferences);
			var referencedModule = resolver.Resolve(r);
			if (referencedModule != null)
			{
				var module = (MetadataModule)referencedModule.GetTypeSystemWithCurrentOptionsOrNull(SettingsService)?.MainModule;
				foreach (var childRef in referencedModule.AssemblyReferences)
					this.Children.Add(new AssemblyReferenceTreeNode(module, childRef, parentAssembly));
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			var loaded = parentAssembly.LoadedAssembly.LoadedAssemblyReferencesInfo.TryGetInfo(r.FullName, out var info);
			if (r.IsWindowsRuntime)
			{
				output.WriteLine(r.FullName + " [WinRT]" + (!loaded ? " (unresolved)" : ""));
			}
			else
			{
				output.WriteLine(r.FullName + (!loaded ? " (unresolved)" : ""));
			}
			if (loaded)
			{
				output.Indent();
				output.WriteLine("Assembly reference loading information:");
				if (info.HasErrors)
				{
					output.WriteLine("There were some problems during assembly reference load, see below for more information!");
				}
				PrintAssemblyLoadLogMessages(output, info);
				output.Unindent();
				output.WriteLine();
			}
		}

		internal static void PrintAssemblyLoadLogMessages(ITextOutput output, UnresolvedAssemblyNameReference asm)
		{
			HighlightingColor red = GetColor(Colors.Red);
			HighlightingColor yellow = GetColor(Colors.Yellow);

			var smartOutput = output as ISmartTextOutput;

			foreach (var item in asm.Messages)
			{
				switch (item.Item1)
				{
					case MessageKind.Error:
						smartOutput?.BeginSpan(red);
						output.Write("Error: ");
						smartOutput?.EndSpan();
						break;
					case MessageKind.Warning:
						smartOutput?.BeginSpan(yellow);
						output.Write("Warning: ");
						smartOutput?.EndSpan();
						break;
					default:
						output.Write(item.Item1 + ": ");
						break;
				}
				output.WriteLine(item.Item2);
			}

			static HighlightingColor GetColor(Color color)
			{
				var hc = new HighlightingColor {
					Foreground = new SimpleHighlightingBrush(color),
					FontWeight = FontWeights.Bold
				};
				if (ThemeManager.Current.IsDarkTheme)
				{
					return ThemeManager.GetColorForDarkTheme(hc);
				}
				return hc;
			}
		}
	}
}
