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
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace ICSharpCode.ILSpy.Docking
{
	public class DockLayoutSettings
	{
		private string rawSettings;

		public bool Valid => rawSettings != null;

		public DockLayoutSettings(XElement element)
		{
			if ((element != null) && element.HasElements) {
				rawSettings = element.Elements().FirstOrDefault()?.ToString();
			}
		}

		public XElement SaveAsXml()
		{
			try {
				return XElement.Parse(rawSettings);
			} catch (Exception) {
				return null;
			}
		}

		public void Deserialize(XmlLayoutSerializer serializer)
		{
			if (!Valid)
				rawSettings = "<LayoutRoot />";
			try {
				Deserialize(rawSettings);
			} catch (Exception) {
				Deserialize("<LayoutRoot />");
			}

			void Deserialize(string settings)
			{
				using (StringReader reader = new StringReader(settings)) {
					serializer.Deserialize(reader);
				}
			}
		}

		public void Serialize(XmlLayoutSerializer serializer)
		{
			using (StringWriter fs = new StringWriter()) {
				serializer.Serialize(fs);
				rawSettings = fs.ToString();
			}
		}
	}
}
