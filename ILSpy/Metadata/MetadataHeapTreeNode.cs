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
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Common parent for the four heap views (#Strings, #US, #GUID, #Blob). Holds the
	/// shared icon and the <see cref="HandleKind"/> that distinguishes them — so token
	/// navigation in Phase 3 can dispatch a heap-typed handle to the right node by walking
	/// children and matching <see cref="Kind"/>. Concrete subclasses own row materialisation
	/// and rendering because each heap has a different row shape.
	/// </summary>
	public abstract class MetadataHeapTreeNode : ILSpyTreeNode
	{
		protected readonly MetadataFile metadataFile;

		public HandleKind Kind { get; }

		protected MetadataHeapTreeNode(HandleKind kind, MetadataFile metadataFile)
		{
			Kind = kind;
			this.metadataFile = metadataFile ?? throw new ArgumentNullException(nameof(metadataFile));
		}

		public override object Icon => Images.Heap;
	}

	/// <summary>
	/// Typed companion that owns the lazy row cache, the "{name} Heap (count)" header, and the
	/// DataGrid tab. Subclasses supply the heap's display name and materialise its rows, which is
	/// all that differs between the four heaps.
	/// </summary>
	public abstract class MetadataHeapTreeNode<TEntry> : MetadataHeapTreeNode
		where TEntry : class
	{
		List<TEntry>? entries;

		protected MetadataHeapTreeNode(HandleKind kind, MetadataFile metadataFile)
			: base(kind, metadataFile)
		{
		}

		/// <summary>The heap's display name, e.g. "Blob Heap".</summary>
		protected abstract string HeapName { get; }

		public override object Text => $"{HeapName} ({EnsureEntries().Count})";
		public override string ToString() => HeapName;

		public override ContentPageModel CreateTab()
		{
			var page = new MetadataTablePageModel {
				Title = HeapName,
				Items = EnsureEntries(),
			};
			MetadataColumnBuilder.Populate<TEntry>(page);
			return page;
		}

		protected List<TEntry> EnsureEntries()
		{
			if (entries is { } cached)
				return cached;
			var list = new List<TEntry>();
			LoadEntries(metadataFile.Metadata, list);
			entries = list;
			return list;
		}

		/// <summary>Materialises every heap row into <paramref name="list"/>.</summary>
		protected abstract void LoadEntries(MetadataReader metadata, List<TEntry> list);

		/// <summary>
		/// Shared walk for the offset-chained heaps (#Strings, #US, #Blob). Handle 0 is a
		/// real first entry (the heap's empty/zero slot) even though it is the nil handle,
		/// so each entry is added before <paramref name="getNextHandle"/> is consulted; the
		/// walk ends when the returned handle is nil.
		/// </summary>
		protected static void WalkHeap<THandle>(List<TEntry> list, THandle firstHandle,
			Func<THandle, TEntry> createEntry, Func<THandle, THandle> getNextHandle,
			Predicate<THandle> isNil)
			where THandle : struct
		{
			var handle = firstHandle;
			do
			{
				list.Add(createEntry(handle));
				handle = getNextHandle(handle);
			} while (!isNil(handle));
		}
	}
}
