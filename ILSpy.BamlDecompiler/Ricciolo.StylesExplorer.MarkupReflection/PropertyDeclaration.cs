// Copyright (c) Cristian Civera (cristian@aspitalia.com)
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)


namespace Ricciolo.StylesExplorer.MarkupReflection
{
	internal class PropertyDeclaration
	{
		// Methods
		public PropertyDeclaration(string name)
		{
			this.Name = name;
			this.DeclaringType = null;
		}

		public PropertyDeclaration(string name, TypeDeclaration declaringType)
		{
			this.Name = name;
			this.DeclaringType = declaringType;
		}

		public override string ToString()
		{
			if (((this.DeclaringType != null) && (this.DeclaringType.Name == "XmlNamespace")) && ((this.DeclaringType.Namespace == null) && (this.DeclaringType.Assembly == null)))
			{
				if ((this.Name == null) || (this.Name.Length == 0))
				{
					return "xmlns";
				}
				return ("xmlns:" + this.Name);
			}
			return this.Name;
		}

		// Properties
		public TypeDeclaration DeclaringType { get; }

		public string Name { get; }
	}
}
