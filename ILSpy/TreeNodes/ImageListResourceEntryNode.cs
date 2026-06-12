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
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.IL;

using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.ImageList
{
	/// <summary>
	/// Tree node for a <c>System.Windows.Forms.ImageListStreamer</c> resource entry. Lazily
	/// decodes the underlying NRBF blob into per-frame <see cref="ImageListFrameNode"/>
	/// children, and writes an inline horizontal preview of the whole sheet when the parent
	/// itself is selected.
	/// </summary>
	public sealed class ImageListResourceEntryNode : ILSpyTreeNode
	{
		readonly string key;
		readonly Func<Stream> openStream;
		IReadOnlyList<DecodedImage>? cachedFrames;
		Exception? cachedDecodeError;

		public ImageListResourceEntryNode(string key, Func<Stream> openStream)
		{
			this.key = key ?? throw new ArgumentNullException(nameof(key));
			this.openStream = openStream ?? throw new ArgumentNullException(nameof(openStream));
			LazyLoading = true;
		}

		public override object Text => ILAmbience.EscapeName(key);

		public override object Icon => Images.Resource;

		// Decode-once cache shared between LoadChildren and Decompile so opening the parent
		// twice (tree expand + editor render) hits the NRBF parser exactly once.
		IReadOnlyList<DecodedImage>? TryDecode(out Exception? error)
		{
			if (cachedFrames != null)
			{ error = null; return cachedFrames; }
			if (cachedDecodeError != null)
			{ error = cachedDecodeError; return null; }
			try
			{
				using var stream = openStream();
				cachedFrames = ImageListDecoder.Decode(stream);
				error = null;
				return cachedFrames;
			}
			catch (Exception ex)
			{
				cachedDecodeError = ex;
				error = ex;
				return null;
			}
		}

		protected override void LoadChildren()
		{
			var frames = TryDecode(out var error);
			if (frames == null)
			{
				// Surface decode failures as a single child node so the user sees them in
				// the tree rather than getting a silently-empty resource.
				Children.Add(new DecodeErrorNode(error!));
				return;
			}
			for (int i = 0; i < frames.Count; i++)
				Children.Add(new ImageListFrameNode($"Image{i}", frames[i]));
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			var frames = TryDecode(out var error);
			if (frames == null)
			{
				language.WriteCommentLine(output, $"{key}: failed to decode ImageList — {error!.Message}");
				return;
			}
			language.WriteCommentLine(output,
				$"{key} — ImageList with {frames.Count} frame{(frames.Count == 1 ? "" : "s")} " +
				$"({(frames.Count > 0 ? frames[0].Width + "x" + frames[0].Height : "0x0")} each)");

			if (output is not ISmartTextOutput smart)
				return;
			smart.WriteLine();
			smart.AddUIElement(() => BuildInlinePreview(frames));
			smart.WriteLine();
		}

		static Control BuildInlinePreview(IReadOnlyList<DecodedImage> frames)
		{
			var panel = new WrapPanel { Margin = new Thickness(4), Orientation = Orientation.Horizontal };
			foreach (var f in frames)
			{
				var image = new Image {
					Source = ImageListBitmap.Create(f),
					Width = f.Width,
					Height = f.Height,
					Margin = new Thickness(2),
				};
				panel.Children.Add(image);
			}
			return panel;
		}

		sealed class DecodeErrorNode : ILSpyTreeNode
		{
			readonly Exception ex;
			public DecodeErrorNode(Exception ex) { this.ex = ex; }
			public override object Text => $"<decode failed: {ex.Message}>";
			public override object Icon => Images.Resource;
		}
	}

	/// <summary>
	/// Single ImageList frame surfaced as its own tree node — selecting it shows the image
	/// in the editor with a Save-as-PNG button.
	/// </summary>
	public sealed class ImageListFrameNode : ILSpyTreeNode
	{
		readonly string key;
		readonly DecodedImage frame;

		public ImageListFrameNode(string key, DecodedImage frame)
		{
			this.key = key ?? throw new ArgumentNullException(nameof(key));
			this.frame = frame ?? throw new ArgumentNullException(nameof(frame));
		}

		public override object Text => key;
		public override object Icon => Images.Resource;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, $"{key}: {frame.Width}x{frame.Height} BGRA");
			if (output is not ISmartTextOutput smart)
				return;
			smart.WriteLine();
			smart.AddUIElement(() => new Image {
				Source = ImageListBitmap.Create(frame),
				Width = frame.Width,
				Height = frame.Height,
			});
			smart.WriteLine();
			smart.AddButton(Images.Save, "Save", async (_, _) => await SaveAsync().ConfigureAwait(false));
		}

		public override bool Save()
		{
			SaveAsync().HandleExceptions();
			return true;
		}

		async Task SaveAsync()
		{
			var defaultName = Path.GetFileNameWithoutExtension(
				WholeProjectDecompiler.SanitizeFileName(key)) + ".png";
			var path = await FilePickers.SaveAsync("PNG image (*.png)|*.png", defaultName).ConfigureAwait(false);
			if (path == null)
				return;
			using var bmp = ImageListBitmap.Create(frame);
			using var dst = File.Create(path);
			bmp.Save(dst);
		}
	}

	/// <summary>
	/// Builds an Avalonia <see cref="WriteableBitmap"/> from a <see cref="DecodedImage"/>.
	/// Lives in its own static class so the bitmap copy + stride handling can be unit-tested
	/// independently of the tree node.
	/// </summary>
	public static class ImageListBitmap
	{
		public static WriteableBitmap Create(DecodedImage image)
		{
			ArgumentNullException.ThrowIfNull(image);
			var bitmap = new WriteableBitmap(
				new PixelSize(image.Width, image.Height),
				new Vector(96, 96),
				PixelFormat.Bgra8888,
				AlphaFormat.Unpremul);
			using var fb = bitmap.Lock();
			int srcStride = image.Width * 4;
			if (fb.RowBytes == srcStride)
			{
				Marshal.Copy(image.BgraPixels, 0, fb.Address, image.BgraPixels.Length);
			}
			else
			{
				for (int y = 0; y < image.Height; y++)
					Marshal.Copy(image.BgraPixels, y * srcStride, fb.Address + y * fb.RowBytes, srcStride);
			}
			return bitmap;
		}
	}
}
