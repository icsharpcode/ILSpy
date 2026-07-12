// Copyright (c) 2017 Christoph Wille
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
