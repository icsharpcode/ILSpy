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
using System.IO;
using System.Threading.Tasks;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.TreeNodes
{
	[Export(typeof(IResourceNodeFactory))]
	sealed class JsonResourceNodeFactory : IResourceNodeFactory
	{
		private readonly static string[] jsonFileExtensions = { ".json" };

		public ILSpyTreeNode CreateNode(Resource resource)
		{
			Stream stream = resource.TryOpenStream();
			if (stream == null)
				return null;
			return CreateNode(resource.Name, stream);
		}
		
		public ILSpyTreeNode CreateNode(string key, object data)
		{
			if (!(data is Stream))
			    return null;
			foreach (string fileExt in jsonFileExtensions)
			{
				if (key.EndsWith(fileExt, StringComparison.OrdinalIgnoreCase))
					return new JsonResourceEntryNode(key, (Stream)data);
			}
			return null;
		}
	}
	
	sealed class JsonResourceEntryNode : ResourceEntryNode
	{
		string json;
		
		public JsonResourceEntryNode(string key, Stream data)
			: base(key, data)
		{
		}

		// TODO : add Json Icon
		public override object Icon => Images.Resource;

		public override bool View(TabPageModel tabPage)
		{
			AvalonEditTextOutput output = new AvalonEditTextOutput();
			IHighlightingDefinition highlighting = null;

			tabPage.ShowTextView(textView => textView.RunWithCancellation(
				token => Task.Factory.StartNew(
					() => {
						try {
							// cache read XAML because stream will be closed after first read
							if (json == null) {
								using (var reader = new StreamReader(Data)) {
									json = reader.ReadToEnd();
								}
							}
							output.Write(json);
							highlighting = HighlightingManager.Instance.GetDefinitionByExtension(".json");
						}
						catch (Exception ex) {
							output.Write(ex.ToString());
						}
						return output;
					}, token)
			).Then(t => textView.ShowNode(t, this, highlighting))
			.HandleExceptions());
			tabPage.SupportsLanguageSwitching = false;
			return true;
		}
	}
}
