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

using Avalonia.Media;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Compare
{
	/// <summary>
	/// Tree-node façade over a merged <see cref="Entry"/>. The text combines both sides'
	/// formatted entity strings (with a <c>" -> "</c> separator when they differ); the
	/// background brush is a light tint that mirrors the diff kind (green for additions,
	/// pink for removals, blue for updates). Identical entries render with a transparent
	/// background and stay hidden unless the toolbar's "Show Identical" toggle is on.
	/// </summary>
	public sealed class ComparisonEntryTreeNode : ILSpyTreeNode
	{
		readonly Entry entry;
		readonly CompareTabPageModel pane;

		internal Entry Entry => entry;

		public ComparisonEntryTreeNode(Entry entry, CompareTabPageModel pane)
		{
			this.entry = entry;
			this.pane = pane;
			LazyLoading = entry.Children != null;
		}

		protected override void LoadChildren()
		{
			if (entry.Children == null)
				return;
			// Order: changes first (Add/Remove/Update lexically before None via the negated
			// enum cast), then by SymbolKind so types group together, then alphabetically
			// by signature. Matches the WPF view's display order.
			foreach (var item in entry.Children.OrderBy(e => (-(int)e.RecursiveKind, e.Entity.SymbolKind, e.Signature)))
				Children.Add(new ComparisonEntryTreeNode(item, pane));
		}

		public override object Text {
			get {
				var leftText = FormatEntity(entry.Entity);
				var rightText = FormatEntity(entry.OtherEntity);
				return leftText + (rightText != null && leftText != rightText ? " -> " + rightText : "");
			}
		}

		string? FormatEntity(ISymbol? symbol) => symbol switch {
			ITypeDefinition t => Language.TypeToString(t, ConversionFlags.None),
			IEntity e => Language.EntityToString(e,
				ConversionFlags.All & ~(ConversionFlags.ShowDeclaringType
					| ConversionFlags.UseFullyQualifiedEntityNames
					| ConversionFlags.UseFullyQualifiedTypeNames)),
			INamespace n => n.FullName,
			IModule m => m.FullAssemblyName,
			_ => null,
		};

		public override object Icon => entry.Entity switch {
			ITypeDefinition t => TypeIconForKind(t),
			IMethod => ICSharpCode.ILSpy.Images.Method,
			IField => ICSharpCode.ILSpy.Images.Field,
			IProperty => ICSharpCode.ILSpy.Images.Property,
			IEvent => ICSharpCode.ILSpy.Images.Event,
			INamespace => ICSharpCode.ILSpy.Images.Namespace,
			IModule => ICSharpCode.ILSpy.Images.Assembly,
			_ => ICSharpCode.ILSpy.Images.Class,
		};

		static object TypeIconForKind(ITypeDefinition t) => t.Kind switch {
			TypeKind.Interface => ICSharpCode.ILSpy.Images.Interface,
			TypeKind.Struct => ICSharpCode.ILSpy.Images.Struct,
			TypeKind.Enum => ICSharpCode.ILSpy.Images.Enum,
			TypeKind.Delegate => ICSharpCode.ILSpy.Images.Delegate,
			_ => ICSharpCode.ILSpy.Images.Class,
		};

		public override void Decompile(Language language, ICSharpCode.Decompiler.ITextOutput output, DecompilationOptions options)
		{
			// Compare nodes aren't decompilable on their own — they're a diff overlay.
			// Clicking a row jumps to the underlying entity in the assembly tree, which
			// then drives the normal decompile flow on whichever tab the user picks.
		}

		public override FilterResult Filter(LanguageSettings settings)
		{
			// Hide identical rows unless the toolbar toggle says otherwise. Recursive
			// because a parent node whose ALL descendants are unchanged should also hide.
			return pane.ShowIdentical || entry.RecursiveKind != DiffKind.None
				? FilterResult.Match
				: FilterResult.Hidden;
		}

		/// <summary>
		/// Pastel-tint background per diff kind: green = added on the right, pink = removed
		/// from the right (i.e. only on the left), blue = signatures match but contents
		/// differ. Identical rows fall through to the parent background.
		/// </summary>
		public IBrush BackgroundBrush => entry.RecursiveKind switch {
			DiffKind.Add => new SolidColorBrush(Color.FromArgb(0xFF, 0x90, 0xEE, 0x90)),
			DiffKind.Remove => new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xB6, 0xC1)),
			DiffKind.Update => new SolidColorBrush(Color.FromArgb(0xFF, 0xAD, 0xD8, 0xE6)),
			_ => Brushes.Transparent,
		};
	}
}
