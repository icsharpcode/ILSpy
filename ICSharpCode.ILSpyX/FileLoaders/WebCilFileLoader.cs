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

using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpyX.FileLoaders
{
	public sealed class WebCilFileLoader : IFileLoader
	{
		public Task<LoadResult?> Load(string fileName, Stream stream, FileLoadContext settings)
		{
			if (settings.ParentBundle != null)
			{
				return Task.FromResult<LoadResult?>(null);
			}

			MetadataReaderOptions options = settings.ApplyWinRTProjections
							? MetadataReaderOptions.ApplyWindowsRuntimeProjections
							: MetadataReaderOptions.None;

			var wasm = WebCilFile.FromFile(fileName, options);
			var result = wasm != null ? new LoadResult { MetadataFile = wasm } : null;
			return Task.FromResult(result);
		}
	}
}
