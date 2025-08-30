using System.Management.Automation;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.PowerShell
{
	[Cmdlet(VerbsCommon.Get, "TargetFramework")]
	[OutputType(typeof(string))]
	public class GetTargetFramework : PSCmdlet
	{
		[Parameter(Position = 0, Mandatory = true)]
		public CSharpDecompiler Decompiler { get; set; }

		protected override void ProcessRecord()
		{
			MetadataFile module = Decompiler.TypeSystem.MainModule.MetadataFile;
			WriteObject(module.Metadata.DetectTargetFrameworkId());
		}
	}
}
