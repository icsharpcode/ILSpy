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

using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpyX.Settings;

using Microsoft.Win32;

namespace ICSharpCode.ILSpy.Options
{
	public class MiscSettingsViewModel : IMiscSettings, INotifyPropertyChanged
	{
		bool allowMultipleInstances;
		bool loadPreviousAssemblies = true;

		public MiscSettingsViewModel(MiscSettings s)
		{
			AllowMultipleInstances = s.AllowMultipleInstances;
			LoadPreviousAssemblies = s.LoadPreviousAssemblies;

			AddRemoveShellIntegrationCommand = new DelegateCommand<object>(AddRemoveShellIntegration);
		}

		/// <summary>
		/// Allow multiple instances.
		/// </summary>
		public bool AllowMultipleInstances {
			get { return allowMultipleInstances; }
			set {
				if (allowMultipleInstances != value)
				{
					allowMultipleInstances = value;
					OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Load assemblies that were loaded in the previous instance
		/// </summary>
		public bool LoadPreviousAssemblies {
			get { return loadPreviousAssemblies; }
			set {
				if (loadPreviousAssemblies != value)
				{
					loadPreviousAssemblies = value;
					OnPropertyChanged();
				}
			}
		}

		public ICommand AddRemoveShellIntegrationCommand { get; }

		const string rootPath = @"Software\Classes\{0}\shell";
		const string fullPath = @"Software\Classes\{0}\shell\Open with ILSpy\command";

		private void AddRemoveShellIntegration(object obj)
		{
			string commandLine = NativeMethods.ArgumentArrayToCommandLine(Path.ChangeExtension(Assembly.GetEntryAssembly().Location, ".exe")) + " \"%L\"";
			if (RegistryEntriesExist())
			{
				if (MessageBox.Show(string.Format(Properties.Resources.RemoveShellIntegrationMessage, commandLine), "ILSpy", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					Registry.CurrentUser.CreateSubKey(string.Format(rootPath, "dllfile")).DeleteSubKeyTree("Open with ILSpy");
					Registry.CurrentUser.CreateSubKey(string.Format(rootPath, "exefile")).DeleteSubKeyTree("Open with ILSpy");
				}
			}
			else
			{
				if (MessageBox.Show(string.Format(Properties.Resources.AddShellIntegrationMessage, commandLine), "ILSpy", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					Registry.CurrentUser.CreateSubKey(string.Format(fullPath, "dllfile"))?
						.SetValue("", commandLine);
					Registry.CurrentUser.CreateSubKey(string.Format(fullPath, "exefile"))?
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

		#region INotifyPropertyChanged Implementation

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
