// Copyright (c) 2017 AlphaSierraPapa for the SharpDevelop Team
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

using System.Composition;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpyX.Settings;

using Microsoft.Win32;

using TomsToolbox.Wpf;

namespace ICSharpCode.ILSpy.Options
{
	[ExportOptionPage(Order = 30)]
	[NonShared]
	public class MiscSettingsViewModel : ObservableObject, IOptionPage
	{
		private MiscSettings settings;
		public MiscSettings Settings {
			get => settings;
			set => SetProperty(ref settings, value);
		}

		public ICommand AddRemoveShellIntegrationCommand => new DelegateCommand(() => AppEnvironment.IsWindows, AddRemoveShellIntegration);

		const string rootPath = @"Software\Classes\{0}\shell";
		const string fullPath = @"Software\Classes\{0}\shell\Open with ILSpy\command";

		private void AddRemoveShellIntegration()
		{
			string commandLine = CommandLineTools.ArgumentArrayToCommandLine(Path.ChangeExtension(Assembly.GetEntryAssembly()?.Location, ".exe")) + " \"%L\"";
			if (RegistryEntriesExist())
			{
				if (MessageBox.Show(string.Format(Properties.Resources.RemoveShellIntegrationMessage, commandLine), "ILSpy", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					Registry.CurrentUser
						.CreateSubKey(string.Format(rootPath, "dllfile"))?
						.DeleteSubKeyTree("Open with ILSpy");
					Registry.CurrentUser
						.CreateSubKey(string.Format(rootPath, "exefile"))?
						.DeleteSubKeyTree("Open with ILSpy");
				}
			}
			else
			{
				if (MessageBox.Show(string.Format(Properties.Resources.AddShellIntegrationMessage, commandLine), "ILSpy", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					Registry.CurrentUser
						.CreateSubKey(string.Format(fullPath, "dllfile"))?
						.SetValue("", commandLine);
					Registry.CurrentUser
						.CreateSubKey(string.Format(fullPath, "exefile"))?
						.SetValue("", commandLine);
				}
			}
			OnPropertyChanged(nameof(AddRemoveShellIntegrationText));
		}

		private static bool RegistryEntriesExist()
		{
			return Registry.CurrentUser.OpenSubKey(string.Format(fullPath, "dllfile")) != null
				&& Registry.CurrentUser.OpenSubKey(string.Format(fullPath, "exefile")) != null;
		}

		public string AddRemoveShellIntegrationText {
			get {
				return RegistryEntriesExist() ? Properties.Resources.RemoveShellIntegration : Properties.Resources.AddShellIntegration;
			}
		}

		public string Title => Properties.Resources.Misc;

		public void Load(SettingsSnapshot settings)
		{
			Settings = settings.GetSettings<MiscSettings>();
		}

		public void LoadDefaults()
		{
			Settings.LoadFromXml(new XElement("dummy"));
		}
	}
}
