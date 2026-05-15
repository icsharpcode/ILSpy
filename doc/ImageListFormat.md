# `ImageListStreamer` on-disk format

This file is the format reference for [`ImageListDecoder.cs`](ImageListDecoder.cs).
Read it before touching the decoder — the byte layout is three layers deep and the
field semantics are easy to get wrong if you only have one byte dump in front of you.

```
.resources value (ResourceSerializedObject, TypeName=System.Windows.Forms.ImageListStreamer)
  └─ NRBF payload                                       (layer 1, System.Formats.Nrbf)
       ClassRecord "System.Windows.Forms.ImageListStreamer"
         member "Data" : byte[]
            └─ "MSFt"-prefixed RLE blob                 (layer 2, ~15-line RLE)
                  └─ ILHEAD (28 bytes) + color DIB [+ mask DIB]   (layer 3, ImageList_Write stream)
```

`ResXResourceWriter` / `ResXResourceReader` only base64-wrap the NRBF payload — they
don't impose any structure of their own.

---

## Layer 1 — NRBF envelope

Specification: [[MS-NRBF]](https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-nrbf/).
Reader: [`System.Formats.Nrbf.NrbfDecoder`](https://learn.microsoft.com/en-us/dotnet/standard/serialization/binaryformatter-migration-guide/read-nrbf-payloads),
shipped as the `System.Formats.Nrbf` NuGet (netstandard2.0+, works on `net10.0`
without a WinForms reference).

What `ImageListStreamer.GetObjectData` writes (from dotnet/winforms
`src/System.Windows.Forms/System/Windows/Forms/Controls/ImageList/ImageListStreamer.cs`):

```csharp
si.AddValue("Data", Serialize());
```

and the round-trip constructor:

```csharp
private ImageListStreamer(SerializationInfo info, StreamingContext context)
{
    if (info.GetValue<byte[]>("Data") is { } data)
    {
        Deserialize(data);
    }
}
```

Records in order: `SerializationHeaderRecord` → `(System)ClassWithMembersAndTypes`
naming `System.Windows.Forms.ImageListStreamer` with a single `"Data"` member of
`PrimitiveArray<Byte>` → `ArraySinglePrimitive` of `Byte` → `MessageEnd`.

`NrbfDecoder` parses the type *name* — it never loads or instantiates
`ImageListStreamer`. Decoder path:

```csharp
ClassRecord root = NrbfDecoder.DecodeClassRecord(stream);
byte[] data = ((SZArrayRecord<byte>)root.GetArrayRecord("Data")).GetArray();
```

---

## Layer 2 — `MSFt` RLE wrapper

From `ImageListStreamer.cs`:

```csharp
private static ReadOnlySpan<byte> HeaderMagic => "MSFt"u8;   // 0x4D 0x53 0x46 0x74

// Compress
writer.TryWrite(HeaderMagic);
RunLengthEncoder.TryEncode(input, writer.Span[writer.Position..], out int written);

// Decompress — note the early-out:
if (!reader.TryAdvancePast(HeaderMagic)) { return input; }
RunLengthEncoder.TryDecode(remaining, output, out int written);
```

The decoder must mirror that fall-through: if the first four bytes are not
`M S F t`, treat the buffer as already-uncompressed and pass it straight to
layer 3.

### The RLE itself

From dotnet/winforms `src/System.Private.Windows.Core/src/System/IO/Compression/RunLengthEncoder.cs`
— class-level comment, verbatim:

> "Format used is a byte for the count, followed by a byte for the value."

```csharp
// Decode
while (reader.TryRead(out byte count))
{
    reader.TryRead(out byte value);
    writer.TryWriteCount(count, value);
}
```

Byte layout: stream of `(uint8 count, uint8 value)` pairs after the 4-byte magic.
`count` is 1..255 — a literal byte costs two bytes, a run of 255 identical bytes
also costs two. Runs longer than 255 are split into multiple `(0xFF, v)` pairs
followed by a `(remainder, v)` tail. There is no end marker and no escape; the
encoded length equals `2 × (number-of-pairs)` and is determined by the surrounding
`byte[]` length from NRBF.

---

## Layer 3 — `ILHEAD` + DIBs

This is the Win32 comctl32 `ImageList_Write` stream. Reference implementation:
Wine `dlls/comctl32/imagelist.c` (LGPL — *format reference only*, not for
copy-paste). Verbatim:

```c
#pragma pack(push,2)
typedef struct _ILHEAD
{
    USHORT  usMagic;      // 0x00  'I','L'  = 0x4C49  ((('L'<<8)|'I'))
    USHORT  usVersion;    // 0x02  0x0101
    WORD    cCurImage;    // 0x04  number of images currently stored
    WORD    cMaxImage;    // 0x06  capacity
    WORD    cGrow;        // 0x08  growth increment
    WORD    cx;           // 0x0A  per-image width  (pixels)
    WORD    cy;           // 0x0C  per-image height (pixels)
    COLORREF bkcolor;     // 0x0E  4 bytes — Win32 0x00BBGGRR or CLR_NONE = 0xFFFFFFFF
    WORD    flags;        // 0x12  ILC_* flags, see below
    SHORT   ovls[4];      // 0x14  overlay-image indices (4 × INT16)
} ILHEAD;                 // total 0x1C = 28 bytes, packed at 2
#pragma pack(pop)
```

`flags` carries the `ILC_*` bits. The colour-depth bits are in `ILC_COLORMASK = 0xFE`:

| Constant       | Value  | Meaning |
| -------------- | ------ | ------- |
| `ILC_MASK`     | 0x0001 | a 1bpp monochrome mask DIB follows the color DIB |
| `ILC_COLOR`    | 0x0000 | default — driver chooses |
| `ILC_COLOR4`   | 0x0004 | 4 bpp |
| `ILC_COLOR8`   | 0x0008 | 8 bpp |
| `ILC_COLOR16`  | 0x0010 | 16 bpp |
| `ILC_COLOR24`  | 0x0018 | 24 bpp |
| `ILC_COLOR32`  | 0x0020 | 32 bpp ARGB |

`ImageList_Write` body, from Wine `imagelist.c` — verbatim summary:

> "Writes the ILHEAD structure, the color bitmap as a DIB (BITMAPFILEHEADER +
> BITMAPINFOHEADER, biCompression = BI_RGB), and — only if ILC_MASK is set —
> the mask bitmap as a 1-bpp DIB, with no further framing."

