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
			bool typeChildrenOnly = false)
		{
			bool useDynamicType = (options & TypeSystemOptions.Dynamic) != 0;
			bool useTupleTypes = (options & TypeSystemOptions.Tuple) != 0;
			bool hasDynamicAttribute = false;
			bool[] dynamicAttributeData = null;
			string[] tupleElementNames = null;
			if (attributes != null && (useDynamicType || useTupleTypes)) {
				foreach (var attrHandle in attributes.Value) {
					var attr = metadata.GetCustomAttribute(attrHandle);
					var attrType = attr.GetAttributeType(metadata);
					if (useDynamicType && attrType.IsKnownType(metadata, KnownAttribute.Dynamic)) {
						hasDynamicAttribute = true;
						var ctor = attr.DecodeValue(Metadata.MetadataExtensions.minimalCorlibTypeProvider);
						if (ctor.FixedArguments.Length == 1) {
							var arg = ctor.FixedArguments[0];
							if (arg.Value is ImmutableArray<SRM.CustomAttributeTypedArgument<IType>> values
								&& values.All(v => v.Value is bool)) {
								dynamicAttributeData = values.SelectArray(v => (bool)v.Value);
							}
						}
					} else if (useTupleTypes && attrType.IsKnownType(metadata, KnownAttribute.TupleElementNames)) {
						var ctor = attr.DecodeValue(Metadata.MetadataExtensions.minimalCorlibTypeProvider);
						if (ctor.FixedArguments.Length == 1) {
							var arg = ctor.FixedArguments[0];
							if (arg.Value is ImmutableArray<SRM.CustomAttributeTypedArgument<IType>> values
								&& values.All(v => v.Value is string || v.Value == null)) {
								tupleElementNames = values.SelectArray(v => (string)v.Value);
							}
						}
					}
				}
			}
			if (hasDynamicAttribute || (options & (TypeSystemOptions.Tuple | TypeSystemOptions.KeepModifiers)) != TypeSystemOptions.KeepModifiers) {
				var visitor = new ApplyAttributeTypeVisitor(
					compilation, hasDynamicAttribute, dynamicAttributeData, options, tupleElementNames
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
		readonly TypeSystemOptions options;
		readonly string[] tupleElementNames;
		int dynamicTypeIndex = 0;
		int tupleTypeIndex = 0;

		private ApplyAttributeTypeVisitor(ICompilation compilation, bool hasDynamicAttribute, bool[] dynamicAttributeData, TypeSystemOptions options, string[] tupleElementNames)
		{
			this.compilation = compilation ?? throw new ArgumentNullException(nameof(compilation));
			this.hasDynamicAttribute = hasDynamicAttribute;
			this.dynamicAttributeData = dynamicAttributeData;
			this.options = options;
			this.tupleElementNames = tupleElementNames;
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
					var elementTypes = ImmutableArray.CreateBuilder<IType>(tupleCardinality);
					do {
						int normalArgCount = Math.Min(type.TypeArguments.Count, TupleType.RestPosition - 1);
						for (int i = 0; i < normalArgCount; i++) {
							dynamicTypeIndex++;
							elementTypes.Add(type.TypeArguments[i].AcceptVisitor(this));
						}
						if (type.TypeArguments.Count == TupleType.RestPosition) {
							type = type.TypeArguments.Last() as ParameterizedType;
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
			if (type.KnownTypeCode == KnownTypeCode.Object && hasDynamicAttribute) {
				if (dynamicAttributeData == null || dynamicTypeIndex >= dynamicAttributeData.Length)
					return SpecialType.Dynamic;
				if (dynamicAttributeData[dynamicTypeIndex])
					return SpecialType.Dynamic;
				return type;
			}
			return type;
		}
	}
}
