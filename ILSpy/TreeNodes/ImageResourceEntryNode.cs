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
using System.Composition;
using System.IO;

using global::Avalonia.Controls;
using global::Avalonia.Media.Imaging;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX.Abstractions;

using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.TreeNodes
{
	[Export(typeof(IResourceNodeFactory))]
	[Shared]
	sealed class ImageResourceNodeFactory : IResourceNodeFactory
	{
		static readonly string[] imageFileExtensions = { ".png", ".gif", ".bmp", ".jpg" };

		public ITreeNode? CreateNode(Resource resource)
		{
			foreach (var ext in imageFileExtensions)
			{
				if (resource.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
					return new ImageResourceEntryNode(resource.Name, resource.TryOpenStream);
			}
			return null;
		}
	}

	sealed class ImageResourceEntryNode : ResourceEntryNode
	{
		public ImageResourceEntryNode(string key, Func<Stream?> openStream)
			: base(key, () => openStream() ?? Stream.Null)
		{
		}

		// TODO: ship a ResourceImage.svg asset; the generic resource glyph is used for now.
		public override object Icon => Images.Resource;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			if (output is not ISmartTextOutput smart)
				return;
			byte[] snapshot;
			using (var src = OpenStream())
			{
				if (src == Stream.Null)
				{
					output.WriteLine("ILSpy: Failed opening resource stream.");
					return;
				}
				using var ms = new MemoryStream();
				src.CopyTo(ms);
				snapshot = ms.ToArray();
			}
			// Bitmap consumes the stream, so capture into a byte[] first — the original may be
			// one-shot, and AddUIElement runs lazily on the UI thread well after Decompile returns.
			smart.AddUIElement(() => new Image { Source = new Bitmap(new MemoryStream(snapshot)) });
			smart.WriteLine();
			smart.AddButton(Images.Save, "Save", async (_, _) => await SaveSnapshotAsync(snapshot).ConfigureAwait(false));
			smart.WriteLine();
		}

		async System.Threading.Tasks.Task SaveSnapshotAsync(byte[] snapshot)
		{
			var defaultName = Path.GetFileName(ICSharpCode.Decompiler.CSharp.ProjectDecompiler
				.WholeProjectDecompiler.SanitizeFileName((string)Text));
			var path = await Commands.FilePickers.SaveAsync("All files|*.*", defaultName).ConfigureAwait(false);
			if (path == null)
				return;
			await File.WriteAllBytesAsync(path, snapshot).ConfigureAwait(false);
		}
	}
}
