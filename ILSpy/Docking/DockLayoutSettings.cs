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
				return;

			using (StringReader reader = new StringReader(rawSettings)) {
				serializer.Deserialize(reader);
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
