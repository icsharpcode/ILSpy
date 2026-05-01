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
using System.IO;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.IL;

using ILSpy.Languages;

namespace ILSpy.TreeNodes
{
	/// <summary>
	/// One entry inside a <see cref="ResourcesFileTreeNode"/>'s <c>.resources</c> file —
	/// a key plus an opener for the underlying byte payload.
	/// </summary>
	public class ResourceEntryNode : ILSpyTreeNode
	{
		readonly string key;
		readonly Func<Stream> openStream;

		public ResourceEntryNode(string key, Func<Stream> openStream)
		{
			this.key = key ?? throw new ArgumentNullException(nameof(key));
			this.openStream = openStream ?? throw new ArgumentNullException(nameof(openStream));
		}

		public override object Text => ILAmbience.EscapeName(key);

		public override object Icon => Images.Images.Resource;

		protected Stream OpenStream() => openStream();

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			using var data = OpenStream();
			language.WriteCommentLine(output, $"{key} = {data.Length} bytes");
		}

		public static ILSpyTreeNode Create(string name, byte[] data)
			=> new ResourceEntryNode(name, () => new MemoryStream(data));
	}
}
