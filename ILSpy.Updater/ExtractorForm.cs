using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ICSharpCode.ILSpy.Updater
{
	public partial class ExtractorForm : Form
	{
		public ExtractorForm()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			ExtractUpdate();
		}

		public int GetIfUpdateFileExists(String path)
		{
			if (File.Exists(path + "\\ILSpy_Update.zip"))
			{
				return 1;
			}
			else
			{
				return 2;
			}
		}

		private void ExtractUpdate()
		{
			int result = GetIfUpdateFileExists(Application.StartupPath);
			if (result == 1)
			{
				/*	Deleting files not Required first for Ionic.Zip unlike System.IO.Compression because of Overwrites.
					File.Delete(Application.StartupPath + "\\ILSpy.exe");
					File.Delete(Application.StartupPath + "\\ICSharpCode.AvalonEdit.dll");
					File.Delete(Application.StartupPath + "\\ILSpy.exe.config");
					File.Delete(Application.StartupPath + "\\ICSharpCode.Decompiler.dll");
					File.Delete(Application.StartupPath + "\\ILSpy.BamlDecompiler.Plugin.dll");
					File.Delete(Application.StartupPath + "\\ICSharpCode.NRefactory.CSharp.dll");
					File.Delete(Application.StartupPath + "\\ICSharpCode.NRefactory.dll");
					File.Delete(Application.StartupPath + "\\ICSharpCode.NRefactory.VB.dll");
					File.Delete(Application.StartupPath + "\\ICSharpCode.TreeView.dll");
					File.Delete(Application.StartupPath + "\\Mono.Cecil.dll");
					File.Delete(Application.StartupPath + "\\Mono.Cecil.Pdb.dll");
				*/
				ReadAndExtract(Application.StartupPath + "\\ILSpy_Update.zip", Application.StartupPath);
			}
			else if (result == 2)
			{
				MessageBox.Show("Error while Attempting to Extract the Update. The Update file does not exist.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.Close();
			}
		}

		int fileCount = 0;
		long totalSize = 0, total = 0, lastVal = 0, sum = 0;

		public void ReadAndExtract(string openPath, string savePath)
		{
			try
			{
				fileCount = 0;
				Ionic.Zip.ZipFile zar = new Ionic.Zip.ZipFile();
				zar = Ionic.Zip.ZipFile.Read(openPath);
				foreach (var entry in zar)
				{
					fileCount++;
					totalSize += entry.UncompressedSize;
				}
				progressBar1.Maximum = (Int32)totalSize;
				zar.ExtractProgress += new EventHandler<Ionic.Zip.ExtractProgressEventArgs>(zar_ExtractProgress);
				zar.ExtractAll(savePath, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.Close();
			}
		}

		void zar_ExtractProgress(object sender, Ionic.Zip.ExtractProgressEventArgs e)
		{
			Application.DoEvents();
			if (total != e.TotalBytesToTransfer)
			{
				sum += total - lastVal + e.BytesTransferred;
				total = e.TotalBytesToTransfer;
			}
			else
				sum += e.BytesTransferred - lastVal;
			lastVal = e.BytesTransferred;
			progressBar1.Value = (Int32)sum;
			if (progressBar1.Value == total)
			{
				RestartILSpy();
			}
		}

		private void RestartILSpy()
		{
			Process.Start(Application.StartupPath + "\\ILSpy.exe");
			this.Close();
		}
	}
}
