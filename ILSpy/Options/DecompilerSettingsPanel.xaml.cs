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

using System.ComponentModel.Composition;
using System.Xml.Linq;

using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Interaction logic for DecompilerSettingsPanel.xaml
	/// </summary>
	[ExportOptionPage(Title = nameof(Properties.Resources.Decompiler), Order = 10)]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	internal partial class DecompilerSettingsPanel : IOptionPage
	{
		public DecompilerSettingsPanel()
		{
			InitializeComponent();
		}

		public static Decompiler.DecompilerSettings LoadDecompilerSettings(ILSpySettings settings)
		{
			return ISettingsProvider.LoadDecompilerSettings(settings);
		}

		public void Load(ILSpySettings settings)
		{
			this.DataContext = new DecompilerSettingsViewModel(LoadDecompilerSettings(settings));
		}

		public void Save(XElement root)
		{
			var newSettings = ((DecompilerSettingsViewModel)this.DataContext).ToDecompilerSettings();
			ISettingsProvider.SaveDecompilerSettings(root, newSettings);

			MainWindow.Instance.CurrentDecompilerSettings = newSettings;
			MainWindow.Instance.AssemblyListManager.ApplyWinRTProjections = newSettings.ApplyWindowsRuntimeProjections;
			MainWindow.Instance.AssemblyListManager.UseDebugSymbols = newSettings.UseDebugSymbols;
		}

		public void LoadDefaults()
		{
			MainWindow.Instance.CurrentDecompilerSettings = new();
			this.DataContext = new DecompilerSettingsViewModel(MainWindow.Instance.CurrentDecompilerSettings);
		}
	}
}
