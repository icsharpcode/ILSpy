// Copyright (c) 2024 Siegfried Pammer
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

using System.Buffers;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Metadata;

using K4os.Compression.LZ4;

namespace ICSharpCode.ILSpyX.FileLoaders
{
	public sealed class XamarinCompressedFileLoader : IFileLoader
	{
		public async Task<LoadResult?> Load(string fileName, Stream stream, FileLoadContext context)
		{
			const uint CompressedDataMagic = 0x5A4C4158; // Magic used for Xamarin compressed module header ('XALZ', little-endian)
			using var fileReader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
			// Read compressed file header
			uint magic;
			try
			{
				magic = fileReader.ReadUInt32();
			}
			catch (EndOfStreamException)
			{
				// Too short to contain the magic, so this cannot be an XALZ module; pass it through.
				return null;
			}
			if (magic != CompressedDataMagic)
				return null;
			uint declaredUncompressedLength;
			try
			{
				_ = fileReader.ReadUInt32(); // skip index into descriptor table, unused
				declaredUncompressedLength = fileReader.ReadUInt32();
			}
			catch (EndOfStreamException ex)
			{
				// The magic identifies this as an XALZ module, so it must carry the full 12-byte
				// header: magic, descriptor table index, and uncompressed length. A shorter stream
				// is a truncated/corrupt module; fail consistently as InvalidDataException.
				throw new InvalidDataException("Invalid Xamarin compressed module: truncated header.", ex);
			}
			// The compressed payload is whatever follows the 12-byte header, not the whole file.
			long compressedLength = stream.Length - stream.Position;
			// The declared uncompressed length is attacker-controlled. Reject implausible values
			// before renting buffers: a negative/oversized size (both lengths are sized for
			// int-based ArrayPool and MemoryStream), or one larger than any LZ4 block of this
			// payload could possibly produce. An LZ4 block expands by at most 255x, so a smaller
			// payload claiming a larger output is a malformed or decompression-bomb header.
			const long MaxLZ4ExpansionRatio = 255;
			if (compressedLength <= 0 || compressedLength > int.MaxValue
				|| declaredUncompressedLength == 0
				|| declaredUncompressedLength > int.MaxValue
				|| declaredUncompressedLength > compressedLength * MaxLZ4ExpansionRatio)
			{
				throw new InvalidDataException("Invalid Xamarin compressed module: declared length is out of range.");
			}
			int uncompressedLength = (int)declaredUncompressedLength;
			int compressed = (int)compressedLength;
			ArrayPool<byte> pool = ArrayPool<byte>.Shared;
			var src = pool.Rent(compressed);
			var dst = pool.Rent(uncompressedLength);
			try
			{
				// fileReader stream position is now at compressed module data.
				// stream.Length above is only a sizing hint; if the stream ends early (e.g. the
				// file was truncated after the length was queried), treat that as corrupt input
				// rather than surfacing EndOfStreamException.
				try
				{
					await stream.ReadExactlyAsync(src, 0, compressed).ConfigureAwait(false);
				}
				catch (EndOfStreamException ex)
				{
					throw new InvalidDataException("Invalid Xamarin compressed module: truncated payload.", ex);
				}
				// Decompress; Decode returns the number of bytes written, or negative on failure.
				// The header declares the exact decompressed size, so anything other than an exact
				// match (a negative error code or a short, truncated decode) means the payload is
				// corrupt and must not be parsed as a partial module.
				int decodedLength = LZ4Codec.Decode(src, 0, compressed, dst, 0, uncompressedLength);
				if (decodedLength != uncompressedLength)
				{
					throw new InvalidDataException("Invalid Xamarin compressed module: decompressed size does not match the header.");
				}
				// Load module from the decompressed data buffer, sliced to the declared length (the
				// rented buffer may be larger than the decompressed data).
				using (var uncompressedStream = new MemoryStream(dst, 0, uncompressedLength, writable: false))
				{
					MetadataReaderOptions options = context.ApplyWinRTProjections
						? MetadataReaderOptions.ApplyWindowsRuntimeProjections
						: MetadataReaderOptions.None;

					return new LoadResult {
						MetadataFile = new PEFile(fileName, uncompressedStream, PEStreamOptions.PrefetchEntireImage, metadataOptions: options)
					};
				}
			}
			finally
			{
				pool.Return(dst);
				pool.Return(src);
			}
		}
	}
}
