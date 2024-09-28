﻿// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
		TypeParameterSubstitution substitution;

		IType declaringType;
		IType returnType;

		protected SpecializedMember(IMember memberDefinition)
		{
			if (memberDefinition == null)
				throw new ArgumentNullException(nameof(memberDefinition));
			if (memberDefinition is SpecializedMember)
				throw new ArgumentException("Member definition cannot be specialized. Please use IMember.Specialize() instead of directly constructing SpecializedMember instances.");

			this.baseMember = memberDefinition;
			this.substitution = TypeParameterSubstitution.Identity;
		}

		/// <summary>
		/// Performs a substitution. This method may only be called by constructors in derived classes.
		/// </summary>
		protected void AddSubstitution(TypeParameterSubstitution newSubstitution)
		{
			Debug.Assert(declaringType == null);
			Debug.Assert(returnType == null);
			this.substitution = TypeParameterSubstitution.Compose(newSubstitution, this.substitution);
		}

		internal IMethod? WrapAccessor(ref IMethod cachingField, IMethod accessorDefinition)
		{
			if (accessorDefinition == null)
				return null;
			var result = LazyInit.VolatileRead(ref cachingField);
			if (result != null)
			{
				return result;
			}
			else
			{
				var sm = accessorDefinition.Specialize(substitution);
				//sm.AccessorOwner = this;
				return LazyInit.GetOrSet(ref cachingField, sm);
			}
		}

		/// <summary>
		/// Gets the substitution belonging to this specialized member.
		/// </summary>
		public TypeParameterSubstitution Substitution {
			get { return substitution; }
		}

		public IType DeclaringType {
			get {
				var result = LazyInit.VolatileRead(ref this.declaringType);
				if (result != null)
					return result;
				IType definitionDeclaringType = baseMember.DeclaringType;
				ITypeDefinition? definitionDeclaringTypeDef = definitionDeclaringType as ITypeDefinition;
				if (definitionDeclaringTypeDef != null && definitionDeclaringType.TypeParameterCount > 0)
				{
					if (substitution.ClassTypeArguments != null && substitution.ClassTypeArguments.Count == definitionDeclaringType.TypeParameterCount)
					{
						result = new ParameterizedType(definitionDeclaringTypeDef, substitution.ClassTypeArguments);
					}
					else
					{
						result = new ParameterizedType(definitionDeclaringTypeDef, definitionDeclaringTypeDef.TypeParameters).AcceptVisitor(substitution);
					}
				}
				else if (definitionDeclaringType != null)
				{
					result = definitionDeclaringType.AcceptVisitor(substitution);
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

		public IMember MemberDefinition {
			get { return baseMember.MemberDefinition; }
		}

		public IType ReturnType {
			get {
				var result = LazyInit.VolatileRead(ref this.returnType);
				if (result != null)
					return result;
				else
					return LazyInit.GetOrSet(ref this.returnType, baseMember.ReturnType.AcceptVisitor(substitution));
			}
			protected set {
				// This setter is used for LiftedUserDefinedOperator, a special case of specialized member
				// (not a normal type parameter substitution).

				// As this setter is used only during construction before the member is published
				// to other threads, we don't need a volatile write.
				this.returnType = value;
			}
		}

		public System.Reflection.Metadata.EntityHandle MetadataToken => baseMember.MetadataToken;

		public bool IsVirtual {
			get { return baseMember.IsVirtual; }
		}

		public bool IsOverride {
			get { return baseMember.IsOverride; }
		}

		public bool IsOverridable {
			get { return baseMember.IsOverridable; }
		}

		public SymbolKind SymbolKind {
			get { return baseMember.SymbolKind; }
		}

		public ITypeDefinition? DeclaringTypeDefinition {
			get { return baseMember.DeclaringTypeDefinition; }
		}

		IEnumerable<IAttribute> IEntity.GetAttributes() => baseMember.GetAttributes();
		bool IEntity.HasAttribute(KnownAttribute attribute) => baseMember.HasAttribute(attribute);
		IAttribute IEntity.GetAttribute(KnownAttribute attribute) => baseMember.GetAttribute(attribute);


		public IEnumerable<IMember> ExplicitlyImplementedInterfaceMembers {
			get {
				// Note: if the interface is generic, then the interface members should already be specialized,
				// so we only need to append our substitution.
				return baseMember.ExplicitlyImplementedInterfaceMembers.Select(m => m.Specialize(substitution));
			}
		}

		public bool IsExplicitInterfaceImplementation {
			get { return baseMember.IsExplicitInterfaceImplementation; }
		}

		public Accessibility Accessibility {
			get { return baseMember.Accessibility; }
		}

		public bool IsStatic {
			get { return baseMember.IsStatic; }
		}

		public bool IsAbstract {
			get { return baseMember.IsAbstract; }
		}

		public bool IsSealed {
			get { return baseMember.IsSealed; }
		}

		public string FullName {
			get { return baseMember.FullName; }
		}

		public string Name {
			get { return baseMember.Name; }
		}

		public string Namespace {
			get { return baseMember.Namespace; }
		}

		public string ReflectionName {
			get { return baseMember.ReflectionName; }
		}

		public ICompilation Compilation {
			get { return baseMember.Compilation; }
		}

		public IModule? ParentModule {
			get { return baseMember.ParentModule; }
		}

		public virtual IMember Specialize(TypeParameterSubstitution newSubstitution)
		{
			return baseMember.Specialize(TypeParameterSubstitution.Compose(newSubstitution, this.substitution));
		}

		public virtual bool Equals(IMember? obj, TypeVisitor typeNormalization)
		{
			SpecializedMember? other = obj as SpecializedMember;
			if (other == null)
				return false;
			return this.baseMember.Equals(other.baseMember, typeNormalization)
				&& this.substitution.Equals(other.substitution, typeNormalization);
		}

		public override bool Equals(object? obj)
		{
			SpecializedMember? other = obj as SpecializedMember;
			if (other == null)
				return false;
			return this.baseMember.Equals(other.baseMember) && this.substitution.Equals(other.substitution);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return 1000000007 * baseMember.GetHashCode() + 1000000009 * substitution.GetHashCode();
			}
		}

		public override string ToString()
		{
			StringBuilder b = new StringBuilder("[");
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
					return LazyInit.GetOrSet(ref this.parameters, CreateParameters(t => t.AcceptVisitor(this.Substitution)));
			}
			protected set {
				// This setter is used for LiftedUserDefinedOperator, a special case of specialized member
				// (not a normal type parameter substitution).

				// As this setter is used only during construction before the member is published
				// to other threads, we don't need a volatile write.
				this.parameters = value;
			}
		}

		protected IParameter[] CreateParameters(Func<IType, IType> substitution)
		{
			var paramDefs = ((IParameterizedMember)this.baseMember).Parameters;
			if (paramDefs.Count == 0)
			{
				return Empty<IParameter>.Array;
			}
			else
			{
				var parameters = new IParameter[paramDefs.Count];
				for (int i = 0; i < parameters.Length; i++)
				{
					var p = paramDefs[i];
					IType newType = substitution(p.Type);
					parameters[i] = new SpecializedParameter(p, newType, this);
				}
				return parameters;
			}
		}

		public override string ToString()
		{
			StringBuilder b = new StringBuilder("[");
			b.Append(GetType().Name);
			b.Append(' ');
			b.Append(this.DeclaringType.ReflectionName);
			b.Append('.');
			b.Append(this.Name);
			b.Append('(');
			for (int i = 0; i < this.Parameters.Count; i++)
			{
				if (i > 0)
					b.Append(", ");
				b.Append(this.Parameters[i].ToString());
			}
			b.Append("):");
			b.Append(this.ReturnType.ReflectionName);
			b.Append(']');
			return b.ToString();
		}
	}
}
