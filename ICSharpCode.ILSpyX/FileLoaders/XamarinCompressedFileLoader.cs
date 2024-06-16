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
			var magic = fileReader.ReadUInt32();
			if (magic != CompressedDataMagic)
				return null;
			_ = fileReader.ReadUInt32(); // skip index into descriptor table, unused
			int uncompressedLength = (int)fileReader.ReadUInt32();
			int compressedLength = (int)stream.Length;  // Ensure we read all of compressed data
			ArrayPool<byte> pool = ArrayPool<byte>.Shared;
			var src = pool.Rent(compressedLength);
			var dst = pool.Rent(uncompressedLength);
			try
			{
				// fileReader stream position is now at compressed module data
				await stream.ReadAsync(src, 0, compressedLength).ConfigureAwait(false);
				// Decompress
				LZ4Codec.Decode(src, 0, compressedLength, dst, 0, uncompressedLength);
				// Load module from decompressed data buffer
				using (var uncompressedStream = new MemoryStream(dst, writable: false))
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
