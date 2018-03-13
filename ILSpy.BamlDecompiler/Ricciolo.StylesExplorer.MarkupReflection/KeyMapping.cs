// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)

using System.Collections.Generic;

namespace Ricciolo.StylesExplorer.MarkupReflection
{
	public class KeyMapping
	{
		public List<object> StaticResources { get; }

		public bool HasStaticResource(int identifier)
		{
			return StaticResources != null && StaticResources.Count > identifier;
		}
		
		public string KeyString { get; set; }
		public bool Shared { get; set; }
		public bool SharedSet { get; set; }
		
		public int Position { get; set; }
		
		public KeyMapping()
		{
			this.StaticResources = new List<object>();
			this.Position = -1;
		}
		
		public KeyMapping(string key)
		{
			this.KeyString = key;
			this.StaticResources = new List<object>();
			this.Position = -1;
		}
		
		public override string ToString()
		{
			return '"' + KeyString + '"';
		}

	}
}
