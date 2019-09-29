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
using System.Reflection;
using System.Text;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Represents a specialized IMethod (e.g. after type substitution).
	/// </summary>
	public class SpecializedMethod : SpecializedParameterizedMember, IMethod
	{
		internal static IMethod Create(IMethod methodDefinition, TypeParameterSubstitution substitution)
		{
			if (TypeParameterSubstitution.Identity.Equals(substitution))
				return methodDefinition;
			if (methodDefinition.DeclaringType is ArrayType)
				return new SpecializedMethod(methodDefinition, substitution);
			if (methodDefinition.TypeParameters.Count == 0) {
				if (methodDefinition.DeclaringType.TypeParameterCount == 0)
					return methodDefinition;
				if (substitution.MethodTypeArguments != null && substitution.MethodTypeArguments.Count > 0)
					substitution = new TypeParameterSubstitution(substitution.ClassTypeArguments, EmptyList<IType>.Instance);
			}
			return new SpecializedMethod(methodDefinition, substitution);
		}

		readonly IMethod methodDefinition;
		readonly ITypeParameter[] specializedTypeParameters;
		readonly bool isParameterized;
		readonly TypeParameterSubstitution substitutionWithoutSpecializedTypeParameters;
		
		public SpecializedMethod(IMethod methodDefinition, TypeParameterSubstitution substitution)
			: base(methodDefinition)
		{
			if (substitution == null)
				throw new ArgumentNullException("substitution");
			this.methodDefinition = methodDefinition;
			this.isParameterized = substitution.MethodTypeArguments != null;
			if (methodDefinition.TypeParameters.Count > 0) {
				// The method is generic, so we need to specialize the type parameters
				// (for specializing the constraints, and also to set the correct Owner)
				specializedTypeParameters = new ITypeParameter[methodDefinition.TypeParameters.Count];
				for (int i = 0; i < specializedTypeParameters.Length; i++) {
					specializedTypeParameters[i] = new SpecializedTypeParameter(methodDefinition.TypeParameters[i], this);
				}
				if (!isParameterized) {
					// Add substitution that replaces the base method's type parameters with our specialized version
					// but do this only if the type parameters on the baseMember have not already been substituted
					substitutionWithoutSpecializedTypeParameters = this.Substitution;
					AddSubstitution(new TypeParameterSubstitution(null, specializedTypeParameters));
				}
			}
			// Add the main substitution after the method type parameter specialization.
			AddSubstitution(substitution);
			if (substitutionWithoutSpecializedTypeParameters != null) {
				// If we already have a substitution without specialized type parameters, update that:
				substitutionWithoutSpecializedTypeParameters = TypeParameterSubstitution.Compose(substitution, substitutionWithoutSpecializedTypeParameters);
			} else {
				// Otherwise just use the whole substitution, as that doesn't contain specialized type parameters
				// in this case.
				substitutionWithoutSpecializedTypeParameters = this.Substitution;
			}
			if (specializedTypeParameters != null) {
				// Set the substitution on the type parameters to the final composed substitution
				foreach (var tp in specializedTypeParameters.OfType<SpecializedTypeParameter>()) {
					if (tp.Owner == this)
						tp.substitution = base.Substitution;
				}
			}
		}
		
		public IReadOnlyList<IType> TypeArguments {
			get { return this.Substitution.MethodTypeArguments ?? EmptyList<IType>.Instance; }
		}

		public IEnumerable<IAttribute> GetReturnTypeAttributes() => methodDefinition.GetReturnTypeAttributes();
		public bool ReturnTypeIsRefReadOnly => methodDefinition.ReturnTypeIsRefReadOnly;

		bool IMethod.ThisIsRefReadOnly => methodDefinition.ThisIsRefReadOnly;

		public IReadOnlyList<ITypeParameter> TypeParameters {
			get {
				return specializedTypeParameters ?? methodDefinition.TypeParameters;
			}
		}

		public bool IsExtensionMethod {
			get { return methodDefinition.IsExtensionMethod; }
		}

		public bool IsLocalFunction {
			get { return methodDefinition.IsLocalFunction; }
		}

		public bool IsConstructor {
			get { return methodDefinition.IsConstructor; }
		}
		
		public bool IsDestructor {
			get { return methodDefinition.IsDestructor; }
		}
		
		public bool IsOperator {
			get { return methodDefinition.IsOperator; }
		}
		
		public bool HasBody {
			get { return methodDefinition.HasBody; }
		}
		
		public bool IsAccessor {
			get { return methodDefinition.IsAccessor; }
		}

		public MethodSemanticsAttributes AccessorKind => methodDefinition.AccessorKind;

		public IMethod ReducedFrom {
			get { return null; }
		}

		IMember accessorOwner;
		
		public IMember AccessorOwner {
			get {
				var result = LazyInit.VolatileRead(ref accessorOwner);
				if (result != null) {
					return result;
				} else {
					var ownerDefinition = methodDefinition.AccessorOwner;
					if (ownerDefinition == null)
						return null;
					result = ownerDefinition.Specialize(this.Substitution);
					return LazyInit.GetOrSet(ref accessorOwner, result);
				}
			}
			internal set {
				accessorOwner = value;
			}
		}

		public override bool Equals(IMember obj, TypeVisitor typeNormalization)
		{
			SpecializedMethod other = obj as SpecializedMethod;
			if (other == null)
				return false;
			return this.baseMember.Equals(other.baseMember, typeNormalization)
				&& this.substitutionWithoutSpecializedTypeParameters.Equals(other.substitutionWithoutSpecializedTypeParameters, typeNormalization);
		}

		public override bool Equals(object obj)
		{
			SpecializedMethod other = obj as SpecializedMethod;
			if (other == null)
				return false;
			return this.baseMember.Equals(other.baseMember) && this.substitutionWithoutSpecializedTypeParameters.Equals(other.substitutionWithoutSpecializedTypeParameters);
		}
		
		public override int GetHashCode()
		{
			unchecked {
				return 1000000013 * baseMember.GetHashCode() + 1000000009 * substitutionWithoutSpecializedTypeParameters.GetHashCode();
			}
		}

		public override IMember Specialize(TypeParameterSubstitution newSubstitution)
		{
			return methodDefinition.Specialize(TypeParameterSubstitution.Compose(newSubstitution, substitutionWithoutSpecializedTypeParameters));
		}
		
		IMethod IMethod.Specialize(TypeParameterSubstitution newSubstitution)
		{
			return methodDefinition.Specialize(TypeParameterSubstitution.Compose(newSubstitution, substitutionWithoutSpecializedTypeParameters));
		}
		
		public override string ToString()
		{
			StringBuilder b = new StringBuilder("[");
			b.Append(GetType().Name);
			b.Append(' ');
			b.Append(this.DeclaringType.ReflectionName);
			b.Append('.');
			b.Append(this.Name);
			if (this.TypeArguments.Count > 0) {
				b.Append('[');
				for (int i = 0; i < this.TypeArguments.Count; i++) {
					if (i > 0) b.Append(", ");
					b.Append(this.TypeArguments[i].ToString());
				}
				b.Append(']');
			} else if (this.TypeParameters.Count > 0) {
				b.Append("``");
				b.Append(this.TypeParameters.Count);
			}
			b.Append('(');
			for (int i = 0; i < this.Parameters.Count; i++) {
				if (i > 0) b.Append(", ");
				b.Append(this.Parameters[i].ToString());
			}
			b.Append("):");
			b.Append(this.ReturnType.ToString());
			b.Append(']');
			return b.ToString();
		}
		
		sealed class SpecializedTypeParameter : AbstractTypeParameter
		{
			readonly ITypeParameter baseTp;
			
			// The substition is set at the end of SpecializedMethod constructor
			internal TypeVisitor substitution;
			
			public SpecializedTypeParameter(ITypeParameter baseTp, IMethod specializedOwner)
				: base(specializedOwner, baseTp.Index, baseTp.Name, baseTp.Variance)
			{
				// We don't have to consider already-specialized baseTps because
				// we read the baseTp directly from the unpacked memberDefinition.
				this.baseTp = baseTp;
			}

			public override IEnumerable<IAttribute> GetAttributes() => baseTp.GetAttributes();

			public override int GetHashCode()
			{
				return baseTp.GetHashCode() ^ this.Owner.GetHashCode();
			}
			
			public override bool Equals(IType other)
			{
				// Compare the owner, not the substitution, because the substitution may contain this specialized type parameter recursively
				SpecializedTypeParameter o = other as SpecializedTypeParameter;
				return o != null && baseTp.Equals(o.baseTp) && this.Owner.Equals(o.Owner);
			}
			
			public override bool HasValueTypeConstraint => baseTp.HasValueTypeConstraint;
			public override bool HasReferenceTypeConstraint => baseTp.HasReferenceTypeConstraint;
			public override bool HasDefaultConstructorConstraint => baseTp.HasDefaultConstructorConstraint;
			public override bool HasUnmanagedConstraint => baseTp.HasUnmanagedConstraint;

			public override Nullability NullabilityConstraint => baseTp.NullabilityConstraint;

			IReadOnlyList<TypeConstraint> typeConstraints;

			public override IReadOnlyList<TypeConstraint> TypeConstraints {
				get {
					var typeConstraints = LazyInit.VolatileRead(ref this.typeConstraints);
					if (typeConstraints == null) {
						typeConstraints = baseTp.TypeConstraints.SelectReadOnlyArray(c => new TypeConstraint(c.Type.AcceptVisitor(substitution), c.Attributes));
						typeConstraints = LazyInit.GetOrSet(ref this.typeConstraints, typeConstraints);
					}
					return typeConstraints;
				}
			}
		}
	}
}
