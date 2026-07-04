// Copyright (c) 2026 Siegfried Pammer
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
using System.Composition;
using System.IO;
using System.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.ILSpyX.Abstractions;

using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.TreeNodes
{
	[Export(typeof(IResourceNodeFactory))]
	[Shared]
	sealed class AvaloniaResourcesFileTreeNodeFactory : IResourceNodeFactory
	{
		public ITreeNode? CreateNode(Resource resource)
		{
			if (AvaloniaResourcesFile.IsAvaloniaResourcesEntry(resource.Name))
				return new AvaloniaResourcesFileTreeNode(resource);
			return null;
		}
	}

	/// <summary>
	/// The <c>!AvaloniaResources</c> manifest resource an Avalonia application embeds. It packs
	/// every Avalonia resource (compiled XAML, images, ...) behind a single index; each packed
	/// file is unpacked into a <see cref="ResourceEntryNode"/> child that can be viewed and saved
	/// individually.
	/// </summary>
	public sealed class AvaloniaResourcesFileTreeNode : ResourceTreeNode
	{
		public AvaloniaResourcesFileTreeNode(Resource r) : base(r)
		{
			LazyLoading = true;
		}

		public override object Icon => Images.ResourceResourcesFile;

		protected override void LoadChildren()
		{
			// Unlike ResourcesFile, AvaloniaResourcesFile copies everything out of the stream in
			// its constructor, so the stream can be disposed as soon as parsing is done.
			using var s = Resource.TryOpenStream();
			if (s == null)
				return;
			s.Position = 0;
			try
			{
				foreach (var entry in new AvaloniaResourcesFile(s).OrderBy(e => e.Key, NaturalStringComparer.Instance))
					Children.Add(ResourceEntryNode.Create(entry.Key, entry.Value));
			}
			catch (BadImageFormatException) { /* malformed — ignore */ }
			catch (EndOfStreamException) { /* truncated — ignore */ }
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			EnsureLazyChildren();
			base.Decompile(language, output, options);
			WriteEntryList(output);
		}
	}
}
