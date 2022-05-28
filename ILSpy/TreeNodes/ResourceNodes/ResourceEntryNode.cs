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
using System.IO;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX.Abstractions;

using Microsoft.Win32;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Entry in a .resources file
	/// </summary>
	public class ResourceEntryNode : ILSpyTreeNode
	{
		private readonly string key;
		private readonly Func<Stream> openStream;

		public override object Text => this.key;

		public override object Icon => Images.Resource;

		protected Stream OpenStream()
		{
			return openStream();
		}

		public ResourceEntryNode(string key, Func<Stream> openStream)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			if (openStream == null)
				throw new ArgumentNullException(nameof(openStream));
			this.key = key;
			this.openStream = openStream;
		}

		public static ILSpyTreeNode Create(Resource resource)
		{
			ILSpyTreeNode result = null;
			foreach (var factory in App.ExportProvider.GetExportedValues<IResourceNodeFactory>())
			{
				result = factory.CreateNode(resource) as ILSpyTreeNode;
				if (result != null)
					break;
			}
			return result ?? new ResourceTreeNode(resource);
		}

		public static ILSpyTreeNode Create(string name, byte[] data)
		{
			return Create(new ByteArrayResource(name, data));
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			using var data = OpenStream();
			language.WriteCommentLine(output, string.Format("{0} = {1}", key, data));
		}

		public override bool Save(ViewModels.TabPageModel tabPage)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.FileName = Path.GetFileName(WholeProjectDecompiler.SanitizeFileName(key));
			if (dlg.ShowDialog() == true)
			{
				using var data = OpenStream();
				using var fs = dlg.OpenFile();
				data.CopyTo(fs);
			}
			return true;
		}
	}
}
