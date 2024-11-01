// Copyright (c) 2024 Tom Englert for the SharpDevelop Team
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
using System.Diagnostics;

namespace ICSharpCode.ILSpy.Util
{
	static class GlobalUtils
	{
		public static void OpenLink(string link)
		{
			try
			{
				Process.Start(new ProcessStartInfo { FileName = link, UseShellExecute = true });
			}
			catch (Exception)
			{
				// Process.Start can throw several errors (not all of them documented),
				// just ignore all of them.
			}
		}

		public static void ExecuteCommand(string fileName, string arguments)
		{
			try
			{
				Process.Start(fileName, arguments);
			}
			catch (Exception)
			{
				// Process.Start can throw several errors (not all of them documented),
				// just ignore all of them.
			}
		}
	}
}