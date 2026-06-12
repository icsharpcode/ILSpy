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
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.ILSpyX.Abstractions;

using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Controls;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.TreeNodes
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
		readonly List<SerializedObjectRepresentation> otherEntries = new();

		public IReadOnlyList<KeyValuePair<string, string>> StringTableEntries => stringTableEntries;
		public IReadOnlyList<SerializedObjectRepresentation> OtherEntries => otherEntries;

		public ResourcesFileTreeNode(Resource r) : base(r)
		{
			LazyLoading = true;
		}

		public override object Icon => Images.ResourceResourcesFile;

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
				case null:
					otherEntries.Add(new SerializedObjectRepresentation(entry.Key, "null", string.Empty));
					break;
				case ResourceSerializedObject so:
					if (!TryDispatchSerializedObject(entry.Key, so))
						otherEntries.Add(new SerializedObjectRepresentation(entry.Key, so.TypeName ?? string.Empty, "<serialized>"));
					break;
				default:
					otherEntries.Add(new SerializedObjectRepresentation(
						entry.Key,
						entry.Value.GetType().FullName ?? entry.Value.GetType().Name,
						entry.Value.ToString() ?? string.Empty));
					break;
			}
		}

		/// <summary>
		/// Walks <see cref="ILSpyTreeNode.ResourceNodeFactories"/> for a specialised tree
		/// node that claims this serialized-object entry (e.g. an <c>ImageListStreamer</c>
		/// blob). Returns true and adds the node to <see cref="Children"/> when claimed;
		/// returns false to let the caller fall back to the generic "&lt;serialized&gt;"
		/// representation.
		/// </summary>
		bool TryDispatchSerializedObject(string key, ResourceSerializedObject so)
		{
			foreach (var factory in ResourceNodeFactories)
			{
				if (factory.CreateNode(key, so) is ILSpyTreeNode node)
				{
					Children.Add(node);
					return true;
				}
			}
			return false;
		}

		public override bool Save()
		{
			SaveDialogAsync().HandleExceptions();
			return true;
		}

		async System.Threading.Tasks.Task SaveDialogAsync()
		{
			var defaultName = System.IO.Path.GetFileName(WholeProjectDecompiler.SanitizeFileName(Resource.Name));
			var path = await FilePickers.SaveAsync(
				"Resources file (*.resources)|*.resources|Resource XML (*.resx)|*.resx",
				defaultName).ConfigureAwait(false);
			if (path == null)
				return;
			if (path.EndsWith(".resx", System.StringComparison.OrdinalIgnoreCase))
			{
				using var dst = File.Create(path);
				WriteResX(dst);
			}
			else
			{
				using var src = Resource.TryOpenStream();
				if (src == null)
					return;
				src.Position = 0;
				using var dst = File.Create(path);
				src.CopyTo(dst);
			}
		}

		/// <summary>
		/// Re-emits the underlying <c>.resources</c> stream as a ResX (XML) document into
		/// <paramref name="target"/>. Skips entries that fail to deserialize so the export still
		/// produces a usable file when one entry is malformed.
		/// </summary>
		public void WriteResX(Stream target)
		{
			using var src = Resource.TryOpenStream();
			if (src == null)
				return;
			src.Position = 0;
			using var writer = new ICSharpCode.Decompiler.Util.ResXResourceWriter(target);
			try
			{
				foreach (var entry in new ResourcesFile(src))
					writer.AddResource(entry.Key, entry.Value);
			}
			catch (BadImageFormatException) { /* malformed — what we got is what we got */ }
			catch (EndOfStreamException) { /* truncated — same */ }
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			EnsureLazyChildren();
			base.Decompile(language, output, options);
			if (output is not ISmartTextOutput smart)
				return;
			if (stringTableEntries.Count > 0)
			{
				output.WriteLine();
				smart.AddUIElement(() => new ResourceStringTable(stringTableEntries));
				output.WriteLine();
			}
			if (otherEntries.Count > 0)
			{
				output.WriteLine();
				smart.AddUIElement(() => new ResourceObjectTable(otherEntries));
				output.WriteLine();
			}
		}
	}
}
