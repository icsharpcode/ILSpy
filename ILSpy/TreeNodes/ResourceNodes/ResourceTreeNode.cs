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
using System.Reflection;
using System.Text;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Abstractions;

using Microsoft.Win32;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// This is the default resource entry tree node, which is used if no specific
	/// <see cref="IResourceNodeFactory"/> exists for the given resource type. 
	/// </summary>
	public class ResourceTreeNode : ILSpyTreeNode, IResourcesFileTreeNode
	{
		public ResourceTreeNode(Resource r)
		{
			if (r == null)
				throw new ArgumentNullException(nameof(r));
			this.Resource = r;
		}

		public Resource Resource { get; }

		public override object Text => Resource.Name;

		public override object Icon => Images.Resource;

		public override FilterResult Filter(FilterSettings settings)
		{
			if (settings.ShowApiLevel == ApiVisibility.PublicOnly && (Resource.Attributes & ManifestResourceAttributes.VisibilityMask) == ManifestResourceAttributes.Private)
				return FilterResult.Hidden;
			if (settings.SearchTermMatches(Resource.Name))
				return FilterResult.Match;
			else
				return FilterResult.Hidden;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, string.Format("{0} ({1}, {2})", Resource.Name, Resource.ResourceType, Resource.Attributes));

			ISmartTextOutput smartOutput = output as ISmartTextOutput;
			if (smartOutput != null)
			{
				smartOutput.AddButton(Images.Save, Resources.Save, delegate { Save(Docking.DockWorkspace.Instance.ActiveTabPage); });
				output.WriteLine();
			}
		}

		public override bool View(TabPageModel tabPage)
		{
			Stream s = Resource.TryOpenStream();
			if (s != null && s.Length < DecompilerTextView.DefaultOutputLengthLimit)
			{
				s.Position = 0;
				FileType type = GuessFileType.DetectFileType(s);
				if (type != FileType.Binary)
				{
					s.Position = 0;
					AvalonEditTextOutput output = new AvalonEditTextOutput();
					output.Title = Resource.Name;
					output.Write(FileReader.OpenStream(s, Encoding.UTF8).ReadToEnd());
					string ext;
					if (type == FileType.Xml)
						ext = ".xml";
					else
						ext = Path.GetExtension(WholeProjectDecompiler.SanitizeFileName(Resource.Name));
					tabPage.ShowTextView(textView => textView.ShowNode(output, this, HighlightingManager.Instance.GetDefinitionByExtension(ext)));
					tabPage.SupportsLanguageSwitching = false;
					return true;
				}
			}
			return false;
		}

		public override bool Save(TabPageModel tabPage)
		{
			Stream s = Resource.TryOpenStream();
			if (s == null)
				return false;
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.FileName = Path.GetFileName(WholeProjectDecompiler.SanitizeFileName(Resource.Name));
			if (dlg.ShowDialog() == true)
			{
				s.Position = 0;
				using (var fs = dlg.OpenFile())
				{
					s.CopyTo(fs);
				}
			}
			return true;
		}

		public static ILSpyTreeNode Create(Resource resource)
		{
			return ResourceEntryNode.Create(resource);
		}
	}
}
