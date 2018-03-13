// Copyright (c) Cristian Civera (cristian@aspitalia.com)
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)

using System.Xml;

namespace Ricciolo.StylesExplorer.MarkupReflection
{
	internal class XmlBamlText : XmlBamlNode
	{
		public XmlBamlText(string text)
		{
			Text = text;
		}

		public string Text { get; }

		public override System.Xml.XmlNodeType NodeType => XmlNodeType.Text;
	}
}
