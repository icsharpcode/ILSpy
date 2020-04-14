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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using ICSharpCode.ILSpy.Commands;
using Microsoft.Win32;

namespace ICSharpCode.ILSpy.Options
{
	public class MiscSettings : INotifyPropertyChanged
	{
		bool allowMultipleInstances;
		bool loadPreviousAssemblies = true;

		public MiscSettings()
		{
			AddRemoveShellIntegrationCommand = new DelegateCommand<object>(AddRemoveShellIntegration);
		}

		/// <summary>
		/// Allow multiple instances.
		/// </summary>
		public bool AllowMultipleInstances
		{
			get { return allowMultipleInstances; }
			set {
				if (allowMultipleInstances != value) {
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
				if (loadPreviousAssemblies != value) {
					loadPreviousAssemblies = value;
					OnPropertyChanged();
				}
			}
		}

		public ICommand AddRemoveShellIntegrationCommand { get; }

		private void AddRemoveShellIntegration(object obj)
		{
			if (!IsElevated()) {
				MessageBox.Show(Properties.Resources.RestartElevatedMessage, "ILSpy", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			string commandLine = NativeMethods.ArgumentArrayToCommandLine(Assembly.GetEntryAssembly().Location, "%L");
			if (RegistryEntriesExist()) {
				if (MessageBox.Show(string.Format(Properties.Resources.RemoveShellIntegrationMessage, commandLine), "ILSpy", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
					Registry.ClassesRoot.CreateSubKey(@"dllfile\shell").DeleteSubKeyTree("Open with ILSpy");
					Registry.ClassesRoot.CreateSubKey(@"exefile\shell").DeleteSubKeyTree("Open with ILSpy");
				}
			} else {
				if (MessageBox.Show(string.Format(Properties.Resources.AddShellIntegrationMessage, commandLine), "ILSpy", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
					Registry.ClassesRoot.CreateSubKey(@"dllfile\shell\Open with ILSpy\command")?
						.SetValue("", commandLine);
					Registry.ClassesRoot.CreateSubKey(@"exefile\shell\Open with ILSpy\command")?
						.SetValue("", commandLine);
				}
			}
			OnPropertyChanged(nameof(AddRemoveShellIntegrationText));

			bool IsElevated()
			{
				try {
					return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
				} catch (System.Security.SecurityException) {
					return false;
				}
			}
		}

		private static bool RegistryEntriesExist()
		{
			return Registry.ClassesRoot.OpenSubKey(@"dllfile\shell\Open with ILSpy\command") != null
				&& Registry.ClassesRoot.OpenSubKey(@"exefile\shell\Open with ILSpy\command") != null;
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
