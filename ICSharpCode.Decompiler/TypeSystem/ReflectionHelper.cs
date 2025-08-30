// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.Diagnostics;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Static helper methods for reflection names.
	/// </summary>
	public static class ReflectionHelper
	{
		#region ICompilation.FindType
		/// <summary>
		/// Retrieves the specified type in this compilation.
		/// Returns <see cref="SpecialType.UnknownType"/> if the type cannot be found in this compilation.
		/// </summary>
		/// <remarks>
		/// This method cannot be used with open types; all type parameters will be substituted
		/// with <see cref="SpecialType.UnknownType"/>.
		/// </remarks>
		public static IType FindType(this ICompilation compilation, Type type)
		{
			return ParseReflectionName(type.AssemblyQualifiedName, new SimpleTypeResolveContext(compilation));
		}

		public static IType FindType(this ICompilation compilation, StackType stackType, Sign sign = Sign.None)
		{
			switch (stackType)
			{
				case StackType.Unknown:
					return SpecialType.UnknownType;
				case StackType.Ref:
					return new ByReferenceType(SpecialType.UnknownType);
				default:
					return compilation.FindType(stackType.ToKnownTypeCode(sign));
			}
		}
		#endregion

		#region SplitTypeParameterCountFromReflectionName
		/// <summary>
		/// Removes the ` with type parameter count from the reflection name.
		/// </summary>
		/// <remarks>Do not use this method with the full name of inner classes.</remarks>
		public static string SplitTypeParameterCountFromReflectionName(string reflectionName)
		{
			int pos = reflectionName.LastIndexOf('`');
			if (pos < 0)
			{
				return reflectionName;
			}
			else
			{
				return reflectionName.Substring(0, pos);
			}
		}

		/// <summary>
		/// Removes the ` with type parameter count from the reflection name.
		/// </summary>
		/// <remarks>Do not use this method with the full name of inner classes.</remarks>
		public static string SplitTypeParameterCountFromReflectionName(string reflectionName, out int typeParameterCount)
		{
			int pos = reflectionName.LastIndexOf('`');
			if (pos < 0)
			{
				typeParameterCount = 0;
				return reflectionName;
			}
			else
			{
				string typeCount = reflectionName.Substring(pos + 1);
				if (int.TryParse(typeCount, out typeParameterCount))
					return reflectionName.Substring(0, pos);
				else
					return reflectionName;
			}
		}
		#endregion

		#region TypeCode support
		/// <summary>
		/// Retrieves a built-in type using the specified type code.
		/// </summary>
		public static IType FindType(this ICompilation compilation, TypeCode typeCode)
		{
			return compilation.FindType((KnownTypeCode)typeCode);
		}

		/// <summary>
		/// Gets the type code for the specified type, or TypeCode.Empty if none of the other type codes match.
		/// </summary>
		public static TypeCode GetTypeCode(this IType type)
		{
			ITypeDefinition def = type as ITypeDefinition;
			if (def != null)
			{
				KnownTypeCode typeCode = def.KnownTypeCode;
				if (typeCode <= KnownTypeCode.String && typeCode != KnownTypeCode.Void)
					return (TypeCode)typeCode;
				else
					return TypeCode.Empty;
			}
			return TypeCode.Empty;
		}
		#endregion

		#region ParseReflectionName
		/// <summary>
		/// Parses a reflection name into a type reference.
		/// </summary>
		/// <param name="reflectionTypeName">The reflection name of the type.</param>
		/// <returns>A type reference that represents the reflection name.</returns>
		/// <exception cref="ReflectionNameParseException">The syntax of the reflection type name is invalid</exception>
		/// <remarks>
		/// If the type is open (contains type parameters '`0' or '``0'),
		/// an <see cref="ITypeResolveContext"/> with the appropriate CurrentTypeDefinition/CurrentMember is required
		/// to resolve the reference to the ITypeParameter.
		/// For looking up closed, assembly qualified type names, the root type resolve context for the compilation
		/// is sufficient.
		/// When looking up a type name that isn't assembly qualified, the type reference will look in
		/// <see cref="ITypeResolveContext.CurrentModule"/> first, and if the type is not found there,
		/// it will look in all other assemblies of the compilation.
		/// </remarks>
		/// <seealso cref="FullTypeName(string)"/>
		public static IType ParseReflectionName(string reflectionTypeName, ITypeResolveContext resolveContext)
		{
			if (reflectionTypeName == null)
				throw new ArgumentNullException(nameof(reflectionTypeName));
			if (!TypeName.TryParse(reflectionTypeName.AsSpan(), out var result))
			{
				throw new ReflectionNameParseException(0, "Invalid type name: " + reflectionTypeName);
			}
			return ResolveTypeName(result, resolveContext);
		}

		private static IType ResolveTypeName(TypeName result, ITypeResolveContext resolveContext)
		{
			if (result.IsArray)
			{
				return new ArrayType(
					resolveContext.Compilation,
					ResolveTypeName(result.GetElementType(), resolveContext),
					result.GetArrayRank()
				);
			}
			else if (result.IsByRef)
			{
				return new ByReferenceType(
					ResolveTypeName(result.GetElementType(), resolveContext)
				);
			}
			else if (result.IsConstructedGenericType)
			{
				IType genericType = ResolveTypeName(result.GetGenericTypeDefinition(), resolveContext);
				var genericArgs = result.GetGenericArguments();
				if (genericType.TypeParameterCount == 0)
				{
					return genericType;
				}

				IType[] resolvedTypes = new IType[genericType.TypeParameterCount];
				for (int i = 0; i < genericArgs.Length; i++)
				{
					if (i < genericArgs.Length)
						resolvedTypes[i] = ResolveTypeName(genericArgs[i], resolveContext);
					else
						resolvedTypes[i] = SpecialType.UnknownType;
				}
				return new ParameterizedType(genericType, resolvedTypes);
			}
			else if (result.IsNested)
			{
				var declaringType = ResolveTypeName(result.DeclaringType, resolveContext).GetDefinition();
				var plainName = SplitTypeParameterCountFromReflectionName(result.Name, out int tpc);
				if (declaringType != null)
				{
					foreach (var type in declaringType.NestedTypes)
					{
						if (type.Name == plainName && type.TypeParameterCount == tpc + declaringType.TypeParameterCount)
							return type;
					}
				}
				return new UnknownType(new FullTypeName(result.FullName));
			}
			else if (result.IsPointer)
			{
				return new PointerType(
					ResolveTypeName(result.GetElementType(), resolveContext)
				);
			}
			else
			{
				Debug.Assert(result.IsSimple);
				if (result.FullName.Length > 1 && result.FullName[0] == '`')
				{
					if (result.FullName.Length > 2 && result.FullName[1] == '`')
					{
						if (int.TryParse(result.FullName.Substring(2), out int index))
						{
							if (resolveContext.CurrentMember is IMethod m && index < m.TypeParameters.Count)
							{
								return m.TypeParameters[index];
							}
							return DummyTypeParameter.GetMethodTypeParameter(index);
						}
					}
					else if (int.TryParse(result.FullName.Substring(1), out int index))
					{
						if (resolveContext.CurrentTypeDefinition != null && index < resolveContext.CurrentTypeDefinition.TypeParameterCount)
						{
							return resolveContext.CurrentTypeDefinition.TypeParameters[index];
						}
						return DummyTypeParameter.GetClassTypeParameter(index);
					}
				}
				var topLevelTypeName = new TopLevelTypeName(result.FullName);
				if (result.AssemblyName != null)
				{
					var module = resolveContext.Compilation.FindModuleByAssemblyNameInfo(result.AssemblyName);
					if (module != null)
					{
						return (IType)module.GetTypeDefinition(topLevelTypeName) ?? new UnknownType(topLevelTypeName);
					}
				}

				foreach (var module in resolveContext.Compilation.Modules)
				{
					var type = module.GetTypeDefinition(topLevelTypeName);
					if (type != null)
						return type;
				}

				return new UnknownType(topLevelTypeName);
			}
		}

		internal static int ReadTypeParameterCount(string reflectionTypeName, ref int pos)
		{
			int startPos = pos;
			while (pos < reflectionTypeName.Length)
			{
				char c = reflectionTypeName[pos];
				if (c < '0' || c > '9')
					break;
				pos++;
			}
			int tpc;
			if (!int.TryParse(reflectionTypeName.Substring(startPos, pos - startPos), out tpc))
				throw new ReflectionNameParseException(pos, "Expected type parameter count");
			return tpc;
		}
		#endregion
	}
}
