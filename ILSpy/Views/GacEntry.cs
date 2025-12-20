// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Text;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy
{
	public partial class OpenFromGacDialog
	{
		sealed class GacEntry
		{
			readonly AssemblyNameReference r;
			readonly string fileName;
			string formattedVersion;

			public GacEntry(AssemblyNameReference r, string fileName)
			{
				this.r = r;
				this.fileName = fileName;
			}

			public string FullName {
				get { return r.FullName; }
			}

			public string ShortName {
				get { return r.Name; }
			}

			public string FileName {
				get { return fileName; }
			}

			public Version Version {
				get { return r.Version; }
			}

			public string FormattedVersion {
				get {
					if (formattedVersion == null)
						formattedVersion = Version.ToString();
					return formattedVersion;
				}
			}

			public string Culture {
				get {
					if (string.IsNullOrEmpty(r.Culture))
						return "neutral";
					return r.Culture;
				}
			}

			public string PublicKeyToken {
				get {
					StringBuilder s = new StringBuilder();
					foreach (byte b in r.PublicKeyToken)
						s.Append(b.ToString("x2"));
					return s.ToString();
				}
			}

			public override string ToString()
			{
				return r.FullName;
			}
		}
	}
}