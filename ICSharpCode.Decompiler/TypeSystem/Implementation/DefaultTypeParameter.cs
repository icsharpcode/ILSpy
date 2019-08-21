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
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	public class DefaultTypeParameter : AbstractTypeParameter
	{
		readonly bool hasValueTypeConstraint;
		readonly bool hasReferenceTypeConstraint;
		readonly bool hasDefaultConstructorConstraint;
		readonly Nullability nullabilityConstraint;
		readonly IReadOnlyList<IType> constraints;
		readonly IReadOnlyList<IAttribute> attributes;

		public DefaultTypeParameter(
			IEntity owner,
			int index, string name = null,
			VarianceModifier variance = VarianceModifier.Invariant,
			IReadOnlyList<IAttribute> attributes = null,
			bool hasValueTypeConstraint = false, bool hasReferenceTypeConstraint = false, bool hasDefaultConstructorConstraint = false,
			IReadOnlyList<IType> constraints = null, Nullability nullabilityConstraint = Nullability.Oblivious)
			: base(owner, index, name, variance)
		{
			this.hasValueTypeConstraint = hasValueTypeConstraint;
			this.hasReferenceTypeConstraint = hasReferenceTypeConstraint;
			this.hasDefaultConstructorConstraint = hasDefaultConstructorConstraint;
			this.nullabilityConstraint = nullabilityConstraint;
			this.TypeConstraints = MakeConstraints(constraints);
			this.attributes = attributes ?? EmptyList<IAttribute>.Instance;
		}

		public DefaultTypeParameter(
			ICompilation compilation, SymbolKind ownerType,
			int index, string name = null,
			VarianceModifier variance = VarianceModifier.Invariant,
			IReadOnlyList<IAttribute> attributes = null,
			bool hasValueTypeConstraint = false, bool hasReferenceTypeConstraint = false, bool hasDefaultConstructorConstraint = false,
			IReadOnlyList<IType> constraints = null, Nullability nullabilityConstraint = Nullability.Oblivious)
			: base(compilation, ownerType, index, name, variance)
		{
			this.hasValueTypeConstraint = hasValueTypeConstraint;
			this.hasReferenceTypeConstraint = hasReferenceTypeConstraint;
			this.hasDefaultConstructorConstraint = hasDefaultConstructorConstraint;
			this.nullabilityConstraint = nullabilityConstraint;
			this.TypeConstraints = MakeConstraints(constraints);
			this.attributes = attributes ?? EmptyList<IAttribute>.Instance;
		}

		public override IEnumerable<IAttribute> GetAttributes() => attributes;

		public override bool HasValueTypeConstraint => hasValueTypeConstraint;
		public override bool HasReferenceTypeConstraint => hasReferenceTypeConstraint;
		public override bool HasDefaultConstructorConstraint => hasDefaultConstructorConstraint;
		public override bool HasUnmanagedConstraint => false;
		public override Nullability NullabilityConstraint => nullabilityConstraint;

		public override IReadOnlyList<TypeConstraint> TypeConstraints { get; }

		IReadOnlyList<TypeConstraint> MakeConstraints(IReadOnlyList<IType> constraints)
		{
			var result = new List<TypeConstraint>();
			bool hasNonInterfaceConstraint = false;
			if (constraints != null) {
				foreach (IType c in constraints) {
					result.Add(new TypeConstraint(c));
					if (c.Kind != TypeKind.Interface)
						hasNonInterfaceConstraint = true;
				}
			}
			// Do not add the 'System.Object' constraint if there is another constraint with a base class.
			if (this.HasValueTypeConstraint || !hasNonInterfaceConstraint) {
				result.Add(new TypeConstraint(this.Compilation.FindType(this.HasValueTypeConstraint ? KnownTypeCode.ValueType : KnownTypeCode.Object)));
			}
			return result;
		}
	}
}
