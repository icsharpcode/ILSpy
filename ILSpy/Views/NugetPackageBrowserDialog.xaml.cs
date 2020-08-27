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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

using ICSharpCode.ILSpy.Controls;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Interaction logic for NugetPackageBrowserDialog.xaml
	/// </summary>
	public partial class NugetPackageBrowserDialog : Window, INotifyPropertyChanged
	{
		public LoadedNugetPackage Package { get; }

		public NugetPackageBrowserDialog()
		{
			InitializeComponent();
		}

		public NugetPackageBrowserDialog(LoadedNugetPackage package)
		{
			InitializeComponent();
			this.Package = package;
			this.Package.PropertyChanged += Package_PropertyChanged;
			DataContext = this;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		void Package_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Package.SelectedEntries))
			{
				OnPropertyChanged(new PropertyChangedEventArgs("HasSelection"));
			}
		}

		void OKButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			Close();
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		public Entry[] SelectedItems {
			get {
				return Package.SelectedEntries.ToArray();
			}
		}

		public bool HasSelection {
			get { return SelectedItems.Length > 0; }
		}
	}
}