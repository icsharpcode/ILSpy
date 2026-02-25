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
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy
{

	/// <summary>
	/// C# decompiler integration into ILSpy.
	/// Note: if you're interested in using the decompiler without the ILSpy UI,
	/// please directly use the CSharpDecompiler class.
	/// </summary>
	partial class CSharpLanguage
	{
		void AddWarningMessage(MetadataFile module, ITextOutput output, string line1, string line2 = null,
			string buttonText = null, System.Windows.Media.ImageSource buttonImage = null, RoutedEventHandler buttonClickHandler = null)
		{
			if (output is ISmartTextOutput fancyOutput)
			{
				string text = line1;
				if (!string.IsNullOrEmpty(line2))
					text += Environment.NewLine + line2;
				fancyOutput.AddUIElement(() => new StackPanel {
					Margin = new Thickness(5),
					Orientation = Orientation.Horizontal,
					Children = {
						new Image {
							Width = 32,
							Height = 32,
							Source = Images.Load(this, "Images/Warning")
						},
						new TextBlock {
							Margin = new Thickness(5, 0, 0, 0),
							Text = text
						}
					}
				});
				fancyOutput.WriteLine();
				if (buttonText != null && buttonClickHandler != null)
				{
					fancyOutput.AddButton(buttonImage, buttonText, buttonClickHandler);
					fancyOutput.WriteLine();
				}
			}
			else
			{
				WriteCommentLine(output, line1);
				if (!string.IsNullOrEmpty(line2))
					WriteCommentLine(output, line2);
			}
		}
	}
}
