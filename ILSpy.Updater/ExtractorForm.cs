using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			// Method intentionally left empty.
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
				ZipArchive zar = ZipFile.OpenRead(Application.StartupPath + "\\ILSpy_Update.zip");
				/*
					Not Sure how I can get it to fill the Progress bar. I think I would need some help here on this part.
					Maybe this is why 4.5 was not good enough before.
				*/
				zar.ExtractToDirectory(Application.StartupPath);
				zar.Dispose();
				this.Close();
			}
			else if (result == 2)
			{
				/*
					File is missing so that means the File was either not Downloaded of got removed for some reason before this check.
					I like to check everything unlike most developers just to save time and to be sure that IT DOES in fact exist.
				*/
				MessageBox.Show("Error while Attempting to Extract the Update. The Update file does not exist.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.Close();
			}
		}
	}
}
