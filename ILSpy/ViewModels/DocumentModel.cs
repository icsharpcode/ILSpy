using System;
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
			: base("//Decompiled", "View")
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
	}
}
