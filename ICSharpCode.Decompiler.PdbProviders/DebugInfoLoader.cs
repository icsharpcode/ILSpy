using System;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.PdbProvider;
using ICSharpCode.Decompiler.PdbProvider.Cecil;

namespace ICSharpCode.Decompiler.PdbProviders
{
	public class DebugInfoLoader
	{

		public static IDebugInfoProvider LoadSymbols(PEFile module)
		{
			try
			{
				var reader = module.Reader;
				// try to open portable pdb file/embedded pdb info:
				if (TryOpenPortablePdb(module, out var provider, out var pdbFileName))
				{
					return new PortableDebugInfoProvider(pdbFileName, provider);
				}
				else
				{
					// search for pdb in same directory as dll
					string pdbDirectory = Path.GetDirectoryName(module.FileName);
					pdbFileName = Path.Combine(pdbDirectory, Path.GetFileNameWithoutExtension(module.FileName) + ".pdb");
					if (File.Exists(pdbFileName))
					{
						return new MonoCecilDebugInfoProvider(module, pdbFileName);
					}

					// TODO: use symbol cache, get symbols from microsoft
				}
			}
			catch (Exception ex) when (ex is BadImageFormatException || ex is COMException)
			{
				// Ignore PDB load errors
			}
			return null;
		}

		const string LegacyPDBPrefix = "Microsoft C/C++ MSF 7.00";
		

		static private bool TryOpenPortablePdb(PEFile module, out MetadataReaderProvider provider, out string pdbFileName)
		{
			provider = null;
			pdbFileName = null;
			var reader = module.Reader;
			foreach (var entry in reader.ReadDebugDirectory())
			{
				if (entry.IsPortableCodeView)
				{
					return reader.TryOpenAssociatedPortablePdb(module.FileName, OpenStream, out provider, out pdbFileName);
				}
				if (entry.Type == DebugDirectoryEntryType.CodeView)
				{
					string pdbDirectory = Path.GetDirectoryName(module.FileName);
					pdbFileName = Path.Combine(pdbDirectory, Path.GetFileNameWithoutExtension(module.FileName) + ".pdb");
					if (File.Exists(pdbFileName))
					{
						byte[] buffer = new byte[LegacyPDBPrefix.Length];
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

		private static Stream OpenStream(string fileName)
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