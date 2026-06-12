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
using System.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.TreeNodes
{
	public sealed class NamespaceTreeNode : ILSpyTreeNode
	{
		readonly string name;
		readonly string fullName;
		readonly MetadataFile module;

		/// <summary>
		/// Display label for this namespace node. In flat mode this is the full dotted
		/// namespace path; in nested mode it's just the last segment (the parent chain
		/// supplies the rest). Tests use this to distinguish the two modes.
		/// </summary>
		public string Name => name;

		/// <summary>
		/// Full dotted namespace path used to query metadata for member types. Equals
		/// <see cref="Name"/> in flat mode; in nested mode it stays the dotted path
		/// even though the display label is the last segment only.
		/// </summary>
		public string FullName => fullName;

		public NamespaceTreeNode(string name, MetadataFile module)
			: this(name, name, module)
		{
		}

		/// <summary>
		/// Nested-mode constructor: separates the display label (last segment) from the
		/// full dotted path used to locate types in metadata.
		/// </summary>
		public NamespaceTreeNode(string displayName, string fullName, MetadataFile module)
		{
			this.name = displayName ?? throw new ArgumentNullException(nameof(displayName));
			this.fullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			LazyLoading = true;
		}

		public override object Text => name.Length == 0 ? "-" : name;

		public override object Icon => Images.Namespace;

		// Stable identity for SessionSettings.ActiveTreeViewPath (used by AssemblyTreeModel.
		// FindNodeByPath / GetPathForNode). Without this override the default Object.ToString
		// returns the type name, which makes save/restore of namespace selections silently fail.
		// Uses the full dotted path so saved-and-restored selections survive a toggle of the
		// nested-namespace setting.
		public override string ToString() => fullName;

		protected override void LoadChildren()
		{
			var metadata = module.Metadata;
			var types = metadata.TypeDefinitions
				.Where(t => {
					var td = metadata.GetTypeDefinition(t);
					return td.GetDeclaringType().IsNil
						&& metadata.GetString(td.Namespace) == fullName;
				})
				.OrderBy(t => metadata.GetString(metadata.GetTypeDefinition(t).Name), NaturalStringComparer.Instance);

			foreach (var t in types)
				Children.Add(new TypeTreeNode(t, module));
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			// Enumerate via the type system matching this run's settings so the namespace
			// listing agrees with what the per-type decompilation below produces.
			var typeSystem = module.GetTypeSystemWithDecompilerSettingsOrNull(options.DecompilerSettings);
			if (typeSystem == null)
			{
				language.WriteCommentLine(output, "(type system unavailable)");
				return;
			}
			var types = typeSystem.MainModule.TypeDefinitions
				.Where(t => t.Namespace == name && t.DeclaringTypeDefinition == null);
			language.DecompileNamespace(name, types, output, options);
		}

		// A namespace counts as public-API iff at least one type it contains is public-API.
		// Forces lazy children once so the aggregate is available before the cell template
		// queries it for the gray-foreground binding; the result is cached because types
		// don't change accessibility at runtime. Mirrors WPF's AssemblyTreeNode.SetPublicAPI
		// recursive walk.
		bool? cachedIsPublicAPI;
		public override bool IsPublicAPI {
			get {
				if (cachedIsPublicAPI is { } cached)
					return cached;
				EnsureLazyChildren();
				cachedIsPublicAPI = Children.OfType<ILSpyTreeNode>().Any(c => c.IsPublicAPI);
				return cachedIsPublicAPI.Value;
			}
		}

		public override FilterResult Filter(LanguageSettings settings)
		{
			if (settings.SearchTermMatches(name))
				return FilterResult.MatchAndRecurse;
			else
				return FilterResult.Recurse;
		}
	}
}
