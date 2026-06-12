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
using System.Composition;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.ILSpyX.Abstractions;

namespace ICSharpCode.ILSpy.ImageList
{
	/// <summary>
	/// MEF-discovered factory that claims serialized <c>ImageListStreamer</c> entries inside
	/// a <c>.resources</c> file and wraps them in an <see cref="ImageListResourceEntryNode"/>.
	/// Claims by matching the entry's serialized type-name prefix; doesn't try to deserialize
	/// at factory time so opening a tree node stays cheap.
	/// </summary>
	[Export(typeof(IResourceNodeFactory))]
	[Shared]
	public sealed class ImageListResourceNodeFactory : IResourceNodeFactory
	{
		// We don't claim plain Resource entries — only serialized-object entries get the
		// ImageListStreamer treatment, and those come through the (string, ResourceSerializedObject) overload.
		public ITreeNode? CreateNode(Resource resource) => null;

		public ITreeNode? CreateNode(string key, ResourceSerializedObject serializedObject)
		{
			ArgumentNullException.ThrowIfNull(key);
			ArgumentNullException.ThrowIfNull(serializedObject);
			var typeName = serializedObject.TypeName;
			if (typeName == null
				|| !typeName.StartsWith(ImageListDecoder.TargetTypeNamePrefix, StringComparison.Ordinal))
			{
				return null;
			}
			return new ImageListResourceEntryNode(key, serializedObject.GetStream);
		}
	}
}
