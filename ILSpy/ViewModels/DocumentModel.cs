// Copyright (c) 2019 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.ViewModels
{
	public abstract class DocumentModel : PaneModel
	{
		public override PanePosition DefaultPosition => PanePosition.Document;

		protected DocumentModel(string contentId, string title)
		{
			this.ContentId = contentId;
			this.Title = title;
		}
	}

	public class DecompiledDocumentModel : DocumentModel
	{
		public DecompiledDocumentModel()
			: base("//Decompiled", Resources.View)
		{
		}

		public DecompiledDocumentModel(string id, string title)
			: base("//Decompiled/" + id, title)
		{
		}

		private DecompilerTextView textView;
		public DecompilerTextView TextView {
			get => textView;
			set {
				if (textView != value) {
					textView = value;
					RaisePropertyChanged(nameof(TextView));
				}
			}
		}

		private Language language;
		public Language Language {
			get => language;
			set {
				if (language != value) {
					language = value;
					RaisePropertyChanged(nameof(Language));
				}
			}
		}

		private LanguageVersion languageVersion;
		public LanguageVersion LanguageVersion {
			get => languageVersion;
			set {
				if (languageVersion != value) {
					languageVersion = value;
					RaisePropertyChanged(nameof(LanguageVersion));
				}
			}
		}
	}
}