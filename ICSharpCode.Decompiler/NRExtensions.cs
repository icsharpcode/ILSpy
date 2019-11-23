// Copyright (c) 2015 Siegfried Pammer
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
using System.Collections.Generic;
using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler
{
	public static class NRExtensions
	{
		public static bool IsCompilerGenerated(this IEntity entity)
		{
			if (entity != null) {
				return entity.HasAttribute(KnownAttribute.CompilerGenerated);
			}
			return false;
		}
		
		public static bool IsCompilerGeneratedOrIsInCompilerGeneratedClass(this IEntity entity)
		{
			if (entity == null)
				return false;
			if (entity.IsCompilerGenerated())
				return true;
			return IsCompilerGeneratedOrIsInCompilerGeneratedClass(entity.DeclaringTypeDefinition);
		}
		
		public static bool HasGeneratedName(this IMember member)
		{
			return member.Name.StartsWith("<", StringComparison.Ordinal);
		}
		
		public static bool HasGeneratedName(this IType type)
		{
			return type.Name.StartsWith("<", StringComparison.Ordinal) || type.Name.Contains("<");
		}
		
		public static bool IsAnonymousType(this IType type)
		{
			if (type == null)
				return false;
			if (string.IsNullOrEmpty(type.Namespace) && type.HasGeneratedName() && (type.Name.Contains("AnonType") || type.Name.Contains("AnonymousType"))) {
				ITypeDefinition td = type.GetDefinition();
				return td != null && td.IsCompilerGenerated();
			}
			return false;
		}
		
		public static bool ContainsAnonymousType(this IType type)
		{
			var visitor = new ContainsAnonTypeVisitor();
			type.AcceptVisitor(visitor);
			return visitor.ContainsAnonType;
		}
		
		class ContainsAnonTypeVisitor : TypeVisitor
		{
			public bool ContainsAnonType;
			
			public override IType VisitOtherType(IType type)
			{
				if (IsAnonymousType(type))
					ContainsAnonType = true;
				return base.VisitOtherType(type);
			}
			
			public override IType VisitTypeDefinition(ITypeDefinition type)
			{
				if (IsAnonymousType(type))
					ContainsAnonType = true;
				return base.VisitTypeDefinition(type);
			}
		}

		internal static string GetDocumentation(this IEntity entity)
		{
			var docProvider = XmlDocLoader.LoadDocumentation(entity.ParentModule.PEFile);
			if (docProvider == null)
				return null;
			return docProvider.GetDocumentation(entity);
		}
	}
}
