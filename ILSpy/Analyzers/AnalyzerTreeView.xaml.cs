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

using System.ComponentModel.Composition;
using System.Windows.Controls;

using ICSharpCode.ILSpyX.TreeView;

using TomsToolbox.Wpf.Composition.Mef;

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// Interaction logic for AnalyzerTreeView.xaml
	/// </summary>
	[DataTemplate(typeof(AnalyzerTreeViewModel))]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	[Export]
	public partial class AnalyzerTreeView
	{
		public AnalyzerTreeView()
		{
			InitializeComponent();
			ContextMenuProvider.Add(this);
		}

		private void AnalyzerTreeView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (SelectedItem is SharpTreeNode sharpTreeNode)
			{
				FocusNode(sharpTreeNode);
			}
		}
	}
}
