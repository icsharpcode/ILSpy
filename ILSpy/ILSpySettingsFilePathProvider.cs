// Copyright (c) 2022 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy
{
	internal class ILSpySettingsFilePathProvider : ISettingsFilePathProvider
	{
		public string GetSettingsFilePath()
		{
			if (App.CommandLineArguments.ConfigFile != null)
				return App.CommandLineArguments.ConfigFile;
			string localPath = Path.Combine(Path.GetDirectoryName(typeof(MainWindow).Assembly.Location), "ILSpy.xml");
			if (File.Exists(localPath))
				return localPath;
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ICSharpCode\\ILSpy.xml");
		}
	}
}
