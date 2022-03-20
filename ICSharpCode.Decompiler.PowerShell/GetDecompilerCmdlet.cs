using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection.PortableExecutable;
using System.Text;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX.PdbProvider;

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

		[Parameter(HelpMessage = "C# Language version to be used by the decompiler")]
		public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Latest;

		[Parameter(HelpMessage = "Remove dead stores")]
		public bool RemoveDeadStores { get; set; }

		[Parameter(HelpMessage = "Remove dead code")]
		public bool RemoveDeadCode { get; set; }

		[Parameter(HelpMessage = "Use PDB")]
		public string PDBFilePath { get; set; }

		protected override void ProcessRecord()
		{
			string path = GetUnresolvedProviderPathFromPSPath(LiteralPath);

			try
			{
				var module = new PEFile(LiteralPath, new FileStream(LiteralPath, FileMode.Open, FileAccess.Read), PEStreamOptions.Default);
				var debugInfo = DebugInfoUtils.FromFile(module, PDBFilePath);
				var decompiler = new CSharpDecompiler(path, new DecompilerSettings(LanguageVersion) {
					ThrowOnAssemblyResolveErrors = false,
					RemoveDeadCode = RemoveDeadCode,
					RemoveDeadStores = RemoveDeadStores,
					UseDebugSymbols = debugInfo != null,
					ShowDebugInfo = debugInfo != null,
				});
				decompiler.DebugInfoProvider = debugInfo;
				WriteObject(decompiler);
			}
			catch (Exception e)
			{
				WriteVerbose(e.ToString());
				WriteError(new ErrorRecord(e, ErrorIds.AssemblyLoadFailed, ErrorCategory.OperationStopped, null));
			}
		}
	}
}
