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
		/// Gets the ID string (C# 4.0 spec, §A.3.1) for the specified entity,
		/// in the form the C# compiler writes into xml documentation files.
		/// </summary>
		public static string GetIdString(this MetadataFile module, EntityHandle handle)
		{
			return GetIdString(module, handle, cppCliDialect: false);
		}

		/// <summary>
		/// Gets the ID string candidates for the entity, most specific first: the MSVC
		/// C++/CLI (ECMA-372-style) form when it differs from the C#/Roslyn form, then the
		/// C#/Roslyn form. Documentation lookup should try the candidates in order, so that
		/// xml doc files written by either compiler can be matched. The dialects differ in
		/// signatures only: Roslyn ignores custom modifiers and strips the arity marker of
		/// instantiated generic types, while MSVC renders modifiers ('!' or '|' followed by
		/// the modifier type), keeps arity markers, and refers to a default indexed
		/// property as 'default'. The C++/CLI form comes first because wherever it differs
		/// it contains character sequences Roslyn never writes, so it can only match
		/// MSVC-generated keys; the stripped Roslyn form of one member can collide with the
		/// key of a different member in an MSVC-generated file (e.g. overloads differing
		/// only in a custom modifier).
		/// </summary>
		public static IEnumerable<string> GetIdStringCandidates(this MetadataFile module, EntityHandle handle)
		{
			string primary = GetIdString(module, handle, cppCliDialect: false);
			string cppCli = GetIdString(module, handle, cppCliDialect: true);
			if (cppCli != primary)
				yield return cppCli;
			yield return primary;
		}

		static string GetIdString(MetadataFile module, EntityHandle handle, bool cppCliDialect)
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
					AppendMethodIdString(b, metadata, (MethodDefinitionHandle)handle, cppCliDialect);
					break;

				case HandleKind.PropertyDefinition:
					b.Append("P:");
					AppendPropertyIdString(b, metadata, (PropertyDefinitionHandle)handle, cppCliDialect);
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

		static void AppendMethodIdString(StringBuilder b, MetadataReader metadata, MethodDefinitionHandle handle, bool cppCliDialect)
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
				new IdStringSignatureTypeProvider(cppCliDialect),
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

		static void AppendPropertyIdString(StringBuilder b, MetadataReader metadata, PropertyDefinitionHandle handle, bool cppCliDialect)
		{
			var propertyDef = metadata.GetPropertyDefinition(handle);

			var declaringType = FindDeclaringTypeOfProperty(metadata, handle);
			AppendTypeDefinitionName(b, metadata, declaringType);
			b.Append('.');

			var signature = propertyDef.DecodeSignature(
				new IdStringSignatureTypeProvider(cppCliDialect),
				new MetadataGenericContext(declaringType, metadata));

			string name = metadata.GetString(propertyDef.Name);
			// The MSVC xml doc generator refers to a type's default indexed property by the
			// C++/CLI keyword 'default' instead of the property's metadata name.
			if (cppCliDialect && signature.ParameterTypes.Length > 0
				&& name == GetDefaultMemberName(metadata, declaringType))
			{
				b.Append("default");
			}
			else
			{
				b.Append(name.Replace('.', '#').Replace('<', '{').Replace('>', '}'));
			}

			// Indexers have parameters
			AppendParameterList(b, signature.ParameterTypes);
		}

		static string GetDefaultMemberName(MetadataReader metadata, TypeDefinitionHandle declaringType)
		{
			foreach (var h in metadata.GetTypeDefinition(declaringType).GetCustomAttributes())
			{
				var customAttribute = metadata.GetCustomAttribute(h);
				if (!customAttribute.IsKnownAttribute(metadata, KnownAttribute.DefaultMember))
					continue;
				try
				{
					var value = customAttribute.DecodeValue(Metadata.MetadataExtensions.MinimalAttributeTypeProvider);
					if (value.FixedArguments.Length == 1 && value.FixedArguments[0].Value is string name)
						return name;
				}
				catch (BadImageFormatException)
				{
				}
				catch (Metadata.EnumUnderlyingTypeResolveException)
				{
				}
			}
			return null;
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
		/// Signature type provider that produces ID string fragments. With
		/// <paramref name="cppCliDialect"/> set, produces the MSVC C++/CLI form
		/// (custom modifiers rendered, arity markers kept on generic instantiations)
		/// instead of the C#/Roslyn form.
		/// </summary>
		readonly struct IdStringSignatureTypeProvider(bool cppCliDialect) : ISignatureTypeProvider<string, MetadataGenericContext>
		{
			readonly bool cppCliDialect = cppCliDialect;

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
						// MSVC keeps the arity marker in front of the argument list
						// (List`1{System.Int32}); Roslyn strips it (List{System.Int32}).
						if (cppCliDialect)
							sb.Append(genericType, i, markerEnd - i);
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
				// Roslyn ignores custom modifiers entirely (e.g. a virtual method's 'in'
				// parameter carries modreq(InAttribute) but is documented as T@).
				if (!cppCliDialect)
					return unmodifiedType;
				// The MSVC xml doc generator renders a modifier after the modified type,
				// e.g. System.Int32!System.Runtime.CompilerServices.IsConst for a C++/CLI
				// 'const int' parameter. Its documented mapping is '!' for modopt and '|'
				// for modreq, but observed output uses '|' only for modreq(IsVolatile);
				// modreq(IsByValue) on conversion operator operands is rendered with '!'.
				char prefix = isRequired && modifier == "System.Runtime.CompilerServices.IsVolatile"
					? '|' : '!';
				return unmodifiedType + prefix + modifier;
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

		/// <summary>
		/// Finds the entity with the given ID string in the provided modules.
		/// </summary>
		/// <param name="idString">ID string of the entity (e.g., "T:System.String", "M:System.String.Contains(System.String)").</param>
		/// <param name="modules">The list of modules to search, in priority order.</param>
		/// <returns>
		/// A tuple of (MetadataFile, EntityHandle) for the found entity.
		/// Returns default if the entity is not found.
		/// </returns>
		/// <exception cref="ReflectionNameParseException">The syntax of the ID string is invalid.</exception>
		/// <remarks>
		/// <para>
		/// The ID string format cannot represent all names valid in metadata: GetIdString
		/// emits raw metadata names, but a name that itself contains ID string special
		/// characters (e.g. a dot in a type name) is ambiguous when parsed back, because
		/// namespace/type-name splits are only tried at dots. Function pointer parameter
		/// types render as empty (matching Roslyn), so overloads differing only by a
		/// function pointer type share an ID and resolve to the first candidate.
		/// </para>
		/// <para>
		/// A type only present as a type forwarder is returned as its ExportedType handle;
		/// members of such a type are not followed into the target assembly unless that
		/// assembly is itself part of <paramref name="modules"/>.
		/// </para>
		/// </remarks>
		public static (MetadataFile Module, EntityHandle Handle) FindEntity(string idString, IReadOnlyList<MetadataFile> modules)
		{
			if (idString == null)
				throw new ArgumentNullException(nameof(idString));
			if (modules == null)
				throw new ArgumentNullException(nameof(modules));
			if (idString.Length < 2 || idString[1] != ':')
				throw new ReflectionNameParseException(0, "Missing type tag");

			char typeChar = idString[0];

			if (typeChar == 'T')
			{
				return FindTypeDefinition(idString.Substring(2), modules);
			}
			else
			{
				return FindMember(typeChar, idString, modules);
			}
		}

		/// <summary>
		/// Resolves a type name from an ID string to a TypeDefinitionHandle or ExportedTypeHandle.
		/// Tries all possible namespace/type-name boundary splits (mirrors the algorithm from
		/// GetPotentiallyNestedClassTypeReference.ResolveInPEFile).
		/// </summary>
		static (MetadataFile, EntityHandle) FindTypeDefinition(string typeName, IReadOnlyList<MetadataFile> modules)
		{
			var parts = ParseTypeNameParts(typeName);

			foreach (var module in modules)
			{
				if (module == null)
					continue;
				var result = ResolveTypeInModule(parts, module);
				if (!result.IsNil)
					return (module, result);
			}

			return default;
		}

		/// <summary>
		/// Finds a member (field, method, property, event) by its ID string.
		/// First resolves the declaring type, then enumerates candidate members
		/// and compares their computed ID strings.
		/// </summary>
		/// <summary>
		/// Finds the '.' separating the declaring type name from the member name: the last
		/// '.' before '(' or '~' or end-of-string. Returns a negative value if there is none.
		/// </summary>
		static int FindMemberNameDot(string idString)
		{
			int parenPos = idString.IndexOf('(');
			if (parenPos < 0)
				parenPos = idString.LastIndexOf('~');
			if (parenPos < 0)
				parenPos = idString.Length;
			return idString.LastIndexOf('.', parenPos - 1);
		}

		static (MetadataFile, EntityHandle) FindMember(char typeChar, string idString, IReadOnlyList<MetadataFile> modules)
		{
			int dotPos = FindMemberNameDot(idString);
			if (dotPos < 0)
				throw new ReflectionNameParseException(0, "Could not find '.' separating type name from member name");

			// The type name portion is from index 2 (after "X:") to dotPos.
			string typeName = idString.Substring(2, dotPos - 2);
			var typeParts = ParseTypeNameParts(typeName);

			foreach (var module in modules)
			{
				if (module == null)
					continue;

				var typeHandle = ResolveTypeInModule(typeParts, module);
				if (typeHandle.IsNil || typeHandle.Kind != HandleKind.TypeDefinition)
					continue;

				var typeDef = module.Metadata.GetTypeDefinition((TypeDefinitionHandle)typeHandle);
				EntityHandle memberHandle = FindMemberInType(module, typeDef, typeChar, idString);
				if (!memberHandle.IsNil)
					return (module, memberHandle);
			}

			return default;
		}

		/// <summary>
		/// Searches for a member within a resolved type definition by computing the ID
		/// string of each member and comparing, so that IDs in either dialect (e.g. crefs
		/// from MSVC-generated xml files) resolve. The dialects are matched in two passes
		/// over the whole member list, most specific first: the stripped C#/Roslyn form of
		/// one member can equal the C++/CLI-dialect key of a different member (overloads
		/// differing only in a custom modifier), so mixing dialects within a single pass
		/// could resolve to the wrong member.
		/// </summary>
		static EntityHandle FindMemberInType(MetadataFile module, TypeDefinition typeDef, char typeChar, string idString)
		{
			for (int pass = 0; pass < 2; pass++)
			{
				bool cppCliDialect = pass == 0;
				switch (typeChar)
				{
					case 'F':
						foreach (var handle in typeDef.GetFields())
						{
							if (GetIdString(module, handle, cppCliDialect) == idString)
								return handle;
						}
						break;

					case 'M':
						foreach (var handle in typeDef.GetMethods())
						{
							if (GetIdString(module, handle, cppCliDialect) == idString)
								return handle;
						}
						break;

					case 'P':
						foreach (var handle in typeDef.GetProperties())
						{
							if (GetIdString(module, handle, cppCliDialect) == idString)
								return handle;
						}
						break;

					case 'E':
						foreach (var handle in typeDef.GetEvents())
						{
							if (GetIdString(module, handle, cppCliDialect) == idString)
								return handle;
						}
						break;
				}
			}

			return default;
		}
		#endregion

		#region Type Name Parsing and Resolution
		/// <summary>
		/// Represents a parsed segment of a potentially nested type name in an ID string.
		/// The first part's Name may contain dots (namespace + top-level type name);
		/// subsequent parts are nested type names without dots.
		/// </summary>
		struct TypeNamePart
		{
			public string Name;
			public int TypeParameterCount;
		}

		/// <summary>
		/// Parses a type name (without the "T:" prefix) into its constituent parts,
		/// handling nested types separated by '.', and generic arity via `n or {args}.
		/// 
		/// The first part's Name contains the full dotted name (namespace + top-level type),
		/// because we don't know where the namespace ends. Resolution will try all splits.
		/// 
		/// Examples:
		///   "System.Collections.Generic.Dictionary`2.KeyCollection"
		///   → [{Name="System.Collections.Generic.Dictionary", TPC=2}, {Name="KeyCollection", TPC=0}]
		///   
		///   "Outer.Inner{System.Int32}"
		///   → [{Name="Outer", TPC=0}, {Name="Inner", TPC=1}]
		/// </summary>
		static List<TypeNamePart> ParseTypeNameParts(string typeName)
		{
			var parts = new List<TypeNamePart>();
			int pos = 0;

			string firstName = ReadTypeNameSegment(typeName, ref pos, allowDots: true);
			int firstTpc = ReadTypeParameterCountFromIdString(typeName, ref pos);
			parts.Add(new TypeNamePart { Name = firstName, TypeParameterCount = firstTpc });

			while (pos < typeName.Length && typeName[pos] == '.')
			{
				pos++;
				string nestedName = ReadTypeNameSegment(typeName, ref pos, allowDots: false);
				int nestedTpc = ReadTypeParameterCountFromIdString(typeName, ref pos);
				parts.Add(new TypeNamePart { Name = nestedName, TypeParameterCount = nestedTpc });
			}

			return parts;
		}

		/// <summary>
		/// Reads a type name segment (no special characters). If allowDots is true,
		/// dots are included in the segment (for the top-level name which includes namespace).
		/// </summary>
		static string ReadTypeNameSegment(string typeName, ref int pos, bool allowDots)
		{
			int start = pos;
			while (pos < typeName.Length)
			{
				char c = typeName[pos];
				if (IsIDStringSpecialCharacter(c))
					break;
				if (!allowDots && c == '.')
					break;
				pos++;
			}
			if (pos == start)
				throw new ReflectionNameParseException(pos, "Expected type name");
			return typeName.Substring(start, pos - start);
		}

		/// <summary>
		/// Reads a type parameter count from the current position in an ID string.
		/// Handles both `n (unbound) and {T1,T2,...} (bound) syntax.
		/// For bound syntax, counts the arguments without fully parsing them
		/// (we only need the arity for type definition lookup).
		/// </summary>
		static int ReadTypeParameterCountFromIdString(string typeName, ref int pos)
		{
			if (pos >= typeName.Length)
				return 0;

			if (typeName[pos] == '`')
			{
				pos++;
				return ReflectionHelper.ReadTypeParameterCount(typeName, ref pos);
			}
			else if (typeName[pos] == '{')
			{
				int count = 1;
				int depth = 0;
				pos++; // skip '{'
				while (pos < typeName.Length)
				{
					char c = typeName[pos];
					if (c == '{')
						depth++;
					else if (c == '}')
					{
						if (depth == 0)
						{
							pos++;
							break;
						}
						depth--;
					}
					else if (c == ',' && depth == 0)
					{
						count++;
					}
					pos++;
				}
				return count;
			}

			return 0;
		}

		/// <summary>
		/// Attempts to resolve a parsed type name within a single module.
		/// The first part's Name is a dotted name like "A.B.C", and we try all possible
		/// splits between namespace and top-level type name, from right to left.
		/// For each candidate top-level type, we walk the nested types.
		/// Also checks type forwarders.
		/// </summary>
		static EntityHandle ResolveTypeInModule(List<TypeNamePart> parts, MetadataFile module)
		{
			var metadata = module.Metadata;
			string topLevelDottedName = parts[0].Name;
			string[] dotParts = topLevelDottedName.Split('.');

			for (int i = dotParts.Length - 1; i >= 0; i--)
			{
				string ns = string.Join(".", dotParts, 0, i);
				string name = dotParts[i];
				int topLevelTpc = (i == dotParts.Length - 1) ? parts[0].TypeParameterCount : 0;
				var topLevelName = new TopLevelTypeName(ns, name, topLevelTpc);

				var typeHandle = module.GetTypeDefinition(topLevelName);

				// Walk remaining dotParts as nested types, then explicit nested parts
				for (int j = i + 1; j < dotParts.Length && !typeHandle.IsNil; j++)
				{
					int tpc = (j == dotParts.Length - 1 && parts.Count == 1) ? parts[0].TypeParameterCount : 0;
					typeHandle = FindNestedType(metadata, typeHandle, dotParts[j], tpc);
				}

				// Walk explicit nested parts (from '.' after `n or {args})
				for (int j = 1; j < parts.Count && !typeHandle.IsNil; j++)
				{
					typeHandle = FindNestedType(metadata, typeHandle, parts[j].Name, parts[j].TypeParameterCount);
				}

				if (!typeHandle.IsNil)
					return typeHandle;

				// Try as type forwarder with the same structure
				FullTypeName fullTypeName = topLevelName;
				for (int j = i + 1; j < dotParts.Length; j++)
				{
					int tpc = (j == dotParts.Length - 1 && parts.Count == 1) ? parts[0].TypeParameterCount : 0;
					fullTypeName = fullTypeName.NestedType(dotParts[j], tpc);
				}
				for (int j = 1; j < parts.Count; j++)
				{
					fullTypeName = fullTypeName.NestedType(parts[j].Name, parts[j].TypeParameterCount);
				}
				var exportedType = module.GetTypeForwarder(fullTypeName);
				if (!exportedType.IsNil)
					return exportedType;
			}

			return default;
		}

		/// <summary>
		/// Finds a nested type by name and type parameter count within a type definition.
		/// Returns a nil handle if not found.
		/// </summary>
		static TypeDefinitionHandle FindNestedType(MetadataReader metadata, TypeDefinitionHandle declaringTypeHandle, string name, int typeParameterCount)
		{
			var typeDef = metadata.GetTypeDefinition(declaringTypeHandle);
			string lookupName = typeParameterCount > 0 ? name + "`" + typeParameterCount : name;
			foreach (var nestedHandle in typeDef.GetNestedTypes())
			{
				var nestedDef = metadata.GetTypeDefinition(nestedHandle);
				if (metadata.StringComparer.Equals(nestedDef.Name, lookupName))
					return nestedHandle;
			}
			return default;
		}
		#endregion
	}
}
