// Copyright (c) 2024 Christoph Wille
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

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	/// <summary>
	/// Centralizes all file-path generation for compilation output (assemblies)
	/// Here a redirect can be added to a different location for the output (ie a directory that is excluded from virus scanning)
	/// </summary>
	internal static class TestsAssemblyOutput
	{
		public static string GetFilePath(string testCasePath, string testName, string computedExtension)
		{
			return Path.Combine(testCasePath, testName) + computedExtension;
		}

		public static string GetTempFileName()
		{
			return Path.GetTempFileName();
		}
	}
}
