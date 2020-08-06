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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// An artificial "assembly" that contains all known types (<see cref="KnownTypeCode"/>) and no other types.
	/// It does not contain any members.
	/// </summary>
	public sealed class MinimalCorlib : IModule
	{
		/// <summary>
		/// Minimal corlib instance containing all known types.
		/// </summary>
		public static readonly IModuleReference Instance = new CorlibModuleReference(KnownTypeReference.AllKnownTypes);

		public static IModuleReference CreateWithTypes(IEnumerable<KnownTypeReference> types)
		{
			return new CorlibModuleReference(types);
		}

		public ICompilation Compilation { get; }
		CorlibTypeDefinition[] typeDefinitions;
		readonly CorlibNamespace rootNamespace;

		private MinimalCorlib(ICompilation compilation, IEnumerable<KnownTypeReference> types)
		{
			this.Compilation = compilation;
			this.typeDefinitions = types.Select(ktr => new CorlibTypeDefinition(this, ktr.KnownTypeCode)).ToArray();
			this.rootNamespace = new CorlibNamespace(this, null, string.Empty, string.Empty);
		}

		bool IModule.IsMainModule => Compilation.MainModule == this;

		string IModule.AssemblyName => "corlib";
		string IModule.FullAssemblyName => "corlib";
		string ISymbol.Name => "corlib";
		SymbolKind ISymbol.SymbolKind => SymbolKind.Module;

		Metadata.PEFile IModule.PEFile => null;
		INamespace IModule.RootNamespace => rootNamespace;

		public IEnumerable<ITypeDefinition> TopLevelTypeDefinitions => typeDefinitions.Where(td => td != null);
		public IEnumerable<ITypeDefinition> TypeDefinitions => TopLevelTypeDefinitions;

		public ITypeDefinition GetTypeDefinition(TopLevelTypeName topLevelTypeName)
		{
			foreach (var typeDef in typeDefinitions) {
				if (typeDef.FullTypeName == topLevelTypeName)
					return typeDef;
			}
			return null;
		}

		IEnumerable<IAttribute> IModule.GetAssemblyAttributes() => EmptyList<IAttribute>.Instance;
		IEnumerable<IAttribute> IModule.GetModuleAttributes() => EmptyList<IAttribute>.Instance;

		bool IModule.InternalsVisibleTo(IModule module)
		{
			return module == this;
		}

		sealed class CorlibModuleReference : IModuleReference
		{
			readonly IEnumerable<KnownTypeReference> types;

			public CorlibModuleReference(IEnumerable<KnownTypeReference> types)
			{
				this.types = types;
			}

			IModule IModuleReference.Resolve(ITypeResolveContext context)
			{
				return new MinimalCorlib(context.Compilation, types);
			}
		}

		sealed class CorlibNamespace : INamespace
		{
			readonly MinimalCorlib corlib;
			internal List<INamespace> childNamespaces = new List<INamespace>();
			public INamespace ParentNamespace { get; }
			public string FullName { get; }
			public string Name { get; }

			public CorlibNamespace(MinimalCorlib corlib, INamespace parentNamespace, string fullName, string name)
			{
				this.corlib = corlib;
				this.ParentNamespace = parentNamespace;
				this.FullName = fullName;
				this.Name = name;
			}

			string INamespace.ExternAlias => string.Empty;

			IEnumerable<INamespace> INamespace.ChildNamespaces => childNamespaces;
			IEnumerable<ITypeDefinition> INamespace.Types => corlib.TopLevelTypeDefinitions.Where(td => td.Namespace == FullName);

			IEnumerable<IModule> INamespace.ContributingModules => new[] { corlib };

			SymbolKind ISymbol.SymbolKind => SymbolKind.Namespace;
			ICompilation ICompilationProvider.Compilation => corlib.Compilation;

			INamespace INamespace.GetChildNamespace(string name)
			{
				return childNamespaces.FirstOrDefault(ns => ns.Name == name);
			}

			ITypeDefinition INamespace.GetTypeDefinition(string name, int typeParameterCount)
			{
				return corlib.GetTypeDefinition(this.FullName, name, typeParameterCount);
			}
		}

		sealed class CorlibTypeDefinition : ITypeDefinition
		{
			readonly MinimalCorlib corlib;
			readonly KnownTypeCode typeCode;
			readonly TypeKind typeKind;

			public CorlibTypeDefinition(MinimalCorlib corlib, KnownTypeCode typeCode)
			{
				this.corlib = corlib;
				this.typeCode = typeCode;
				this.typeKind = KnownTypeReference.Get(typeCode).typeKind;
			}

			IReadOnlyList<ITypeDefinition> ITypeDefinition.NestedTypes => EmptyList<ITypeDefinition>.Instance;
			IReadOnlyList<IMember> ITypeDefinition.Members => EmptyList<IMember>.Instance;
			IEnumerable<IField> ITypeDefinition.Fields => EmptyList<IField>.Instance;
			IEnumerable<IMethod> ITypeDefinition.Methods => EmptyList<IMethod>.Instance;
			IEnumerable<IProperty> ITypeDefinition.Properties => EmptyList<IProperty>.Instance;
			IEnumerable<IEvent> ITypeDefinition.Events => EmptyList<IEvent>.Instance;

			KnownTypeCode ITypeDefinition.KnownTypeCode => typeCode;

			IType ITypeDefinition.EnumUnderlyingType => SpecialType.UnknownType;

			public FullTypeName FullTypeName => KnownTypeReference.Get(typeCode).TypeName;

			ITypeDefinition IEntity.DeclaringTypeDefinition => null;
			IType ITypeDefinition.DeclaringType => null;
			IType IType.DeclaringType => null;
			IType IEntity.DeclaringType => null;

			bool ITypeDefinition.HasExtensionMethods => false;
			bool ITypeDefinition.IsReadOnly => false;

			TypeKind IType.Kind => typeKind;

			bool? IType.IsReferenceType {
				get {
					switch (typeKind) {
						case TypeKind.Class:
						case TypeKind.Interface:
							return true;
						case TypeKind.Struct:
						case TypeKind.Enum:
							return false;
						default:
							return null;
					}
				}
			}

			bool IType.IsByRefLike => false;
			Nullability IType.Nullability => Nullability.Oblivious;
			Nullability ITypeDefinition.NullableContext => Nullability.Oblivious;

			IType IType.ChangeNullability(Nullability nullability)
			{
				if (nullability == Nullability.Oblivious)
					return this;
				else
					return new NullabilityAnnotatedType(this, nullability);
			}

			int IType.TypeParameterCount => KnownTypeReference.Get(typeCode).TypeParameterCount;

			IReadOnlyList<ITypeParameter> IType.TypeParameters => DummyTypeParameter.GetClassTypeParameterList(KnownTypeReference.Get(typeCode).TypeParameterCount);
			IReadOnlyList<IType> IType.TypeArguments => DummyTypeParameter.GetClassTypeParameterList(KnownTypeReference.Get(typeCode).TypeParameterCount);

			IEnumerable<IType> IType.DirectBaseTypes {
				get {
					var baseType = KnownTypeReference.Get(typeCode).baseType;
					if (baseType != KnownTypeCode.None)
						return new[] { corlib.typeDefinitions[(int)baseType] };
					else
						return EmptyList<IType>.Instance;
				}
			}

			EntityHandle IEntity.MetadataToken => MetadataTokens.TypeDefinitionHandle(0);

			public string Name => KnownTypeReference.Get(typeCode).Name;

			IModule IEntity.ParentModule => corlib;

			Accessibility IEntity.Accessibility => Accessibility.Public;

			bool IEntity.IsStatic => false;
			bool IEntity.IsAbstract => typeKind == TypeKind.Interface;
			bool IEntity.IsSealed => typeKind == TypeKind.Struct;

			SymbolKind ISymbol.SymbolKind => SymbolKind.TypeDefinition;

			ICompilation ICompilationProvider.Compilation => corlib.Compilation;

			string INamedElement.FullName {
				get {
					var ktr = KnownTypeReference.Get(typeCode);
					return ktr.Namespace + "." + ktr.Name;
				}
			}

			string INamedElement.ReflectionName => KnownTypeReference.Get(typeCode).TypeName.ReflectionName;

			string INamedElement.Namespace => KnownTypeReference.Get(typeCode).Namespace;

			bool IEquatable<IType>.Equals(IType other)
			{
				return this == other;
			}

			IEnumerable<IMethod> IType.GetAccessors(Predicate<IMethod> filter, GetMemberOptions options)
			{
				return EmptyList<IMethod>.Instance;
			}

			IEnumerable<IAttribute> IEntity.GetAttributes()
			{
				return EmptyList<IAttribute>.Instance;
			}

			IEnumerable<IMethod> IType.GetConstructors(Predicate<IMethod> filter, GetMemberOptions options)
			{
				return EmptyList<IMethod>.Instance;
			}

			IEnumerable<IEvent> IType.GetEvents(Predicate<IEvent> filter, GetMemberOptions options)
			{
				return EmptyList<IEvent>.Instance;
			}

			IEnumerable<IField> IType.GetFields(Predicate<IField> filter, GetMemberOptions options)
			{
				return EmptyList<IField>.Instance;
			}

			IEnumerable<IMember> IType.GetMembers(Predicate<IMember> filter, GetMemberOptions options)
			{
				return EmptyList<IMember>.Instance;
			}

			IEnumerable<IMethod> IType.GetMethods(Predicate<IMethod> filter, GetMemberOptions options)
			{
				return EmptyList<IMethod>.Instance;
			}

			IEnumerable<IMethod> IType.GetMethods(IReadOnlyList<IType> typeArguments, Predicate<IMethod> filter, GetMemberOptions options)
			{
				return EmptyList<IMethod>.Instance;
			}

			IEnumerable<IType> IType.GetNestedTypes(Predicate<ITypeDefinition> filter, GetMemberOptions options)
			{
				return EmptyList<IType>.Instance;
			}

			IEnumerable<IType> IType.GetNestedTypes(IReadOnlyList<IType> typeArguments, Predicate<ITypeDefinition> filter, GetMemberOptions options)
			{
				return EmptyList<IType>.Instance;
			}

			IEnumerable<IProperty> IType.GetProperties(Predicate<IProperty> filter, GetMemberOptions options)
			{
				return EmptyList<IProperty>.Instance;
			}

			ITypeDefinition IType.GetDefinition() => this;
			TypeParameterSubstitution IType.GetSubstitution() => TypeParameterSubstitution.Identity;

			IType IType.AcceptVisitor(TypeVisitor visitor)
			{
				return visitor.VisitTypeDefinition(this);
			}

			IType IType.VisitChildren(TypeVisitor visitor)
			{
				return this;
			}

			public override string ToString()
			{
				return $"[MinimalCorlibType {typeCode}]";
			}
		}
	}
}
