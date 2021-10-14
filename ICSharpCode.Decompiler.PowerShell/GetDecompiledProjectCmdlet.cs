using System;
using System.Collections.Concurrent;
using System.IO;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.PowerShell
{
	[Cmdlet(VerbsCommon.Get, "DecompiledProject")]
	[OutputType(typeof(string))]
	public class GetDecompiledProjectCmdlet : PSCmdlet, IProgress<DecompilationProgress>
	{
		[Parameter(Position = 0, Mandatory = true)]
		public CSharpDecompiler Decompiler { get; set; }

		[Parameter(Position = 1, Mandatory = true)]
		[Alias("PSPath", "OutputPath")]
		[ValidateNotNullOrEmpty]
		public string LiteralPath { get; set; }

		readonly object syncObject = new object();
		int completed;
		string fileName;
		ProgressRecord progress;

		public void Report(DecompilationProgress value)
		{
			lock (syncObject)
			{
				completed++;
				progress = new ProgressRecord(1, "Decompiling " + fileName, $"Completed {completed} of {value.TotalNumberOfFiles}: {value.Status}") {
					PercentComplete = (int)(completed * 100.0 / value.TotalNumberOfFiles)
				};
			}
		}

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
				var task = Task.Run(() => DoDecompile(path));
				int timeout = 100;

				// Give the decompiler some time to spin up all threads
				Thread.Sleep(timeout);

				while (!task.IsCompleted)
				{
					ProgressRecord progress;
					lock (syncObject)
					{
						progress = this.progress;
						this.progress = null;
					}
					if (progress != null)
					{
						timeout = 100;
						WriteProgress(progress);
					}
					else
					{
						Thread.Sleep(timeout);
						timeout = Math.Min(1000, timeout * 2);
					}
				}

				task.Wait();

				WriteProgress(new ProgressRecord(1, "Decompiling " + fileName, "Decompilation finished") { RecordType = ProgressRecordType.Completed });
			}
			catch (Exception e)
			{
				WriteVerbose(e.ToString());
				WriteError(new ErrorRecord(e, ErrorIds.DecompilationFailed, ErrorCategory.OperationStopped, null));
			}
		}

		private void DoDecompile(string path)
		{
			PEFile module = Decompiler.TypeSystem.MainModule.PEFile;
			var assemblyResolver = new UniversalAssemblyResolver(module.FileName, false, module.Reader.DetectTargetFrameworkId());
			WholeProjectDecompiler decompiler = new WholeProjectDecompiler(assemblyResolver);
			decompiler.ProgressIndicator = this;
			fileName = module.FileName;
			completed = 0;
			decompiler.DecompileProject(module, path);
		}
	}
}
