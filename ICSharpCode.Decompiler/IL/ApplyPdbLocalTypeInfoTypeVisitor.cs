using System;
using System.Collections.Immutable;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Heavily based on <see cref="ApplyAttributeTypeVisitor"/>
	/// </summary>
	sealed class ApplyPdbLocalTypeInfoTypeVisitor : TypeVisitor
	{
		private readonly bool[] dynamicData;
		private readonly string[] tupleElementNames;
		private int dynamicTypeIndex = 0;
		private int tupleTypeIndex = 0;

		private ApplyPdbLocalTypeInfoTypeVisitor(bool[] dynamicData, string[] tupleElementNames)
		{
			this.dynamicData = dynamicData;
			this.tupleElementNames = tupleElementNames;
		}

		public static IType Apply(IType type, PdbExtraTypeInfo pdbExtraTypeInfo)
		{
			if (pdbExtraTypeInfo.DynamicFlags is null && pdbExtraTypeInfo.TupleElementNames is null)
				return type;
			return type.AcceptVisitor(new ApplyPdbLocalTypeInfoTypeVisitor(pdbExtraTypeInfo.DynamicFlags, pdbExtraTypeInfo.TupleElementNames));
		}

		public override IType VisitModOpt(ModifiedType type)
		{
			dynamicTypeIndex++;
			return base.VisitModOpt(type);
		}

		public override IType VisitModReq(ModifiedType type)
		{
			dynamicTypeIndex++;
			return base.VisitModReq(type);
		}

		public override IType VisitPointerType(PointerType type)
		{
			dynamicTypeIndex++;
			return base.VisitPointerType(type);
		}

		public override IType VisitArrayType(ArrayType type)
		{
			dynamicTypeIndex++;
			return base.VisitArrayType(type);
		}

		public override IType VisitByReferenceType(ByReferenceType type)
		{
			dynamicTypeIndex++;
			return base.VisitByReferenceType(type);
		}

		public override IType VisitTupleType(TupleType type)
		{
			if (tupleElementNames != null && tupleTypeIndex < tupleElementNames.Length)
			{
				int tupleCardinality = type.Cardinality;
				string[] extractedValues = new string[tupleCardinality];
				Array.Copy(tupleElementNames, tupleTypeIndex, extractedValues, 0,
					Math.Min(tupleCardinality, tupleElementNames.Length - tupleTypeIndex));
				var elementNames = ImmutableArray.CreateRange(extractedValues);
				tupleTypeIndex += tupleCardinality;

				int level = 0;
				var elementTypes = new IType[type.ElementTypes.Length];
				for (int i = 0; i < type.ElementTypes.Length; i++)
				{
					dynamicTypeIndex++;
					IType elementType = type.ElementTypes[i];
					if (i != 0 && (i - level) % TupleType.RestPosition == 0 && elementType is TupleType tuple)
					{
						tupleTypeIndex += tuple.Cardinality;
						level++;
					}
					elementTypes[i] = elementType.AcceptVisitor(this);
				}

				return new TupleType(
					type.Compilation,
					elementTypes.ToImmutableArray(),
					elementNames,
					type.GetDefinition()?.ParentModule
				);
			}
			return base.VisitTupleType(type);
		}

		public override IType VisitParameterizedType(ParameterizedType type)
		{
			if (TupleType.IsTupleCompatible(type, out var tupleCardinality))
				tupleTypeIndex += tupleCardinality;
			// Visit generic type and type arguments.
			// Like base implementation, except that it increments dynamicTypeIndex.
			var genericType = type.GenericType.AcceptVisitor(this);
			bool changed = type.GenericType != genericType;
			var arguments = new IType[type.TypeArguments.Count];
			for (int i = 0; i < type.TypeArguments.Count; i++)
			{
				dynamicTypeIndex++;
				arguments[i] = type.TypeArguments[i].AcceptVisitor(this);
				changed = changed || arguments[i] != type.TypeArguments[i];
			}
			if (!changed)
				return type;
			return new ParameterizedType(genericType, arguments);
		}

		public override IType VisitFunctionPointerType(FunctionPointerType type)
		{
			dynamicTypeIndex++;
			if (type.ReturnIsRefReadOnly)
			{
				dynamicTypeIndex++;
			}
			var returnType = type.ReturnType.AcceptVisitor(this);
			bool changed = type.ReturnType != returnType;
			var parameters = new IType[type.ParameterTypes.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				dynamicTypeIndex += type.ParameterReferenceKinds[i] switch {
					ReferenceKind.None => 1,
					ReferenceKind.Ref => 1,
					ReferenceKind.Out => 2, // in/out also count the modreq
					ReferenceKind.In => 2,
					_ => throw new NotSupportedException()
				};
				parameters[i] = type.ParameterTypes[i].AcceptVisitor(this);
				changed = changed || parameters[i] != type.ParameterTypes[i];
			}
			if (!changed)
				return type;
			return type.WithSignature(returnType, parameters.ToImmutableArray());
		}

		public override IType VisitTypeDefinition(ITypeDefinition type)
		{
			IType newType = type;
			var ktc = type.KnownTypeCode;
			if (ktc == KnownTypeCode.Object && dynamicData is not null)
			{
				if (dynamicTypeIndex >= dynamicData.Length)
					newType = SpecialType.Dynamic;
				else if (dynamicData[dynamicTypeIndex])
					newType = SpecialType.Dynamic;
			}
			return newType;
		}
	}
}
