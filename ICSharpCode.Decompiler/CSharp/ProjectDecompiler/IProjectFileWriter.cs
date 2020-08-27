// Copyright (c) 2020 Siegfried Pammer
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
using System.IO;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.CSharp.ProjectDecompiler
{
	/// <summary>
	/// An interface for a service that creates and writes a project file structure
	/// for a specific module being decompiled.
	/// </summary>
	interface IProjectFileWriter
	{
		/// <summary>
		/// Writes the content of a new project file for the specified <paramref name="module"/> being decompiled.
		/// </summary>
		/// <param name="target">The target to write to.</param>
		/// <param name="project">The information about the project being created.</param>
		/// <param name="files">A collection of source files to be included into the project, each item is a pair
		/// of the project entry type and the file path.</param>
		/// <param name="module">The module being decompiled.</param>
		void Write(TextWriter target, IProjectInfoProvider project, IEnumerable<(string itemType, string fileName)> files, PEFile module);
	}
}
