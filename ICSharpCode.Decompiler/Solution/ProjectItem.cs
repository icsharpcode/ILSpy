// Copyright (c) 2019 AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.Decompiler.Solution
{
	/// <summary>
	/// A container class that holds information about a Visual Studio project.
	/// </summary>
	public sealed class ProjectItem : ProjectId
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectItem"/> class.
		/// </summary>
		/// <param name="projectFile">The full path of the project file.</param>
		/// <param name="projectPlatform">The project platform.</param>
		/// <param name="projectGuid">The project GUID.</param>
		/// 
		/// <exception cref="ArgumentException">Thrown when <paramref name="projectFile"/> 
		/// or <paramref name="projectPlatform"/> is null or empty.</exception>
		public ProjectItem(string projectFile, string projectPlatform, Guid projectGuid)
			: base(projectPlatform, projectGuid)
		{
			ProjectName = Path.GetFileNameWithoutExtension(projectFile);
			FilePath = projectFile;
		}

		/// <summary>
		/// Gets the name of the project.
		/// </summary>
		public string ProjectName { get; }

		/// <summary>
		/// Gets the full path to the project file.
		/// </summary>
		public string FilePath { get; }
	}
}
