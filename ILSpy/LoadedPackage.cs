// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// NuGet package or .NET bundle:
	/// </summary>
	public class LoadedPackage
	{
		public enum PackageKind
		{
			Zip,
		}

		public PackageKind Kind { get; }

		public List<PackageEntry> Entries { get; } = new List<PackageEntry>();

		public static LoadedPackage FromZipFile(string file)
		{
			using (var archive = ZipFile.OpenRead(file))
			{
				LoadedPackage result = new LoadedPackage();
				foreach (var entry in archive.Entries)
				{
					result.Entries.Add(new ZipFileEntry(file, entry));
				}
				return result;
			}
		}

		class ZipFileEntry : PackageEntry
		{
			readonly string zipFile;
			public override string Name { get; }
			public override string FullName => $"zip://{zipFile};{Name}";

			public ZipFileEntry(string zipFile, ZipArchiveEntry entry)
			{
				this.zipFile = zipFile;
				this.Name = entry.FullName;
			}

			public override Stream TryOpenStream()
			{
				Debug.WriteLine("Decompress " + Name);
				using (var archive = ZipFile.OpenRead(zipFile))
				{
					var entry = archive.GetEntry(Name);
					if (entry == null)
						return null;
					var memoryStream = new MemoryStream();
					using (var s = entry.Open())
					{
						s.CopyTo(memoryStream);
					}
					memoryStream.Position = 0;
					return memoryStream;
				}
			}
		}
	}

	public abstract class PackageEntry : Resource
	{
		/// <summary>
		/// Gets the file name of the entry (may include path components, relative to the package root).
		/// </summary>
		public abstract override string Name { get; }

		/// <summary>
		/// Gets the full file name for the entry.
		/// </summary>
		public abstract string FullName { get; }
	}
}
