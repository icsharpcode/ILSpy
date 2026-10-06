// Copyright (c) 2026 Piero Viano
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

using Avalonia.Media;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Navigation
{
	/// <summary>
	/// Most-recently-used lists behind the Recent Files and Recent Locations popups. Both lists are
	/// most-recent-first, distinct and capped at <see cref="Capacity"/>: a file is one tree node, a
	/// location is one line of one node's document. Fed by <see cref="Docking.DockWorkspace"/> as
	/// navigation history is recorded; entries of unloaded assemblies are dropped through
	/// <see cref="RemoveAll"/>.
	/// </summary>
	public sealed class RecentNavigation
	{
		public const int Capacity = 50;

		readonly List<RecentFile> files = new();
		readonly List<RecentLocation> locations = new();

		public IReadOnlyList<RecentFile> Files => files;
		public IReadOnlyList<RecentLocation> Locations => locations;

		/// <summary>Moves <paramref name="node"/> to the top of the recent files.</summary>
		public void RecordFile(SharpTreeNode node)
		{
			ArgumentNullException.ThrowIfNull(node);
			files.RemoveAll(f => ReferenceEquals(f.Node, node));
			files.Insert(0, new RecentFile(node));
			Trim(files);
		}

		/// <summary>Moves <paramref name="location"/> to the top of the recent locations, replacing an
		/// older entry for the same line of the same node.</summary>
		public void RecordLocation(RecentLocation location)
		{
			ArgumentNullException.ThrowIfNull(location);
			locations.RemoveAll(l => l.IsSamePlace(location));
			locations.Insert(0, location);
			Trim(locations);
		}

		/// <summary>Drops every file and location whose node matches <paramref name="predicate"/>.</summary>
		public void RemoveAll(Predicate<SharpTreeNode> predicate)
		{
			ArgumentNullException.ThrowIfNull(predicate);
			files.RemoveAll(f => predicate(f.Node));
			locations.RemoveAll(l => predicate(l.Node));
		}

		public void Clear()
		{
			files.Clear();
			locations.Clear();
		}

		static void Trim<T>(List<T> list)
		{
			if (list.Count > Capacity)
				list.RemoveRange(Capacity, list.Count - Capacity);
		}

		internal static string GetDisplayText(SharpTreeNode node)
			=> ((node is ILSpyTreeNode ilspy ? ilspy.NavigationText : node.Text) ?? string.Empty).ToString() ?? string.Empty;

		internal static string? GetAssemblyName(SharpTreeNode node)
			=> node.AncestorsAndSelf().OfType<AssemblyTreeNode>().LastOrDefault()?.LoadedAssembly.ShortName;
	}

	/// <summary>A tree node (type, member, resource, ...) the user recently viewed.</summary>
	public sealed class RecentFile(SharpTreeNode node)
	{
		public SharpTreeNode Node { get; } = node ?? throw new ArgumentNullException(nameof(node));

		public string DisplayText => RecentNavigation.GetDisplayText(Node);

		public IImage? Icon => Node.Icon as IImage;
	}

	/// <summary>
	/// A caret position the user recently left: the node, the tab that showed it, the captured view
	/// state, and the 1-based line/column plus the trimmed text of that line as a preview.
	/// </summary>
	public sealed class RecentLocation
	{
		const int MaxPreviewLength = 200;

		public SharpTreeNode Node { get; }
		public TabPageModel? Tab { get; }
		public DecompilerTextViewState State { get; }
		public int Line { get; }
		public int Column { get; }
		public string Preview { get; }

		public RecentLocation(SharpTreeNode node, TabPageModel? tab, DecompilerTextViewState state, int line, int column, string preview)
		{
			Node = node ?? throw new ArgumentNullException(nameof(node));
			Tab = tab;
			State = state;
			Line = line;
			Column = column;
			Preview = preview ?? string.Empty;
		}

		public string DisplayText => RecentNavigation.GetDisplayText(Node);

		public IImage? Icon => Node.Icon as IImage;

		/// <summary>
		/// Builds a location from the document <paramref name="text"/> the caret offset of
		/// <paramref name="state"/> refers to. The offset is clamped to the text.
		/// </summary>
		public static RecentLocation Create(SharpTreeNode node, TabPageModel? tab, DecompilerTextViewState state, string? text)
		{
			text ??= string.Empty;
			int offset = Math.Clamp(state.CaretOffset, 0, text.Length);
			int lineStart = offset == 0 ? 0 : text.LastIndexOf('\n', offset - 1) + 1;
			int lineEnd = text.IndexOf('\n', offset);
			if (lineEnd < 0)
				lineEnd = text.Length;
			int line = 1;
			for (int i = 0; i < lineStart; i++)
			{
				if (text[i] == '\n')
					line++;
			}
			var preview = text.Substring(lineStart, lineEnd - lineStart).Trim();
			if (preview.Length > MaxPreviewLength)
				preview = preview.Substring(0, MaxPreviewLength);
			return new RecentLocation(node, tab, state, line, offset - lineStart + 1, preview);
		}

		internal bool IsSamePlace(RecentLocation other)
			=> ReferenceEquals(Node, other.Node) && Line == other.Line;
	}
}
