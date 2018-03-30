// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Metadata;
using SRM = System.Reflection.Metadata;
using ICSharpCode.Decompiler.Util;
using System.Collections.Immutable;

namespace ICSharpCode.Decompiler.Documentation
{
	/// <summary>
	/// Provides XML documentation tags.
	/// </summary>
	public static class XmlDocKeyProvider
	{
		#region GetKey
		public static string GetKey(Entity entity)
		{
			return GetKey(entity.Module.GetMetadataReader(), entity.Handle);
		}

		public static string GetKey(SRM.MetadataReader metadata, SRM.EntityHandle member)
		{
			StringBuilder b = new StringBuilder();

			void AppendTypeName(SRM.EntityHandle type)
			{
				switch (type.Kind) {
					case SRM.HandleKind.TypeDefinition:
						b.Append("T:");
						b.Append(((SRM.TypeDefinitionHandle)type).GetFullTypeName(metadata));
						break;
					case SRM.HandleKind.TypeReference:
						b.Append("T:");
						b.Append(((SRM.TypeReferenceHandle)type).GetFullTypeName(metadata));
						break;
					default:
						throw new NotImplementedException();
					/*case SRM.HandleKind.TypeSpecification:
						b.Append("T:");
						var typeSpec = metadata.GetTypeSpecification((SRM.TypeSpecificationHandle)type);
						b.Append(typeSpec.DecodeSignature(new DocumentationKeySignatureTypeProvider(), default(Unit)));
						break;*/
				}
			}

			void AppendSignature(SRM.MethodSignature<string> signature, bool printExplicitReturnType = false)
			{
				if (signature.GenericParameterCount > 0) {
					b.Append("``");
					b.Append(signature.GenericParameterCount);
				}
				b.Append('(');
				for (int i = 0; i < signature.ParameterTypes.Length; i++) {
					if (i > 0)
						b.Append(',');
					b.Append(signature.ParameterTypes[i]);
				}
				b.Append(')');
				if (printExplicitReturnType) {
					b.Append('~');
					b.Append(signature.ReturnType);
				}
			}

			switch (member.Kind) {
				case SRM.HandleKind.TypeDefinition:
				case SRM.HandleKind.TypeReference:
				case SRM.HandleKind.TypeSpecification:
					b.Append("T:");
					AppendTypeName(member);
					break;
				case SRM.HandleKind.FieldDefinition:
					b.Append("F:");
					var field = metadata.GetFieldDefinition((SRM.FieldDefinitionHandle)member);
					AppendTypeName(field.GetDeclaringType());
					b.Append('.');
					b.Append(metadata.GetString(field.Name).Replace('.', '#'));
					break;
				case SRM.HandleKind.PropertyDefinition: {
					b.Append("P:");
					var property = metadata.GetPropertyDefinition((SRM.PropertyDefinitionHandle)member);
					var accessors = property.GetAccessors();
					SRM.TypeDefinitionHandle declaringType;
					if (!accessors.Getter.IsNil) {
						declaringType = metadata.GetMethodDefinition(accessors.Getter).GetDeclaringType();
					} else {
						declaringType = metadata.GetMethodDefinition(accessors.Setter).GetDeclaringType();
					}
					AppendTypeName(declaringType);
					b.Append('.');
					b.Append(metadata.GetString(property.Name).Replace('.', '#'));
					AppendSignature(property.DecodeSignature(new DocumentationKeySignatureTypeProvider(), default(Unit)));
					break;
				}
				case SRM.HandleKind.MethodDefinition:
					b.Append("M:");
					var method = metadata.GetMethodDefinition((SRM.MethodDefinitionHandle)member);
					AppendTypeName(method.GetDeclaringType());
					b.Append('.');
					var methodName = metadata.GetString(method.Name);
					b.Append(metadata.GetString(method.Name).Replace('.', '#'));
					AppendSignature(method.DecodeSignature(new DocumentationKeySignatureTypeProvider(), default(Unit)), methodName == "op_Implicit" || methodName == "op_Explicit");
					break;
				case SRM.HandleKind.EventDefinition: {
					b.Append("E:");
					var @event = metadata.GetEventDefinition((SRM.EventDefinitionHandle)member);
					var accessors = @event.GetAccessors();
					SRM.TypeDefinitionHandle declaringType;
					if (!accessors.Adder.IsNil) {
						declaringType = metadata.GetMethodDefinition(accessors.Adder).GetDeclaringType();
					} else if (!accessors.Remover.IsNil) {
						declaringType = metadata.GetMethodDefinition(accessors.Remover).GetDeclaringType();
					} else {
						declaringType = metadata.GetMethodDefinition(accessors.Raiser).GetDeclaringType();
					}
					AppendTypeName(declaringType);
					b.Append('.');
					b.Append(metadata.GetString(@event.Name).Replace('.', '#'));
					break;
				}
				default:
					throw new NotImplementedException();
			}
			return b.ToString();
		}

