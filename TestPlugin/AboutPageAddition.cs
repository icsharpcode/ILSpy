// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System.Composition;

using Avalonia.Controls;
using Avalonia.Media;

using AvaloniaEdit.Highlighting;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.TextView;

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
				FontStyle = FontStyle.Italic,
				Foreground = new SimpleHighlightingBrush(Colors.Aquamarine)
			});
			textOutput.Write("DO NOT PRESS THIS BUTTON --> ");
			textOutput.AddButton(null, "Test!", (sender, args) => {
				if (sender is Button button)
					button.Content = "Naughty Naughty!";
			});
			textOutput.Write(" <--");
			textOutput.WriteLine();
			textOutput.EndSpan();
			textOutput.WriteLine();
		}
	}
}
