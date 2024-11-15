// Copyright (c) 2024 Holger Schmidt
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

using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.MermaidDiagrammer.Extensions;

namespace ICSharpCode.ILSpyX.MermaidDiagrammer
{
	using CD = ClassDiagrammer;

	partial class ClassDiagrammerFactory
	{
		private CD.Type BuildEnum(ITypeDefinition type)
		{
			IField[] fields = type.GetFields(f => f.IsConst && f.IsStatic && f.Accessibility == Accessibility.Public).ToArray();
			Dictionary<string, string>? docs = xmlDocs?.GetXmlDocs(type, fields);
			string name = GetName(type), typeId = GetId(type);

			var body = fields.Select(f => f.Name).Prepend("<<Enumeration>>")
				.Join(CD.NewLine + "    ", pad: true).TrimEnd(' ');

			return new CD.Type {
				Id = typeId,
				Name = name == typeId ? null : name,
				Body = $"class {typeId} {{{body}}}",
				XmlDocs = docs
			};
		}

		private CD.Type BuildType(ITypeDefinition type)
		{
			string typeId = GetId(type);
			IMethod[] methods = GetMethods(type).ToArray();
			IProperty[] properties = type.GetProperties().ToArray();
			IProperty[] hasOneRelations = GetHasOneRelations(properties);
			(IProperty property, IType elementType)[] hasManyRelations = GetManyRelations(properties);

			var propertyNames = properties.Select(p => p.Name).ToArray();
			IField[] fields = GetFields(type, properties);

			#region split members up by declaring type
			// enables the diagrammer to exclude inherited members from derived types if they are already rendered in a base type
			Dictionary<IType, IProperty[]> flatPropertiesByType = properties.Except(hasOneRelations)
				.Except(hasManyRelations.Select(r => r.property)).GroupByDeclaringType();

			Dictionary<IType, IProperty[]> hasOneRelationsByType = hasOneRelations.GroupByDeclaringType();
			Dictionary<IType, (IProperty property, IType elementType)[]> hasManyRelationsByType = hasManyRelations.GroupByDeclaringType(r => r.property);
			Dictionary<IType, IField[]> fieldsByType = fields.GroupByDeclaringType();
			Dictionary<IType, IMethod[]> methodsByType = methods.GroupByDeclaringType();
			#endregion

			#region build diagram definitions for the type itself and members declared by it
			string members = flatPropertiesByType.GetValue(type).FormatAll(FormatFlatProperty)
				.Concat(methodsByType.GetValue(type).FormatAll(FormatMethod))
				.Concat(fieldsByType.GetValue(type).FormatAll(FormatField))
				.Join(CD.NewLine + "    ", pad: true);

			// see https://mermaid.js.org/syntax/classDiagram.html#annotations-on-classes
			string? annotation = type.IsInterface() ? "Interface" : type.IsAbstract ? type.IsSealed ? "Service" : "Abstract" : null;

			string body = annotation == null ? members.TrimEnd(' ') : members + $"<<{annotation}>>" + CD.NewLine;
			#endregion

			Dictionary<string, string>? docs = xmlDocs?.GetXmlDocs(type, fields, properties, methods);

			#region build diagram definitions for inherited members by declaring type
			string explicitTypePrefix = typeId + " : ";

			// get ancestor types this one is inheriting members from
			Dictionary<string, CD.Type.InheritedMembers> inheritedMembersByType = type.GetNonInterfaceBaseTypes().Where(t => t != type && !t.IsObject())
				// and group inherited members by declaring type
				.ToDictionary(GetId, t => {
					IEnumerable<string> flatMembers = flatPropertiesByType.GetValue(t).FormatAll(p => explicitTypePrefix + FormatFlatProperty(p))
						.Concat(methodsByType.GetValue(t).FormatAll(m => explicitTypePrefix + FormatMethod(m)))
						.Concat(fieldsByType.GetValue(t).FormatAll(f => explicitTypePrefix + FormatField(f)));

					return new CD.Type.InheritedMembers {
						FlatMembers = flatMembers.Any() ? flatMembers.Join(CD.NewLine) : null,
						HasOne = MapHasOneRelations(hasOneRelationsByType, t),
						HasMany = MapHasManyRelations(hasManyRelationsByType, t)
					};
				});
			#endregion

			string typeName = GetName(type);

			return new CD.Type {
				Id = typeId,
				Name = typeName == typeId ? null : typeName,
				Body = $"class {typeId} {{{body}}}",
				HasOne = MapHasOneRelations(hasOneRelationsByType, type),
				HasMany = MapHasManyRelations(hasManyRelationsByType, type),
				BaseType = GetBaseType(type),
				Interfaces = GetInterfaces(type),
				Inherited = inheritedMembersByType,
				XmlDocs = docs
			};
		}
	}
}