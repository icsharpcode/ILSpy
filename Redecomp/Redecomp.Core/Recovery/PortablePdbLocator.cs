// Copyright (c) 2026 Franck
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

using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace Redecomp.Core.Recovery;

/// <summary>An opened portable PDB.</summary>
/// <param name="Provider">Reader provider over the PDB metadata.</param>
/// <param name="FilePath">Path of the PDB file, or null when the PDB is embedded in the assembly.</param>
public sealed record PortablePdb(MetadataReaderProvider Provider, string? FilePath) : IDisposable
{
	public void Dispose() => Provider.Dispose();
}

/// <summary>Finds the portable PDB of an assembly: embedded in it, or as a file referenced by or next to it.</summary>
public static class PortablePdbLocator
{
	/// <summary>Opens the PDB, or returns null when the assembly has no portable PDB.</summary>
	public static PortablePdb? Open(string assemblyPath)
	{
		using var stream = File.OpenRead(assemblyPath);
		using var pe = new PEReader(stream);
		foreach (var entry in pe.ReadDebugDirectory())
		{
			if (entry.Type == DebugDirectoryEntryType.EmbeddedPortablePdb)
				return new PortablePdb(pe.ReadEmbeddedPortablePdbDebugDirectoryData(entry), null);
		}
		foreach (var entry in pe.ReadDebugDirectory())
		{
			if (!entry.IsPortableCodeView)
				continue;
			string recordedPath = pe.ReadCodeViewDebugDirectoryData(entry).Path;
			foreach (string candidate in Candidates(assemblyPath, recordedPath))
			{
				if (File.Exists(candidate) && IsPortablePdb(candidate))
					return new PortablePdb(MetadataReaderProvider.FromPortablePdbStream(File.OpenRead(candidate)), Path.GetFullPath(candidate));
			}
		}
		return null;
	}

	static IEnumerable<string> Candidates(string assemblyPath, string recordedPath)
	{
		string directory = Path.GetDirectoryName(Path.GetFullPath(assemblyPath))!;
		yield return Path.Combine(directory, Path.GetFileName(recordedPath.Replace('\\', '/')));
		yield return Path.ChangeExtension(assemblyPath, ".pdb");
		yield return recordedPath;
	}

	static bool IsPortablePdb(string path)
	{
		using var stream = File.OpenRead(path);
		Span<byte> signature = stackalloc byte[4];
		// Portable PDBs are ECMA-335 metadata blobs starting with the "BSJB" signature.
		return stream.Read(signature) == 4 && signature.SequenceEqual("BSJB"u8);
	}
}
