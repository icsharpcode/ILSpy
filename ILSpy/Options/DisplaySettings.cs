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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Description of DisplaySettings.
	/// </summary>
	public class DisplaySettings : INotifyPropertyChanged
	{
		public DisplaySettings()
		{
		}
		
		#region INotifyPropertyChanged implementation
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
		
		FontFamily selectedFont;
		
		public FontFamily SelectedFont {
			get { return selectedFont; }
			set {
				if (selectedFont != value) {
					selectedFont = value;
					OnPropertyChanged();
				}
			}
		}
		
		double selectedFontSize;
		
		public double SelectedFontSize {
			get { return selectedFontSize; }
			set {
				if (selectedFontSize != value) {
					selectedFontSize = value;
					OnPropertyChanged();
				}
			}
		}
		
		bool showLineNumbers;
		
		public bool ShowLineNumbers {
			get { return showLineNumbers; }
			set {
				if (showLineNumbers != value) {
					showLineNumbers = value;
					OnPropertyChanged();
				}
			}
		}

		bool showMetadataTokens;

		public bool ShowMetadataTokens
		{
			get { return showMetadataTokens; }
			set {
				if (showMetadataTokens != value) {
					showMetadataTokens = value;
					OnPropertyChanged();
				}
			}
		}

		bool enableWordWrap;

		public bool EnableWordWrap
		{
			get { return enableWordWrap; }
			set
			{
				if (enableWordWrap != value) {
					enableWordWrap = value;
					OnPropertyChanged();
				}
			}
		}

		bool sortResults = true;

		public bool SortResults
		{
			get { return sortResults; }
			set
			{
				if (sortResults != value)
				{
					sortResults = value;
					OnPropertyChanged();
				}
			}
		}

		public void CopyValues(DisplaySettings s)
		{
			this.SelectedFont = s.selectedFont;
			this.SelectedFontSize = s.selectedFontSize;
			this.ShowLineNumbers = s.showLineNumbers;
			this.ShowMetadataTokens = s.showMetadataTokens;
			this.EnableWordWrap = s.enableWordWrap;
			this.SortResults = s.sortResults;
		}
	}
}
