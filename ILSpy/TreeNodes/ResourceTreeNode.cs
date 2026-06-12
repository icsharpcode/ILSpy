// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Default resource entry node — used when no specialised handler (image, XAML, .resources
	/// file, …) is registered for a given resource. The Avalonia port currently always falls
	/// back to this plain node since the typed handlers are not yet ported.
	/// </summary>
	public class ResourceTreeNode : ILSpyTreeNode
	{
		public ResourceTreeNode(Resource r)
		{
			Resource = r ?? throw new ArgumentNullException(nameof(r));
		}

		public Resource Resource { get; }

		public override object Text => ILAmbience.EscapeName(Resource.Name);

		public override object Icon => Images.Resource;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			var sizeInBytes = Resource.TryGetLength();
			var sizeInBytesText = sizeInBytes == null ? "" : ", " + sizeInBytes + " bytes";
			language.WriteCommentLine(output, $"{Resource.Name} ({Resource.ResourceType}, {Resource.Attributes}{sizeInBytesText})");
			if (output is ISmartTextOutput smart)
			{
				smart.WriteLine();
				// Dispatch through the virtual Save() so subclasses (ResourcesFileTreeNode)
				// can present their own format-specific dialog instead of the generic one.
				smart.AddButton(Images.Save, "Save", (_, _) => Save());
				smart.WriteLine();
			}
		}

		public override bool Save()
		{
			SaveAsync().HandleExceptions();
			return true;
		}

		async Task SaveAsync()
		{
			var defaultName = Path.GetFileName(WholeProjectDecompiler.SanitizeFileName(Resource.Name));
			var path = await FilePickers.SaveAsync("All files|*.*", defaultName).ConfigureAwait(false);
			if (path == null)
				return;
			using var src = Resource.TryOpenStream();
			if (src == null)
				return;
			src.Position = 0;
			using var dst = File.Create(path);
			src.CopyTo(dst);
		}

		// Keep the symmetric name from WPF: callers can use ResourceTreeNode.Create or
		// ResourceEntryNode.Create interchangeably.
		public static ILSpyTreeNode Create(Resource resource) => ResourceEntryNode.Create(resource);
	}
}
