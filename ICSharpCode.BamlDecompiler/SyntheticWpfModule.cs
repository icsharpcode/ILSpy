// Copyright (c) 2026 Siegfried Pammer
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
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.BamlDecompiler
{
	/// <summary>
	/// An artificial stand-in for one of the well-known BAML assemblies (e.g. PresentationFramework)
	/// when the real assembly cannot be resolved - for instance when a WPF binary is inspected on a
	/// machine that has no WPF installed (Linux/macOS).
	/// <para>
	/// It mirrors <see cref="MinimalCorlib"/>: it materializes type definitions on request but carries
	/// no members. Only the types the BAML decompiler explicitly seeds via <see cref="RegisterType"/>
	/// are materialized; every other lookup returns <c>null</c>, so references to non-well-known types
	/// keep decomposing to <c>UnknownType</c> exactly as they do when the assembly is entirely absent.
	/// </para>
	/// </summary>
	sealed class SyntheticWpfModule : IModule
	{
		/// <summary>
		/// Creates a module reference that resolves to a <see cref="SyntheticWpfModule"/> standing in
		/// for <paramref name="assemblyName"/>. When <paramref name="presentationXmlnsNamespace"/> is
		/// non-null, the synthesized assembly exposes an <c>XmlnsDefinitionAttribute</c> mapping every
		/// seeded CLR namespace to that XML namespace, so known WPF types serialize with the clean
		/// presentation xmlns instead of a clr-namespace fallback.
		/// </summary>
		public static IModuleReference CreateReference(IAssemblyReference assemblyName, string presentationXmlnsNamespace)
		{
			return new SyntheticModuleReference(assemblyName, presentationXmlnsNamespace);
		}

		public ICompilation Compilation { get; }

		readonly IAssemblyReference assemblyName;
		readonly string presentationXmlnsNamespace;
		readonly Dictionary<TopLevelTypeName, SyntheticTypeDefinition> typeDefinitions = new();
		readonly SyntheticNamespace rootNamespace;
		IReadOnlyList<IAttribute> assemblyAttributes;

		SyntheticWpfModule(ICompilation compilation, IAssemblyReference assemblyName, string presentationXmlnsNamespace)
		{
			this.Compilation = compilation;
			this.assemblyName = assemblyName;
			this.presentationXmlnsNamespace = presentationXmlnsNamespace;
			this.rootNamespace = new SyntheticNamespace(this, null, string.Empty, string.Empty);
		}

		/// <summary>
		/// Seeds this module with a synthetic definition for the given type and returns it.
		/// Repeated calls for the same type return the cached instance. This is the only way a type
		/// becomes visible on the module; plain <see cref="GetTypeDefinition(TopLevelTypeName)"/>
		/// lookups never create new types.
		/// </summary>
		public ITypeDefinition RegisterType(string ns, string name)
		{
			var typeName = new TopLevelTypeName(ns, name);
			if (!typeDefinitions.TryGetValue(typeName, out var typeDef))
			{
				typeDef = new SyntheticTypeDefinition(this, typeName);
				typeDefinitions.Add(typeName, typeDef);
				// Invalidate the cached xmlns attributes; a namespace may have appeared.
				assemblyAttributes = null;
			}
			return typeDef;
		}

		bool IModule.IsMainModule => Compilation.MainModule == this;

		string IModule.AssemblyName => assemblyName.Name;
		Version IModule.AssemblyVersion => assemblyName.Version;
		string IModule.FullAssemblyName => assemblyName.FullName;
		string ISymbol.Name => assemblyName.Name;
		SymbolKind ISymbol.SymbolKind => SymbolKind.Module;

		ICSharpCode.Decompiler.Metadata.MetadataFile IModule.MetadataFile => null;
		INamespace IModule.RootNamespace => rootNamespace;

		public IEnumerable<ITypeDefinition> TopLevelTypeDefinitions => typeDefinitions.Values;
		public IEnumerable<ITypeDefinition> TypeDefinitions => typeDefinitions.Values;

		public ITypeDefinition GetTypeDefinition(TopLevelTypeName topLevelTypeName)
		{
			return typeDefinitions.TryGetValue(topLevelTypeName, out var typeDef) ? typeDef : null;
		}

		IEnumerable<IAttribute> IModule.GetAssemblyAttributes() => GetAssemblyAttributes();
		IEnumerable<IAttribute> IModule.GetModuleAttributes() => EmptyList<IAttribute>.Instance;

		bool IModule.InternalsVisibleTo(IModule module) => module == this;

		IReadOnlyList<IAttribute> GetAssemblyAttributes()
		{
			if (presentationXmlnsNamespace == null)
				return EmptyList<IAttribute>.Instance;
			var attributes = assemblyAttributes;
			if (attributes != null)
				return attributes;

			// Reconstruct the XmlnsDefinitionAttribute set that the real WPF assembly would carry:
			// one entry per seeded CLR namespace, all mapping to the presentation XML namespace.
			var attributeType = new SyntheticTypeDefinition(this,
				new TopLevelTypeName("System.Windows.Markup", "XmlnsDefinitionAttribute"));
			var stringType = Compilation.FindType(KnownTypeCode.String);
			var namespaces = typeDefinitions.Keys.Select(t => t.Namespace).Distinct();
			var list = new List<IAttribute>();
			foreach (var ns in namespaces)
			{
				var fixedArguments = ImmutableArray.Create(
					new CustomAttributeTypedArgument<IType>(stringType, presentationXmlnsNamespace),
					new CustomAttributeTypedArgument<IType>(stringType, ns));
				list.Add(new DefaultAttribute(attributeType, fixedArguments,
					ImmutableArray<CustomAttributeNamedArgument<IType>>.Empty));
			}
			attributes = list;
			assemblyAttributes = attributes;
			return attributes;
		}

		sealed class SyntheticModuleReference : IModuleReference
		{
			readonly IAssemblyReference assemblyName;
			readonly string presentationXmlnsNamespace;

			public SyntheticModuleReference(IAssemblyReference assemblyName, string presentationXmlnsNamespace)
			{
				this.assemblyName = assemblyName;
				this.presentationXmlnsNamespace = presentationXmlnsNamespace;
			}

			IModule IModuleReference.Resolve(ITypeResolveContext context)
			{
				return new SyntheticWpfModule(context.Compilation, assemblyName, presentationXmlnsNamespace);
			}
		}

		sealed class SyntheticNamespace : INamespace
		{
			readonly SyntheticWpfModule module;

			public INamespace ParentNamespace { get; }
			public string FullName { get; }
			public string Name { get; }

			public SyntheticNamespace(SyntheticWpfModule module, INamespace parentNamespace, string fullName, string name)
			{
				this.module = module;
				this.ParentNamespace = parentNamespace;
				this.FullName = fullName;
				this.Name = name;
			}

			string INamespace.ExternAlias => string.Empty;

			IEnumerable<INamespace> INamespace.ChildNamespaces => EmptyList<INamespace>.Instance;
			IEnumerable<ITypeDefinition> INamespace.Types => module.TopLevelTypeDefinitions.Where(td => td.Namespace == FullName);
			IEnumerable<IModule> INamespace.ContributingModules => new[] { module };

			SymbolKind ISymbol.SymbolKind => SymbolKind.Namespace;
			ICompilation ICompilationProvider.Compilation => module.Compilation;

			INamespace INamespace.GetChildNamespace(string name) => null;

			ITypeDefinition INamespace.GetTypeDefinition(string name, int typeParameterCount)
			{
				if (typeParameterCount != 0)
					return null;
				return module.GetTypeDefinition(new TopLevelTypeName(FullName, name));
			}
		}

		sealed class SyntheticTypeDefinition : ITypeDefinition
		{
			readonly SyntheticWpfModule module;
			readonly TopLevelTypeName typeName;

			public SyntheticTypeDefinition(SyntheticWpfModule module, TopLevelTypeName typeName)
			{
				this.module = module;
				this.typeName = typeName;
			}

			IReadOnlyList<ITypeDefinition> ITypeDefinition.NestedTypes => EmptyList<ITypeDefinition>.Instance;
			IReadOnlyList<IMember> ITypeDefinition.Members => EmptyList<IMember>.Instance;
			IEnumerable<IField> ITypeDefinition.Fields => EmptyList<IField>.Instance;
			IEnumerable<IMethod> ITypeDefinition.Methods => EmptyList<IMethod>.Instance;
			IEnumerable<IProperty> ITypeDefinition.Properties => EmptyList<IProperty>.Instance;
			IEnumerable<IEvent> ITypeDefinition.Events => EmptyList<IEvent>.Instance;

			KnownTypeCode ITypeDefinition.KnownTypeCode => KnownTypeCode.None;

			IType ITypeDefinition.EnumUnderlyingType => SpecialType.UnknownType;

			public FullTypeName FullTypeName => typeName;

			public string MetadataName => typeName.Name;

			ITypeDefinition IEntity.DeclaringTypeDefinition => null;
			IType ITypeDefinition.DeclaringType => null;
			IType IType.DeclaringType => null;
			IType IEntity.DeclaringType => null;

			bool ITypeDefinition.HasExtensions => false;
			ExtensionInfo ITypeDefinition.ExtensionInfo => null;
			bool ITypeDefinition.IsReadOnly => false;

			// Materialized WPF types carry no metadata, so treat them all as (reference-type) classes.
			// The BAML decompiler only reads their identity and null-guards everything else.
			TypeKind IType.Kind => TypeKind.Class;
			bool? IType.IsReferenceType => true;
			bool IType.IsByRefLike => false;
			Nullability IType.Nullability => Nullability.Oblivious;
			Nullability ITypeDefinition.NullableContext => Nullability.Oblivious;

			// Synthetic WPF types are always nullability-oblivious and BAML never annotates them.
			IType IType.ChangeNullability(Nullability nullability) => this;

			int IType.TypeParameterCount => 0;
			IReadOnlyList<ITypeParameter> IType.TypeParameters => EmptyList<ITypeParameter>.Instance;
			IReadOnlyList<IType> IType.TypeArguments => EmptyList<IType>.Instance;
			IEnumerable<IType> IType.DirectBaseTypes => EmptyList<IType>.Instance;

			EntityHandle IEntity.MetadataToken => MetadataTokens.TypeDefinitionHandle(0);

			public string Name => typeName.Name;

			IModule IEntity.ParentModule => module;

			Accessibility IEntity.Accessibility => Accessibility.Public;

			bool IEntity.IsStatic => false;
			bool IEntity.IsAbstract => false;
			bool IEntity.IsSealed => false;

			SymbolKind ISymbol.SymbolKind => SymbolKind.TypeDefinition;

			ICompilation ICompilationProvider.Compilation => module.Compilation;

			string INamedElement.FullName => typeName.ReflectionName;
			string INamedElement.ReflectionName => typeName.ReflectionName;
			string INamedElement.Namespace => typeName.Namespace;

			bool IEquatable<IType>.Equals(IType other) => this == other;

			IEnumerable<IMethod> IType.GetAccessors(Predicate<IMethod> filter, GetMemberOptions options) => EmptyList<IMethod>.Instance;
			IEnumerable<IAttribute> IEntity.GetAttributes() => EmptyList<IAttribute>.Instance;
			bool IEntity.HasAttribute(KnownAttribute attribute) => false;
			IAttribute IEntity.GetAttribute(KnownAttribute attribute) => null;
			IEnumerable<IMethod> IType.GetConstructors(Predicate<IMethod> filter, GetMemberOptions options) => EmptyList<IMethod>.Instance;
			IEnumerable<IEvent> IType.GetEvents(Predicate<IEvent> filter, GetMemberOptions options) => EmptyList<IEvent>.Instance;
			IEnumerable<IField> IType.GetFields(Predicate<IField> filter, GetMemberOptions options) => EmptyList<IField>.Instance;
			IEnumerable<IMember> IType.GetMembers(Predicate<IMember> filter, GetMemberOptions options) => EmptyList<IMember>.Instance;
			IEnumerable<IMethod> IType.GetMethods(Predicate<IMethod> filter, GetMemberOptions options) => EmptyList<IMethod>.Instance;
			IEnumerable<IMethod> IType.GetMethods(IReadOnlyList<IType> typeArguments, Predicate<IMethod> filter, GetMemberOptions options) => EmptyList<IMethod>.Instance;
			IEnumerable<IType> IType.GetNestedTypes(Predicate<ITypeDefinition> filter, GetMemberOptions options) => EmptyList<IType>.Instance;
			IEnumerable<IType> IType.GetNestedTypes(IReadOnlyList<IType> typeArguments, Predicate<ITypeDefinition> filter, GetMemberOptions options) => EmptyList<IType>.Instance;
			IEnumerable<IProperty> IType.GetProperties(Predicate<IProperty> filter, GetMemberOptions options) => EmptyList<IProperty>.Instance;

			bool ITypeDefinition.IsRecord => false;

			ITypeDefinition IType.GetDefinition() => this;
			ITypeDefinitionOrUnknown IType.GetDefinitionOrUnknown() => this;
			TypeParameterSubstitution IType.GetSubstitution() => TypeParameterSubstitution.Identity;

			IType IType.AcceptVisitor(TypeVisitor visitor) => visitor.VisitTypeDefinition(this);
			IType IType.VisitChildren(TypeVisitor visitor) => this;

			public override string ToString() => $"[SyntheticWpfType {typeName.ReflectionName}]";
		}
	}
}
