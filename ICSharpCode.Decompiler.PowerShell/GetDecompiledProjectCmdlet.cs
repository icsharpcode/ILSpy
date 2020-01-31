using System;
using System.Collections.Concurrent;
using System.IO;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.CSharp;
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

		int completed;
		string fileName;
		ConcurrentQueue<ProgressRecord> progress = new ConcurrentQueue<ProgressRecord>();

		public void Report(DecompilationProgress value)
		{
			int current = completed;
			int next = current + 1;
			next = Interlocked.CompareExchange(ref completed, next, current);
			progress.Enqueue(new ProgressRecord(1, "Decompiling " + fileName, $"Completed {next} of {value.TotalNumberOfFiles}: {value.Status}") {
				PercentComplete = (int)(next * 100.0 / value.TotalNumberOfFiles)
			});
		}

		protected override void ProcessRecord()
		{
			string path = GetUnresolvedProviderPathFromPSPath(LiteralPath);
			if (!Directory.Exists(path)) {
				WriteObject("Destination directory must exist prior to decompilation");
				return;
			}

			try {
				var task = Task.Run(() => DoDecompile(path));
				int timeout = 100;

				// Give the decompiler some time to spin up all threads
				Thread.Sleep(timeout);

				while (!task.IsCompleted) {
					if (progress.TryDequeue(out var record)) {
						while (progress.TryDequeue(out var next)) {
							record = next;
						}
						timeout = 100;
						WriteProgress(record);
					} else {
						Thread.Sleep(timeout);
						timeout = Math.Min(1000, timeout * 2);
					}
				}

				task.Wait();

				WriteProgress(new ProgressRecord(1, "Decompiling " + fileName, "Decompilation finished") { RecordType = ProgressRecordType.Completed });
			} catch (Exception e) {
				WriteVerbose(e.ToString());
				WriteError(new ErrorRecord(e, ErrorIds.DecompilationFailed, ErrorCategory.OperationStopped, null));
			}
		}

		private void DoDecompile(string path)
		{
			WholeProjectDecompiler decompiler = new WholeProjectDecompiler();
			PEFile module = Decompiler.TypeSystem.MainModule.PEFile;
			decompiler.AssemblyResolver = new UniversalAssemblyResolver(module.FileName, false, module.Reader.DetectTargetFrameworkId());
			decompiler.ProgressIndicator = this;
			fileName = module.FileName;
			completed = 0;
			decompiler.DecompileProject(module, path);
		}
	}
}
