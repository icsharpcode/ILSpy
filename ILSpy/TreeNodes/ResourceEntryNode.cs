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
using System.Linq;
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
	/// One entry inside a <see cref="ResourcesFileTreeNode"/>'s <c>.resources</c> file —
	/// a key plus an opener for the underlying byte payload.
	/// </summary>
	public class ResourceEntryNode : ILSpyTreeNode
	{
		readonly string key;
		readonly Func<Stream> openStream;

		public ResourceEntryNode(string key, Func<Stream> openStream)
		{
			this.key = key ?? throw new ArgumentNullException(nameof(key));
			this.openStream = openStream ?? throw new ArgumentNullException(nameof(openStream));
		}

		public override object Text => ILAmbience.EscapeName(key);

		public override object Icon => Images.Resource;

		protected Stream OpenStream() => openStream();

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			using var data = OpenStream();
			if (WriteTextContent(key, data, output))
				return;
			language.WriteCommentLine(output, $"{key} = {data.Length} bytes");
			if (output is ISmartTextOutput smart)
			{
				smart.WriteLine();
				smart.AddButton(Images.Save, "Save", async (_, _) => await SaveAsync().ConfigureAwait(false));
				smart.WriteLine();
			}
		}

		/// <summary>
		/// Renders a textual payload as the view's whole content and switches the text view's
		/// highlighting to the detected format (XML, JSON, ...), mirroring how the XML/XAML
		/// resource nodes present theirs. Returns false for binary payloads, which keep the
		/// byte-count-plus-Save presentation instead.
		/// </summary>
		internal static bool WriteTextContent(string resourceName, Stream data, ITextOutput output)
		{
			var content = TextResourceDetector.TryDetectText(resourceName, data);
			if (content == null)
				return false;
			output.Write(content.Text);
			if (output is AvaloniaEditTextOutput aeto)
			{
				// An empty extension is a deliberate override to plain text: without it the tab
				// would fall back to the active language's highlighting (e.g. C#).
				aeto.SyntaxExtensionOverride = content.SyntaxExtension;
			}
			return true;
		}

		public override bool Save()
		{
			SaveAsync().HandleExceptions();
			return true;
		}

		async Task SaveAsync()
		{
			var defaultName = Path.GetFileName(WholeProjectDecompiler.SanitizeFileName(key));
			var path = await FilePickers.SaveAsync("All files|*.*", defaultName).ConfigureAwait(false);
			if (path == null)
				return;
			using var src = OpenStream();
			using var dst = File.Create(path);
			src.CopyTo(dst);
		}

		/// <summary>
		/// Creates the node for an entry unpacked from a container resource (.resources /
		/// !AvaloniaResources). Dispatches through the factory pipeline so typed entries
		/// (.baml, images, XML, ...) get their specialised nodes; unclaimed entries fall back
		/// to this generic entry node.
		/// </summary>
		public static ILSpyTreeNode Create(string name, byte[] data)
		{
			var resource = new ByteArrayResource(name, data);
			return ResourceNodeFactories
				.Select(f => f.CreateNode(resource))
				.OfType<ILSpyTreeNode>()
				.FirstOrDefault()
				?? new ResourceEntryNode(name, () => new MemoryStream(data));
		}

		/// <summary>
		/// Walks <see cref="ILSpyTreeNode.ResourceNodeFactories"/> and returns the first node any
		/// factory builds for <paramref name="resource"/>. Falls back to a plain
		/// <see cref="ResourceTreeNode"/> when no factory claims it. The single dispatch entry
		/// point used by both <see cref="ResourceListTreeNode"/> and packages.
		/// </summary>
		public static ILSpyTreeNode Create(Resource resource)
		{
			ArgumentNullException.ThrowIfNull(resource);
			return ResourceNodeFactories
				.Select(f => f.CreateNode(resource))
				.OfType<ILSpyTreeNode>()
				.FirstOrDefault()
				?? new ResourceTreeNode(resource);
		}
	}
}
