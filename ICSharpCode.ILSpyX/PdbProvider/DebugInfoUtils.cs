// Copyright (c) 2018 Siegfried Pammer
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
using System.Buffers.Binary;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;

#nullable enable

namespace ICSharpCode.ILSpyX.PdbProvider
{
	public static class DebugInfoUtils
	{
		internal const int MaxEmbeddedPortablePdbSize = 128 * 1024 * 1024;

		enum PortablePdbOpenResult
		{
			NotFound,
			Opened,
			Rejected
		}

		/// <summary>
		/// Determines whether an embedded Portable PDB declares a decompressed size above the supported limit.
		/// </summary>
		/// <param name="reader">A PE reader with access to the entire image.</param>
		/// <param name="entry">The embedded Portable PDB debug directory entry to inspect.</param>
		/// <returns><see langword="true"/> if the declared decompressed size exceeds the limit; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="NotSupportedException">The reader does not provide access to the entire image.</exception>
		public static bool IsEmbeddedPortablePdbTooLarge(PEReader reader, DebugDirectoryEntry entry)
		{
			if (!reader.IsEntireImageAvailable)
				throw new NotSupportedException("The PE reader must provide access to the entire image.");
			if (entry.DataSize < 8)
				return false;
			try
			{
				int offset = reader.IsLoadedImage ? entry.DataRelativeVirtualAddress : entry.DataPointer;
				var data = reader.GetEntireImage().GetContent(offset, 8);
				return BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan().Slice(4, 4)) > MaxEmbeddedPortablePdbSize;
			}
			catch (ArgumentOutOfRangeException)
			{
				return false;
			}
		}

		public static IDebugInfoProvider? LoadSymbols(PEFile module)
		{
			try
			{
				// try to open portable pdb file/embedded pdb info:
				var result = TryOpenPortablePdb(module, out var provider, out var pdbFileName);
				if (result == PortablePdbOpenResult.Opened)
				{
					return new PortableDebugInfoProvider(module.FileName, provider!, MetadataReaderOptions.Default, pdbFileName);
				}
				if (result == PortablePdbOpenResult.Rejected)
					return null;
				// search for pdb in same directory as dll
				pdbFileName = Path.Combine(
					Path.GetDirectoryName(module.FileName)!,
					Path.GetFileNameWithoutExtension(module.FileName) + ".pdb"
				);
				if (File.Exists(pdbFileName))
				{
					return new MonoCecilDebugInfoProvider(module, pdbFileName);
				}
			}
			catch (Exception ex) when (ex is BadImageFormatException || ex is COMException)
			{
				// Ignore PDB load errors
			}
			return null;
		}

		public static IDebugInfoProvider? FromFile(PEFile module, string pdbFileName)
		{
			if (string.IsNullOrEmpty(pdbFileName))
				return null;

			Stream? stream = OpenStream(pdbFileName);
			if (stream == null)
				return null;

			if (IsWindowsPdb(stream))
			{
				stream.Dispose();
				return new MonoCecilDebugInfoProvider(module, pdbFileName);
			}

			stream.Position = 0;
			MetadataReaderProvider? provider = null;
			try
			{
				provider = MetadataReaderProvider.FromPortablePdbStream(stream);
				var debugInfoProvider = new PortableDebugInfoProvider(
					module.FileName, provider, MetadataReaderOptions.Default, pdbFileName);
				provider = null;
				return debugInfoProvider;
			}
			catch
			{
				provider?.Dispose();
				stream.Dispose();
				throw;
			}
		}

		public static IDebugInfoProvider? FromStream(PEFile module, Stream? stream, string pdbFileName)
		{
			if (stream == null)
				return null;
			if (IsWindowsPdb(stream))
			{
				stream.Dispose();
				return null;
			}
			stream.Position = 0;
			try
			{
				var provider = MetadataReaderProvider.FromPortablePdbStream(stream);
				return new PortableDebugInfoProvider(module.FileName, provider, MetadataReaderOptions.Default, pdbFileName);
			}
			catch (Exception ex) when (ex is BadImageFormatException || ex is IOException || ex is ArgumentException)
			{
				stream.Dispose();
				return null;
			}
		}

		static ReadOnlySpan<byte> LegacyPDBPrefix => "Microsoft C/C++ MSF 7.00"u8;

		/// <summary>
		/// True for a Windows (native MSF) PDB, false for a Portable PDB. Reads from the current
		/// position and leaves the stream positioned after the signature.
		/// </summary>
		public static bool IsWindowsPdb(Stream stream)
		{
			var signature = new byte[LegacyPDBPrefix.Length];
			int bytesRead = 0;
			while (bytesRead < signature.Length)
			{
				int count = stream.Read(signature, bytesRead, signature.Length - bytesRead);
				if (count == 0)
					return false;
				bytesRead += count;
			}
			return signature.AsSpan().SequenceEqual(LegacyPDBPrefix);
		}

		static PortablePdbOpenResult TryOpenPortablePdb(PEFile module,
			out MetadataReaderProvider? provider, out string? pdbFileName)
		{
			provider = null;
			pdbFileName = null;
			var reader = module.Reader;
			var entries = reader.ReadDebugDirectory();
			bool hasPortableCodeView = false;
			// Check all embedded entries before calling TryOpenAssociatedPortablePdb, which may
			// decompress an embedded PDB after checking for a matching external PDB.
			foreach (var entry in entries)
			{
				if (entry.Type == DebugDirectoryEntryType.EmbeddedPortablePdb
					&& IsEmbeddedPortablePdbTooLarge(reader, entry))
					return PortablePdbOpenResult.Rejected;
				hasPortableCodeView |= entry.IsPortableCodeView;
			}
			// Native PDBs have CodeView entries but are not recognized as Portable CodeView.
			// Let LoadSymbols route those sidecars to Mono.Cecil instead of the portable reader.
			if (!hasPortableCodeView)
			{
				foreach (var entry in entries)
				{
					if (entry.Type == DebugDirectoryEntryType.CodeView)
					{
						pdbFileName = Path.Combine(
							Path.GetDirectoryName(module.FileName)!,
							Path.GetFileNameWithoutExtension(module.FileName) + ".pdb");
						Stream? portablePdbStream = OpenStream(pdbFileName);
						if (portablePdbStream != null)
						{
							if (IsWindowsPdb(portablePdbStream))
							{
								portablePdbStream.Dispose();
								return PortablePdbOpenResult.NotFound;
							}
							try
							{
								portablePdbStream.Position = 0;
								provider = MetadataReaderProvider.FromPortablePdbStream(portablePdbStream);
								return PortablePdbOpenResult.Opened;
							}
							catch (Exception ex) when (ex is BadImageFormatException || ex is IOException || ex is ArgumentException)
							{
								portablePdbStream.Dispose();
							}
						}
					}
				}
			}
			// Delegate external PDB matching and the now-validated embedded fallback to SRM.
			return reader.TryOpenAssociatedPortablePdb(module.FileName, OpenStream,
				out provider, out pdbFileName)
				? PortablePdbOpenResult.Opened : PortablePdbOpenResult.NotFound;
		}

		internal static Stream? OpenStream(string fileName)
		{
			if (!File.Exists(fileName))
				return null;
			var memory = new MemoryStream();
			using (var stream = File.OpenRead(fileName))
				stream.CopyTo(memory);
			memory.Position = 0;
			return memory;
		}
	}
}
