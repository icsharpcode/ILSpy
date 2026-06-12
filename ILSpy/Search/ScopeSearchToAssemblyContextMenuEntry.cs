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

using System.Composition;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Search
{
	/// <summary>
	/// Right-click an <see cref="AssemblyTreeNode"/> → "Search within this assembly".
	/// Prepends <c>inassembly:&lt;short-name&gt;</c> to the live search term so subsequent
	/// matches are restricted to that assembly. The minimal parser in
	/// <see cref="RunningSearch"/> recognises the prefix and feeds it into the
	/// <c>SearchRequest.InAssembly</c> filter the ILSpyX strategies consult.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.ScopeSearchToThisAssembly), Category = nameof(Resources.Analyze), Icon = "Images/FindAssembly", Order = 210)]
	[Shared]
	public sealed class ScopeSearchToAssemblyContextMenuEntry : IContextMenuEntry
	{
		readonly SearchPaneModel searchPane;

		[ImportingConstructor]
		public ScopeSearchToAssemblyContextMenuEntry(SearchPaneModel searchPane)
		{
			this.searchPane = searchPane;
		}

		public bool IsVisible(TextViewContext context) => AssemblyName(context) is not null;

		public bool IsEnabled(TextViewContext context) => IsVisible(context);

		public void Execute(TextViewContext context)
		{
			if (AssemblyName(context) is not { } name)
				return;
			searchPane.SearchTerm = MergeScopePrefix(searchPane.SearchTerm, "inassembly", name);
		}

		// The assembly to scope to: a selected AssemblyTreeNode in the tree, or the assembly that owns
		// the symbol under a right-clicked code reference.
		static string? AssemblyName(TextViewContext context)
		{
			if (context.SelectedTreeNodes is { Length: > 0 } nodes && nodes.All(n => n is AssemblyTreeNode))
				return nodes.OfType<AssemblyTreeNode>().FirstOrDefault()?.LoadedAssembly.ShortName;
			return (context.Reference?.Reference as IEntity)?.ParentModule?.AssemblyName;
		}

		internal static string MergeScopePrefix(string current, string prefix, string value)
		{
			// Strip any existing same-prefix token so consecutive scope clicks REPLACE rather
			// than stack. The value may need quoting if it contains spaces — assembly short
			// names typically don't, but quote anyway for safety.
			var tokens = (current ?? string.Empty)
				.Split(' ', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries)
				.Where(t => !t.StartsWith(prefix + ":", System.StringComparison.OrdinalIgnoreCase))
				.ToList();
			var quoted = value.Contains(' ') ? "\"" + value + "\"" : value;
			tokens.Insert(0, prefix + ":" + quoted);
			return string.Join(' ', tokens);
		}
	}
}
