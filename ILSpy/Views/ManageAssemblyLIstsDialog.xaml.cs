﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using ICSharpCode.ILSpy.Themes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Interaction logic for ManageAssemblyListsDialog.xaml
	/// </summary>
	public partial class ManageAssemblyListsDialog : Window
	{
		public ManageAssemblyListsDialog()
		{
			InitializeComponent();
			DataContext = new ManageAssemblyListsViewModel(this);
		}

		private void PreconfiguredAssemblyListsMenuClick(object sender, RoutedEventArgs e)
		{
			var menu = (ContextMenu)Resources["PreconfiguredAssemblyListsMenu"];
			menu.PlacementTarget = (Button)sender;
			menu.Placement = PlacementMode.Bottom;
			menu.IsOpen = true;
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			ThemeManager.Current.ApplyThemeForWindow(this);
		}
	}
}
