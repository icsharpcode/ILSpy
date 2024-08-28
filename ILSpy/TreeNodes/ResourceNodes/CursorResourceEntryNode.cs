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
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX.Abstractions;

namespace ICSharpCode.ILSpy.TreeNodes
{
	[Export(typeof(IResourceNodeFactory))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	sealed class CursorResourceNodeFactory : IResourceNodeFactory
	{
		static readonly string[] imageFileExtensions = { ".cur" };

		public ITreeNode CreateNode(Resource resource)
		{
			string key = resource.Name;
			foreach (string fileExt in imageFileExtensions)
			{
				if (key.EndsWith(fileExt, StringComparison.OrdinalIgnoreCase))
					return new CursorResourceEntryNode(key, resource.TryOpenStream);
			}
			return null;
		}
	}

	sealed class CursorResourceEntryNode : ResourceEntryNode
	{
		public CursorResourceEntryNode(string key, Func<Stream> openStream)
			: base(key, openStream)
		{
		}

		public override object Icon => Images.ResourceImage;

		public override bool View(TabPageModel tabPage)
		{
			try
			{
				AvalonEditTextOutput output = new AvalonEditTextOutput();
				BitmapImage image = new BitmapImage();
				byte[] curData;
				using (var data = OpenStream())
				{
					if (data == null)
						return false;
					//HACK: windows imaging does not understand that .cur files have the same layout as .ico
					// so load to data, and modify the ResourceType in the header to make look like an icon...
					MemoryStream s = data as MemoryStream;
					if (s == null)
					{
						// data was stored in another stream type (e.g. PinnedBufferedMemoryStream)
						s = new MemoryStream();
						data.CopyTo(s);
					}
					curData = s.ToArray();
				}
				curData[2] = 1;
				using (Stream stream = new MemoryStream(curData))
				{
					image.BeginInit();
					image.StreamSource = stream;
					image.EndInit();
				}

				output.AddUIElement(() => new Image { Source = image });
				output.WriteLine();
				output.AddButton(Images.Save, Resources.Save, delegate {
					Save(null);
				});
				tabPage.ShowTextView(textView => textView.ShowNode(output, this));
				tabPage.SupportsLanguageSwitching = false;
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
