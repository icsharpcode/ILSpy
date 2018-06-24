// Copyright (c) 2018 Daniel Grunwald
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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Type definition backed by System.Reflection.Metadata
	/// </summary>
	sealed class MetadataTypeDefinition : ITypeDefinition
	{
		readonly MetadataAssembly assembly;
		readonly TypeDefinitionHandle handle;

		// eagerly loaded:
		readonly FullTypeName fullTypeName;
		readonly TypeAttributes attributes;
		public TypeKind Kind { get; }
		public ITypeDefinition DeclaringTypeDefinition { get; }
		public IReadOnlyList<ITypeParameter> TypeParameters { get; }
		public KnownTypeCode KnownTypeCode { get; }
		public IType EnumUnderlyingType { get; }

		// lazy-loaded:
		IAttribute[] customAttributes;

		internal MetadataTypeDefinition(MetadataAssembly assembly, TypeDefinitionHandle handle)
		{
			Debug.Assert(assembly != null);
			Debug.Assert(!handle.IsNil);
			this.assembly = assembly;
			this.handle = handle;
			var metadata = assembly.metadata;
			var td = metadata.GetTypeDefinition(handle);
			this.attributes = td.Attributes;
			this.fullTypeName = td.GetFullTypeName(metadata);
			// Find DeclaringType + KnownTypeCode:
			if (fullTypeName.IsNested) {
				this.DeclaringTypeDefinition = assembly.GetDefinition(td.GetDeclaringType());
			} else {
				var topLevelTypeName = fullTypeName.TopLevelTypeName;
				for (int i = 0; i < KnownTypeReference.KnownTypeCodeCount; i++) {
					var ktr = KnownTypeReference.Get((KnownTypeCode)i);
					if (ktr != null && ktr.TypeName == topLevelTypeName) {
						this.KnownTypeCode = (KnownTypeCode)i;
						break;
					}
				}
			}
			// Find type kind:
			if ((attributes & TypeAttributes.ClassSemanticsMask) == TypeAttributes.Interface) {
				this.Kind = TypeKind.Interface;
			} else if (td.IsEnum(metadata, out var underlyingType)) {
				this.Kind = TypeKind.Enum;
				this.EnumUnderlyingType = assembly.Compilation.FindType(underlyingType.ToKnownTypeCode());
			} else if (td.IsValueType(metadata)) {
				this.Kind = TypeKind.Struct;
			} else if (td.IsDelegate(metadata)) {
				this.Kind = TypeKind.Delegate;
			} else {
				this.Kind = TypeKind.Class;
			}
			// Create type parameters:
			this.TypeParameters = MetadataTypeParameter.Create(assembly, this, td.GetGenericParameters());
		}

		public override string ToString()
		{
			return $"{MetadataTokens.GetToken(handle):X8} {fullTypeName}";
		}

		public IReadOnlyList<ITypeDefinition> NestedTypes => throw new NotImplementedException();

		IMember[] members;

		public IReadOnlyList<IMember> Members {
			get {
				var members = LazyInit.VolatileRead(ref this.members);
				if (members != null)
					return members;
				members = this.Fields.Concat<IMember>(this.Methods).Concat(this.Properties).Concat(this.Events).ToArray();
				return LazyInit.GetOrSet(ref this.members, members);
			}
		}

		IField[] fields;

		public IEnumerable<IField> Fields {
			get {
				var fields = LazyInit.VolatileRead(ref this.fields);
				if (fields != null)
					return fields;
				var metadata = assembly.metadata;
				var fieldCollection = metadata.GetTypeDefinition(handle).GetFields();
				var fieldList = new List<IField>(fieldCollection.Count);
				foreach (FieldDefinitionHandle h in fieldCollection) {
					var field = metadata.GetFieldDefinition(h);
					var attr = field.Attributes;
					if (assembly.IsVisible(attr) && (attr & FieldAttributes.SpecialName) == 0) {
						fieldList.Add(assembly.GetDefinition(h));
					}
				}
				return LazyInit.GetOrSet(ref this.fields, fieldList.ToArray());
			}
		}

		public IEnumerable<IMethod> Methods => throw new NotImplementedException();

		public IEnumerable<IProperty> Properties => throw new NotImplementedException();

		public IEnumerable<IEvent> Events => throw new NotImplementedException();



		public IType DeclaringType => DeclaringTypeDefinition;

		public bool HasExtensionMethods => throw new NotImplementedException();

		public bool? IsReferenceType {
			get {
				switch (Kind) {
					case TypeKind.Struct:
					case TypeKind.Enum:
					case TypeKind.Void:
						return false;
					default:
						return true;
				}
			}
		}

		public int TypeParameterCount => TypeParameters.Count;

		IReadOnlyList<IType> IType.TypeArguments => TypeParameters;

		public IEnumerable<IType> DirectBaseTypes => throw new NotImplementedException();

		public EntityHandle MetadataToken => handle;

		public FullTypeName FullTypeName => fullTypeName;
		public string Name => fullTypeName.Name;

		public IAssembly ParentAssembly => assembly;

		#region Type Attributes
		public IReadOnlyList<IAttribute> Attributes {
			get {
				var attr = LazyInit.VolatileRead(ref this.customAttributes);
				if (attr != null)
					return attr;
				return LazyInit.GetOrSet(ref this.customAttributes, DecodeAttributes());
			}
		}

		IAttribute[] DecodeAttributes()
		{
			var b = new AttributeListBuilder(assembly);
			var metadata = assembly.metadata;
			var typeDefinition = metadata.GetTypeDefinition(handle);

			// SerializableAttribute
			if ((typeDefinition.Attributes & TypeAttributes.Serializable) != 0)
				b.Add(KnownAttribute.Serializable);

			// ComImportAttribute
			if ((typeDefinition.Attributes & TypeAttributes.Import) != 0)
				b.Add(KnownAttribute.ComImport);

			#region StructLayoutAttribute
			LayoutKind layoutKind = LayoutKind.Auto;
			switch (typeDefinition.Attributes & TypeAttributes.LayoutMask) {
				case TypeAttributes.SequentialLayout:
					layoutKind = LayoutKind.Sequential;
					break;
				case TypeAttributes.ExplicitLayout:
					layoutKind = LayoutKind.Explicit;
					break;
			}
			CharSet charSet = CharSet.None;
			switch (typeDefinition.Attributes & TypeAttributes.StringFormatMask) {
				case TypeAttributes.AnsiClass:
					charSet = CharSet.Ansi;
					break;
				case TypeAttributes.AutoClass:
					charSet = CharSet.Auto;
					break;
				case TypeAttributes.UnicodeClass:
					charSet = CharSet.Unicode;
					break;
			}
			var layout = typeDefinition.GetLayout();
			LayoutKind defaultLayoutKind = Kind == TypeKind.Struct ? LayoutKind.Sequential : LayoutKind.Auto;
			if (layoutKind != defaultLayoutKind || charSet != CharSet.Ansi || layout.PackingSize > 0 || layout.Size > 0) {
				var structLayoutAttributeType = Compilation.FindType(KnownAttribute.StructLayout);
				var layoutKindType = Compilation.FindType(new TopLevelTypeName("System.Runtime.InteropServices", "LayoutKind"));
				var positionalArguments = new ResolveResult[] {
					new ConstantResolveResult(layoutKindType, (int)layoutKind)
				};
				var namedArguments = new List<KeyValuePair<IMember, ResolveResult>>(3);
				if (charSet != CharSet.Ansi) {
					var charSetType = Compilation.FindType(new TopLevelTypeName("System.Runtime.InteropServices", "CharSet"));
					namedArguments.Add(b.MakeNamedArg(structLayoutAttributeType,
						"CharSet", charSetType, (int)charSet));
				}
				if (layout.PackingSize > 0) {
					namedArguments.Add(b.MakeNamedArg(structLayoutAttributeType,
						"Pack", KnownTypeCode.Int32, (int)layout.PackingSize));
				}
				if (layout.Size > 0) {
					namedArguments.Add(b.MakeNamedArg(structLayoutAttributeType,
						"Size", KnownTypeCode.Int32, (int)layout.Size));
				}
				b.Add(new DefaultAttribute(
					structLayoutAttributeType,
					positionalArguments, namedArguments
				));
			}
			#endregion

			b.Add(typeDefinition.GetCustomAttributes());
			b.AddSecurityAttributes(typeDefinition.GetDeclarativeSecurityAttributes());

			return b.Build();
		}
		#endregion

		public Accessibility Accessibility {
			get {
				switch (attributes & TypeAttributes.VisibilityMask) {
					case TypeAttributes.NotPublic:
					case TypeAttributes.NestedAssembly:
						return Accessibility.Internal;
					case TypeAttributes.Public:
					case TypeAttributes.NestedPublic:
						return Accessibility.Public;
					case TypeAttributes.NestedPrivate:
						return Accessibility.Private;
					case TypeAttributes.NestedFamily:
						return Accessibility.Protected;
					case TypeAttributes.NestedFamANDAssem:
						return Accessibility.ProtectedAndInternal;
					case TypeAttributes.NestedFamORAssem:
						return Accessibility.ProtectedOrInternal;
					default:
						return Accessibility.None;
				}
			}
		}

		public bool IsStatic => (attributes & (TypeAttributes.Abstract | TypeAttributes.Sealed)) == (TypeAttributes.Abstract | TypeAttributes.Sealed);
		public bool IsAbstract => (attributes & TypeAttributes.Abstract) != 0;
		public bool IsSealed => (attributes & TypeAttributes.Sealed) != 0;

		bool IEntity.IsShadowing => throw new NotImplementedException();

		public SymbolKind SymbolKind => SymbolKind.TypeDefinition;

		public ICompilation Compilation => assembly.Compilation;

		public string FullName => throw new NotImplementedException();
		public string ReflectionName => fullTypeName.ReflectionName;
		public string Namespace => fullTypeName.TopLevelTypeName.Namespace;

		ITypeDefinition IType.GetDefinition() => this;
		TypeParameterSubstitution IType.GetSubstitution() => TypeParameterSubstitution.Identity;
		TypeParameterSubstitution IType.GetSubstitution(IReadOnlyList<IType> methodTypeArguments) => TypeParameterSubstitution.Identity;

		public IType AcceptVisitor(TypeVisitor visitor)
		{
			return visitor.VisitTypeDefinition(this);
		}

		IType IType.VisitChildren(TypeVisitor visitor)
		{
			return this;
		}

		bool IEquatable<IType>.Equals(IType other)
		{
			return this == other;
		}

		#region GetNestedTypes
		public IEnumerable<IType> GetNestedTypes(Predicate<ITypeDefinition> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			const GetMemberOptions opt = GetMemberOptions.IgnoreInheritedMembers | GetMemberOptions.ReturnMemberDefinitions;
			if ((options & opt) == opt) {
				return GetFiltered(this.NestedTypes, filter);
			} else {
				return GetMembersHelper.GetNestedTypes(this, filter, options);
			}
		}
		
		public IEnumerable<IType> GetNestedTypes(IReadOnlyList<IType> typeArguments, Predicate<ITypeDefinition> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return GetMembersHelper.GetNestedTypes(this, typeArguments, filter, options);
		}
		#endregion

		#region GetMembers()
		IEnumerable<T> GetFiltered<T>(IEnumerable<T> input, Predicate<T> filter) where T : class
		{
			if (filter == null)
				return input;
			else
				return ApplyFilter(input, filter);
		}

		IEnumerable<T> ApplyFilter<T>(IEnumerable<T> input, Predicate<T> filter) where T : class
		{
			foreach (var member in input) {
				if (filter(member))
					yield return member;
			}
		}

		public IEnumerable<IMethod> GetMethods(Predicate<IMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers) {
				return GetFiltered(this.Methods, ExtensionMethods.And(m => !m.IsConstructor, filter));
			} else {
				return GetMembersHelper.GetMethods(this, filter, options);
			}
		}

		public IEnumerable<IMethod> GetMethods(IReadOnlyList<IType> typeArguments, Predicate<IMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			return GetMembersHelper.GetMethods(this, typeArguments, filter, options);
		}

		public IEnumerable<IMethod> GetConstructors(Predicate<IMethod> filter = null, GetMemberOptions options = GetMemberOptions.IgnoreInheritedMembers)
		{
			if (ComHelper.IsComImport(this)) {
				IType coClass = ComHelper.GetCoClass(this);
				using (var busyLock = BusyManager.Enter(this)) {
					if (busyLock.Success) {
						return coClass.GetConstructors(filter, options)
							.Select(m => new SpecializedMethod(m, m.Substitution) { DeclaringType = this });
					}
				}
				return EmptyList<IMethod>.Instance;
			}
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers) {
				return GetFiltered(this.Methods, ExtensionMethods.And(m => m.IsConstructor && !m.IsStatic, filter));
			} else {
				return GetMembersHelper.GetConstructors(this, filter, options);
			}
		}

		public IEnumerable<IProperty> GetProperties(Predicate<IProperty> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers) {
				return GetFiltered(this.Properties, filter);
			} else {
				return GetMembersHelper.GetProperties(this, filter, options);
			}
		}

		public IEnumerable<IField> GetFields(Predicate<IField> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers) {
				return GetFiltered(this.Fields, filter);
			} else {
				return GetMembersHelper.GetFields(this, filter, options);
			}
		}

		public IEnumerable<IEvent> GetEvents(Predicate<IEvent> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers) {
				return GetFiltered(this.Events, filter);
			} else {
				return GetMembersHelper.GetEvents(this, filter, options);
			}
		}

		public IEnumerable<IMember> GetMembers(Predicate<IMember> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers) {
				return GetFiltered(this.Members, filter);
			} else {
				return GetMembersHelper.GetMembers(this, filter, options);
			}
		}
		
		public IEnumerable<IMethod> GetAccessors(Predicate<IMethod> filter = null, GetMemberOptions options = GetMemberOptions.None)
		{
			if ((options & GetMemberOptions.IgnoreInheritedMembers) == GetMemberOptions.IgnoreInheritedMembers) {
				return GetFilteredAccessors(filter);
			} else {
				return GetMembersHelper.GetAccessors(this, filter, options);
			}
		}

		IEnumerable<IMethod> GetFilteredAccessors(Predicate<IMethod> filter)
		{
			foreach (var prop in this.Properties) {
				var getter = prop.Getter;
				if (getter != null && (filter == null || filter(getter)))
					yield return getter;
				var setter = prop.Setter;
				if (setter != null && (filter == null || filter(setter)))
					yield return setter;
			}
			foreach (var ev in this.Events) {
				var adder = ev.AddAccessor;
				if (adder != null && (filter == null || filter(adder)))
					yield return adder;
				var remover = ev.RemoveAccessor;
				if (remover != null && (filter == null || filter(remover)))
					yield return remover;
				var invoker = ev.InvokeAccessor;
				if (invoker != null && (filter == null || filter(invoker)))
					yield return remover;
			}
		}
		#endregion

		#region GetInterfaceImplementation
		public IMember GetInterfaceImplementation(IMember interfaceMember)
		{
			return GetInterfaceImplementation(new[] { interfaceMember })[0];
		}

		public IReadOnlyList<IMember> GetInterfaceImplementation(IReadOnlyList<IMember> interfaceMembers)
		{
			// TODO: review the subtle rules for interface reimplementation,
			// write tests and fix this method.
			// Also virtual/override is going to be tricky -
			// I think we'll need to consider the 'virtual' method first for
			// reimplemenatation purposes, but then actually return the 'override'
			// (as that's the method that ends up getting called)

			interfaceMembers = interfaceMembers.ToList(); // avoid evaluating more than once

			var result = new IMember[interfaceMembers.Count];
			var signatureToIndexDict = new MultiDictionary<IMember, int>(SignatureComparer.Ordinal);
			for (int i = 0; i < interfaceMembers.Count; i++) {
				signatureToIndexDict.Add(interfaceMembers[i], i);
			}
			foreach (var member in GetMembers(m => !m.IsExplicitInterfaceImplementation)) {
				foreach (int interfaceMemberIndex in signatureToIndexDict[member]) {
					result[interfaceMemberIndex] = member;
				}
			}
			foreach (var explicitImpl in GetMembers(m => m.IsExplicitInterfaceImplementation)) {
				foreach (var interfaceMember in explicitImpl.ImplementedInterfaceMembers) {
					foreach (int potentialMatchingIndex in signatureToIndexDict[interfaceMember]) {
						if (interfaceMember.Equals(interfaceMembers[potentialMatchingIndex])) {
							result[potentialMatchingIndex] = explicitImpl;
						}
					}
				}
			}
			return result;
		}
		#endregion
	}
}
