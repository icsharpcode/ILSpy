using System;
using System.Collections.Generic;
using System.Text;
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
			TupleToUnderlyingType = true,
			RemoveModOpt = true,
			RemoveModReq = true,
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
		public bool TupleToUnderlyingType = true;

		public override IType VisitTypeParameter(ITypeParameter type)
		{
			if (type.OwnerType == SymbolKind.Method && ReplaceMethodTypeParametersWithDummy) {
				return DummyTypeParameter.GetMethodTypeParameter(type.Index);
			} else if (type.OwnerType == SymbolKind.TypeDefinition && ReplaceClassTypeParametersWithDummy) {
				return DummyTypeParameter.GetClassTypeParameter(type.Index);
			} else {
				return base.VisitTypeParameter(type);
			}
		}

		public override IType VisitTypeDefinition(ITypeDefinition type)
		{
			if (DynamicAndObject && type.KnownTypeCode == KnownTypeCode.Object) {
				// Instead of normalizing dynamic->object,
				// we do this the opposite direction, so that we don't need a compilation to find the object type.
				return SpecialType.Dynamic;
			}
			return base.VisitTypeDefinition(type);
		}

		public override IType VisitTupleType(TupleType type)
		{
			if (TupleToUnderlyingType) {
				return type.UnderlyingType.AcceptVisitor(this);
			} else {
				return base.VisitTupleType(type);
			}
		}

		public override IType VisitModOpt(ModifiedType type)
		{
			if (RemoveModOpt) {
				return type.ElementType.AcceptVisitor(this);
			} else {
				return base.VisitModOpt(type);
			}
		}

		public override IType VisitModReq(ModifiedType type)
		{
			if (RemoveModReq) {
				return type.ElementType.AcceptVisitor(this);
			} else {
				return base.VisitModReq(type);
			}
		}
	}
}
