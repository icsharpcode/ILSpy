// Copyright (c) 2026 Siegfried Pammer
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
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	/// <summary>
	/// Reads the sequence points out of a portable PDB and compares two PDBs (typically the one
	/// the original C# compiler produced versus the one <c>PortablePdbWriter</c> reconstructs)
	/// at the granularity that actually matters to a debugging user: the breakpoint map.
	/// </summary>
	/// <remarks>
	/// The decompiler reconstructs IL ranges, hidden sequence points and local scopes from the
	/// ILAst, so those never match the original compiler byte-for-byte. What it can and should
	/// get right is <em>where the visible (non-hidden) breakpoints land in the source</em>.
	/// The comparison therefore projects each PDB onto the ordered list of visible sequence-point
	/// source locations per method (keyed by method-definition row, which is shared between the
	/// PDB and the PE it describes) and reports the residual difference.
	/// </remarks>
	static class PdbSequencePoints
	{
		internal readonly record struct RawSequencePoint(int Offset, bool IsHidden,
			int StartLine, int StartColumn, int EndLine, int EndColumn)
		{
			/// <summary>Visible source location, formatted with or without columns.</summary>
			public string Format(bool includeColumns)
			{
				return includeColumns
					? $"({StartLine},{StartColumn})-({EndLine},{EndColumn})"
					: $"L{StartLine}-{EndLine}";
			}
		}

		/// <summary>
		/// Reads every method's sequence points (hidden ones included, in document order) from a
		/// portable PDB, keyed by the row number of the owning method definition.
		/// </summary>
		public static Dictionary<int, List<RawSequencePoint>> Read(Stream portablePdb)
		{
			portablePdb.Position = 0;
			using var provider = MetadataReaderProvider.FromPortablePdbStream(portablePdb, MetadataStreamOptions.LeaveOpen);
			var reader = provider.GetMetadataReader();
			var result = new Dictionary<int, List<RawSequencePoint>>();
			foreach (var handle in reader.MethodDebugInformation)
			{
				var info = reader.GetMethodDebugInformation(handle);
				if (info.SequencePointsBlob.IsNil)
					continue;
				var list = new List<RawSequencePoint>();
				foreach (var sp in info.GetSequencePoints())
				{
					list.Add(new RawSequencePoint(sp.Offset, sp.IsHidden,
						sp.StartLine, sp.StartColumn, sp.EndLine, sp.EndColumn));
				}
				if (list.Count > 0)
				{
					// A MethodDebugInformation row is parallel to its MethodDefinition row.
					result[MetadataTokens.GetRowNumber(handle)] = list;
				}
			}
			return result;
		}

		/// <summary>
		/// Resolves a readable "Namespace.Type.Method" label for each method-definition row,
		/// for use in comparison failure messages.
		/// </summary>
		public static Dictionary<int, string> ReadMethodNames(string peFileName)
		{
			using var peStream = File.OpenRead(peFileName);
			using var peReader = new PEReader(peStream);
			var md = peReader.GetMetadataReader();
			var names = new Dictionary<int, string>();
			foreach (var handle in md.MethodDefinitions)
			{
				var method = md.GetMethodDefinition(handle);
				var type = md.GetTypeDefinition(method.GetDeclaringType());
				string typeName = md.GetString(type.Name);
				if (!type.Namespace.IsNil && md.GetString(type.Namespace) is { Length: > 0 } ns)
					typeName = ns + "." + typeName;
				names[MetadataTokens.GetRowNumber(handle)] = typeName + "." + md.GetString(method.Name);
			}
			return names;
		}

		/// <summary>
		/// Compares the visible breakpoint maps of two PDBs and returns the residual difference as
		/// a deterministic, human-readable report. An empty string means the maps are identical.
		/// </summary>
		/// <param name="expected">PDB used as the oracle (the original compiler's PDB).</param>
		/// <param name="actual">PDB under test (the decompiler's reconstruction).</param>
		/// <param name="methodNames">Row-to-label map from <see cref="ReadMethodNames"/>.</param>
		/// <param name="includeColumns">
		/// When true, breakpoints must match on line <em>and</em> column; when false, only the
		/// line span is compared (use for methods where the decompiler's column placement within a
		/// statement legitimately differs).
		/// </param>
		/// <param name="includeHidden">
		/// When true, hidden sequence points are compared too. A hidden point has no source
		/// location, so it is described by the visible point it follows ("hidden after &lt;loc&gt;",
		/// or "hidden at method start"). Anchoring to a source location keeps the comparison
		/// independent of the exact IL offsets, which the decompiler reconstructs differently.
		/// </param>
		public static string CompareBreakpointMaps(
			Dictionary<int, List<RawSequencePoint>> expected,
			Dictionary<int, List<RawSequencePoint>> actual,
			Dictionary<int, string> methodNames,
			bool includeColumns,
			bool includeHidden)
		{
			var report = new StringBuilder();
			foreach (int row in expected.Keys.Union(actual.Keys).OrderBy(r => r))
			{
				var expectedPoints = Tokens(expected, row, includeColumns, includeHidden);
				var actualPoints = Tokens(actual, row, includeColumns, includeHidden);

				if (expectedPoints.SequenceEqual(actualPoints))
					continue;

				methodNames.TryGetValue(row, out var name);
				report.Append($"0x{0x06000000 | row:x8} {name}").Append('\n');
				foreach (var (prefix, point) in OrderedDifference(expectedPoints, actualPoints))
					report.Append(prefix).Append(point).Append('\n');
			}
			return report.ToString();
		}

		/// <summary>
		/// Projects a method's sequence points onto comparable descriptors: each visible point
		/// becomes its source location; each hidden point (when included) is anchored to the visible
		/// point it follows, so the descriptor does not depend on the reconstructed IL offset.
		/// </summary>
		static List<string> Tokens(Dictionary<int, List<RawSequencePoint>> map, int row, bool includeColumns, bool includeHidden)
		{
			var result = new List<string>();
			if (!map.TryGetValue(row, out var list))
				return result;
			string lastVisible = null;
			foreach (var sp in list)
			{
				if (sp.IsHidden)
				{
					if (includeHidden)
						result.Add(lastVisible == null ? "hidden at method start" : "hidden after " + lastVisible);
				}
				else
				{
					lastVisible = sp.Format(includeColumns);
					result.Add(lastVisible);
				}
			}
			return result;
		}

		static List<(string Prefix, string Point)> OrderedDifference(List<string> expected, List<string> actual)
		{
			int[,] lcs = new int[expected.Count + 1, actual.Count + 1];
			for (int i = expected.Count - 1; i >= 0; i--)
			{
				for (int j = actual.Count - 1; j >= 0; j--)
				{
					lcs[i, j] = expected[i] == actual[j]
						? lcs[i + 1, j + 1] + 1
						: Math.Max(lcs[i + 1, j], lcs[i, j + 1]);
				}
			}

			var result = new List<(string Prefix, string Point)>();
			for (int i = 0, j = 0; i < expected.Count || j < actual.Count;)
			{
				if (i < expected.Count && j < actual.Count && expected[i] == actual[j])
				{
					i++;
					j++;
				}
				else if (j < actual.Count && (i == expected.Count || lcs[i, j + 1] >= lcs[i + 1, j]))
				{
					result.Add(("  + decompiler only: ", actual[j]));
					j++;
				}
				else
				{
					result.Add(("  - compiler only: ", expected[i]));
					i++;
				}
			}
			return result;
		}

		/// <summary>
		/// Checks that the decompiler's own PDB is structurally well-formed, independent of any
		/// oracle: within each method the sequence points' IL offsets must be strictly increasing,
		/// i.e. no duplicate or overlapping sequence points (a duplicate is exactly the failure
		/// that aborts <c>PortablePdbWriter</c> in debug builds). Returns the offending methods, or
		/// an empty string when well-formed.
		/// </summary>
		public static string CheckWellFormed(
			Dictionary<int, List<RawSequencePoint>> pdb,
			Dictionary<int, string> methodNames)
		{
			var report = new StringBuilder();
			foreach (int row in pdb.Keys.OrderBy(r => r))
			{
				var list = pdb[row];
				for (int i = 1; i < list.Count; i++)
				{
					if (list[i].Offset <= list[i - 1].Offset)
					{
						methodNames.TryGetValue(row, out var name);
						report.Append($"0x{0x06000000 | row:x8} {name}: non-increasing IL offset 0x{list[i].Offset:x} after 0x{list[i - 1].Offset:x}").Append('\n');
						break;
					}
				}
			}
			return report.ToString();
		}
	}
}
