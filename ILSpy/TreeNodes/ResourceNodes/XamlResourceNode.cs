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

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX.Abstractions;

namespace ICSharpCode.ILSpy.Xaml
{
	[Export(typeof(IResourceNodeFactory))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	sealed class XamlResourceNodeFactory : IResourceNodeFactory
	{
		public ITreeNode CreateNode(Resource resource)
		{
			if (resource.Name.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
				return new XamlResourceEntryNode(resource.Name, resource.TryOpenStream);
			else
				return null;
		}
	}

	sealed class XamlResourceEntryNode : ResourceEntryNode
	{
		string xaml;

		public XamlResourceEntryNode(string key, Func<Stream> openStream) : base(key, openStream)
		{
		}

		public override bool View(TabPageModel tabPage)
		{
			AvalonEditTextOutput output = new AvalonEditTextOutput();
			IHighlightingDefinition highlighting = null;

			tabPage.ShowTextView(textView => textView.RunWithCancellation(
				token => Task.Factory.StartNew(
					() => {
						try
						{
							// cache read XAML because stream will be closed after first read
							if (xaml == null)
							{
								using var data = OpenStream();
								if (data == null)
								{
									output.Write("ILSpy: Failed opening resource stream.");
									output.WriteLine();
									return output;
								}
								using (var reader = new StreamReader(data))
								{
									xaml = reader.ReadToEnd();
								}
							}
							output.Write(xaml);
							highlighting = HighlightingManager.Instance.GetDefinitionByExtension(".xml");
						}
						catch (Exception ex)
						{
							output.Write(ex.ToString());
						}
						return output;
					}, token)
			).Then(t => textView.ShowNode(t, this, highlighting)).HandleExceptions());
			tabPage.SupportsLanguageSwitching = false;
			return true;
		}
	}
}