		public sealed class DocumentationKeySignatureTypeProvider : SRM.ISignatureTypeProvider<string, Unit>
		{
			public string GetArrayType(string elementType, SRM.ArrayShape shape)
			{
				string shapeString = "";
				for (int i = 0; i < shape.Rank; i++) {
					if (i > 0)
						shapeString += ',';
					int? lowerBound = i < shape.LowerBounds.Length ? (int?)shape.LowerBounds[i] : null;
					int? size = i < shape.Sizes.Length ? (int?)shape.Sizes[i] : null;
					if (lowerBound != null || size != null) {
						shapeString += lowerBound.ToString();
						shapeString += ':';
						shapeString += (lowerBound + size - 1).ToString();
					}
				}
				return elementType + "[" + shapeString + "]";
			}

			public string GetByReferenceType(string elementType)
			{
				return elementType + '@';
			}

			public string GetFunctionPointerType(SRM.MethodSignature<string> signature)
			{
				return "";
			}

			public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments)
			{
				string arguments = "";
				for (int i = 0; i < typeArguments.Length; i++) {
					if (i > 0)
						arguments += ',';
					arguments += typeArguments[i];
				}
				return genericType + "{" + arguments + "}";
			}

			public string GetGenericMethodParameter(Unit genericContext, int index)
			{
				return "``" + index;
			}

			public string GetGenericTypeParameter(Unit genericContext, int index)
			{
				return "`" + index;
			}

			public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired)
			{
				throw new NotImplementedException();
			}

			public string GetPinnedType(string elementType)
			{
				throw new NotImplementedException();
			}

			public string GetPointerType(string elementType)
			{
				return elementType + '*';
			}

			public string GetPrimitiveType(SRM.PrimitiveTypeCode typeCode)
			{
				return $"System.{typeCode}";
			}

			public string GetSZArrayType(string elementType)
			{
				return elementType + "[]";
			}

			public string GetTypeFromDefinition(SRM.MetadataReader reader, SRM.TypeDefinitionHandle handle, byte rawTypeKind)
			{
				return handle.GetFullTypeName(reader).ToString();
			}

			public string GetTypeFromReference(SRM.MetadataReader reader, SRM.TypeReferenceHandle handle, byte rawTypeKind)
			{
				return handle.GetFullTypeName(reader).ToString();
			}

