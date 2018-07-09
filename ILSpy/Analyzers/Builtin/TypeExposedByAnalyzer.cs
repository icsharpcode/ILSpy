using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	[Export(typeof(IAnalyzer<ITypeDefinition>))]
	class TypeExposedByAnalyzer : ITypeDefinitionAnalyzer<ITypeDefinition>
	{
		public string Text => "Exposed By";

		public bool Show(ITypeDefinition entity) => true;

		public IEnumerable<IEntity> Analyze(ITypeDefinition analyzedType, ITypeDefinition type, AnalyzerContext context)
		{
			if (analyzedType.Kind == TypeKind.Enum
				&& type.MetadataToken == analyzedType.MetadataToken
				&& type.ParentAssembly.PEFile == analyzedType.ParentAssembly.PEFile)
				yield break;

			if (!context.Language.ShowMember(type))
				yield break;

			var visitor = new TypeDefinitionUsedVisitor(analyzedType);

			foreach (IField field in type.Fields) {
				if (TypeIsExposedBy(visitor, field))
					yield return field;
			}

			foreach (IProperty property in type.Properties) {
				if (TypeIsExposedBy(visitor, property))
					yield return property;
			}

			foreach (IEvent @event in type.Events) {
				if (TypeIsExposedBy(visitor, @event))
					yield return @event;
			}

			foreach (IMethod method in type.Methods) {
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
			if (property.Accessibility == Accessibility.Private) {
				if (!property.IsExplicitInterfaceImplementation)
					return false;
			}

			visitor.Found = false;
			property.ReturnType.AcceptVisitor(visitor);

			foreach (var p in property.Parameters) {
				p.Type.AcceptVisitor(visitor);
			}

			return visitor.Found;
		}

		bool TypeIsExposedBy(TypeDefinitionUsedVisitor visitor, IEvent @event)
		{
			if (@event.Accessibility == Accessibility.Private) {
				if (!@event.IsExplicitInterfaceImplementation)
					return false;
			}

			visitor.Found = false;
			@event.ReturnType.AcceptVisitor(visitor);

			return visitor.Found;
		}

		bool TypeIsExposedBy(TypeDefinitionUsedVisitor visitor, IMethod method)
		{
			if (method.Accessibility == Accessibility.Private) {
				if (!method.IsExplicitInterfaceImplementation)
					return false;
			}

			visitor.Found = false;
			method.ReturnType.AcceptVisitor(visitor);

			foreach (var p in method.Parameters) {
				p.Type.AcceptVisitor(visitor);
			}

			return false;
		}
	}
}
