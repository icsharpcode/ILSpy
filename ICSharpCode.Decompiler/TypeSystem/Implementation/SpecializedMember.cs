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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Represents a SpecializedMember (a member on which type substitution has been performed).
	/// </summary>
	public abstract class SpecializedMember : IMember
	{
		protected readonly IMember baseMember;

		IType declaringType;
		IType returnType;
		
		protected SpecializedMember(IMember memberDefinition)
		{
			if (memberDefinition == null)
				throw new ArgumentNullException("memberDefinition");
			if (memberDefinition is SpecializedMember)
				throw new ArgumentException("Member definition cannot be specialized. Please use IMember.Specialize() instead of directly constructing SpecializedMember instances.");
			
			this.baseMember = memberDefinition;
			this.Substitution = TypeParameterSubstitution.Identity;
		}
		
		/// <summary>
		/// Performs a substitution. This method may only be called by constructors in derived classes.
		/// </summary>
		protected void AddSubstitution(TypeParameterSubstitution newSubstitution)
		{
			Debug.Assert(declaringType == null);
			Debug.Assert(returnType == null);
			this.Substitution = TypeParameterSubstitution.Compose(newSubstitution, this.Substitution);
		}
		
		public virtual IMemberReference ToReference()
		{
			return new SpecializingMemberReference(
				baseMember.ToReference(),
				ToTypeReference(Substitution.ClassTypeArguments),
				null);
		}
		
		ISymbolReference ISymbol.ToReference()
		{
			return ToReference();
		}
		
		internal static IList<ITypeReference> ToTypeReference(IReadOnlyList<IType> typeArguments)
		{
			if (typeArguments == null)
				return null;
			else
				return typeArguments.Select(t => t.ToTypeReference()).ToArray();
		}
		
		internal IMethod WrapAccessor(ref IMethod cachingField, IMethod accessorDefinition)
		{
			if (accessorDefinition == null)
				return null;
			var result = LazyInit.VolatileRead(ref cachingField);
			if (result != null) {
				return result;
			} else {
				var sm = accessorDefinition.Specialize(Substitution);
				//sm.AccessorOwner = this;
				return LazyInit.GetOrSet(ref cachingField, sm);
			}
		}
		
		/// <summary>
		/// Gets the substitution belonging to this specialized member.
		/// </summary>
		public TypeParameterSubstitution Substitution { get; private set; }

		public IType DeclaringType {
			get {
				var result = LazyInit.VolatileRead(ref this.declaringType);
				if (result != null)
					return result;
				var definitionDeclaringType = baseMember.DeclaringType;
				var definitionDeclaringTypeDef = definitionDeclaringType as ITypeDefinition;
				if (definitionDeclaringTypeDef != null && definitionDeclaringType.TypeParameterCount > 0) {
					if (Substitution.ClassTypeArguments != null && Substitution.ClassTypeArguments.Count == definitionDeclaringType.TypeParameterCount) {
						result = new ParameterizedType(definitionDeclaringTypeDef, Substitution.ClassTypeArguments);
					} else {
						result = new ParameterizedType(definitionDeclaringTypeDef, definitionDeclaringTypeDef.TypeParameters).AcceptVisitor(Substitution);
					}
				} else if (definitionDeclaringType != null) {
					result = definitionDeclaringType.AcceptVisitor(Substitution);
				}
				return LazyInit.GetOrSet(ref this.declaringType, result);
			}
			internal set {
				// This setter is used as an optimization when the code constructing
				// the SpecializedMember already knows the declaring type.
				Debug.Assert(this.declaringType == null);
				// As this setter is used only during construction before the member is published
				// to other threads, we don't need a volatile write.
				this.declaringType = value;
			}
		}
		
		public IMember MemberDefinition => baseMember.MemberDefinition;

		public IUnresolvedMember UnresolvedMember => baseMember.UnresolvedMember;

		public IType ReturnType {
			get {
				var result = LazyInit.VolatileRead(ref this.returnType);
				if (result != null)
					return result;
				else
					return LazyInit.GetOrSet(ref this.returnType, baseMember.ReturnType.AcceptVisitor(Substitution));
			}
			protected set => this.returnType = value;
		}
		
		public bool IsVirtual => baseMember.IsVirtual;

		public bool IsOverride => baseMember.IsOverride;

		public bool IsOverridable => baseMember.IsOverridable;

		public SymbolKind SymbolKind => baseMember.SymbolKind;

		public ITypeDefinition DeclaringTypeDefinition => baseMember.DeclaringTypeDefinition;

		public IReadOnlyList<IAttribute> Attributes => baseMember.Attributes;

		IReadOnlyList<IMember> implementedInterfaceMembers;
		
		public IReadOnlyList<IMember> ImplementedInterfaceMembers => LazyInitializer.EnsureInitialized(ref implementedInterfaceMembers, FindImplementedInterfaceMembers);

		IReadOnlyList<IMember> FindImplementedInterfaceMembers()
		{
			var definitionImplementations = baseMember.ImplementedInterfaceMembers;
			var result = new IMember[definitionImplementations.Count];
			for (var i = 0; i < result.Length; i++) {
				result[i] = definitionImplementations[i].Specialize(Substitution);
			}
			return result;
		}
		
		public bool IsExplicitInterfaceImplementation => baseMember.IsExplicitInterfaceImplementation;

		public Accessibility Accessibility => baseMember.Accessibility;

		public bool IsStatic => baseMember.IsStatic;

		public bool IsAbstract => baseMember.IsAbstract;

		public bool IsSealed => baseMember.IsSealed;

		public bool IsShadowing => baseMember.IsShadowing;

		public bool IsSynthetic => baseMember.IsSynthetic;

		public bool IsPrivate => baseMember.IsPrivate;

		public bool IsPublic => baseMember.IsPublic;

		public bool IsProtected => baseMember.IsProtected;

		public bool IsInternal => baseMember.IsInternal;

		public bool IsProtectedOrInternal => baseMember.IsProtectedOrInternal;

		public bool IsProtectedAndInternal => baseMember.IsProtectedAndInternal;

		public string FullName => baseMember.FullName;

		public string Name => baseMember.Name;

		public string Namespace => baseMember.Namespace;

		public string ReflectionName => baseMember.ReflectionName;

		public ICompilation Compilation => baseMember.Compilation;

		public IAssembly ParentAssembly => baseMember.ParentAssembly;

		public virtual IMember Specialize(TypeParameterSubstitution newSubstitution)
		{
			return baseMember.Specialize(TypeParameterSubstitution.Compose(newSubstitution, this.Substitution));
		}

		public override bool Equals(object obj)
		{
			var other = obj as SpecializedMember;
			if (other == null)
				return false;
			return this.baseMember.Equals(other.baseMember) && this.Substitution.Equals(other.Substitution);
		}
		
		public override int GetHashCode()
		{
			unchecked {
				return 1000000007 * baseMember.GetHashCode() + 1000000009 * Substitution.GetHashCode();
			}
		}
		
		public override string ToString()
		{
			var b = new StringBuilder("[");
			b.Append(GetType().Name);
			b.Append(' ');
			b.Append(this.DeclaringType.ToString());
			b.Append('.');
			b.Append(this.Name);
			b.Append(':');
			b.Append(this.ReturnType.ToString());
			b.Append(']');
			return b.ToString();
		}
	}
	
	public abstract class SpecializedParameterizedMember : SpecializedMember, IParameterizedMember
	{
		IReadOnlyList<IParameter> parameters;
		
		protected SpecializedParameterizedMember(IParameterizedMember memberDefinition)
			: base(memberDefinition)
		{
		}
		
		public IReadOnlyList<IParameter> Parameters {
			get {
				var result = LazyInit.VolatileRead(ref this.parameters);
				if (result != null)
					return result;
				else
					return LazyInit.GetOrSet(ref this.parameters, CreateParameters(this.Substitution));
			}
			protected set => this.parameters = value;
		}
		
		protected IParameter[] CreateParameters(TypeVisitor substitution)
		{
			var paramDefs = ((IParameterizedMember)this.baseMember).Parameters;
			if (paramDefs.Count == 0) {
				return Empty<IParameter>.Array;
			} else {
				var parameters = new IParameter[paramDefs.Count];
				for (var i = 0; i < parameters.Length; i++) {
					var p = paramDefs[i];
					var newType = p.Type.AcceptVisitor(substitution);
					parameters[i] = new DefaultParameter(
						newType, p.Name, this,
						p.Attributes, p.IsRef, p.IsOut,
						p.IsParams, p.IsOptional, p.ConstantValue
					);
				}
				return parameters;
			}
		}
		
		public override string ToString()
		{
			var b = new StringBuilder("[");
			b.Append(GetType().Name);
			b.Append(' ');
			b.Append(this.DeclaringType.ReflectionName);
			b.Append('.');
			b.Append(this.Name);
			b.Append('(');
			for (var i = 0; i < this.Parameters.Count; i++) {
				if (i > 0) b.Append(", ");
				b.Append(this.Parameters[i].ToString());
			}
			b.Append("):");
			b.Append(this.ReturnType.ReflectionName);
			b.Append(']');
			return b.ToString();
		}
	}
}
