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


using System;
using System.IO;

using Microsoft.Extensions.Configuration;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	/// <summary>
	/// Centralizes all file-path generation for compilation output (assemblies)
	/// 
	/// DecompilerTests.config.json file format:
	/// {
	/// 	"TestsAssemblyTempPath": "d:\\test\\"
	/// }
	/// </summary>
	internal static class TestsAssemblyOutput
	{
		static string? TestsAssemblyTempPath = null;

		private static bool UseCustomPath => !string.IsNullOrWhiteSpace(TestsAssemblyTempPath);

		static TestsAssemblyOutput()
		{
			if (!File.Exists("DecompilerTests.config.json"))
				return;

			var builder = new ConfigurationBuilder()
				.AddJsonFile("DecompilerTests.config.json", optional: true, reloadOnChange: false);

			IConfigurationRoot configuration = builder.Build();
			var pathRedirectIfAny = configuration["TestsAssemblyTempPath"];

			if (!string.IsNullOrWhiteSpace(pathRedirectIfAny))
			{
				TestsAssemblyTempPath = pathRedirectIfAny;
			}
		}

		public static string GetFilePath(string testCasePath, string testName, string computedExtension)
		{
			if (!UseCustomPath)
				return Path.Combine(testCasePath, testName) + computedExtension;

			// As we are using the TestsAssemblyTempPath flat, we need to make sure that duplicated test names don't create file name clashes
			return Path.Combine(TestsAssemblyTempPath, testName) + Guid.NewGuid().ToString() + computedExtension;
		}

		public static string GetTempFileName()
		{
			if (!UseCustomPath)
				return Path.GetTempFileName();

			return Path.Combine(TestsAssemblyTempPath, Path.GetRandomFileName());
		}
	}
}
