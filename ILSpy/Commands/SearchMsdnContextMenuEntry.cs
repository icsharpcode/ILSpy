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
using System.Composition;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click → "Search Microsoft Docs..." — opens the MS-Docs landing page for the
	/// selected type / member / namespace in the default browser. Visible on the
	/// entity-bearing tree-node kinds plus <see cref="NamespaceTreeNode"/>; enabled only when
	/// the entity is publicly accessible (private members aren't on the docs site).
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.SearchMSDN), Category = nameof(Resources.Analyze), Icon = "Images/Search", Order = 230)]
	[Shared]
	public sealed class SearchMsdnContextMenuEntry : IContextMenuEntry
	{
		const string MsdnPrefix = "https://learn.microsoft.com/dotnet/api/";

		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return false;
			return nodes.All(IsSearchable);
		}

		public bool IsEnabled(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return false;
			return nodes.All(IsAccessible);
		}

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return;
			foreach (var node in nodes)
			{
				if (GetMsdnUrl(node) is { Length: > 0 } url)
					OpenInBrowser(url);
			}
		}

		/// <summary>Public for tests: builds the docs URL for the supplied node, or returns
		/// an empty string when the node has no associated docs page.</summary>
		public string GetMsdnUrl(SharpTreeNode node)
		{
			switch (node)
			{
				case NamespaceTreeNode ns when !string.IsNullOrEmpty(ns.Name):
					return (MsdnPrefix + ns.Name).ToLowerInvariant();
				case IMemberTreeNode m when m.Member is { } entity:
					// Reflection-name → docs path: backticks (generic arity) become hyphens,
					// `+` (nested type) becomes `.`, and ".ctor" gets a "-ctor" tail because
					// the docs site routes constructors that way.
					var name = entity.ReflectionName.Replace('`', '-').Replace('+', '.');
					if (name.EndsWith("..ctor", StringComparison.Ordinal))
						name = name.Substring(0, name.Length - 5) + "-ctor";
					return (MsdnPrefix + name).ToLowerInvariant();
				default:
					return string.Empty;
			}
		}

		static bool IsSearchable(SharpTreeNode node)
			=> node is NamespaceTreeNode or TypeTreeNode or EventTreeNode or FieldTreeNode
				or PropertyTreeNode or MethodTreeNode;

		static bool IsAccessible(SharpTreeNode node)
		{
			switch (node)
			{
				case NamespaceTreeNode ns:
					return !string.IsNullOrEmpty(ns.Name);
				case IMemberTreeNode m when m.Member is { } entity:
					if (!IsExternallyVisible(entity.Accessibility))
						return false;
					if (entity is IMember member)
					{
						var declaring = member.DeclaringTypeDefinition;
						if (declaring is null || !IsExternallyVisible(declaring.Accessibility))
							return false;
						// Members of delegates / enums don't have separate docs pages.
						if (declaring.Kind is TypeKind.Delegate or TypeKind.Enum)
							return false;
					}
					return true;
				default:
					return false;
			}
		}

		static bool IsExternallyVisible(Accessibility a)
			=> a is Accessibility.Public or Accessibility.Protected or Accessibility.ProtectedOrInternal;

		static void OpenInBrowser(string url)
		{
			try
			{
				// UseShellExecute=true delegates to the OS default browser on every supported
				// platform.
				Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
			}
			catch
			{
				// Failure to launch the browser is non-fatal — the menu item already returned.
			}
		}
	}
}
