// Copyright (c) 2020 Daniel Grunwald
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

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.CSharp.ProjectDecompiler
{
	/// <summary>
	/// An interface that provides common information for a project being decompiled to.
	/// </summary>
	interface IProjectInfoProvider
	{
		/// <summary>
		/// Gets the assembly resolver active for the project.
		/// </summary>
		IAssemblyResolver AssemblyResolver { get; }

		/// <summary>
		/// Gets the C# language version of the project.
		/// </summary>
		LanguageVersion LanguageVersion { get; }

		/// <summary>
		/// Gets the unique ID of the project.
		/// </summary>
		Guid ProjectGuid { get; }

		/// <summary>
		/// Gets the target directory of the project
		/// </summary>
		string TargetDirectory { get; }

		/// <summary>
		/// Gets the name of the key file being used for strong name signing. Can be null if no file is available.
		/// </summary>
		string StrongNameKeyFile { get; }
	}
}