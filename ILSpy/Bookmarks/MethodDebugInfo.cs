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
using System.Linq;

namespace ICSharpCode.ILSpy.Bookmarks
{
	/// <summary>
	/// The IL-offset &lt;-&gt; text-line map for one decompiled method, built from the decompiler's
	/// sequence points. A line that starts a statement maps to that statement's IL offset; the IL
	/// offset is what a body bookmark stores, because it is stable when decompiler settings reflow
	/// the C# text. Pure value type (ints/strings) so it can be built and tested without the decompiler.
	/// </summary>
	public sealed class MethodDebugInfo
	{
		// Each statement's first text line and its IL offset, kept in both orders for the two lookups.
		readonly (int Line, int ILOffset)[] byLine;
		readonly (int ILOffset, int Line)[] byOffset;

		public MethodDebugInfo(uint token, string fileName, string assemblyFullName, string moduleName,
			string memberName, IEnumerable<(int Line, int ILOffset)> points)
		{
			Token = token;
			FileName = fileName;
			AssemblyFullName = assemblyFullName;
			ModuleName = moduleName;
			MemberName = memberName;
			var list = points.ToList();
			byLine = list.OrderBy(p => p.Line).ThenBy(p => p.ILOffset).ToArray();
			byOffset = list.Select(p => (p.ILOffset, p.Line)).OrderBy(p => p.ILOffset).ToArray();
		}

		public uint Token { get; }
		public string FileName { get; }
		public string AssemblyFullName { get; }
		public string ModuleName { get; }
		public string MemberName { get; }

		/// <summary>The IL offset of the statement that starts on <paramref name="line"/>, if any.</summary>
		public bool TryGetOffsetForLine(int line, out int ilOffset)
		{
			foreach (var p in byLine)
			{
				if (p.Line == line)
				{
					ilOffset = p.ILOffset; // byLine is ordered, so this is the lowest offset on the line
					return true;
				}
				if (p.Line > line)
					break;
			}
			ilOffset = 0;
			return false;
		}

		/// <summary>
		/// The text line for <paramref name="ilOffset"/>: the statement at that exact offset, or the
		/// last statement starting at or before it (so a body bookmark re-anchors even when a setting
		/// change shifts where the statement lands).
		/// </summary>
		public bool TryGetLineForOffset(int ilOffset, out int line)
		{
			line = 0;
			bool found = false;
			foreach (var p in byOffset)
			{
				if (p.ILOffset > ilOffset)
					break;
				line = p.Line;
				found = true;
			}
			// Nothing at or before the offset -> fall back to the first statement, if any.
			if (!found && byOffset.Length > 0)
			{
				line = byOffset[0].Line;
				found = true;
			}
			return found;
		}
	}

	/// <summary>
	/// All per-method <see cref="MethodDebugInfo"/> maps for one decompiled document, the bridge
	/// between a clicked line and a token+IL-offset body anchor (and back).
	/// </summary>
	public sealed class DecompiledDebugInfo
	{
		public static readonly DecompiledDebugInfo Empty = new(new List<MethodDebugInfo>());

		readonly IReadOnlyList<MethodDebugInfo> methods;
		readonly Dictionary<uint, MethodDebugInfo> byToken;

		public DecompiledDebugInfo(IReadOnlyList<MethodDebugInfo> methods)
		{
			this.methods = methods;
			byToken = new Dictionary<uint, MethodDebugInfo>();
			foreach (var m in methods)
				byToken[m.Token] = m;
		}

		public IReadOnlyList<MethodDebugInfo> Methods => methods;

		/// <summary>
		/// Resolves a clicked <paramref name="line"/> to a body anchor when the line starts a
		/// statement. Lines that don't (blank lines, lone braces) yield no body anchor; the caller
		/// then falls back to a definition (token) anchor.
		/// </summary>
		public bool TryGetBodyAnchor(int line, out MethodDebugInfo method, out int ilOffset)
		{
			foreach (var m in methods)
			{
				if (m.TryGetOffsetForLine(line, out ilOffset))
				{
					method = m;
					return true;
				}
			}
			method = null!;
			ilOffset = 0;
			return false;
		}

		/// <summary>The text line for a stored body anchor, used to place the gutter icon and to scroll on navigation.</summary>
		public bool TryGetLine(uint token, int ilOffset, out int line)
		{
			if (byToken.TryGetValue(token, out var m))
				return m.TryGetLineForOffset(ilOffset, out line);
			line = 0;
			return false;
		}
	}
}
