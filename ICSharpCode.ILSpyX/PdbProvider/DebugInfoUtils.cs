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
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpyX.PdbProvider
{
	public static class DebugInfoUtils
	{
		public static IDebugInfoProvider LoadSymbols(PEFile module)
		{
			try
			{
				// try to open portable pdb file/embedded pdb info:
				if (TryOpenPortablePdb(module, out var provider, out var pdbFileName))
				{
					return new PortableDebugInfoProvider(pdbFileName, provider, module.FileName);
				}
				else
				{
					// search for pdb in same directory as dll
					pdbFileName = Path.Combine(
						Path.GetDirectoryName(module.FileName),
						Path.GetFileNameWithoutExtension(module.FileName) + ".pdb"
					);
					if (File.Exists(pdbFileName))
					{
						return new MonoCecilDebugInfoProvider(module, pdbFileName);
					}
				}
			}
			catch (Exception ex) when (ex is BadImageFormatException || ex is COMException)
			{
				// Ignore PDB load errors
			}
			return null;
		}

		public static IDebugInfoProvider FromFile(PEFile module, string pdbFileName)
		{
			if (string.IsNullOrEmpty(pdbFileName))
				return null;

			Stream stream = OpenStream(pdbFileName);
			if (stream == null)
				return null;

			if (stream.Read(buffer, 0, buffer.Length) == LegacyPDBPrefix.Length
				&& System.Text.Encoding.ASCII.GetString(buffer) == LegacyPDBPrefix)
			{
				stream.Position = 0;
				return new MonoCecilDebugInfoProvider(module, pdbFileName);
			}
			else
			{
				stream.Position = 0;
				var provider = MetadataReaderProvider.FromPortablePdbStream(stream);
				return new PortableDebugInfoProvider(pdbFileName, provider, module.FileName);
			}
		}

		const string LegacyPDBPrefix = "Microsoft C/C++ MSF 7.00";
		static readonly byte[] buffer = new byte[LegacyPDBPrefix.Length];

		static bool TryOpenPortablePdb(PEFile module,
			out MetadataReaderProvider provider, out string pdbFileName)
		{
			provider = null;
			pdbFileName = null;
			var reader = module.Reader;
			foreach (var entry in reader.ReadDebugDirectory())
			{
				if (entry.IsPortableCodeView)
				{
					return reader.TryOpenAssociatedPortablePdb(module.FileName, OpenStream,
						out provider, out pdbFileName);
				}
				if (entry.Type == DebugDirectoryEntryType.CodeView)
				{
					string pdbDirectory = Path.GetDirectoryName(module.FileName);
					pdbFileName = Path.Combine(
						pdbDirectory, Path.GetFileNameWithoutExtension(module.FileName) + ".pdb");
					if (File.Exists(pdbFileName))
					{
						Stream stream = OpenStream(pdbFileName);
						if (stream.Read(buffer, 0, buffer.Length) == LegacyPDBPrefix.Length
							&& System.Text.Encoding.ASCII.GetString(buffer) == LegacyPDBPrefix)
						{
							return false;
						}
						stream.Position = 0;
						provider = MetadataReaderProvider.FromPortablePdbStream(stream);
						return true;
					}
				}
			}
			return false;
		}

		static Stream OpenStream(string fileName)
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
