// Copyright (c) 2010-2018 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Text;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.Documentation
{
	/// <summary>
	/// Provides ID strings for entities. (C# 4.0 spec, §A.3.1)
	/// ID strings are used to identify members in XML documentation files.
	/// </summary>
	public static class IdStringProvider
	{
		#region GetIdString
		/// <summary>
		/// Gets the ID string (C# 4.0 spec, §A.3.1) for the specified entity.
		/// </summary>
		public static string GetIdString(this MetadataFile module, EntityHandle handle)
		{
			if (handle.IsNil)
				throw new ArgumentException("The handle must not be nil.", nameof(handle));

			var metadata = module.Metadata;
			var b = new StringBuilder();

			switch (handle.Kind)
			{
				case HandleKind.TypeDefinition:
					b.Append("T:");
					AppendTypeDefinitionName(b, metadata, (TypeDefinitionHandle)handle);
					break;

				case HandleKind.FieldDefinition:
					b.Append("F:");
					AppendFieldIdString(b, metadata, (FieldDefinitionHandle)handle);
					break;

				case HandleKind.MethodDefinition:
					b.Append("M:");
					AppendMethodIdString(b, metadata, (MethodDefinitionHandle)handle);
					break;

				case HandleKind.PropertyDefinition:
					b.Append("P:");
					AppendPropertyIdString(b, metadata, (PropertyDefinitionHandle)handle);
					break;

				case HandleKind.EventDefinition:
					b.Append("E:");
					AppendEventIdString(b, metadata, (EventDefinitionHandle)handle);
					break;

				default:
					throw new ArgumentException($"Unsupported handle kind: {handle.Kind}", nameof(handle));
			}

			return b.ToString();
		}

		/// <summary>
		/// Gets the ID string (C# 4.0 spec, §A.3.1) for the specified entity.
		/// </summary>
		/// <remarks>
		/// The ID string is computed from the entity's metadata; for specialized members
		/// it describes the underlying member definition.
		/// </remarks>
		public static string GetIdString(this IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException(nameof(entity));
			var module = entity.ParentModule?.MetadataFile;
			if (module == null || entity.MetadataToken.IsNil)
				throw new NotSupportedException("Cannot compute an ID string for an entity that is not backed by metadata.");
			return GetIdString(module, entity.MetadataToken);
		}

		/// <summary>
		/// Appends the fully qualified name of a type definition, handling nested types.
		/// The metadata name already contains the `n arity suffix for generic types.
		/// </summary>
		static void AppendTypeDefinitionName(StringBuilder b, MetadataReader metadata, TypeDefinitionHandle handle)
		{
			var typeDef = metadata.GetTypeDefinition(handle);
			var declaringType = typeDef.GetDeclaringType();

			if (declaringType.IsNil)
			{
				var ns = metadata.GetString(typeDef.Namespace);
				if (!string.IsNullOrEmpty(ns))
				{
					b.Append(ns);
					b.Append('.');
				}
				b.Append(metadata.GetString(typeDef.Name));
			}
			else
			{
				AppendTypeDefinitionName(b, metadata, declaringType);
				b.Append('.');
				b.Append(metadata.GetString(typeDef.Name));
			}
		}

		static void AppendTypeReferenceName(StringBuilder b, MetadataReader metadata, TypeReference typeRef)
		{
			if (typeRef.ResolutionScope.Kind == HandleKind.TypeReference)
			{
				var outerRef = metadata.GetTypeReference((TypeReferenceHandle)typeRef.ResolutionScope);
				AppendTypeReferenceName(b, metadata, outerRef);
				b.Append('.');
				b.Append(metadata.GetString(typeRef.Name));
			}
			else
			{
				var ns = metadata.GetString(typeRef.Namespace);
				if (!string.IsNullOrEmpty(ns))
				{
					b.Append(ns);
					b.Append('.');
				}
				b.Append(metadata.GetString(typeRef.Name));
			}
		}

		static void AppendFieldIdString(StringBuilder b, MetadataReader metadata, FieldDefinitionHandle handle)
		{
			var fieldDef = metadata.GetFieldDefinition(handle);
			var declaringType = fieldDef.GetDeclaringType();
			AppendTypeDefinitionName(b, metadata, declaringType);
			b.Append('.');
			b.Append(metadata.GetString(fieldDef.Name));
		}

		static void AppendMethodIdString(StringBuilder b, MetadataReader metadata, MethodDefinitionHandle handle)
		{
			var methodDef = metadata.GetMethodDefinition(handle);
			var declaringType = methodDef.GetDeclaringType();

			AppendTypeDefinitionName(b, metadata, declaringType);
			b.Append('.');

			var methodName = metadata.GetString(methodDef.Name);
			b.Append(methodName.Replace('.', '#').Replace('<', '{').Replace('>', '}'));

			// Method type parameter count
			var genericParams = methodDef.GetGenericParameters();
			if (genericParams.Count > 0)
			{
				b.Append("``");
				b.Append(genericParams.Count);
			}

			// Parameters
			var signature = methodDef.DecodeSignature(
				new IdStringSignatureTypeProvider(),
				new MetadataGenericContext(handle, metadata));
			AppendParameterList(b, signature.ParameterTypes);

			// Return type for conversion operators
			if (methodName is "op_Implicit" or "op_Explicit" or "op_CheckedExplicit")
			{
				b.Append('~');
				b.Append(signature.ReturnType);
			}
		}

		static void AppendParameterList(StringBuilder b, ImmutableArray<string> parameters)
		{
			if (parameters.Length > 0)
			{
				b.Append('(');
				for (int i = 0; i < parameters.Length; i++)
				{
					if (i > 0)
						b.Append(',');
					b.Append(parameters[i]);
				}
				b.Append(')');
			}
		}

		static void AppendPropertyIdString(StringBuilder b, MetadataReader metadata, PropertyDefinitionHandle handle)
		{
			var propertyDef = metadata.GetPropertyDefinition(handle);

			var declaringType = FindDeclaringTypeOfProperty(metadata, handle);
			AppendTypeDefinitionName(b, metadata, declaringType);
			b.Append('.');

			var signature = propertyDef.DecodeSignature(
				new IdStringSignatureTypeProvider(),
				new MetadataGenericContext(declaringType, metadata));

			b.Append(metadata.GetString(propertyDef.Name).Replace('.', '#').Replace('<', '{').Replace('>', '}'));

			// Indexers have parameters
			AppendParameterList(b, signature.ParameterTypes);
		}

		static TypeDefinitionHandle FindDeclaringTypeOfProperty(MetadataReader metadata, PropertyDefinitionHandle propertyHandle)
		{
			var accessors = metadata.GetPropertyDefinition(propertyHandle).GetAccessors();
			var accessor = !accessors.Getter.IsNil ? accessors.Getter
				: !accessors.Setter.IsNil ? accessors.Setter
				: accessors.Others.Length > 0 ? accessors.Others[0]
				: default;
			if (!accessor.IsNil)
				return metadata.GetMethodDefinition(accessor).GetDeclaringType();
			// Accessor-less properties are invalid metadata; fall back to scanning all types.
			foreach (var typeHandle in metadata.TypeDefinitions)
			{
				var typeDef = metadata.GetTypeDefinition(typeHandle);
				foreach (var ph in typeDef.GetProperties())
				{
					if (ph == propertyHandle)
						return typeHandle;
				}
			}
			return default;
		}

		static void AppendEventIdString(StringBuilder b, MetadataReader metadata, EventDefinitionHandle handle)
		{
			var eventDef = metadata.GetEventDefinition(handle);

			var declaringType = FindDeclaringTypeOfEvent(metadata, handle);
			AppendTypeDefinitionName(b, metadata, declaringType);
			b.Append('.');
			b.Append(metadata.GetString(eventDef.Name).Replace('.', '#').Replace('<', '{').Replace('>', '}'));
		}

		static TypeDefinitionHandle FindDeclaringTypeOfEvent(MetadataReader metadata, EventDefinitionHandle eventHandle)
		{
			var accessors = metadata.GetEventDefinition(eventHandle).GetAccessors();
			var accessor = !accessors.Adder.IsNil ? accessors.Adder
				: !accessors.Remover.IsNil ? accessors.Remover
				: !accessors.Raiser.IsNil ? accessors.Raiser
				: accessors.Others.Length > 0 ? accessors.Others[0]
				: default;
			if (!accessor.IsNil)
				return metadata.GetMethodDefinition(accessor).GetDeclaringType();
			// Accessor-less events are invalid metadata; fall back to scanning all types.
			foreach (var typeHandle in metadata.TypeDefinitions)
			{
				var typeDef = metadata.GetTypeDefinition(typeHandle);
				foreach (var eh in typeDef.GetEvents())
				{
					if (eh == eventHandle)
						return typeHandle;
				}
			}
			return default;
		}

		static bool IsAsciiDigit(char c) => c >= '0' && c <= '9';

		/// <summary>
		/// Signature type provider that produces ID string fragments.
		/// </summary>
		readonly struct IdStringSignatureTypeProvider : ISignatureTypeProvider<string, MetadataGenericContext>
		{
			public string GetPrimitiveType(PrimitiveTypeCode typeCode)
			{
				return typeCode switch {
					PrimitiveTypeCode.Void => "System.Void",
					PrimitiveTypeCode.Boolean => "System.Boolean",
					PrimitiveTypeCode.Char => "System.Char",
					PrimitiveTypeCode.SByte => "System.SByte",
					PrimitiveTypeCode.Byte => "System.Byte",
					PrimitiveTypeCode.Int16 => "System.Int16",
					PrimitiveTypeCode.UInt16 => "System.UInt16",
					PrimitiveTypeCode.Int32 => "System.Int32",
					PrimitiveTypeCode.UInt32 => "System.UInt32",
					PrimitiveTypeCode.Int64 => "System.Int64",
					PrimitiveTypeCode.UInt64 => "System.UInt64",
					PrimitiveTypeCode.Single => "System.Single",
					PrimitiveTypeCode.Double => "System.Double",
					PrimitiveTypeCode.String => "System.String",
					PrimitiveTypeCode.Object => "System.Object",
					PrimitiveTypeCode.IntPtr => "System.IntPtr",
					PrimitiveTypeCode.UIntPtr => "System.UIntPtr",
					PrimitiveTypeCode.TypedReference => "System.TypedReference",
					_ => throw new ArgumentOutOfRangeException(nameof(typeCode))
				};
			}

			public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
			{
				var sb = new StringBuilder();
				AppendTypeDefinitionName(sb, reader, handle);
				return sb.ToString();
			}

			public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
			{
				var sb = new StringBuilder();
				var typeRef = reader.GetTypeReference(handle);
				AppendTypeReferenceName(sb, reader, typeRef);
				return sb.ToString();
			}

			public string GetTypeFromSpecification(MetadataReader reader, MetadataGenericContext genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
			{
				var typeSpec = reader.GetTypeSpecification(handle);
				return typeSpec.DecodeSignature(this, genericContext);
			}

			public string GetGenericTypeParameter(MetadataGenericContext genericContext, int index)
			{
				return "`" + index;
			}

			public string GetGenericMethodParameter(MetadataGenericContext genericContext, int index)
			{
				return "``" + index;
			}

			public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments)
			{
				// The generic arguments must be distributed to their nesting level:
				// "Ns.Outer`1.Inner`2" + [A, B, C] => "Ns.Outer{A}.Inner{B,C}".
				// The uninstantiated name carries a `k arity marker at every generic
				// nesting level and the arguments are ordered outermost-first, so each
				// marker consumes the next k arguments.
				var sb = new StringBuilder(genericType.Length + typeArguments.Length * 16);
				int nextArgument = 0;
				for (int i = 0; i < genericType.Length; i++)
				{
					char c = genericType[i];
					if (c != '`' || i + 1 >= genericType.Length || !IsAsciiDigit(genericType[i + 1]))
					{
						sb.Append(c);
						continue;
					}
					int markerEnd = i + 1;
					int arity = 0;
					while (markerEnd < genericType.Length && IsAsciiDigit(genericType[markerEnd]))
					{
						arity = arity * 10 + (genericType[markerEnd] - '0');
						markerEnd++;
					}
					if (arity > typeArguments.Length - nextArgument)
					{
						// The name does not follow the `k arity convention; keep the
						// marker verbatim rather than inventing an argument split.
						sb.Append(genericType, i, markerEnd - i);
					}
					else
					{
						sb.Append('{');
						for (int k = 0; k < arity; k++)
						{
							if (k > 0)
								sb.Append(',');
							sb.Append(typeArguments[nextArgument++]);
						}
						sb.Append('}');
					}
					i = markerEnd - 1;
				}
				if (nextArgument < typeArguments.Length)
				{
					// Arity markers did not account for all arguments; append the
					// remainder so no argument is silently dropped.
					sb.Append('{');
					for (int k = nextArgument; k < typeArguments.Length; k++)
					{
						if (k > nextArgument)
							sb.Append(',');
						sb.Append(typeArguments[k]);
					}
					sb.Append('}');
				}
				return sb.ToString();
			}

			public string GetArrayType(string elementType, ArrayShape shape)
			{
				// C# 4.0 spec, section A.3.1: each dimension is rendered as
				// "lowerbound:size", omitting either part when it is unspecified.
				// Compilers emit neither bound for single-dimensional arrays and
				// a zero lower bound with unspecified size ("0:") otherwise.
				var sb = new StringBuilder(elementType);
				sb.Append('[');
				if (shape.Rank > 1 || shape.LowerBounds.Length > 0 || shape.Sizes.Length > 0)
				{
					for (int i = 0; i < shape.Rank; i++)
					{
						if (i > 0)
							sb.Append(',');
						if (i < shape.LowerBounds.Length)
						{
							sb.Append(shape.LowerBounds[i]);
							sb.Append(':');
						}
						if (i < shape.Sizes.Length)
							sb.Append(shape.Sizes[i]);
					}
				}
				sb.Append(']');
				return sb.ToString();
			}

			public string GetSZArrayType(string elementType)
			{
				return elementType + "[]";
			}

			public string GetPointerType(string elementType)
			{
				return elementType + "*";
			}

			public string GetByReferenceType(string elementType)
			{
				return elementType + "@";
			}

			public string GetFunctionPointerType(MethodSignature<string> signature)
			{
				//var sb = new StringBuilder("method ");
				//sb.Append(signature.ReturnType);
				//sb.Append(" *(");
				//for (int i = 0; i < signature.ParameterTypes.Length; i++)
				//{
				//	if (i > 0)
				//		sb.Append(',');
				//	sb.Append(signature.ParameterTypes[i]);
				//}
				//sb.Append(')');
				//return sb.ToString();
				// The C# spec does not define a syntax for function pointer types in ID strings
				// Roslyn just returns an empty string, so we'll do the same to avoid confusion.
				return "";
			}

			public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired)
			{
				// Custom modifiers are not part of the ID string: Roslyn ignores them (e.g. a
				// virtual method's 'in' parameter carries modreq(InAttribute) but is
				// documented as T@).
				return unmodifiedType;
			}

			public string GetPinnedType(string elementType)
			{
				// '^' following the modified type, per the MSVC xml doc format. Pinned
				// types cannot occur in member signatures, only in local variable
				// signatures, so no compiler ever generates this in an ID string.
				return elementType + "^";
			}
		}
		#endregion

		#region GetTypeName
		public static string GetTypeName(IType type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			StringBuilder b = new StringBuilder();
			AppendTypeName(b, type, false);
			return b.ToString();
		}

		static void AppendTypeName(StringBuilder b, IType type, bool explicitInterfaceImpl)
		{
			switch (type.Kind)
			{
				case TypeKind.Dynamic:
					b.Append(explicitInterfaceImpl ? "System#Object" : "System.Object");
					break;
				case TypeKind.TypeParameter:
					ITypeParameter tp = (ITypeParameter)type;
					if (explicitInterfaceImpl)
					{
						b.Append(tp.Name);
					}
					else
					{
						b.Append('`');
						if (tp.OwnerType == SymbolKind.Method)
							b.Append('`');
						b.Append(tp.Index);
					}
					break;
				case TypeKind.Array:
					ArrayType array = (ArrayType)type;
					AppendTypeName(b, array.ElementType, explicitInterfaceImpl);
					b.Append('[');
					if (array.Dimensions > 1)
					{
						for (int i = 0; i < array.Dimensions; i++)
						{
							if (i > 0)
								b.Append(explicitInterfaceImpl ? '@' : ',');
							if (!explicitInterfaceImpl)
								b.Append("0:");
						}
					}
					b.Append(']');
					break;
				case TypeKind.Pointer:
					AppendTypeName(b, ((PointerType)type).ElementType, explicitInterfaceImpl);
					b.Append('*');
					break;
				case TypeKind.ByReference:
					AppendTypeName(b, ((ByReferenceType)type).ElementType, explicitInterfaceImpl);
					b.Append('@');
					break;
				default:
					IType declType = type.DeclaringType;
					if (declType != null)
					{
						AppendTypeName(b, declType, explicitInterfaceImpl);
						b.Append(explicitInterfaceImpl ? '#' : '.');
						b.Append(type.Name);
						AppendTypeParameters(b, type, declType.TypeParameterCount, explicitInterfaceImpl);
					}
					else
					{
						if (explicitInterfaceImpl)
							b.Append(type.FullName.Replace('.', '#'));
						else
							b.Append(type.FullName);
						AppendTypeParameters(b, type, 0, explicitInterfaceImpl);
					}
					break;
			}
		}

		static void AppendTypeParameters(StringBuilder b, IType type, int outerTypeParameterCount, bool explicitInterfaceImpl)
		{
			int tpc = type.TypeParameterCount - outerTypeParameterCount;
			if (tpc > 0)
			{
				ParameterizedType pt = type as ParameterizedType;
				if (pt != null)
				{
					b.Append('{');
					var ta = pt.TypeArguments;
					for (int i = outerTypeParameterCount; i < ta.Count; i++)
					{
						if (i > outerTypeParameterCount)
							b.Append(explicitInterfaceImpl ? '@' : ',');
						AppendTypeName(b, ta[i], explicitInterfaceImpl);
					}
					b.Append('}');
				}
				else
				{
					b.Append('`');
					b.Append(tpc);
				}
			}
		}
		#endregion

		#region ParseMemberName
		/// <summary>
		/// Parse the ID string into a member reference.
		/// </summary>
		/// <param name="memberIdString">The ID string representing the member (with "M:", "F:", "P:" or "E:" prefix).</param>
		/// <returns>A member reference that represents the ID string.</returns>
		/// <exception cref="ReflectionNameParseException">The syntax of the ID string is invalid</exception>
		/// <remarks>
		/// The member reference will look in <see cref="ITypeResolveContext.CurrentModule"/> first,
		/// and if the member is not found there,
		/// it will look in all other assemblies of the compilation.
		/// </remarks>
		public static IMemberReference ParseMemberIdString(string memberIdString)
		{
			if (memberIdString == null)
				throw new ArgumentNullException(nameof(memberIdString));
			if (memberIdString.Length < 2 || memberIdString[1] != ':')
				throw new ReflectionNameParseException(0, "Missing type tag");
			char typeChar = memberIdString[0];
			int parenPos = memberIdString.IndexOf('(');
			if (parenPos < 0)
				parenPos = memberIdString.LastIndexOf('~');
			if (parenPos < 0)
				parenPos = memberIdString.Length;
			int dotPos = memberIdString.LastIndexOf('.', parenPos - 1);
			if (dotPos < 0)
				throw new ReflectionNameParseException(0, "Could not find '.' separating type name from member name");
			string typeName = memberIdString.Substring(0, dotPos);
			int pos = 2;
			ITypeReference typeReference = ParseTypeName(typeName, ref pos);
			if (pos != typeName.Length)
				throw new ReflectionNameParseException(pos, "Expected end of type name");
			//			string memberName = memberIDString.Substring(dotPos + 1, parenPos - (dotPos + 1));
			//			pos = memberName.LastIndexOf("``");
			//			if (pos > 0)
			//				memberName = memberName.Substring(0, pos);
			//			memberName = memberName.Replace('#', '.');
			return new IdStringMemberReference(typeReference, typeChar, memberIdString);
		}
		#endregion

		#region ParseTypeName
		/// <summary>
		/// Parse the ID string type name into a type reference.
		/// </summary>
		/// <param name="typeName">The ID string representing the type (the "T:" prefix is optional).</param>
		/// <returns>A type reference that represents the ID string.</returns>
		/// <exception cref="ReflectionNameParseException">The syntax of the ID string is invalid</exception>
		/// <remarks>
		/// <para>
		/// The type reference will look in <see cref="ITypeResolveContext.CurrentModule"/> first,
		/// and if the type is not found there,
		/// it will look in all other assemblies of the compilation.
		/// </para>
		/// <para>
		/// If the type is open (contains type parameters '`0' or '``0'),
		/// an <see cref="ITypeResolveContext"/> with the appropriate CurrentTypeDefinition/CurrentMember is required
		/// to resolve the reference to the ITypeParameter.
		/// </para>
		/// </remarks>
		public static ITypeReference ParseTypeName(string typeName)
		{
			if (typeName == null)
				throw new ArgumentNullException(nameof(typeName));
			int pos = 0;
			if (typeName.StartsWith("T:", StringComparison.Ordinal))
				pos = 2;
			ITypeReference r = ParseTypeName(typeName, ref pos);
			if (pos < typeName.Length)
				throw new ReflectionNameParseException(pos, "Expected end of type name");
			return r;
		}

		static bool IsIDStringSpecialCharacter(char c)
		{
			switch (c)
			{
				case ':':
				case '{':
				case '}':
				case '[':
				case ']':
				case '(':
				case ')':
				case '`':
				case '*':
				case '@':
				case ',':
					return true;
				default:
					return false;
			}
		}

		static ITypeReference ParseTypeName(string typeName, ref int pos)
		{
			string reflectionTypeName = typeName;
			if (pos == typeName.Length)
				throw new ReflectionNameParseException(pos, "Unexpected end");
			ITypeReference result;
			if (reflectionTypeName[pos] == '`')
			{
				// type parameter reference
				pos++;
				if (pos == reflectionTypeName.Length)
					throw new ReflectionNameParseException(pos, "Unexpected end");
				if (reflectionTypeName[pos] == '`')
				{
					// method type parameter reference
					pos++;
					int index = ReflectionHelper.ReadTypeParameterCount(reflectionTypeName, ref pos);
					result = TypeParameterReference.Create(SymbolKind.Method, index);
				}
				else
				{
					// class type parameter reference
					int index = ReflectionHelper.ReadTypeParameterCount(reflectionTypeName, ref pos);
					result = TypeParameterReference.Create(SymbolKind.TypeDefinition, index);
				}
			}
			else
			{
				// not a type parameter reference: read the actual type name
				List<ITypeReference> typeArguments = new List<ITypeReference>();
				string typeNameWithoutSuffix = ReadTypeName(typeName, ref pos, true, out int typeParameterCount, typeArguments);
				result = new GetPotentiallyNestedClassTypeReference(typeNameWithoutSuffix, typeParameterCount);
				while (pos < typeName.Length && typeName[pos] == '.')
				{
					pos++;
					string nestedTypeName = ReadTypeName(typeName, ref pos, false, out typeParameterCount, typeArguments);
					result = new NestedTypeReference(result, nestedTypeName, typeParameterCount);
				}
				if (typeArguments.Count > 0)
				{
					result = new ParameterizedTypeReference(result, typeArguments);
				}
			}
			while (pos < typeName.Length)
			{
				switch (typeName[pos])
				{
					case '[':
						int dimensions = 1;
						do
						{
							pos++;
							if (pos == typeName.Length)
								throw new ReflectionNameParseException(pos, "Unexpected end");
							if (typeName[pos] == ',')
								dimensions++;
						} while (typeName[pos] != ']');
						result = new ArrayTypeReference(result, dimensions);
						break;
					case '*':
						result = new PointerTypeReference(result);
						break;
					case '@':
						result = new ByReferenceTypeReference(result);
						break;
					default:
						return result;
				}
				pos++;
			}
			return result;
		}

		static string ReadTypeName(string typeName, ref int pos, bool allowDottedName, out int typeParameterCount, List<ITypeReference> typeArguments)
		{
			int startPos = pos;
			// skip the simple name portion:
			while (pos < typeName.Length && !IsIDStringSpecialCharacter(typeName[pos]) && (allowDottedName || typeName[pos] != '.'))
				pos++;
			if (pos == startPos)
				throw new ReflectionNameParseException(pos, "Expected type name");
			string shortTypeName = typeName.Substring(startPos, pos - startPos);
			// read type arguments:
			typeParameterCount = 0;
			if (pos < typeName.Length && typeName[pos] == '`')
			{
				// unbound generic type
				pos++;
				typeParameterCount = ReflectionHelper.ReadTypeParameterCount(typeName, ref pos);
			}
			else if (pos < typeName.Length && typeName[pos] == '{')
			{
				// bound generic type
				do
				{
					pos++;
					typeArguments.Add(ParseTypeName(typeName, ref pos));
					typeParameterCount++;
					if (pos == typeName.Length)
						throw new ReflectionNameParseException(pos, "Unexpected end");
				} while (typeName[pos] == ',');
				if (typeName[pos] != '}')
					throw new ReflectionNameParseException(pos, "Expected '}'");
				pos++;
			}
			return shortTypeName;
		}
		#endregion

		#region FindEntity
		/// <summary>
		/// Finds the entity in the given type resolve context.
		/// </summary>
		/// <param name="idString">ID string of the entity.</param>
		/// <param name="context">Type resolve context</param>
		/// <returns>Returns the entity, or null if it is not found.</returns>
		/// <exception cref="ReflectionNameParseException">The syntax of the ID string is invalid</exception>
		public static IEntity FindEntity(string idString, ITypeResolveContext context)
		{
			if (idString == null)
				throw new ArgumentNullException(nameof(idString));
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			if (idString.StartsWith("T:", StringComparison.Ordinal))
			{
				return ParseTypeName(idString.Substring(2)).Resolve(context).GetDefinition();
			}
			else
			{
				return ParseMemberIdString(idString).Resolve(context);
			}
		}
		#endregion
	}
}
