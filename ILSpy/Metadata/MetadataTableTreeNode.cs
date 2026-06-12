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
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Common parent for the per-table leaves under "Tables". Each subclass corresponds to
	/// one CLI <see cref="TableIndex"/> and surfaces its rows. Phase 1 emits a fixed-width
	/// text dump via <see cref="ILSpyTreeNode.Decompile"/>; Phase 2 swaps to a DataGrid tab
	/// via the per-node <c>CreateTab</c> override added in that phase.
	/// </summary>
	public abstract class MetadataTableTreeNode : ILSpyTreeNode
	{
		protected readonly MetadataFile metadataFile;

		public TableIndex Kind { get; }

		public int RowCount => metadataFile.Metadata.GetTableRowCount(Kind);

		protected MetadataTableTreeNode(TableIndex kind, MetadataFile metadataFile)
		{
			Kind = kind;
			this.metadataFile = metadataFile ?? throw new ArgumentNullException(nameof(metadataFile));
		}

		public override object Icon => Images.MetadataTable;

		/// <summary>
		/// Byte offset of row <paramref name="rid"/> (1-based) in <paramref name="table"/>, relative to
		/// the start of the PE file. Every typed row viewmodel exposes this from its <c>Offset</c>
		/// property so the hex view can jump to the raw bytes.
		/// </summary>
		protected static int GetRowOffset(MetadataFile file, TableIndex table, int rid)
			=> GetRowOffset(file.Metadata, file.MetadataOffset, table, rid);

		/// <summary>
		/// Overload for the few entries that hold a raw <see cref="MetadataReader"/> + offset rather
		/// than a <see cref="MetadataFile"/>.
		/// </summary>
		protected static int GetRowOffset(MetadataReader metadata, int metadataOffset, TableIndex table, int rid)
			=> metadataOffset + metadata.GetTableMetadataOffset(table) + metadata.GetTableRowSize(table) * (rid - 1);

		/// <summary>
		/// Builds (and caches into <paramref name="tooltip"/>) a human-readable description of the
		/// entity a token column points at, e.g. <c>(AssemblyReference) System.Runtime, ...</c>.
		/// Entries expose this from their <c>{Column}Tooltip</c> properties so hovering a token cell
		/// shows what it refers to. Returns <see langword="null"/> for a nil handle.
		/// </summary>
		protected static string? GenerateTooltip(ref string? tooltip, MetadataFile module, EntityHandle handle)
		{
			if (tooltip == null)
			{
				if (handle.IsNil)
					return null;
				ITextOutput output = new PlainTextOutput();
				var context = new MetadataGenericContext(default(TypeDefinitionHandle), module.Metadata);
				var metadata = module.Metadata;
				switch (handle.Kind)
				{
					case HandleKind.ModuleDefinition:
						output.Write(metadata.GetString(metadata.GetModuleDefinition().Name));
						output.Write(" (this module)");
						break;
					case HandleKind.ModuleReference:
						ModuleReference moduleReference = metadata.GetModuleReference((ModuleReferenceHandle)handle);
						output.Write(metadata.GetString(moduleReference.Name));
						break;
					case HandleKind.AssemblyReference:
						var asmRef = new ICSharpCode.Decompiler.Metadata.AssemblyReference(metadata, (AssemblyReferenceHandle)handle);
						output.Write(asmRef.ToString());
						break;
					case HandleKind.Parameter:
						var param = metadata.GetParameter((ParameterHandle)handle);
						output.Write(param.SequenceNumber + " - " + metadata.GetString(param.Name));
						break;
					case HandleKind.EventDefinition:
						var @event = metadata.GetEventDefinition((EventDefinitionHandle)handle);
						output.Write(metadata.GetString(@event.Name));
						break;
					case HandleKind.PropertyDefinition:
						var prop = metadata.GetPropertyDefinition((PropertyDefinitionHandle)handle);
						output.Write(metadata.GetString(prop.Name));
						break;
					case HandleKind.AssemblyDefinition:
						var ad = metadata.GetAssemblyDefinition();
						output.Write(metadata.GetString(ad.Name));
						output.Write(" (this assembly)");
						break;
					case HandleKind.AssemblyFile:
						var af = metadata.GetAssemblyFile((AssemblyFileHandle)handle);
						output.Write(metadata.GetString(af.Name));
						break;
					case HandleKind.GenericParameter:
						var gp = metadata.GetGenericParameter((GenericParameterHandle)handle);
						output.Write(metadata.GetString(gp.Name));
						break;
					case HandleKind.ManifestResource:
						var mfr = metadata.GetManifestResource((ManifestResourceHandle)handle);
						output.Write(metadata.GetString(mfr.Name));
						break;
					case HandleKind.Document:
						var doc = metadata.GetDocument((DocumentHandle)handle);
						output.Write(metadata.GetString(doc.Name));
						break;
					default:
						handle.WriteTo(module, output, context);
						break;
				}
				tooltip = "(" + handle.Kind + ") " + output.ToString();
			}
			return tooltip;
		}
	}

	/// <summary>
	/// Typed companion: holds the row materialiser and routes selection to the DataGrid
	/// view. Rows are loaded lazily and cached so repeated activations don't re-walk the
	/// metadata. Token-bearing columns surface the runtime hex value here; Phase 3b makes
	/// them clickable hyperlinks via <see cref="MetadataColumnBuilder"/>.
	/// </summary>
	public abstract class MetadataTableTreeNode<TEntry> : MetadataTableTreeNode
		where TEntry : class
	{
		IReadOnlyList<TEntry>? cached;

		protected MetadataTableTreeNode(TableIndex kind, MetadataFile metadataFile)
			: base(kind, metadataFile)
		{
		}

		public override object Text => $"{(byte)Kind:X2} {Kind} ({RowCount})";
		public override string ToString() => Kind.ToString();

		protected abstract IReadOnlyList<TEntry> LoadTable();

		public override ContentPageModel CreateTab()
		{
			// IReadOnlyList<T> is covariant on T, so a strongly-typed list passes through to
			// MetadataTablePageModel.Items (declared as IReadOnlyList<object>) without a
			// reflective copy. The runtime type stays IReadOnlyList<TEntry>, which lets
			// DataGridCollectionView.GetItemType find the IEnumerable<TEntry> interface and
			// resolve property-path sorts correctly — see DataGridSortDescription.Initialize
			// + ReflectionHelper.GetNestedPropertyValue, which return null for every row when
			// the source's element type is object.
			var page = new MetadataTablePageModel {
				// Lead with the token-kind byte, mirroring the tree node's Text so the tab header
				// reads e.g. "02 TypeDef (1234)".
				Title = $"{(byte)Kind:X2} {Kind} ({RowCount})",
				Items = cached ??= LoadTable(),
			};
			MetadataColumnBuilder.Populate<TEntry>(page);
			ConfigurePage(page);
			return page;
		}

		/// <summary>
		/// Per-table hook for page settings beyond the reflected columns — e.g. the
		/// CustomDebugInformation table attaches a row-details template here. The default
		/// leaves the page as the column builder produced it.
		/// </summary>
		protected virtual void ConfigurePage(MetadataTablePageModel page)
		{
		}
	}
}
