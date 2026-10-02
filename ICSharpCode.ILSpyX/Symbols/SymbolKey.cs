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
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;

#nullable enable

namespace ICSharpCode.ILSpyX.Symbols
{
	public enum SymbolFileKind
	{
		PortablePdb,
		WindowsPdb,
		PE,
	}

	/// <summary>
	/// A symbol-server lookup key in the SSQP / SymSrv layout <c>name/id/name</c>
	/// (dotnet/symstore docs/specs/SSQP_Key_Conventions.md). Keys are lower-cased; symbol servers
	/// compare them case-insensitively.
	/// </summary>
	public sealed record SymbolKey(string Key, string FileName, SymbolFileKind Kind, Guid Guid,
		uint AgeOrStamp, IReadOnlyList<string> Checksums)
	{
		/// <summary>
		/// Value of the <c>SymbolChecksum</c> request header (required by symbols.nuget.org), or
		/// <c>null</c> when the PE carries no PDB checksum entries.
		/// </summary>
		public string? ChecksumHeader => Checksums.Count == 0 ? null : string.Join(";", Checksums);

		public static string ForPortablePdb(string pdbFileName, Guid guid)
			=> Build(pdbFileName, guid.ToString("N") + "FFFFFFFF");

		public static string ForWindowsPdb(string pdbFileName, Guid guid, uint age)
			=> Build(pdbFileName, guid.ToString("N") + age.ToString("x", CultureInfo.InvariantCulture));

		public static string ForPE(string fileName, uint timeStamp, uint sizeOfImage)
			=> Build(fileName, timeStamp.ToString("x8", CultureInfo.InvariantCulture)
				+ sizeOfImage.ToString("x", CultureInfo.InvariantCulture));

		static string Build(string fileName, string id)
		{
			string name = GetFileName(fileName).ToLowerInvariant();
			return name + "/" + id.ToLowerInvariant() + "/" + name;
		}

		/// <summary>
		/// Returns the last path segment, splitting on both separators: the CodeView path is the
		/// build machine's path and may use either convention regardless of the current OS.
		/// </summary>
		public static string GetFileName(string path)
		{
			int index = path.LastIndexOfAny(new[] { '/', '\\' });
			return index < 0 ? path : path.Substring(index + 1);
		}

		/// <summary>
		/// Returns the keys of the PDBs a PE file references through its CodeView debug-directory
		/// entries, in directory order.
		/// </summary>
		public static IReadOnlyList<SymbolKey> GetPdbKeys(PEReader reader)
		{
			var entries = reader.ReadDebugDirectory();
			var checksums = entries
				.Where(e => e.Type == DebugDirectoryEntryType.PdbChecksum)
				.Select(e => reader.ReadPdbChecksumDebugDirectoryData(e))
				.Select(c => c.AlgorithmName + ":" + Convert.ToHexString(c.Checksum.AsSpan()).ToLowerInvariant())
				.ToList();
			var keys = new List<SymbolKey>();
			foreach (var entry in entries)
			{
				if (entry.Type != DebugDirectoryEntryType.CodeView)
					continue;
				var data = reader.ReadCodeViewDebugDirectoryData(entry);
				string pdbFileName = GetFileName(data.Path);
				if (pdbFileName.Length == 0)
					continue;
				if (entry.IsPortableCodeView)
				{
					keys.Add(new SymbolKey(ForPortablePdb(pdbFileName, data.Guid), pdbFileName,
						SymbolFileKind.PortablePdb, data.Guid, entry.Stamp, checksums));
				}
				else
				{
					keys.Add(new SymbolKey(ForWindowsPdb(pdbFileName, data.Guid, (uint)data.Age), pdbFileName,
						SymbolFileKind.WindowsPdb, data.Guid, (uint)data.Age, Array.Empty<string>()));
				}
			}
			return keys;
		}

		/// <summary>
		/// Returns the key under which a symbol server stores the PE file itself (used by debuggers
		/// for dump analysis).
		/// </summary>
		public static SymbolKey? GetPEKey(PEReader reader, string fileName)
		{
			var headers = reader.PEHeaders;
			if (headers.PEHeader == null)
				return null;
			string name = GetFileName(fileName);
			uint timeStamp = unchecked((uint)headers.CoffHeader.TimeDateStamp);
			return new SymbolKey(ForPE(name, timeStamp, unchecked((uint)headers.PEHeader.SizeOfImage)), name,
				SymbolFileKind.PE, Guid.Empty, timeStamp, Array.Empty<string>());
		}
	}
}
