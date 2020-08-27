// Copyright (c) 2018 Siegfried Pammer
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
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Finds all entities that expose a type.
	/// </summary>
	[ExportAnalyzer(Header = "Exposed By", Order = 40)]
	class TypeExposedByAnalyzer : IAnalyzer
	{
		public bool Show(ISymbol entity) => entity is ITypeDefinition;

		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			Debug.Assert(analyzedSymbol is ITypeDefinition);
			var scope = context.GetScopeOf((ITypeDefinition)analyzedSymbol);
			foreach (var type in scope.GetTypesInScope(context.CancellationToken))
			{
				foreach (var result in ScanType((ITypeDefinition)analyzedSymbol, type, context))
					yield return result;
			}
		}

		IEnumerable<IEntity> ScanType(ITypeDefinition analyzedType, ITypeDefinition type, AnalyzerContext context)
		{
			if (analyzedType.Kind == TypeKind.Enum
				&& type.MetadataToken == analyzedType.MetadataToken
				&& type.ParentModule.PEFile == analyzedType.ParentModule.PEFile)
				yield break;

			if (!context.Language.ShowMember(type))
				yield break;

			var visitor = new TypeDefinitionUsedVisitor(analyzedType, true);

			foreach (IField field in type.Fields)
			{
				if (TypeIsExposedBy(visitor, field))
					yield return field;
			}

			foreach (IProperty property in type.Properties)
			{
				if (TypeIsExposedBy(visitor, property))
					yield return property;
			}

			foreach (IEvent @event in type.Events)
			{
				if (TypeIsExposedBy(visitor, @event))
					yield return @event;
			}

			foreach (IMethod method in type.Methods)
			{
				if (TypeIsExposedBy(visitor, method))
					yield return method;
			}
		}

		bool TypeIsExposedBy(TypeDefinitionUsedVisitor visitor, IField field)
		{
			if (field.Accessibility == Accessibility.Private)
				return false;

			visitor.Found = false;
			field.ReturnType.AcceptVisitor(visitor);

			return visitor.Found;
		}

		bool TypeIsExposedBy(TypeDefinitionUsedVisitor visitor, IProperty property)
		{
			if (property.Accessibility == Accessibility.Private)
			{
				if (!property.IsExplicitInterfaceImplementation)
					return false;
			}

			visitor.Found = false;
			property.ReturnType.AcceptVisitor(visitor);

			foreach (var p in property.Parameters)
			{
				p.Type.AcceptVisitor(visitor);
			}

			return visitor.Found;
		}

		bool TypeIsExposedBy(TypeDefinitionUsedVisitor visitor, IEvent @event)
		{
			if (@event.Accessibility == Accessibility.Private)
			{
				if (!@event.IsExplicitInterfaceImplementation)
					return false;
			}

			visitor.Found = false;
			@event.ReturnType.AcceptVisitor(visitor);

			return visitor.Found;
		}

		bool TypeIsExposedBy(TypeDefinitionUsedVisitor visitor, IMethod method)
		{
			if (method.Accessibility == Accessibility.Private)
			{
				if (!method.IsExplicitInterfaceImplementation)
					return false;
			}

			visitor.Found = false;
			method.ReturnType.AcceptVisitor(visitor);

			foreach (var p in method.Parameters)
			{
				p.Type.AcceptVisitor(visitor);
			}

			return visitor.Found;
		}
	}
}
