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

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Compare
{
	/// <summary>
	/// How an <see cref="Entry"/> compares between the left and right assemblies. Stored on
	/// leaf entries by <see cref="CompareEngine"/>; interior nodes derive a recursive
	/// summary via <see cref="Entry.RecursiveKind"/>.
	/// </summary>
	public enum DiffKind
	{
		/// <summary>Identical on both sides.</summary>
		None = ' ',
		/// <summary>Only present on the right.</summary>
		Add = '+',
		/// <summary>Only present on the left.</summary>
		Remove = '-',
		/// <summary>Present on both but the bodies differ.</summary>
		Update = '~',
	}

	/// <summary>
	/// One node in the merged comparison tree. The <see cref="Entity"/> is the symbol on
	/// the left assembly; <see cref="OtherEntity"/> is the right-side counterpart when one
	/// exists. <see cref="Signature"/> is the C#-formatted text the engine keys on for
	/// matching pairs across sides.
	/// </summary>
	[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
	public sealed class Entry
	{
		DiffKind? kind;

		/// <summary>
		/// Per-node diff kind for leaves; for interior nodes, summarises the children:
		/// all-Add → Add; all-Remove → Remove; any-non-None → Update; otherwise None.
		/// </summary>
		public DiffKind RecursiveKind {
			get {
				if (kind != null)
					return kind.Value;
				if (Children == null)
					return DiffKind.None;
				int addCount = 0, removeCount = 0, updateCount = 0;
				foreach (var item in Children)
				{
					switch (item.RecursiveKind)
					{
						case DiffKind.Add:
							addCount++;
							break;
						case DiffKind.Remove:
							removeCount++;
							break;
						case DiffKind.Update:
							updateCount++;
							break;
					}
				}
				if (addCount == Children.Count)
					return DiffKind.Add;
				if (removeCount == Children.Count)
					return DiffKind.Remove;
				if (addCount > 0 || removeCount > 0 || updateCount > 0)
					return DiffKind.Update;
				return DiffKind.None;
			}
		}

		public DiffKind Kind {
			get => kind ?? DiffKind.None;
			set => kind = value;
		}

		public required string Signature { get; init; }
		public required ISymbol Entity { get; init; }
		public ISymbol? OtherEntity { get; init; }
		public Entry? Parent { get; set; }
		public List<Entry>? Children { get; set; }

		string GetDebuggerDisplay() => $"Entry{Kind} {Entity?.ToString() ?? Signature}";
	}

	/// <summary>
	/// Two entries match when their <see cref="Entry.Signature"/> string is equal — that's
	/// the only stable key across an assembly version, since metadata tokens are not
	/// preserved across rebuilds.
	/// </summary>
	public sealed class EntryComparer : IEqualityComparer<Entry>
	{
		public static readonly EntryComparer Instance = new();

		public bool Equals(Entry? x, Entry? y) => x?.Signature == y?.Signature;

		public int GetHashCode([DisallowNull] Entry obj) => obj.Signature.GetHashCode();
	}
}
