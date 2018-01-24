using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using ICSharpCode.Decompiler.CSharp;

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

            try {
                var decompiler = new CSharpDecompiler(path, new DecompilerSettings() {
					ThrowOnAssemblyResolveErrors = false
                });
                WriteObject(decompiler);

            } catch (Exception e) {
                WriteVerbose(e.ToString());
                WriteError(new ErrorRecord(e, ErrorIds.AssemblyLoadFailed, ErrorCategory.OperationStopped, null));
            }
        }
    }
}
