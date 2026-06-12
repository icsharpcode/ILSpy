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
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX.Abstractions;

using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.TreeNodes
{
	[Export(typeof(IResourceNodeFactory))]
	[Shared]
	sealed class CursorResourceNodeFactory : IResourceNodeFactory
	{
		public ITreeNode? CreateNode(Resource resource)
		{
			if (resource.Name.EndsWith(".cur", StringComparison.OrdinalIgnoreCase))
				return new CursorResourceEntryNode(resource.Name, resource.TryOpenStream);
			return null;
		}
	}

	sealed class CursorResourceEntryNode : ResourceEntryNode
	{
		public CursorResourceEntryNode(string key, Func<Stream?> openStream)
			: base(key, () => openStream() ?? Stream.Null)
		{
		}

		public override object Icon => Images.Resource;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			if (output is not ISmartTextOutput smart)
				return;
			byte[] cursor;
			using (var src = OpenStream())
			{
				if (src == Stream.Null)
				{
					output.WriteLine("ILSpy: Failed opening resource stream.");
					return;
				}
				using var ms = new MemoryStream();
				src.CopyTo(ms);
				cursor = ms.ToArray();
			}
			// CUR and ICO share the same on-disk layout; the only difference is byte 2 of the
			// header (1=icon, 2=cursor). Skia's image decoder only accepts the icon variant, so
			// flip the bit before handing it off — preview only, doesn't affect Save (which
			// writes the original snapshot).
			byte[] iconView = cursor.Length >= 3 ? (byte[])cursor.Clone() : cursor;
			if (iconView.Length >= 3)
				iconView[2] = 1;
			smart.AddUIElement(() => new Image { Source = new Bitmap(new MemoryStream(iconView)) });
			smart.WriteLine();
			smart.AddButton(Images.Save, "Save", async (_, _) => await SaveSnapshotAsync(cursor).ConfigureAwait(false));
			smart.WriteLine();
		}

		async System.Threading.Tasks.Task SaveSnapshotAsync(byte[] snapshot)
		{
			var defaultName = Path.GetFileName(WholeProjectDecompiler.SanitizeFileName((string)Text));
			var path = await FilePickers.SaveAsync("All files|*.*", defaultName).ConfigureAwait(false);
			if (path == null)
				return;
			await File.WriteAllBytesAsync(path, snapshot).ConfigureAwait(false);
		}
	}
}
