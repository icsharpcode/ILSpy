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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;
using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Introduces 'dynamic' and tuple types based on attribute values.
	/// </summary>
	sealed class ApplyAttributeTypeVisitor : TypeVisitor
	{
		public static IType ApplyAttributesToType(
			IType inputType,
			ICompilation compilation,
			SRM.CustomAttributeHandleCollection? attributes,
			SRM.MetadataReader metadata,
			TypeSystemOptions options,
			Nullability nullableContext,
			bool typeChildrenOnly = false)
		{
			bool hasDynamicAttribute = false;
			bool[] dynamicAttributeData = null;
			bool hasNativeIntegersAttribute = false;
			bool[] nativeIntegersAttributeData = null;
			string[] tupleElementNames = null;
			Nullability nullability;
			Nullability[] nullableAttributeData = null;
			if ((options & TypeSystemOptions.NullabilityAnnotations) != 0) {
				nullability = nullableContext;
			} else {
				nullability = Nullability.Oblivious;
			}
			const TypeSystemOptions relevantOptions = TypeSystemOptions.Dynamic | TypeSystemOptions.Tuple | TypeSystemOptions.NullabilityAnnotations | TypeSystemOptions.NativeIntegers;
			if (attributes != null && (options & relevantOptions) != 0) {
				foreach (var attrHandle in attributes.Value) {
					var attr = metadata.GetCustomAttribute(attrHandle);
					var attrType = attr.GetAttributeType(metadata);
					if ((options & TypeSystemOptions.Dynamic) != 0 && attrType.IsKnownType(metadata, KnownAttribute.Dynamic)) {
						hasDynamicAttribute = true;
						var ctor = attr.DecodeValue(Metadata.MetadataExtensions.minimalCorlibTypeProvider);
						if (ctor.FixedArguments.Length == 1) {
							var arg = ctor.FixedArguments[0];
							if (arg.Value is ImmutableArray<SRM.CustomAttributeTypedArgument<IType>> values
								&& values.All(v => v.Value is bool)) {
								dynamicAttributeData = values.SelectArray(v => (bool)v.Value);
							}
						}
					} else if ((options & TypeSystemOptions.NativeIntegers) != 0 && attrType.IsKnownType(metadata, KnownAttribute.NativeInteger)) {
						hasNativeIntegersAttribute = true;
						var ctor = attr.DecodeValue(Metadata.MetadataExtensions.minimalCorlibTypeProvider);
						if (ctor.FixedArguments.Length == 1) {
							var arg = ctor.FixedArguments[0];
							if (arg.Value is ImmutableArray<SRM.CustomAttributeTypedArgument<IType>> values
								&& values.All(v => v.Value is bool)) {
								nativeIntegersAttributeData = values.SelectArray(v => (bool)v.Value);
							}
						}
					} else if ((options & TypeSystemOptions.Tuple) != 0 && attrType.IsKnownType(metadata, KnownAttribute.TupleElementNames)) {
						var ctor = attr.DecodeValue(Metadata.MetadataExtensions.minimalCorlibTypeProvider);
						if (ctor.FixedArguments.Length == 1) {
							var arg = ctor.FixedArguments[0];
							if (arg.Value is ImmutableArray<SRM.CustomAttributeTypedArgument<IType>> values
								&& values.All(v => v.Value is string || v.Value == null)) {
								tupleElementNames = values.SelectArray(v => (string)v.Value);
							}
						}
					} else if ((options & TypeSystemOptions.NullabilityAnnotations) != 0 && attrType.IsKnownType(metadata, KnownAttribute.Nullable)) {
						var ctor = attr.DecodeValue(Metadata.MetadataExtensions.minimalCorlibTypeProvider);
						if (ctor.FixedArguments.Length == 1) {
							var arg = ctor.FixedArguments[0];
							if (arg.Value is ImmutableArray<SRM.CustomAttributeTypedArgument<IType>> values
								&& values.All(v => v.Value is byte b && b <= 2)) {
								nullableAttributeData = values.SelectArray(v => (Nullability)(byte)v.Value);
							} else if (arg.Value is byte b && b <= 2) {
								nullability = (Nullability)b;
							}
						}
					}
				}
			}
			if (hasDynamicAttribute || hasNativeIntegersAttribute || nullability != Nullability.Oblivious || nullableAttributeData != null 
				|| (options & (TypeSystemOptions.Tuple | TypeSystemOptions.KeepModifiers)) != TypeSystemOptions.KeepModifiers)
			{
				var visitor = new ApplyAttributeTypeVisitor(
					compilation, hasDynamicAttribute, dynamicAttributeData,
					hasNativeIntegersAttribute, nativeIntegersAttributeData,
					options, tupleElementNames,
					nullability, nullableAttributeData
				);
				if (typeChildrenOnly) {
					return inputType.VisitChildren(visitor);
				} else {
					return inputType.AcceptVisitor(visitor);
				}
			} else {
				return inputType;
			}
		}

		readonly ICompilation compilation;
		readonly bool hasDynamicAttribute;
		readonly bool[] dynamicAttributeData;
		readonly bool hasNativeIntegersAttribute;
		readonly bool[] nativeIntegersAttributeData;
		readonly TypeSystemOptions options;
		readonly string[] tupleElementNames;
		readonly Nullability defaultNullability;
		readonly Nullability[] nullableAttributeData;
		int dynamicTypeIndex = 0;
		int tupleTypeIndex = 0;
		int nullabilityTypeIndex = 0;

		private ApplyAttributeTypeVisitor(ICompilation compilation, 
			bool hasDynamicAttribute, bool[] dynamicAttributeData,
			bool hasNativeIntegersAttribute, bool[] nativeIntegersAttributeData,
			TypeSystemOptions options, string[] tupleElementNames,
			Nullability defaultNullability, Nullability[] nullableAttributeData)
		{
			this.compilation = compilation ?? throw new ArgumentNullException(nameof(compilation));
			this.hasDynamicAttribute = hasDynamicAttribute;
			this.dynamicAttributeData = dynamicAttributeData;
			this.hasNativeIntegersAttribute = hasNativeIntegersAttribute;
			this.nativeIntegersAttributeData = nativeIntegersAttributeData;
			this.options = options;
			this.tupleElementNames = tupleElementNames;
			this.defaultNullability = defaultNullability;
			this.nullableAttributeData = nullableAttributeData;
		}

		public override IType VisitModOpt(ModifiedType type)
		{
			if ((options & TypeSystemOptions.KeepModifiers) != 0)
				return base.VisitModOpt(type);
			else
				return type.ElementType.AcceptVisitor(this);
		}

		public override IType VisitModReq(ModifiedType type)
		{
			if ((options & TypeSystemOptions.KeepModifiers) != 0)
				return base.VisitModReq(type);
			else
				return type.ElementType.AcceptVisitor(this);
		}

		public override IType VisitPointerType(PointerType type)
		{
			dynamicTypeIndex++;
			return base.VisitPointerType(type);
		}

		Nullability GetNullability()
		{
			if (nullabilityTypeIndex < nullableAttributeData?.Length)
				return nullableAttributeData[nullabilityTypeIndex++];
			else
				return defaultNullability;
		}

		void ExpectDummyNullabilityForGenericValueType()
		{
			var n = GetNullability();
			Debug.Assert(n == Nullability.Oblivious);
		}

		public override IType VisitArrayType(ArrayType type)
		{
			var nullability = GetNullability();
			dynamicTypeIndex++;
			return base.VisitArrayType(type).ChangeNullability(nullability);
		}

		public override IType VisitByReferenceType(ByReferenceType type)
		{
			dynamicTypeIndex++;
			return base.VisitByReferenceType(type);
		}

		public override IType VisitParameterizedType(ParameterizedType type)
		{
			bool useTupleTypes = (options & TypeSystemOptions.Tuple) != 0;
			if (useTupleTypes && TupleType.IsTupleCompatible(type, out int tupleCardinality)) {
				if (tupleCardinality > 1) {
					var valueTupleAssembly = type.GetDefinition()?.ParentModule;
					ImmutableArray<string> elementNames = default;
					if (tupleElementNames != null && tupleTypeIndex < tupleElementNames.Length) {
						string[] extractedValues = new string[tupleCardinality];
						Array.Copy(tupleElementNames, tupleTypeIndex, extractedValues, 0,
							Math.Min(tupleCardinality, tupleElementNames.Length - tupleTypeIndex));
						elementNames = ImmutableArray.CreateRange(extractedValues);
					}
					tupleTypeIndex += tupleCardinality;
					ExpectDummyNullabilityForGenericValueType();
					var elementTypes = ImmutableArray.CreateBuilder<IType>(tupleCardinality);
					do {
						int normalArgCount = Math.Min(type.TypeArguments.Count, TupleType.RestPosition - 1);
						for (int i = 0; i < normalArgCount; i++) {
							dynamicTypeIndex++;
							elementTypes.Add(type.TypeArguments[i].AcceptVisitor(this));
						}
						if (type.TypeArguments.Count == TupleType.RestPosition) {
							type = type.TypeArguments.Last() as ParameterizedType;
							ExpectDummyNullabilityForGenericValueType();
							dynamicTypeIndex++;
							if (type != null && TupleType.IsTupleCompatible(type, out int nestedCardinality)) {
								tupleTypeIndex += nestedCardinality;
							} else {
								Debug.Fail("TRest should be another value tuple");
								type = null;
							}
						} else {
							type = null;
						}
					} while (type != null);
					Debug.Assert(elementTypes.Count == tupleCardinality);
					return new TupleType(
						compilation,
						elementTypes.MoveToImmutable(),
						elementNames,
						valueTupleAssembly
					);
				} else {
					// C# doesn't have syntax for tuples of cardinality <= 1
					tupleTypeIndex += tupleCardinality;
				}
			}
			// Visit generic type and type arguments.
			// Like base implementation, except that it increments dynamicTypeIndex.
			var genericType = type.GenericType.AcceptVisitor(this);
			if (genericType.IsReferenceType != true && !genericType.IsKnownType(KnownTypeCode.NullableOfT)) {
				ExpectDummyNullabilityForGenericValueType();
			}
			bool changed = type.GenericType != genericType;
			var arguments = new IType[type.TypeArguments.Count];
			for (int i = 0; i < type.TypeArguments.Count; i++) {
				dynamicTypeIndex++;
				arguments[i] = type.TypeArguments[i].AcceptVisitor(this);
				changed = changed || arguments[i] != type.TypeArguments[i];
			}
			if (!changed)
				return type;
			return new ParameterizedType(genericType, arguments);
		}

		public override IType VisitTypeDefinition(ITypeDefinition type)
		{
			IType newType = type;
			var ktc = type.KnownTypeCode;
			if (ktc == KnownTypeCode.Object && hasDynamicAttribute) {
				if (dynamicAttributeData == null || dynamicTypeIndex >= dynamicAttributeData.Length)
					newType = SpecialType.Dynamic;
				else if (dynamicAttributeData[dynamicTypeIndex])
					newType = SpecialType.Dynamic;
			} else if ((ktc == KnownTypeCode.IntPtr || ktc == KnownTypeCode.UIntPtr) && hasNativeIntegersAttribute) {
				// native integers use the same indexing logic as 'dynamic'
				if (nativeIntegersAttributeData == null || dynamicTypeIndex > nativeIntegersAttributeData.Length)
					newType = (ktc == KnownTypeCode.IntPtr ? SpecialType.NInt : SpecialType.NUInt);
				else if (nativeIntegersAttributeData[dynamicTypeIndex])
					newType = (ktc == KnownTypeCode.IntPtr ? SpecialType.NInt : SpecialType.NUInt);
			}
			if (type.IsReferenceType == true) {
				Nullability nullability = GetNullability();
				return newType.ChangeNullability(nullability);
			} else {
				return newType;
			}
		}

		public override IType VisitOtherType(IType type)
		{
			type = base.VisitOtherType(type);
			if (type.Kind == TypeKind.Unknown && type.IsReferenceType == true) {
				Nullability nullability = GetNullability();
				type = type.ChangeNullability(nullability);
			}
			return type;
		}

		public override IType VisitTypeParameter(ITypeParameter type)
		{
			Nullability nullability = GetNullability();
			return type.ChangeNullability(nullability);
		}
	}
}
