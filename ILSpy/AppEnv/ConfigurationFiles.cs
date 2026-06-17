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

using System.IO;

using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AppEnv
{
	/// <summary>
	/// Resolves auxiliary configuration files that live as JSON sidecars next to the main
	/// <c>ILSpy.xml</c> settings file (the dock layout, the bookmark list, ...). Keeping them
	/// in the same directory the XML settings live in — local-to-binary on portable installs,
	/// %APPDATA%/ICSharpCode/ otherwise — makes "delete settings to reset" a single-folder action.
	/// </summary>
	public static class ConfigurationFiles
	{
		/// <summary>
		/// Returns the full path for <paramref name="fileName"/> in the settings directory.
		/// Falls back to a bare relative name when the settings path can't be resolved (e.g. in
		/// headless tests that never set <see cref="ILSpySettings.SettingsFilePathProvider"/>).
		/// </summary>
		public static string GetPath(string fileName)
		{
			var xmlPath = ILSpySettings.SettingsFilePathProvider?.Invoke();
			if (string.IsNullOrEmpty(xmlPath))
				return fileName;
			var dir = Path.GetDirectoryName(xmlPath);
			return string.IsNullOrEmpty(dir) ? fileName : Path.Combine(dir, fileName);
		}
	}
}
