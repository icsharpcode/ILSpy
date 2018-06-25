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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	sealed class MetadataProperty : IProperty
	{
		const Accessibility InvalidAccessibility = (Accessibility)0xff;

		readonly MetadataAssembly assembly;
		readonly PropertyDefinitionHandle propertyHandle;
		readonly MethodDefinitionHandle getterHandle;
		readonly MethodDefinitionHandle setterHandle;
		readonly string name;
		readonly SymbolKind symbolKind;

		// lazy-loaded:
		IAttribute[] customAttributes;
		volatile Accessibility cachedAccessiblity = InvalidAccessibility;
		IParameter[] parameters;
		IType returnType;

		internal MetadataProperty(MetadataAssembly assembly, PropertyDefinitionHandle handle)
		{
			Debug.Assert(assembly != null);
			Debug.Assert(!handle.IsNil);
			this.assembly = assembly;
			this.propertyHandle = handle;

			var metadata = assembly.metadata;
			var prop = metadata.GetPropertyDefinition(handle);
			var accessors = prop.GetAccessors();
			getterHandle = accessors.Getter;
			setterHandle = accessors.Setter;
			name = metadata.GetString(prop.Name);
			if (name == (DeclaringTypeDefinition as MetadataTypeDefinition)?.DefaultMemberName) {
				symbolKind = SymbolKind.Indexer;
			} else {
				symbolKind = SymbolKind.Property;
			}
		}

		public EntityHandle MetadataToken => propertyHandle;
		public string Name => name;

		public bool CanGet => !getterHandle.IsNil;
		public bool CanSet => !setterHandle.IsNil;

		public IMethod Getter => assembly.GetDefinition(getterHandle);
		public IMethod Setter => assembly.GetDefinition(setterHandle);

		public bool IsIndexer => symbolKind == SymbolKind.Indexer;
		public SymbolKind SymbolKind => symbolKind;

		#region Signature (ReturnType + Parameters)
		public IReadOnlyList<IParameter> Parameters {
			get {
				var parameters = LazyInit.VolatileRead(ref this.parameters);
				if (parameters != null)
					return parameters;
				DecodeSignature();
				return this.parameters;
			}
		}

		public IType ReturnType {
			get {
				var returnType = LazyInit.VolatileRead(ref this.returnType);
				if (returnType != null)
					return returnType;
				DecodeSignature();
				return this.returnType;
			}
		}

		private void DecodeSignature()
		{
			var propertyDef = assembly.metadata.GetPropertyDefinition(propertyHandle);
			var genericContext = new GenericContext(DeclaringType.TypeParameters);
			var signature = propertyDef.DecodeSignature(assembly.TypeProvider, genericContext);
			ParameterHandleCollection? parameterHandles;
			if (!getterHandle.IsNil)
				parameterHandles = assembly.metadata.GetMethodDefinition(getterHandle).GetParameters();
			else if (!setterHandle.IsNil)
				parameterHandles = assembly.metadata.GetMethodDefinition(setterHandle).GetParameters();
			else
				parameterHandles = null;
			var (returnType, parameters) = MetadataMethod.DecodeSignature(assembly, this, signature, parameterHandles);
			LazyInit.GetOrSet(ref this.returnType, returnType);
			LazyInit.GetOrSet(ref this.parameters, parameters);
		}
		#endregion

		public bool IsExplicitInterfaceImplementation => (Getter ?? Setter)?.IsExplicitInterfaceImplementation ?? false;
		public IEnumerable<IMember> ImplementedInterfaceMembers => GetInterfaceMembersFromAccessor(Getter ?? Setter);

		internal static IEnumerable<IMember> GetInterfaceMembersFromAccessor(IMethod method)
		{
			if (method == null)
				return EmptyList<IMember>.Instance;
			return method.ImplementedInterfaceMembers.Select(m => ((IMethod)m).AccessorOwner).Where(m => m != null);
		}

		public ITypeDefinition DeclaringTypeDefinition => (Getter ?? Setter)?.DeclaringTypeDefinition;
		public IType DeclaringType => (Getter ?? Setter)?.DeclaringType;
		IMember IMember.MemberDefinition => this;
		TypeParameterSubstitution IMember.Substitution => TypeParameterSubstitution.Identity;

		#region Attributes
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
			var propertyDef = metadata.GetPropertyDefinition(propertyHandle);
			b.Add(propertyDef.GetCustomAttributes());
			return b.Build();
		}
		#endregion

		#region Accessibility
		public Accessibility Accessibility {
			get {
				var acc = cachedAccessiblity;
				if (acc == InvalidAccessibility)
					return cachedAccessiblity = ComputeAccessibility();
				else
					return acc;
			}
		}

		Accessibility ComputeAccessibility()
		{
			if (IsOverride && (getterHandle.IsNil || setterHandle.IsNil)) {
				foreach (var baseMember in InheritanceHelper.GetBaseMembers(this, includeImplementedInterfaces: false)) {
					if (!baseMember.IsOverride)
						return baseMember.Accessibility;
				}
			}
			return MergePropertyAccessibility(
				this.Getter?.Accessibility ?? Accessibility.None,
				this.Setter?.Accessibility ?? Accessibility.None);
		}

		static internal Accessibility MergePropertyAccessibility(Accessibility left, Accessibility right)
		{
			if (left == Accessibility.Public || right == Accessibility.Public)
				return Accessibility.Public;

			if (left == Accessibility.ProtectedOrInternal || right == Accessibility.ProtectedOrInternal)
				return Accessibility.ProtectedOrInternal;

			if (left == Accessibility.Protected && right == Accessibility.Internal ||
				left == Accessibility.Internal && right == Accessibility.Protected)
				return Accessibility.ProtectedOrInternal;

			if (left == Accessibility.Protected || right == Accessibility.Protected)
				return Accessibility.Protected;

			if (left == Accessibility.Internal || right == Accessibility.Internal)
				return Accessibility.Internal;

			if (left == Accessibility.ProtectedAndInternal || right == Accessibility.ProtectedAndInternal)
				return Accessibility.ProtectedAndInternal;

			return left;
		}
		#endregion

		public bool IsStatic => (Getter ?? Setter)?.IsStatic ?? false;
		public bool IsAbstract => (Getter ?? Setter)?.IsAbstract ?? false;
		public bool IsSealed => (Getter ?? Setter)?.IsSealed ?? false;
		public bool IsVirtual => (Getter ?? Setter)?.IsVirtual ?? false;
		public bool IsOverride => (Getter ?? Setter)?.IsOverride ?? false;
		public bool IsOverridable => (Getter ?? Setter)?.IsOverridable ?? false;

		bool IEntity.IsShadowing => (Getter ?? Setter)?.IsShadowing ?? false;

		public IAssembly ParentAssembly => assembly;
		public ICompilation Compilation => assembly.Compilation;

		public string FullName => $"{DeclaringType?.FullName}.{Name}";
		public string ReflectionName => $"{DeclaringType?.ReflectionName}.{Name}";
		public string Namespace => DeclaringType?.Namespace ?? string.Empty;

		bool IMember.Equals(IMember obj, TypeVisitor typeNormalization)
		{
			return this == obj;
		}

		public IMember Specialize(TypeParameterSubstitution substitution)
		{
			return SpecializedProperty.Create(this, substitution);
		}
	}
}
