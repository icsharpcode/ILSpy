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

using System;
using System.Collections.Generic;

namespace ICSharpCode.ILSpyX.FileLoaders
{
	public sealed class FileLoaderRegistry
	{
		readonly List<IFileLoader> registeredLoaders = new List<IFileLoader>();

		public IReadOnlyList<IFileLoader> RegisteredLoaders => registeredLoaders;

		public void Register(IFileLoader loader)
		{
			if (loader is null)
			{
				throw new ArgumentNullException(nameof(loader));
			}

			registeredLoaders.Add(loader);
		}

		public FileLoaderRegistry()
		{
			Register(new XamarinCompressedFileLoader());
			Register(new WebCilFileLoader());
			Register(new MetadataFileLoader());
			Register(new BundleFileLoader());
			Register(new ArchiveFileLoader());
		}
	}
}
