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
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Contains extension methods for the type system.
	/// </summary>
	public static class TypeSystemExtensions
	{
		#region GetAllBaseTypes
		/// <summary>
		/// Gets all base types.
		/// </summary>
		/// <remarks>This is the reflexive and transitive closure of <see cref="IType.DirectBaseTypes"/>.
		/// Note that this method does not return all supertypes - doing so is impossible due to contravariance
		/// (and undesirable for covariance as the list could become very large).
		/// 
		/// The output is ordered so that base types occur before derived types.
		/// </remarks>
		public static IEnumerable<IType> GetAllBaseTypes(this IType type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			BaseTypeCollector collector = new BaseTypeCollector();
			collector.CollectBaseTypes(type);
			return collector;
		}

		/// <summary>
		/// Gets all non-interface base types.
		/// </summary>
		/// <remarks>
		/// When <paramref name="type"/> is an interface, this method will also return base interfaces (return same output as GetAllBaseTypes()).
		/// 
		/// The output is ordered so that base types occur before derived types.
		/// </remarks>
		public static IEnumerable<IType> GetNonInterfaceBaseTypes(this IType type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			BaseTypeCollector collector = new BaseTypeCollector();
			collector.SkipImplementedInterfaces = true;
			collector.CollectBaseTypes(type);
			return collector;
		}
		#endregion

		#region GetAllBaseTypeDefinitions
		/// <summary>
		/// Gets all base type definitions.
		/// The output is ordered so that base types occur before derived types.
		/// </summary>
		/// <remarks>
		/// This is equivalent to type.GetAllBaseTypes().Select(t => t.GetDefinition()).Where(d => d != null).Distinct().
		/// </remarks>
		public static IEnumerable<ITypeDefinition> GetAllBaseTypeDefinitions(this IType type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			return type.GetAllBaseTypes().Select(t => t.GetDefinition()).Where(d => d != null).Distinct();
		}

		/// <summary>
		/// Gets whether this type definition is derived from the base type definition.
		/// </summary>
		public static bool IsDerivedFrom(this ITypeDefinition type, ITypeDefinition baseType)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			if (baseType == null)
				return false;
			if (type.Compilation != baseType.Compilation)
			{
				throw new InvalidOperationException("Both arguments to IsDerivedFrom() must be from the same compilation.");
			}
			return type.GetAllBaseTypeDefinitions().Contains(baseType);
		}

		/// <summary>
		/// Gets whether this type definition is derived from a given known type.
		/// </summary>
		public static bool IsDerivedFrom(this ITypeDefinition type, KnownTypeCode baseType)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			if (baseType == KnownTypeCode.None)
				return false;
			return IsDerivedFrom(type, type.Compilation.FindType(baseType).GetDefinition());
		}
		#endregion

		#region GetDeclaringTypeDefinitionsOrThis
		/// <summary>
		/// Returns all declaring type definitions of this type definition.
		/// The output is ordered so that inner types occur before outer types.
		/// </summary>
		public static IEnumerable<ITypeDefinition> GetDeclaringTypeDefinitions(this ITypeDefinition definition)
		{
			if (definition == null)
			{
				throw new ArgumentNullException(nameof(definition));
			}

			while (definition != null)
			{
				yield return definition;
				definition = definition.DeclaringTypeDefinition;
			}
		}
		#endregion

		#region IsOpen / IsUnbound / IsUnmanagedType / IsKnownType
		sealed class TypeClassificationVisitor : TypeVisitor
		{
			internal bool isOpen;
			internal IEntity typeParameterOwner;
			int typeParameterOwnerNestingLevel;

			public override IType VisitTypeParameter(ITypeParameter type)
			{
				isOpen = true;
				// If both classes and methods, or different classes (nested types)
				// are involved, find the most specific one
				int newNestingLevel = GetNestingLevel(type.Owner);
				if (newNestingLevel > typeParameterOwnerNestingLevel)
				{
					typeParameterOwner = type.Owner;
					typeParameterOwnerNestingLevel = newNestingLevel;
				}
				return base.VisitTypeParameter(type);
			}

			static int GetNestingLevel(IEntity entity)
			{
				int level = 0;
				while (entity != null)
				{
					level++;
					entity = entity.DeclaringTypeDefinition;
				}
				return level;
			}
		}

		/// <summary>
		/// Gets whether the type is an open type (contains type parameters).
		/// </summary>
		/// <example>
		/// <code>
		/// class X&lt;T&gt; {
		///   List&lt;T&gt; open;
		///   X&lt;X&lt;T[]&gt;&gt; open;
		///   X&lt;string&gt; closed;
		///   int closed;
		/// }
		/// </code>
		/// </example>
		public static bool IsOpen(this IType type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			TypeClassificationVisitor v = new TypeClassificationVisitor();
			type.AcceptVisitor(v);
			return v.isOpen;
		}

		/// <summary>
		/// Gets the entity that owns the type parameters occurring in the specified type.
		/// If both class and method type parameters are present, the method is returned.
		/// Returns null if the specified type is closed.
		/// </summary>
		/// <seealso cref="IsOpen"/>
		static IEntity GetTypeParameterOwner(IType type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			TypeClassificationVisitor v = new TypeClassificationVisitor();
			type.AcceptVisitor(v);
			return v.typeParameterOwner;
		}

		/// <summary>
		/// Gets whether the type is unbound (is a generic type, but no type arguments were provided).
		/// </summary>
		/// <remarks>
		/// In "<c>typeof(List&lt;Dictionary&lt;,&gt;&gt;)</c>", only the Dictionary is unbound, the List is considered
		/// bound despite containing an unbound type.
		/// This method returns false for partially parameterized types (<c>Dictionary&lt;string, &gt;</c>).
		/// </remarks>
		public static bool IsUnbound(this IType type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			return (type is ITypeDefinition || type is UnknownType) && type.TypeParameterCount > 0;
		}

		/// <summary>
		/// Gets whether the type is considered unmanaged.
		/// </summary>
		/// <remarks>
		/// The C# 6.0 spec lists the following criteria: An unmanaged type is one of the following
		/// * sbyte, byte, short, ushort, int, uint, long, ulong, char, float, double, decimal, or bool
		/// * any enum type
		/// * any pointer type
		/// * any user-defined struct type that is not a constructed (= generic) type and contains fields of unmanaged types only.
		/// 
		/// C# 8.0 removes the restriction that constructed (= generic) types are not considered unmanaged types.
		/// </remarks>
		public static bool IsUnmanagedType(this IType type, bool allowGenerics)
		{
			HashSet<IType> types = null;
			return IsUnmanagedTypeInternal(type);

			bool IsUnmanagedTypeInternal(IType type)
			{
				if (type.Kind is TypeKind.Enum or TypeKind.Pointer or TypeKind.FunctionPointer)
				{
					return true;
				}
				if (type is ITypeParameter tp)
				{
					return tp.HasUnmanagedConstraint;
				}
				var def = type.GetDefinition();
				if (def == null)
				{
					return false;
				}
				switch (def.KnownTypeCode)
				{
					case KnownTypeCode.Void:
					case KnownTypeCode.Boolean:
					case KnownTypeCode.Char:
					case KnownTypeCode.SByte:
					case KnownTypeCode.Byte:
					case KnownTypeCode.Int16:
					case KnownTypeCode.UInt16:
					case KnownTypeCode.Int32:
					case KnownTypeCode.UInt32:
					case KnownTypeCode.Int64:
					case KnownTypeCode.UInt64:
					case KnownTypeCode.Decimal:
					case KnownTypeCode.Single:
					case KnownTypeCode.Double:
					case KnownTypeCode.IntPtr:
					case KnownTypeCode.UIntPtr:
					case KnownTypeCode.TypedReference:
						//case KnownTypeCode.ArgIterator:
						//case KnownTypeCode.RuntimeArgumentHandle:
						return true;
				}
				if (type.Kind == TypeKind.Struct)
				{
					if (!allowGenerics && def.TypeParameterCount > 0)
					{
						return false;
					}
					if (types == null)
					{
						types = new HashSet<IType>();
					}
					types.Add(type);
					foreach (var f in type.GetFields(f => !f.IsStatic))
					{
						if (types.Contains(f.Type))
						{
							types.Remove(type);
							return false;
						}
						if (!IsUnmanagedTypeInternal(f.Type))
						{
							types.Remove(type);
							return false;
						}
					}
					types.Remove(type);
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Gets whether the type is the specified known type.
		/// For generic known types, this returns true for any parameterization of the type (and also for the definition itself).
		/// </summary>
		public static bool IsKnownType(this IType type, KnownTypeCode knownType)
		{
			var def = type.GetDefinition();
			return def != null && def.KnownTypeCode == knownType;
		}

		/// <summary>
		/// Gets whether the type is the specified known type.
		/// For generic known types, this returns true for any parameterization of the type (and also for the definition itself).
		/// </summary>
		internal static bool IsKnownType(this IType type, KnownAttribute knownType)
		{
			var def = type.GetDefinition();
			return def != null && def.FullTypeName.IsKnownType(knownType);
		}

		public static bool IsKnownType(this FullTypeName typeName, KnownTypeCode knownType)
		{
			return typeName == KnownTypeReference.Get(knownType).TypeName;
		}

		public static bool IsKnownType(this TopLevelTypeName typeName, KnownTypeCode knownType)
		{
			return typeName == KnownTypeReference.Get(knownType).TypeName;
		}

		internal static bool IsKnownType(this FullTypeName typeName, KnownAttribute knownType)
		{
			return typeName == knownType.GetTypeName();
		}

		internal static bool IsKnownType(this TopLevelTypeName typeName, KnownAttribute knownType)
		{
			return typeName == knownType.GetTypeName();
		}
		#endregion

		#region GetDelegateInvokeMethod
		/// <summary>
		/// Gets the invoke method for a delegate type.
		/// </summary>
		/// <remarks>
		/// Returns null if the type is not a delegate type; or if the invoke method could not be found.
		/// </remarks>
		public static IMethod GetDelegateInvokeMethod(this IType type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			if (type.Kind == TypeKind.Delegate)
				return type.GetMethods(m => m.Name == "Invoke", GetMemberOptions.IgnoreInheritedMembers).FirstOrDefault();
			else
				return null;
		}
		#endregion

		public static IType SkipModifiers(this IType ty)
		{
			while (ty is ModifiedType mt)
			{
				ty = mt.ElementType;
			}
			return ty;
		}

		public static IType UnwrapByRef(this IType type)
		{
			if (type is ByReferenceType byRef)
			{
				type = byRef.ElementType;
			}
			return type;
		}

		public static bool HasReadonlyModifier(this IMethod accessor)
		{
			return accessor.ThisIsRefReadOnly && accessor.DeclaringTypeDefinition?.IsReadOnly == false;
		}

		public static bool IsAnyPointer(this TypeKind typeKind)
		{
			return typeKind switch {
				TypeKind.Pointer => true,
				TypeKind.FunctionPointer => true,
				_ => false
			};
		}

		#region GetType/Member
		/// <summary>
		/// Gets all type definitions in the compilation.
		/// This may include types from referenced assemblies that are not accessible in the main assembly.
		/// </summary>
		public static IEnumerable<ITypeDefinition> GetAllTypeDefinitions(this ICompilation compilation)
		{
			return compilation.Modules.SelectMany(a => a.TypeDefinitions);
		}

		/// <summary>
		/// Gets all top level type definitions in the compilation.
		/// This may include types from referenced assemblies that are not accessible in the main assembly.
		/// </summary>
		public static IEnumerable<ITypeDefinition> GetTopLevelTypeDefinitions(this ICompilation compilation)
		{
			return compilation.Modules.SelectMany(a => a.TopLevelTypeDefinitions);
		}
		#endregion

		#region Resolve on collections
		public static IReadOnlyList<IType> Resolve(this IList<ITypeReference> typeReferences, ITypeResolveContext context)
		{
			if (typeReferences == null)
				throw new ArgumentNullException(nameof(typeReferences));
			if (typeReferences.Count == 0)
				return EmptyList<IType>.Instance;
			else
				return new ProjectedList<ITypeResolveContext, ITypeReference, IType>(context, typeReferences, (c, t) => t.Resolve(c));
		}

		// There is intentionally no Resolve() overload for IList<IMemberReference>: the resulting IList<Member> would
		// contains nulls when there are resolve errors.
		#endregion

		#region IAssembly.GetTypeDefinition()
		/// <summary>
		/// Retrieves the specified type in this compilation.
		/// Returns an <see cref="UnknownType"/> if the type cannot be found in this compilation.
		/// </summary>
		/// <remarks>
		/// There can be multiple types with the same full name in a compilation, as a
		/// full type name is only unique per assembly.
		/// If there are multiple possible matches, this method will return just one of them.
		/// When possible, use <see cref="IModule.GetTypeDefinition"/> instead to
		/// retrieve a type from a specific assembly.
		/// </remarks>
		public static IType FindType(this ICompilation compilation, FullTypeName fullTypeName)
		{
			if (compilation == null)
				throw new ArgumentNullException(nameof(compilation));
			foreach (IModule asm in compilation.Modules)
			{
				ITypeDefinition def = asm.GetTypeDefinition(fullTypeName);
				if (def != null)
					return def;
			}
			return new UnknownType(fullTypeName);
		}

		/// <summary>
		/// Gets the type definition for the specified unresolved type.
		/// Returns null if the unresolved type does not belong to this assembly.
		/// </summary>
		public static ITypeDefinition GetTypeDefinition(this IModule module, FullTypeName fullTypeName)
		{
			if (module == null)
				throw new ArgumentNullException("assembly");
			TopLevelTypeName topLevelTypeName = fullTypeName.TopLevelTypeName;
			ITypeDefinition typeDef = module.GetTypeDefinition(topLevelTypeName);
			if (typeDef == null)
				return null;
			int typeParameterCount = topLevelTypeName.TypeParameterCount;
			for (int i = 0; i < fullTypeName.NestingLevel; i++)
			{
				string name = fullTypeName.GetNestedTypeName(i);
				typeParameterCount += fullTypeName.GetNestedTypeAdditionalTypeParameterCount(i);
				typeDef = FindNestedType(typeDef, name, typeParameterCount);
				if (typeDef == null)
					break;
			}
			return typeDef;
		}

		static ITypeDefinition FindNestedType(ITypeDefinition typeDef, string name, int typeParameterCount)
		{
			foreach (var nestedType in typeDef.NestedTypes)
			{
				if (nestedType.Name == name && nestedType.TypeParameterCount == typeParameterCount)
					return nestedType;
			}
			return null;
		}
		#endregion

		#region IEntity.GetAttribute
		/// <summary>
		/// Gets whether the entity has an attribute of the specified attribute type.
		/// </summary>
		/// <param name="entity">The entity on which the attributes are declared.</param>
		/// <param name="attributeType">The attribute type to look for.</param>
		/// <param name="inherit">
		/// Specifies whether attributes inherited from base classes and base members
		/// (if the given <paramref name="entity"/> in an <c>override</c>)
		/// should be returned.
		/// </param>
		public static bool HasAttribute(this IEntity entity, KnownAttribute attributeType, bool inherit)
		{
			if (!inherit)
				return entity.HasAttribute(attributeType);

			return GetAttribute(entity, attributeType, inherit) != null;
		}

		/// <summary>
		/// Gets the attribute of the specified attribute type.
		/// </summary>
		/// <param name="entity">The entity on which the attributes are declared.</param>
		/// <param name="attributeType">The attribute type to look for.</param>
		/// <param name="inherit">
		/// Specifies whether attributes inherited from base classes and base members
		/// (if the given <paramref name="entity"/> in an <c>override</c>)
		/// should be returned.
		/// </param>
		/// <returns>
		/// Returns the attribute that was found; or <c>null</c> if none was found.
		/// If inherit is true, an from the entity itself will be returned if possible;
		/// and the base entity will only be searched if none exists.
		/// </returns>
		public static IAttribute GetAttribute(this IEntity entity, KnownAttribute attributeType, bool inherit)
		{
			if (inherit)
			{
				if (entity is ITypeDefinition td)
				{
					return InheritanceHelper.GetAttribute(td, attributeType);
				}
				else if (entity is IMember m)
				{
					return InheritanceHelper.GetAttribute(m, attributeType);
				}
				else
				{
					throw new NotSupportedException("Unknown entity type");
				}
			}
			else
			{
				return entity.GetAttribute(attributeType);
			}
		}

		/// <summary>
		/// Gets the attributes on the entity.
		/// </summary>
		/// <param name="entity">The entity on which the attributes are declared.</param>
		/// <param name="inherit">
		/// Specifies whether attributes inherited from base classes and base members
		/// (if the given <paramref name="entity"/> in an <c>override</c>)
		/// should be returned.
		/// </param>
		/// <returns>
		/// Returns the list of attributes that were found.
		/// If inherit is true, attributes from the entity itself are returned first;
		/// followed by attributes inherited from the base entity.
		/// </returns>
		public static IEnumerable<IAttribute> GetAttributes(this IEntity entity, bool inherit)
		{
			if (inherit)
			{
				if (entity is ITypeDefinition td)
				{
					return InheritanceHelper.GetAttributes(td);
				}
				else if (entity is IMember m)
				{
					return InheritanceHelper.GetAttributes(m);
				}
				else
				{
					throw new NotSupportedException("Unknown entity type");
				}
			}
			else
			{
				return entity.GetAttributes();
			}
		}
		#endregion

		#region IParameter.GetAttribute
		/// <summary>
		/// Gets whether the parameter has an attribute of the specified attribute type.
		/// </summary>
		/// <param name="parameter">The parameter on which the attributes are declared.</param>
		/// <param name="attributeType">The attribute type to look for.</param>
		public static bool HasAttribute(this IParameter parameter, KnownAttribute attributeType)
		{
			return GetAttribute(parameter, attributeType) != null;
		}

		/// <summary>
		/// Gets the attribute of the specified attribute type.
		/// </summary>
		/// <param name="parameter">The parameter on which the attributes are declared.</param>
		/// <param name="attributeType">The attribute type to look for.</param>
		/// <returns>
		/// Returns the attribute that was found; or <c>null</c> if none was found.
		/// </returns>
		public static IAttribute GetAttribute(this IParameter parameter, KnownAttribute attributeType)
		{
			return parameter.GetAttributes().FirstOrDefault(a => a.AttributeType.IsKnownType(attributeType));
		}
		#endregion

		#region IAssembly.GetTypeDefinition(string,string,int)
		/// <summary>
		/// Gets the type definition for a top-level type.
		/// </summary>
		/// <remarks>This method uses ordinal name comparison, not the compilation's name comparer.</remarks>
		public static ITypeDefinition GetTypeDefinition(this IModule module, string namespaceName, string name, int typeParameterCount = 0)
		{
			if (module == null)
				throw new ArgumentNullException("assembly");
			return module.GetTypeDefinition(new TopLevelTypeName(namespaceName, name, typeParameterCount));
		}
		#endregion

		#region ResolveResult
		public static ISymbol GetSymbol(this ResolveResult rr)
		{
			if (rr is LocalResolveResult)
			{
				return ((LocalResolveResult)rr).Variable;
			}
			else if (rr is MemberResolveResult)
			{
				return ((MemberResolveResult)rr).Member;
			}
			else if (rr is TypeResolveResult)
			{
				return ((TypeResolveResult)rr).Type.GetDefinition();
			}
			else if (rr is ConversionResolveResult)
			{
				return ((ConversionResolveResult)rr).Input.GetSymbol();
			}

			return null;
		}
		#endregion

		public static IType GetElementTypeFromIEnumerable(this IType collectionType, ICompilation compilation, bool allowIEnumerator, out bool? isGeneric)
		{
			bool foundNonGenericIEnumerable = false;
			foreach (IType baseType in collectionType.GetAllBaseTypes())
			{
				ITypeDefinition baseTypeDef = baseType.GetDefinition();
				if (baseTypeDef != null)
				{
					KnownTypeCode typeCode = baseTypeDef.KnownTypeCode;
					if (typeCode == KnownTypeCode.IEnumerableOfT || (allowIEnumerator && typeCode == KnownTypeCode.IEnumeratorOfT))
					{
						ParameterizedType pt = baseType as ParameterizedType;
						if (pt != null)
						{
							isGeneric = true;
							return pt.GetTypeArgument(0);
						}
					}
					if (typeCode == KnownTypeCode.IEnumerable || (allowIEnumerator && typeCode == KnownTypeCode.IEnumerator))
						foundNonGenericIEnumerable = true;
				}
			}
			// System.Collections.IEnumerable found in type hierarchy -> Object is element type.
			if (foundNonGenericIEnumerable)
			{
				isGeneric = false;
				return compilation.FindType(KnownTypeCode.Object);
			}
			isGeneric = null;
			return SpecialType.UnknownType;
		}

		public static bool FullNameIs(this IMember member, string type, string name)
		{
			return member.Name == name && member.DeclaringType?.FullName == type;
		}

		public static KnownAttribute IsBuiltinAttribute(this ITypeDefinition type)
		{
			return KnownAttributes.IsKnownAttributeType(type);
		}

		public static IType WithoutNullability(this IType type)
		{
			return type.ChangeNullability(Nullability.Oblivious);
		}

		public static bool IsDirectImportOf(this ITypeDefinition type, IModule module)
		{
			var moduleReference = type.ParentModule;
			foreach (var asmRef in module.PEFile.AssemblyReferences)
			{
				if (asmRef.FullName == moduleReference.FullAssemblyName)
					return true;
				if (asmRef.Name == "netstandard" && asmRef.GetPublicKeyToken() != null)
				{
					var referencedModule = module.Compilation.FindModuleByReference(asmRef);
					if (referencedModule != null && !referencedModule.PEFile.GetTypeForwarder(type.FullTypeName).IsNil)
						return true;
				}
			}
			return false;
		}

		public static IModule FindModuleByReference(this ICompilation compilation, IAssemblyReference assemblyName)
		{
			foreach (var module in compilation.Modules)
			{
				if (string.Equals(module.FullAssemblyName, assemblyName.FullName, StringComparison.OrdinalIgnoreCase))
				{
					return module;
				}
			}
			foreach (var module in compilation.Modules)
			{
				if (string.Equals(module.Name, assemblyName.Name, StringComparison.OrdinalIgnoreCase))
				{
					return module;
				}
			}
			return null;
		}

		/// <summary>
		/// When given a generic type definition, returns the self-parameterized type
		/// (i.e. the type of "this" within the type definition).
		/// When given a non-generic type definition, returns that definition unchanged.
		/// </summary>
		public static IType AsParameterizedType(this ITypeDefinition td)
		{
			if (td.TypeParameterCount == 0)
			{
				return td;
			}
			else
			{
				return new ParameterizedType(td, td.TypeArguments);
			}
		}

		public static INamespace GetNamespaceByFullName(this ICompilation compilation, string name)
		{
			if (string.IsNullOrEmpty(name))
				return compilation.RootNamespace;
			var parts = name.Split('.');
			var ns = compilation.RootNamespace;
			foreach (var part in parts)
			{
				var child = ns.GetChildNamespace(part);
				if (child == null)
					return null;
				ns = child;
			}
			return ns;
		}
	}
}
