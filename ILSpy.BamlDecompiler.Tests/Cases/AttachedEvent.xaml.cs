// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Input;

namespace ILSpy.BamlDecompiler.Tests.Cases
{
	/// <summary>
	/// Interaction logic for AttachedEvent.xaml
	/// </summary>
	public partial class AttachedEvent : Window
	{
		public AttachedEvent()
		{
			InitializeComponent();
		}
		
		void GridAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}