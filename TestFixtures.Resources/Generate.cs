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

// Tiny helper that produces all the binary fixture files (PNG / BMP / JPG / GIF / ICO / CUR /
// .resources / .bin) into the fixtures/ directory. Text fixtures (XML / XSD / XSLT / XAML) are
// checked in as-is. Run with `dotnet run --project Generate.csproj` from this directory.
//
// Each image format renders an obvious recognizable shape (coloured square with a circle and a
// diagonal stripe) at 64×64 so manual ILSpy testing can eyeball that the rendering pipeline
// is decoding the bytes — not just rendering a blank tile.

using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;

using ImageMagick;
using ImageMagick.Drawing;

var dir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "fixtures");
dir = Path.GetFullPath(dir);
Directory.CreateDirectory(dir);

WriteImage(Path.Combine(dir, "logo.png"), MagickFormat.Png);
WriteImage(Path.Combine(dir, "logo.bmp"), MagickFormat.Bmp);
WriteImage(Path.Combine(dir, "logo.jpg"), MagickFormat.Jpeg, quality: 92);
WriteImage(Path.Combine(dir, "logo.gif"), MagickFormat.Gif);

// Multi-frame .ico: 16x16, 32x32, 48x48, each PNG-encoded inside the ICO container so Avalonia
// can decode the largest frame for preview while a savvy viewer can pick any.
File.WriteAllBytes(Path.Combine(dir, "favicon.ico"), BuildIcoOrCur(isCursor: false, sizes: new[] { 16, 32, 48 }));
File.WriteAllBytes(Path.Combine(dir, "pointer.cur"), BuildIcoOrCur(isCursor: true, sizes: new[] { 32 }));

// .resources file with a mix of strings and other-typed entries — the .resources DataGrid view
// splits these into two tables.
var ms = new MemoryStream();
using (var rw = new ResourceWriter(ms))
{
    rw.AddResource("greeting", "Hello, world");
    rw.AddResource("city", "Linz");
    rw.AddResource("year", 2026);
    rw.AddResource("ratio", 1.61803);
    rw.AddResource("flag", true);
}
File.WriteAllBytes(Path.Combine(dir, "Strings.resources"), ms.ToArray());

// Unknown extension — should fall through to the generic ResourceTreeNode.
File.WriteAllBytes(Path.Combine(dir, "blob.bin"), new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0xCA, 0xFE, 0xBA, 0xBE });

Console.WriteLine($"Wrote fixtures into {dir}");

// --- helpers ---

static void WriteImage(string path, MagickFormat format, int size = 64, uint quality = 0)
{
    using var img = RenderShape(size);
    if (quality > 0)
        img.Quality = quality;
    img.Write(path, format);
}

static MagickImage RenderShape(int size)
{
    var img = new MagickImage(new MagickColor("#FFF6E5"), (uint)size, (uint)size);
    float radius = size * 0.35f;
    // Drawables is a single fluent command list: stroke/fill settings persist across the
    // primitives that follow, so each shape resets the ones it does not want.
    new Drawables()
        // Filled blue circle in the centre.
        .FillColor(new MagickColor("#3399CC"))
        .StrokeColor(MagickColors.None)
        .Ellipse(size / 2.0, size / 2.0, radius, radius, 0, 360)
        // Red diagonal stripe.
        .FillColor(MagickColors.None)
        .StrokeColor(new MagickColor("#CC3333"))
        .StrokeWidth(Math.Max(2, size / 16.0))
        .Line(0, size, size, 0)
        // 1px dark border so the bounds are obvious against any background.
        .StrokeColor(new MagickColor("#333333"))
        .StrokeWidth(1)
        .Rectangle(0, 0, size - 1, size - 1)
        .Draw(img);
    return img;
}

// Builds an ICO/CUR file containing one PNG-encoded entry per requested size. The on-disk
// layout: ICONDIR + ICONDIRENTRY[*] + concatenated frame bytes; only byte 2 (image type)
// distinguishes ICO (1) from CUR (2). Hotspot for cursors is centred at (size/2, size/2).
static byte[] BuildIcoOrCur(bool isCursor, int[] sizes)
{
    var frames = new List<byte[]>(sizes.Length);
    foreach (var sz in sizes)
    {
        using var img = RenderShape(sz);
        frames.Add(img.ToByteArray(MagickFormat.Png));
    }
    var fileMs = new MemoryStream();
    var w = new BinaryWriter(fileMs);
    w.Write((ushort)0);                                                  // reserved
    w.Write((ushort)(isCursor ? 2 : 1));                                 // type: 1=ICO, 2=CUR
    w.Write((ushort)sizes.Length);                                       // image count
    int dirSize = 6 + 16 * sizes.Length;
    int offset = dirSize;
    for (int i = 0; i < sizes.Length; i++)
    {
        var sz = sizes[i];
        var dim = (byte)(sz >= 256 ? 0 : sz);
        w.Write(dim);                                                    // width
        w.Write(dim);                                                    // height
        w.Write((byte)0);                                                // palette count
        w.Write((byte)0);                                                // reserved
        if (isCursor)
        {
            w.Write((ushort)(sz / 2));                                   // hotspot X
            w.Write((ushort)(sz / 2));                                   // hotspot Y
        }
        else
        {
            w.Write((ushort)1);                                          // colour planes
            w.Write((ushort)32);                                         // bits per pixel
        }
        w.Write((uint)frames[i].Length);                                 // bytes in frame
        w.Write((uint)offset);                                           // offset to frame
        offset += frames[i].Length;
    }
    foreach (var frame in frames)
        w.Write(frame);
    return fileMs.ToArray();
}
