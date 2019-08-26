using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.PdbProvider.Cecil;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.PowerShell
{
	[Cmdlet(VerbsCommon.Get, "Decompiler")]
	[OutputType(typeof(CSharpDecompiler))]
	public class GetDecompilerCmdlet : PSCmdlet
	{
		[Parameter(Position = 0, Mandatory = true, HelpMessage = "Path to the assembly you want to decompile")]
		[Alias("PSPath")]
		[ValidateNotNullOrEmpty]
		public string LiteralPath { get; set; }

		protected override void ProcessRecord()
		{
			string path = GetUnresolvedProviderPathFromPSPath(LiteralPath);

			try
			{
				var decompiler = GetCSharpDecompiler(path, new DecompilerSettings()
				{
					ThrowOnAssemblyResolveErrors = false,
					UseDebugSymbols = true,
					ShowDebugInfo = true,
					RemoveDeadCode = true
				});
				WriteObject(decompiler);

			}
			catch (Exception e)
			{
				WriteVerbose(e.ToString());
				WriteError(new ErrorRecord(e, ErrorIds.AssemblyLoadFailed, ErrorCategory.OperationStopped, null));
			}
		}

		/// <summary>
		/// Instianciates a CSharpDecompiler from a given assemblyfilename and applies decompilersettings
		/// </summary>
		/// <param name="assemblyFileName">path for assemblyfile to decompile</param>
		/// <param name="settings">optional decompiler settings</param>
		/// <returns>Instance of CSharpDecompiler</returns>
		public CSharpDecompiler GetCSharpDecompiler(string assemblyFileName, DecompilerSettings settings = null)
		{
			if (settings == null)
				settings = new DecompilerSettings();
			using (var file = new FileStream(assemblyFileName, FileMode.Open, FileAccess.Read))
			{
				var module = new PEFile(assemblyFileName, file, PEStreamOptions.PrefetchEntireImage);
				var resolver = new UniversalAssemblyResolver(assemblyFileName, false,
					module.Reader.DetectTargetFrameworkId(), PEStreamOptions.PrefetchMetadata);
				resolver.AddSearchDirectory(Path.GetDirectoryName(module.FullName));
				var typeSystem = new DecompilerTypeSystem(module, resolver, settings);
				var decompiler = new CSharpDecompiler(typeSystem, settings);
				MetadataReaderProvider provider;
				string pdbFileName;
				if (TryOpenPortablePdb(module, out provider, out pdbFileName))
				{
					decompiler.DebugInfoProvider = new PortableDebugInfoProvider(pdbFileName, provider);
				}
				else
				{
					// search for pdb in same directory as dll
					string pdbDirectory = Path.GetDirectoryName(module.FileName);
					pdbFileName = Path.Combine(pdbDirectory, Path.GetFileNameWithoutExtension(module.FileName) + ".pdb");
					if (File.Exists(pdbFileName))
					{
						decompiler.DebugInfoProvider = new MonoCecilDebugInfoProvider(module, pdbFileName);
					}
				}

				return decompiler;
			}
		}

		bool TryOpenPortablePdb(PEFile module, out MetadataReaderProvider provider, out string pdbFileName)
		{
			const string LegacyPDBPrefix = "Microsoft C/C++ MSF 7.00";
			byte[] buffer = new byte[LegacyPDBPrefix.Length];

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

		Stream OpenStream(string fileName)
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

	class PortableDebugInfoProvider : IDebugInfoProvider
	{
		string pdbFileName;
		MetadataReaderProvider provider;

		public PortableDebugInfoProvider(string pdbFileName, MetadataReaderProvider provider)
		{
			this.pdbFileName = pdbFileName;
			this.provider = provider;
		}

		public string Description => pdbFileName == null ? "Embedded in this assembly" : $"Loaded from portable PDB: {pdbFileName}";

		public IList<Decompiler.DebugInfo.SequencePoint> GetSequencePoints(MethodDefinitionHandle method)
		{
			var metadata = provider.GetMetadataReader();
			var debugInfo = metadata.GetMethodDebugInformation(method);
			var sequencePoints = new List<Decompiler.DebugInfo.SequencePoint>();

			foreach (var point in debugInfo.GetSequencePoints())
			{
				string documentFileName;

				if (!point.Document.IsNil)
				{
					var document = metadata.GetDocument(point.Document);
					documentFileName = metadata.GetString(document.Name);
				}
				else
				{
					documentFileName = "";
				}

				sequencePoints.Add(new Decompiler.DebugInfo.SequencePoint()
				{
					Offset = point.Offset,
					StartLine = point.StartLine,
					StartColumn = point.StartColumn,
					EndLine = point.EndLine,
					EndColumn = point.EndColumn,
					DocumentUrl = documentFileName
				});
			}

			return sequencePoints;
		}

		public IList<Variable> GetVariables(MethodDefinitionHandle method)
		{
			var metadata = provider.GetMetadataReader();
			var variables = new List<Variable>();

			foreach (var h in metadata.GetLocalScopes(method))
			{
				var scope = metadata.GetLocalScope(h);
				foreach (var v in scope.GetLocalVariables())
				{
					var var = metadata.GetLocalVariable(v);
					variables.Add(new Variable(var.Index, metadata.GetString(var.Name)));
				}
			}

			return variables;
		}

		public bool TryGetName(MethodDefinitionHandle method, int index, out string name)
		{
			var metadata = provider.GetMetadataReader();
			name = null;

			foreach (var h in metadata.GetLocalScopes(method))
			{
				var scope = metadata.GetLocalScope(h);
				foreach (var v in scope.GetLocalVariables())
				{
					var var = metadata.GetLocalVariable(v);
					if (var.Index == index)
					{
						name = metadata.GetString(var.Name);
						return true;
					}
				}
			}
			return false;
		}
	}

}