```c
IStream_Write(pstm, &ilHead, sizeof(ILHEAD), NULL);
_write_bitmap(himl->hbmImage, pstm);             // color DIB
if (himl->flags & ILC_MASK)
    _write_bitmap(himl->hbmMask, pstm);          // monochrome mask DIB
```

### DIB layout (`_write_bitmap`)

```
BITMAPFILEHEADER  (14 bytes)
  WORD  bfType        = 'BM' = 0x4D42
  DWORD bfSize        = headers-plus-palette (NB: a quirk — not full size)
  WORD  bfReserved1   = 0
  WORD  bfReserved2   = 0
  DWORD bfOffBits     = offset to pixel data

BITMAPINFOHEADER  (40 bytes)
  DWORD biSize        = 40
  LONG  biWidth
  LONG  biHeight      // positive = bottom-up scan order
  WORD  biPlanes      = 1
  WORD  biBitCount    = 4, 8, 16, 24, or 32
  DWORD biCompression = BI_RGB (0)
  DWORD biSizeImage   = stride(width, bitCount) * height
  ...
optional palette (for biBitCount <= 8 only): (1 << biBitCount) * RGBQUAD (4 bytes each)
pixel data         (biSizeImage bytes)
```

Per layer:

- **Color DIB.** Single bitmap whose width is `cx` per tile, packed across columns
  and rows. `ImageList_Read` decides geometry from `ilHead.cCurImage` and `cy`:
  for the 32 bpp + `ILC_COLOR32` path it walks tiles `TILE_COUNT = 4` at a time
  and the bitmap is a grid; lower-depth paths use a flat strip. The safest
  decoding rule is the one `ImageList_Read` uses for 32 bpp + mask: pixel
  `(x, y)` of image `i` lives at strip coordinate
  `(((i % cols) × cx) + x, ((i / cols) × cy) + y)`, where `cols = bmWidth / cx`.
  For ≤ 24 bpp paths the strip is just one row of tiles wide
  (`bmWidth = cCurImage × cx`).
- **Mask DIB** (only if `ILC_MASK`). Same width × height as the color strip,
  but `biBitCount = 1`. Bit `0` = transparent, bit `1` = opaque (Win32 GDI
  AND-mask convention).
