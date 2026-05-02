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
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.ILSpyX.Abstractions;

using ILSpy.Languages;

namespace ILSpy.TreeNodes
{
	[Export(typeof(IResourceNodeFactory))]
	[Shared]
	sealed class ResourcesFileTreeNodeFactory : IResourceNodeFactory
	{
		public ITreeNode? CreateNode(Resource resource)
		{
			if (resource.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
				return new ResourcesFileTreeNode(resource);
			return null;
		}
	}

	/// <summary>
	/// A <c>.resources</c> file embedded in an assembly. Children are
	/// <see cref="ResourceEntryNode"/> for byte-payload entries; string entries are
	/// captured into <see cref="StringTableEntries"/> for inline rendering during
	/// decompilation.
	/// </summary>
	public sealed class ResourcesFileTreeNode : ResourceTreeNode
	{
		readonly List<KeyValuePair<string, string>> stringTableEntries = new();

		public IReadOnlyList<KeyValuePair<string, string>> StringTableEntries => stringTableEntries;

		public ResourcesFileTreeNode(Resource r) : base(r)
		{
			LazyLoading = true;
		}

		public override object Icon => Images.Images.ResourceResourcesFile;

		protected override void LoadChildren()
		{
			var s = Resource.TryOpenStream();
			if (s == null)
				return;
			s.Position = 0;
			try
			{
				foreach (var entry in new ResourcesFile(s).OrderBy(e => e.Key, NaturalStringComparer.Instance))
					ProcessEntry(entry);
			}
			catch (BadImageFormatException) { /* malformed — ignore */ }
			catch (EndOfStreamException) { /* truncated — ignore */ }
		}

		void ProcessEntry(KeyValuePair<string, object?> entry)
		{
			switch (entry.Value)
			{
				case string s:
					stringTableEntries.Add(new KeyValuePair<string, string>(entry.Key, s));
					break;
				case byte[] bytes:
					Children.Add(ResourceEntryNode.Create(entry.Key, bytes));
					break;
				case MemoryStream ms:
					Children.Add(ResourceEntryNode.Create(entry.Key, ms.ToArray()));
					break;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			EnsureLazyChildren();
			base.Decompile(language, output, options);
			if (stringTableEntries.Count == 0)
				return;
			output.WriteLine();
			language.WriteCommentLine(output, "string table:");
			foreach (var pair in stringTableEntries)
				language.WriteCommentLine(output, $"  {pair.Key} = {pair.Value}");
		}
	}
}
