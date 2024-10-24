// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System.Composition;
using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.ILSpy;

namespace TestPlugin
{
	[Export(typeof(IAboutPageAddition))]
	[Shared]
	public class AboutPageAddition : IAboutPageAddition
	{
		public void Write(ISmartTextOutput textOutput)
		{
			textOutput.WriteLine();
			textOutput.Write("This is a test.");
			textOutput.WriteLine();
			textOutput.WriteLine();
			textOutput.BeginSpan(new HighlightingColor {
				Background = new SimpleHighlightingBrush(Colors.Black),
				FontStyle = FontStyles.Italic,
				Foreground = new SimpleHighlightingBrush(Colors.Aquamarine)
			});
			textOutput.Write("DO NOT PRESS THIS BUTTON --> ");
			textOutput.AddButton(null, "Test!", (sender, args) => MessageBox.Show("Naughty Naughty!", "Naughty!", MessageBoxButton.OK, MessageBoxImage.Exclamation));
			textOutput.Write(" <--");
			textOutput.WriteLine();
			textOutput.EndSpan();
			textOutput.WriteLine();
		}
	}
}
