using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text;
using ICSharpCode.Decompiler.CSharp;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.PowerShell
{
    [Cmdlet(VerbsCommon.Get, "DecompiledProject")]
    [OutputType(typeof(string))]
    public class GetDecompiledProjectCmdlet : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public CSharpDecompiler Decompiler { get; set; }

        [Parameter(Position = 1, Mandatory = true)]
        [Alias("PSPath", "OutputPath")]
        [ValidateNotNullOrEmpty]
        public string LiteralPath { get; set; }

        protected override void ProcessRecord()
        {
            string path = GetUnresolvedProviderPathFromPSPath(LiteralPath);
            if (!Directory.Exists(path))
            {
                WriteObject("Destination directory must exist prior to decompilation");
                return;
            }

            try
            {
                string assemblyFileName = Decompiler.TypeSystem.Compilation.MainAssembly.UnresolvedAssembly.Location; // just to keep the API "the same" across all cmdlets
                ModuleDefinition module = UniversalAssemblyResolver.LoadMainModule(assemblyFileName);
                WholeProjectDecompiler decompiler = new WholeProjectDecompiler();
                decompiler.DecompileProject(module, path);

                WriteObject("Decompilation finished");
            } catch (Exception e) {
                WriteVerbose(e.ToString());
                WriteError(new ErrorRecord(e, ErrorIds.DecompilationFailed, ErrorCategory.OperationStopped, null));
            }
        }
    }
}
