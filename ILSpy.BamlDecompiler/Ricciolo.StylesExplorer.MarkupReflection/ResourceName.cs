// Copyright (c) Cristian Civera (cristian@aspitalia.com)
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)


namespace Ricciolo.StylesExplorer.MarkupReflection
{
	public class ResourceName
	{
		public ResourceName(string name)
		{
			this.Name = name;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public string Name { get; }
	}
}
