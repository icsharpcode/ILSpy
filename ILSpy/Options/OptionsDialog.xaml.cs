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
using System.ComponentModel.Composition;
using System.Xml.Linq;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Interaction logic for OptionsDialog.xaml
	/// </summary>
	public sealed partial class OptionsDialog
	{
		public OptionsDialog()
		{
			DataContext = new OptionsDialogViewModel();
			InitializeComponent();
		}
	}

	public interface IOptionsMetadata
	{
		int Order { get; }
	}

	public interface IOptionPage
	{
		string Title { get; }
		void Load(ILSpySettings settings);
		void Save(XElement root);
		void LoadDefaults();
	}

	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ExportOptionPageAttribute() : ExportAttribute("OptionPages", typeof(IOptionPage)), IOptionsMetadata
	{
		public int Order { get; set; }
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._View), Header = nameof(Resources._Options), MenuCategory = nameof(Resources.Options), MenuOrder = 999)]
	[PartCreationPolicy(CreationPolicy.Shared)]
	sealed class ShowOptionsCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			OptionsDialog dlg = new() {
				Owner = MainWindow.Instance,
			};
			if (dlg.ShowDialog() == true)
			{
				new RefreshCommand().Execute(parameter);
			}
		}
	}
}