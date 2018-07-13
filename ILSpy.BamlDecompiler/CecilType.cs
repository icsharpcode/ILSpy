// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)

using System;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;
using Ricciolo.StylesExplorer.MarkupReflection;

namespace ILSpy.BamlDecompiler
{
	public class NRType : IDotNetType
	{
		readonly ITypeDefinition type;

		public ITypeDefinition Type => type;
		
		public NRType(ITypeDefinition type)
		{
			this.type = type ?? throw new ArgumentNullException(nameof(type));
		}
		
		public string AssemblyQualifiedName {
			get {
				return type.FullName +
					", " + type.ParentModule.FullAssemblyName;
			}
		}
		
		public bool IsSubclassOf(IDotNetType type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			if (!(type is NRType baseType))
				return false;

			return this.type.GetAllBaseTypeDefinitions().Any(t => t.Equals(baseType.type));
		}
		
		public bool Equals(IDotNetType type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (!(type is NRType))
				return false;
			
			return this.type.Equals(((NRType)type).type);
		}
		
		public override string ToString()
		{
			return string.Format("[CecilType Type={0}]", type);
		}
		
		public IDotNetType BaseType {
			get {
				var t = type.DirectBaseTypes.First();
				var td = t.GetDefinition();
				if (td == null)
					throw new Exception($"Could not resolve '{t.FullName}'!");
				
				return new NRType(td);
			}
		}
	}
}
