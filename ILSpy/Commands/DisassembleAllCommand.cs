// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

#if DEBUG

using System;
using System.Collections.Concurrent;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy
{
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.DEBUGDisassemble), MenuCategory = nameof(Resources.Open), MenuOrder = 2.5)]
	[Shared]
	sealed class DisassembleAllCommand(AssemblyTreeModel assemblyTreeModel, DockWorkspace dockWorkspace) : SimpleCommand
	{
		public override bool CanExecute(object parameter)
		{
			return System.IO.Directory.Exists("c:\\temp\\disassembled");
		}

		public override void Execute(object parameter)
		{
			dockWorkspace.RunWithCancellation(ct => Task<AvalonEditTextOutput>.Factory.StartNew(() => {
				AvalonEditTextOutput output = new();
				Parallel.ForEach(
					Partitioner.Create(assemblyTreeModel.AssemblyList.GetAssemblies(), loadBalance: true),
					new() { MaxDegreeOfParallelism = Environment.ProcessorCount, CancellationToken = ct },
					asm => {
						if (!asm.HasLoadError)
						{
							Stopwatch w = Stopwatch.StartNew();
							Exception exception = null;
							using (var writer = new System.IO.StreamWriter("c:\\temp\\disassembled\\" + asm.Text.Replace("(", "").Replace(")", "").Replace(' ', '_') + ".il"))
							{
								try
								{
									var options = dockWorkspace.ActiveTabPage.CreateDecompilationOptions();
									options.FullDecompilation = true;
									options.CancellationToken = ct;
									new ILLanguage(dockWorkspace).DecompileAssembly(asm, new Decompiler.PlainTextOutput(writer), options);
								}
								catch (Exception ex)
								{
									writer.WriteLine(ex.ToString());
									exception = ex;
								}
							}
							lock (output)
							{
								output.Write(asm.ShortName + " - " + w.Elapsed);
								if (exception != null)
								{
									output.Write(" - ");
									output.Write(exception.GetType().Name);
								}
								output.WriteLine();
							}
						}
					});
				return output;
			}, ct)).Then(dockWorkspace.ShowText).HandleExceptions();
		}
	}
}

#endif