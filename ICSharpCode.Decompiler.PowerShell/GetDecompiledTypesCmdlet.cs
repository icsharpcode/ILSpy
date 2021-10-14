using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.PowerShell
{
	[Cmdlet(VerbsCommon.Get, "DecompiledTypes")]
	[OutputType(typeof(ITypeDefinition[]))]
	public class GetDecompiledTypesCmdlet : PSCmdlet
	{
		[Parameter(Position = 0, Mandatory = true)]
		public CSharpDecompiler Decompiler { get; set; }

		[Parameter(Mandatory = true)]
		public string[] Types { get; set; }

		protected override void ProcessRecord()
		{
			HashSet<TypeKind> kinds = TypesParser.ParseSelection(Types);

			try
			{
				List<ITypeDefinition> output = new List<ITypeDefinition>();
				foreach (var type in Decompiler.TypeSystem.MainModule.TypeDefinitions)
				{
					if (!kinds.Contains(type.Kind))
						continue;
					output.Add(type);
				}

				WriteObject(output.ToArray());
			}
			catch (Exception e)
			{
				WriteVerbose(e.ToString());
				WriteError(new ErrorRecord(e, ErrorIds.DecompilationFailed, ErrorCategory.OperationStopped, null));
			}
		}
	}
}
