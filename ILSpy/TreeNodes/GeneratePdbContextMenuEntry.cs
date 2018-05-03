using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Pdb;
using ICSharpCode.ILSpy.TextView;
using Microsoft.Win32;

namespace ICSharpCode.ILSpy.TreeNodes
{
	[ExportContextMenuEntry(Header = "Generate portable PDB")]
	class GeneratePdbContextMenuEntry : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			var language = MainWindow.Instance.CurrentLanguage;
			var assembly = (context.SelectedTreeNodes?.FirstOrDefault() as AssemblyTreeNode)?.LoadedAssembly;
			if (assembly == null) return;
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.FileName = DecompilerTextView.CleanUpName(assembly.ShortName) + ".pdb";
			dlg.Filter = "Portable PDB|*.pdb|All files|*.*";
			if (dlg.ShowDialog() != true) return;
			DecompilationOptions options = new DecompilationOptions();
			string fileName = dlg.FileName;
			MainWindow.Instance.TextView.RunWithCancellation(ct => Task<AvalonEditTextOutput>.Factory.StartNew(() => {
				AvalonEditTextOutput output = new AvalonEditTextOutput();
				Stopwatch stopwatch = Stopwatch.StartNew();
				using (FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write)) {
					try {
						var file = assembly.GetPEFileOrNull();
						var decompiler = new CSharpDecompiler(file, options.DecompilerSettings);
						PortablePdbWriter.WritePdb(file, decompiler, options.DecompilerSettings, stream);
					} catch (OperationCanceledException) {
						output.WriteLine();
						output.WriteLine("Generation was cancelled.");
						throw;
					}
				}
				stopwatch.Stop();
				output.WriteLine("Generation complete in " + stopwatch.Elapsed.TotalSeconds.ToString("F1") + " seconds.");
				output.WriteLine();
				output.AddButton(null, "Open Explorer", delegate { Process.Start("explorer", "/select,\"" + fileName + "\""); });
				output.WriteLine();
				return output;
			}, ct)).Then(output => MainWindow.Instance.TextView.ShowText(output)).HandleExceptions();
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context)
		{
			return context.SelectedTreeNodes?.Length == 1
				&& context.SelectedTreeNodes?.FirstOrDefault() is AssemblyTreeNode tn 
				&& !tn.LoadedAssembly.HasLoadError;
		}
	}
}
