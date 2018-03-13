// Copyright (c) Cristian Civera (cristian@aspitalia.com)
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)

using System;

namespace Ricciolo.StylesExplorer.MarkupReflection
{
	internal class XmlBamlPropertyElement : XmlBamlElement
	{
		public XmlBamlPropertyElement(PropertyType propertyType, PropertyDeclaration propertyDeclaration)
		{
			PropertyType = propertyType;
			this.PropertyDeclaration = propertyDeclaration;
		}

		public XmlBamlPropertyElement(XmlBamlElement parent, PropertyType propertyType, PropertyDeclaration propertyDeclaration)
			: base(parent)
		{
			PropertyType = propertyType;
			this.PropertyDeclaration = propertyDeclaration;
			this.TypeDeclaration = propertyDeclaration.DeclaringType;
		}

		public PropertyDeclaration PropertyDeclaration { get; }

		public PropertyType PropertyType { get; }

		public override string ToString()
		{
			return String.Format("PropertyElement: {0}.{1}", TypeDeclaration.Name.Replace('+', '.'), PropertyDeclaration.Name);
		}
	}
}
