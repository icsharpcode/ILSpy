// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)

using System;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;
using Ricciolo.StylesExplorer.MarkupReflection;

namespace ILSpy.BamlDecompiler
{
	public class NRTypeDependencyPropertyDescriptor : IDependencyPropertyDescriptor
	{
		readonly ITypeDefinition typeDefinition;
		readonly string member;

		public NRTypeDependencyPropertyDescriptor(ITypeDefinition type, string name)
		{
			this.typeDefinition = type;
			this.member = name;
		}
		
		public bool IsAttached {
			get {
				return typeDefinition.GetMethods(m => m.Name == "Get" + member, GetMemberOptions.IgnoreInheritedMembers).Any();
			}
		}
		
		public override string ToString()
		{
			return string.Format("[CecilDependencyPropertyDescriptor Member={0}, Type={1}]", member, typeDefinition);
		}
	}
}