			public string GetTypeFromSpecification(SRM.MetadataReader reader, Unit genericContext, SRM.TypeSpecificationHandle handle, byte rawTypeKind)
			{
				return handle.GetFullTypeName(reader).ToString();
			}
		}
		#endregion

		#region FindMemberByKey
		public static Entity FindMemberByKey(PEFile module, string key)
		{
			if (module == null)
				throw new ArgumentNullException(nameof(module));
			if (key == null || key.Length < 2 || key[1] != ':')
				return default(Entity);
			switch (key[0]) {
				case 'T':
					return FindType(module, key.Substring(2));
				case 'F':
					return FindMember(module, key, type => type.This().GetFields().Select(f => new Entity(module, f)));
				case 'P':
					return FindMember(module, key, type => type.This().GetProperties().Select(p => new Entity(module, p)));
				case 'E':
					return FindMember(module, key, type => type.This().GetEvents().Select(e => new Entity(module, e)));
				case 'M':
					return FindMember(module, key, type => type.This().GetMethods().Select(m => new Entity(module, m)));
				default:
					return default(Entity);
			}
		}
		
		static Entity FindMember(PEFile module, string key, Func<TypeDefinition, IEnumerable<Entity>> memberSelector)
		{
			Debug.WriteLine("Looking for member " + key);
			int parenPos = key.IndexOf('(');
			int dotPos;
			if (parenPos > 0) {
				dotPos = key.LastIndexOf('.', parenPos - 1, parenPos);
			} else {
				dotPos = key.LastIndexOf('.');
			}
			if (dotPos < 0)
				return default(Entity);
			TypeDefinition type = FindType(module, key.Substring(2, dotPos - 2));
			if (type == null)
				return default(Entity);
			string shortName;
			if (parenPos > 0) {
				shortName = key.Substring(dotPos + 1, parenPos - (dotPos + 1));
			} else {
				shortName = key.Substring(dotPos + 1);
			}
			Debug.WriteLine("Searching in type {0} for {1}", type.Handle.GetFullTypeName(module.GetMetadataReader()), shortName);
			Entity shortNameMatch = default(Entity);
			var metadata = module.GetMetadataReader();
			foreach (var member in memberSelector(type)) {
				string memberKey = GetKey(member);
				Debug.WriteLine(memberKey);
				if (memberKey == key)
					return member;
				string name;
				switch (member.Handle.Kind) {
					case SRM.HandleKind.MethodDefinition:
						name = metadata.GetString(metadata.GetMethodDefinition((SRM.MethodDefinitionHandle)member.Handle).Name);
						break;
					case SRM.HandleKind.FieldDefinition:
						name = metadata.GetString(metadata.GetFieldDefinition((SRM.FieldDefinitionHandle)member.Handle).Name);
						break;
					case SRM.HandleKind.PropertyDefinition:
						name = metadata.GetString(metadata.GetPropertyDefinition((SRM.PropertyDefinitionHandle)member.Handle).Name);
						break;
					case SRM.HandleKind.EventDefinition:
						name = metadata.GetString(metadata.GetEventDefinition((SRM.EventDefinitionHandle)member.Handle).Name);
						break;
					default:
						throw new NotSupportedException();
				}
				if (shortName == name.Replace('.', '#'))
					shortNameMatch = member;
			}
			// if there's no match by ID string (key), return the match by name.
			return shortNameMatch;
		}
		
		static TypeDefinition FindType(PEFile module, string name)
		{
			var metadata = module.GetMetadataReader();
			string[] segments = name.Split('.');
			var currentNamespace = metadata.GetNamespaceDefinitionRoot();
			int i = 0;
			while (i < segments.Length) {
				string part = segments[i];
				var next = currentNamespace.NamespaceDefinitions.FirstOrDefault(ns => metadata.GetString(metadata.GetNamespaceDefinition(ns).Name) == part);
				if (next.IsNil)
					break;
				currentNamespace = metadata.GetNamespaceDefinition(next);
				i++;
			}
			if (i == segments.Length)
				return default(TypeDefinition);
			var typeDefinitions = currentNamespace.TypeDefinitions;
			while (i < segments.Length) {
				string part = segments[i];
				foreach (var t in typeDefinitions) {
					var type = metadata.GetTypeDefinition(t);
					if (metadata.GetString(type.Name) == part) {
						if (i + 1 == segments.Length)
							return new TypeDefinition(module, t);
						typeDefinitions = type.GetNestedTypes();
						i++;
						break;
					}
				}
			}
			return default(TypeDefinition);
		}
		#endregion
	}
}
