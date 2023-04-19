// Copyright (c) 2019 Daniel Grunwald
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

#nullable enable

using ICSharpCode.Decompiler.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.TypeSystem
{
	sealed class NormalizeTypeVisitor : TypeVisitor
	{
		/// <summary>
		/// NormalizeTypeVisitor that does not normalize type parameters,
		/// but performs type erasure (object->dynamic; tuple->underlying type).
		/// </summary>
		internal static readonly NormalizeTypeVisitor TypeErasure = new NormalizeTypeVisitor {
			ReplaceClassTypeParametersWithDummy = false,
			ReplaceMethodTypeParametersWithDummy = false,
			DynamicAndObject = true,
			IntPtrToNInt = true,
			TupleToUnderlyingType = true,
			RemoveModOpt = true,
			RemoveModReq = true,
			RemoveNullability = true,
		};

		internal static readonly NormalizeTypeVisitor IgnoreNullabilityAndTuples = new NormalizeTypeVisitor {
			ReplaceClassTypeParametersWithDummy = false,
			ReplaceMethodTypeParametersWithDummy = false,
			DynamicAndObject = false,
			IntPtrToNInt = false,
			TupleToUnderlyingType = true,
			RemoveModOpt = true,
			RemoveModReq = true,
			RemoveNullability = true,
		};

		public bool EquivalentTypes(IType a, IType b)
		{
			a = a.AcceptVisitor(this);
			b = b.AcceptVisitor(this);
			return a.Equals(b);
		}

		public bool RemoveModOpt = true;
		public bool RemoveModReq = true;
		public bool ReplaceClassTypeParametersWithDummy = true;
		public bool ReplaceMethodTypeParametersWithDummy = true;
		public bool DynamicAndObject = true;
		public bool IntPtrToNInt = true;
		public bool TupleToUnderlyingType = true;
		public bool RemoveNullability = true;

		public override IType VisitTypeParameter(ITypeParameter type)
		{
			if (type.OwnerType == SymbolKind.Method && ReplaceMethodTypeParametersWithDummy)
			{
				return DummyTypeParameter.GetMethodTypeParameter(type.Index);
			}
			else if (type.OwnerType == SymbolKind.TypeDefinition && ReplaceClassTypeParametersWithDummy)
			{
				return DummyTypeParameter.GetClassTypeParameter(type.Index);
			}
			else if (RemoveNullability && type is NullabilityAnnotatedTypeParameter natp)
			{
				return natp.TypeWithoutAnnotation.AcceptVisitor(this);
			}
			else
			{
				return base.VisitTypeParameter(type);
			}
		}

		public override IType VisitTypeDefinition(ITypeDefinition type)
		{
			switch (type.KnownTypeCode)
			{
				case KnownTypeCode.Object when DynamicAndObject:
					// Instead of normalizing dynamic->object,
					// we do this the opposite direction, so that we don't need a compilation to find the object type.
					if (RemoveNullability)
						return SpecialType.Dynamic;
					else
						return SpecialType.Dynamic.ChangeNullability(type.Nullability);
				case KnownTypeCode.IntPtr when IntPtrToNInt:
					return SpecialType.NInt;
				case KnownTypeCode.UIntPtr when IntPtrToNInt:
					return SpecialType.NUInt;
			}
			return base.VisitTypeDefinition(type);
		}

		public override IType VisitTupleType(TupleType type)
		{
			if (TupleToUnderlyingType)
			{
				return type.UnderlyingType.AcceptVisitor(this);
			}
			else
			{
				return base.VisitTupleType(type);
			}
		}

		public override IType VisitNullabilityAnnotatedType(NullabilityAnnotatedType type)
		{
			if (RemoveNullability)
				return type.TypeWithoutAnnotation.AcceptVisitor(this);
			else
				return base.VisitNullabilityAnnotatedType(type);
		}

		public override IType VisitArrayType(ArrayType type)
		{
			if (RemoveNullability)
				return base.VisitArrayType(type).ChangeNullability(Nullability.Oblivious);
			else
				return base.VisitArrayType(type);
		}

		public override IType VisitModOpt(ModifiedType type)
		{
			if (RemoveModOpt)
			{
				return type.ElementType.AcceptVisitor(this);
			}
			else
			{
				return base.VisitModOpt(type);
			}
		}

		public override IType VisitModReq(ModifiedType type)
		{
			if (RemoveModReq)
			{
				return type.ElementType.AcceptVisitor(this);
			}
			else
			{
				return base.VisitModReq(type);
			}
		}
	}
}
