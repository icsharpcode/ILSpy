// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)

using System;
using System.Linq;
using Mono.Cecil;
using Ricciolo.StylesExplorer.MarkupReflection;

namespace ILSpy.BamlDecompiler
{
	public class CecilDependencyPropertyDescriptor : IDependencyPropertyDescriptor
	{
		readonly string member;
		readonly TypeDefinition type;
		
		public CecilDependencyPropertyDescriptor(string member, TypeDefinition type)
		{
			this.member = member;
			this.type = type ?? throw new ArgumentNullException("type");
		}
		
		public bool IsAttached {
			get {
				return type.Methods.Any(m  => m.Name == "Get" + member);
			}
		}
		
		public override string ToString()
		{
			return string.Format("[CecilDependencyPropertyDescriptor Member={0}, Type={1}]", member, type);
		}
	}
}
