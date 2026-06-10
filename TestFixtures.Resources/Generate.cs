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

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

var dir = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "fixtures");
dir = Path.GetFullPath(dir);
Directory.CreateDirectory(dir);

WriteImage(Path.Combine(dir, "logo.png"), new PngEncoder());
WriteImage(Path.Combine(dir, "logo.bmp"), new BmpEncoder());
WriteImage(Path.Combine(dir, "logo.jpg"), new JpegEncoder { Quality = 92 });
WriteImage(Path.Combine(dir, "logo.gif"), new GifEncoder());

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

static void WriteImage(string path, IImageEncoder encoder, int size = 64)
{
    using var img = RenderShape(size);
    using var fs = File.Create(path);
    img.Save(fs, encoder);
}

static Image<Rgba32> RenderShape(int size)
{
    var img = new Image<Rgba32>(size, size, new Rgba32(0xFF, 0xF6, 0xE5));
    img.Mutate(ctx => {
        // Filled blue circle in the centre.
        ctx.Fill(new Rgba32(0x33, 0x99, 0xCC),
            new SixLabors.ImageSharp.Drawing.EllipsePolygon(size / 2f, size / 2f, size * 0.35f));
        // Red diagonal stripe.
        ctx.DrawLine(new Rgba32(0xCC, 0x33, 0x33),
            Math.Max(2, size / 16f),
            new PointF(0, size),
            new PointF(size, 0));
        // 1px dark border so the bounds are obvious against any background.
        ctx.Draw(new Rgba32(0x33, 0x33, 0x33), 1,
            new RectangleF(0, 0, size - 1, size - 1));
    });
    return img;
}

// Builds an ICO/CUR file containing one PNG-encoded entry per requested size. The on-disk
// layout: ICONDIR + ICONDIRENTRY[*] + concatenated frame bytes; only byte 2 (image type)
// distinguishes ICO (1) from CUR (2). Hotspot for cursors is centred at (size/2, size/2).
static byte[] BuildIcoOrCur(bool isCursor, int[] sizes)
{
    var frames = new List<byte[]>(sizes.Length);
    var pngEncoder = new PngEncoder();
    foreach (var sz in sizes)
    {
        using var img = RenderShape(sz);
        using var ms = new MemoryStream();
        img.Save(ms, pngEncoder);
        frames.Add(ms.ToArray());
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