- **Endianness.** All multi-byte fields are little-endian. For 32 bpp the pixel
  order in memory is `B, G, R, A`.
- **Stride / scan-line padding.** DWORD-aligned:
  `stride = ((width × bitCount + 31) / 32) × 4`. `biSizeImage = stride × height`.
  For 1 bpp masks: `((width + 31) / 32) × 4`.
- **Row order.** Bottom-up when `biHeight > 0` (the case `_write_bitmap`
  writes). Decoder must flip vertically when materialising.
- **Alpha.** Despite `biBitCount = 32` and `biCompression = BI_RGB`, the high
  byte *is* alpha when `ILC_COLOR32` was set. No `BITMAPV5HEADER`, no
  `bV5AlphaMask` — the alpha is conventional. For lower depths
  (`ILC_COLOR24` etc.), transparency comes entirely from the mask DIB.
- **Transparent-colour key.** `ILHEAD.bkcolor` is the design-time background
  colour (Win32 `COLORREF`, `0x00BBGGRR`, or `CLR_NONE = 0xFFFFFFFF`). It is
  **not** a per-image transparent-pixel key — the mask owns transparency.

---

## Stability

- **Framework versions.** The encoding hasn't changed since at least .NET
  Framework 1.1 — `.resx` files compiled against one runtime are routinely read
  by another. The `dotnet/winforms` source above is what ships in .NET 6/7/8/9/10
  and the out-of-tree `Microsoft.WindowsDesktop.App` runtime pack. The
  decompress side's `MSFt` fall-through has never been removed.
- **`ImageList.ColorDepth`.** Only changes `ILHEAD.flags` (`ILC_COLOR*` bits)
  and `biBitCount`. **Default flipped from `Depth8Bit` (≤ .NET 7) to
  `Depth32Bit` (.NET 8+)** — fixture generation needs to cover both.
- **High DPI / Win10/11.** The serialized stream encodes pixel dimensions, not
  logical sizes. WinForms' DPI work hasn't touched the wire bytes — a 16 × 16
  image generated in a 200% DPI session is still a 16 × 16 DIB unless the
  caller pre-scaled.

## Sources

- [`ImageListStreamer.cs` — dotnet/winforms](https://github.com/dotnet/winforms/blob/main/src/System.Windows.Forms/System/Windows/Forms/Controls/ImageList/ImageListStreamer.cs)
  — `HeaderMagic = "MSFt"`, `Compress`/`Decompress`, `GetObjectData` adds
  `"Data"` byte[], deserialisation constructor reads `"Data"`.
- [`RunLengthEncoder.cs` — dotnet/winforms](https://github.com/dotnet/winforms/blob/main/src/System.Private.Windows.Core/src/System/IO/Compression/RunLengthEncoder.cs)
  — "byte for the count, followed by a byte for the value", `0xFF` max per pair.
- [`imagelist.c` — wine-mirror/wine](https://github.com/wine-mirror/wine/blob/master/dlls/comctl32/imagelist.c)
  — `ILHEAD` layout, `ImageList_Write`, `_write_bitmap`, `ImageList_Read`,
  `ILC_*` flags, magic `'IL'` / version `0x0101`.
- [BinaryFormatter migration guide — Read NRBF payloads](https://learn.microsoft.com/en-us/dotnet/standard/serialization/binaryformatter-migration-guide/read-nrbf-payloads)
  — `NrbfDecoder`, `ClassRecord`, `SZArrayRecord<byte>` usage on `net10.0`.
- [[MS-NRBF] SerializationHeaderRecord](https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-nrbf/a7e578d3-400a-4249-9424-7529d10d1b3c)
  — record-type enumeration.
- [BITMAPINFOHEADER (wingdi.h) — Microsoft Learn](https://learn.microsoft.com/en-us/windows/win32/api/wingdi/ns-wingdi-bitmapinfoheader)
  — DIB stride / `BI_RGB` semantics.
- [ImageList.ColorDepth Property — Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.imagelist.colordepth)
  — default `Depth8Bit` pre-.NET 8, `Depth32Bit` thereafter.
- Mono `ImageList.cs` / `ImageListStreamer.cs` (MIT) — closest existing pure-C#
  template for an in-memory reader.
