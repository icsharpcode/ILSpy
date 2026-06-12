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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Resolves metadata-grid interactions (a clicked token cell, a double-clicked row, a token
	/// reference) to the matching assembly-tree node or metadata-table node. Split out of
	/// <c>DockWorkspace</c> -- which keeps only the dock-side actions (select, scroll, history,
	/// open-in-new-tab) -- so the reflection-heavy "read a typed metadata row" knowledge lives in
	/// one place. Each typed table viewmodel buries its <see cref="MetadataFile"/> in a private
	/// readonly field of the same name and exposes the row token as an int property, so both are
	/// read by reflection.
	/// </summary>
	[Export]
	[Shared]
	public sealed class MetadataNavigator
	{
		readonly AssemblyTreeModel assemblyTreeModel;

		[ImportingConstructor]
		public MetadataNavigator(AssemblyTreeModel assemblyTreeModel)
		{
			this.assemblyTreeModel = assemblyTreeModel;
		}

		/// <summary>
		/// Reads the token in the <paramref name="columnName"/> cell of <paramref name="row"/> as a
		/// reference, or null when the cell holds no (non-zero) token.
		/// </summary>
		public MetadataTokenReference? ReadCellToken(object row, string columnName)
			=> TryReadRow(row, columnName, out var file, out var handle)
				? new MetadataTokenReference(file!, handle)
				: null;

		/// <summary>
		/// Resolves a double-clicked metadata row (its <c>Token</c> column + owning module) to the
		/// matching assembly-tree node, or null when the token doesn't resolve to an
		/// <see cref="IEntity"/> the tree models (heap rows, refs into unloaded assemblies, ...).
		/// </summary>
		public ILSpyTreeNode? ResolveRowToTreeNode(object row)
		{
			if (!TryReadRow(row, "Token", out var file, out var handle) || handle.IsNil)
				return null;

			var owningAssembly = assemblyTreeModel.AssemblyList?.GetAssemblies()
				.FirstOrDefault(a => ReferenceEquals(a.GetMetadataFileOrNull(), file));
			if (owningAssembly is null)
				return null;
			if (file?.GetTypeSystemWithCurrentOptionsOrNull()?.MainModule is not MetadataModule metadataModule)
				return null;
			IEntity? entity;
			try
			{ entity = metadataModule.ResolveEntity(handle); }
			catch
			{ return null; }
			return entity is null ? null : assemblyTreeModel.FindTreeNode(entity);
		}

		/// <summary>
		/// Finds the metadata-table tree node a token <paramref name="reference"/> points at, drilling
		/// the referenced module's Metadata -> Tables container and matching the handle's kind (which
		/// doubles as <see cref="TableIndex"/> for table-backed entities). Null for heap handles or an
		/// unloaded module.
		/// </summary>
		public MetadataTableTreeNode? FindTableNode(MetadataTokenReference reference)
		{
			var targetAssembly = assemblyTreeModel.Root?.Children
				.OfType<AssemblyTreeNode>()
				.FirstOrDefault(a => ReferenceEquals(a.LoadedAssembly.GetMetadataFileOrNull(), reference.MetadataFile));
			if (targetAssembly == null)
				return null;
			targetAssembly.EnsureLazyChildren();
			var metaNode = targetAssembly.Children.OfType<MetadataTreeNode>().FirstOrDefault();
			if (metaNode == null)
				return null;
			metaNode.EnsureLazyChildren();
			var tablesNode = metaNode.Children.OfType<MetadataTablesTreeNode>().FirstOrDefault();
			if (tablesNode == null)
				return null;
			tablesNode.EnsureLazyChildren();
			var tableIndex = (TableIndex)(int)reference.Handle.Kind;
			return tablesNode.Children.OfType<MetadataTableTreeNode>()
				.FirstOrDefault(t => t.Kind == tableIndex);
		}

		// Each typed metadata-row viewmodel buries its MetadataFile in a private readonly field of the
		// same name across all ~43 table viewmodels and exposes the row token as an int property;
		// resolve both by reflection. token == 0 means "no token" (e.g. a Nil parent reference).
		static bool TryReadRow(object row, string tokenProperty, out MetadataFile? file, out EntityHandle handle)
		{
			file = null;
			handle = default;
			var fileField = row.GetType().GetField("metadataFile", BindingFlags.Instance | BindingFlags.NonPublic);
			if (fileField?.GetValue(row) is not MetadataFile metadataFile)
				return false;
			if (row.GetType().GetProperty(tokenProperty)?.GetValue(row) is not int token || token == 0)
				return false;
			file = metadataFile;
			handle = MetadataTokens.EntityHandle(token);
			return true;
		}
	}
}
