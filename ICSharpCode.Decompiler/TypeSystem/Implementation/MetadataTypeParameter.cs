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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	sealed class MetadataTypeParameter : AbstractTypeParameter
	{
		readonly MetadataModule module;
		readonly GenericParameterHandle handle;

		readonly GenericParameterAttributes attr;

		// lazy-loaded:
		IReadOnlyList<IType> constraints;
		byte unmanagedConstraint = ThreeState.Unknown;
		const byte nullabilityNotYetLoaded = 255;
		byte nullabilityConstraint = nullabilityNotYetLoaded;

		public static ITypeParameter[] Create(MetadataModule module, ITypeDefinition copyFromOuter, IEntity owner, GenericParameterHandleCollection handles)
		{
			if (handles.Count == 0)
				return Empty<ITypeParameter>.Array;
			var outerTps = copyFromOuter.TypeParameters;
			var tps = new ITypeParameter[handles.Count];
			int i = 0;
			foreach (var handle in handles) {
				if (i < outerTps.Count)
					tps[i] = outerTps[i];
				else
					tps[i] = Create(module, owner, i, handle);
				i++;
			}
			return tps;
		}

		public static ITypeParameter[] Create(MetadataModule module, IEntity owner, GenericParameterHandleCollection handles)
		{
			if (handles.Count == 0)
				return Empty<ITypeParameter>.Array;
			var tps = new ITypeParameter[handles.Count];
			int i = 0;
			foreach (var handle in handles) {
				tps[i] = Create(module, owner, i, handle);
				i++;
			}
			return tps;
		}

		public static MetadataTypeParameter Create(MetadataModule module, IEntity owner, int index, GenericParameterHandle handle)
		{
			var metadata = module.metadata;
			var gp = metadata.GetGenericParameter(handle);
			Debug.Assert(gp.Index == index);
			return new MetadataTypeParameter(module, owner, index, module.GetString(gp.Name), handle, gp.Attributes);
		}

		private MetadataTypeParameter(MetadataModule module, IEntity owner, int index, string name,
			GenericParameterHandle handle, GenericParameterAttributes attr)
			: base(owner, index, name, GetVariance(attr))
		{
			this.module = module;
			this.handle = handle;
			this.attr = attr;
		}

		private static VarianceModifier GetVariance(GenericParameterAttributes attr)
		{
			switch (attr & GenericParameterAttributes.VarianceMask) {
				case GenericParameterAttributes.Contravariant:
					return VarianceModifier.Contravariant;
				case GenericParameterAttributes.Covariant:
					return VarianceModifier.Covariant;
				default:
					return VarianceModifier.Invariant;
			}
		}

		public GenericParameterHandle MetadataToken => handle;

		public override IEnumerable<IAttribute> GetAttributes()
		{
			var metadata = module.metadata;
			var gp = metadata.GetGenericParameter(handle);

			var attributes = gp.GetCustomAttributes();
			var b = new AttributeListBuilder(module, attributes.Count);
			b.Add(attributes, SymbolKind.TypeParameter);
			return b.Build();
		}

		public override bool HasDefaultConstructorConstraint => (attr & GenericParameterAttributes.DefaultConstructorConstraint) != 0;
		public override bool HasReferenceTypeConstraint => (attr & GenericParameterAttributes.ReferenceTypeConstraint) != 0;
		public override bool HasValueTypeConstraint => (attr & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0;

		public override bool HasUnmanagedConstraint {
			get {
				if (unmanagedConstraint == ThreeState.Unknown) {
					unmanagedConstraint = ThreeState.From(LoadUnmanagedConstraint());
				}
				return unmanagedConstraint == ThreeState.True;
			}
		}

		private bool LoadUnmanagedConstraint()
		{
			if ((module.TypeSystemOptions & TypeSystemOptions.UnmanagedConstraints) == 0)
				return false;
			var metadata = module.metadata;
			var gp = metadata.GetGenericParameter(handle);
			return gp.GetCustomAttributes().HasKnownAttribute(metadata, KnownAttribute.IsUnmanaged);
		}

		public override Nullability NullabilityConstraint {
			get {
				if (nullabilityConstraint == nullabilityNotYetLoaded) {
					nullabilityConstraint = (byte)LoadNullabilityConstraint();
				}
				return (Nullability)nullabilityConstraint;
			}
		}

		Nullability LoadNullabilityConstraint()
		{
			if ((module.TypeSystemOptions & TypeSystemOptions.NullabilityAnnotations) == 0)
				return Nullability.Oblivious;

			var metadata = module.metadata;
			var gp = metadata.GetGenericParameter(handle);

			foreach (var handle in gp.GetCustomAttributes()) {
				var customAttribute = metadata.GetCustomAttribute(handle);
				if (customAttribute.IsKnownAttribute(metadata, KnownAttribute.Nullable)) {
					var attrVal = customAttribute.DecodeValue(module.TypeProvider);
					if (attrVal.FixedArguments.Length == 1) {
						if (attrVal.FixedArguments[0].Value is byte b && b <= 2) {
							return (Nullability)b;
						}
					}
				}
			}
			return Nullability.Oblivious;
		}

		public override IEnumerable<IType> DirectBaseTypes {
			get {
				var constraints = LazyInit.VolatileRead(ref this.constraints);
				if (constraints != null)
					return constraints;
				return LazyInit.GetOrSet(ref this.constraints, DecodeConstraints());
			}
		}

		private IReadOnlyList<IType> DecodeConstraints()
		{
			var metadata = module.metadata;
			var gp = metadata.GetGenericParameter(handle);
			Nullability nullableContext;
			if (Owner is ITypeDefinition typeDef) {
				nullableContext = typeDef.NullableContext;
			} else if (Owner is MetadataMethod method) {
				nullableContext = method.NullableContext;
			} else {
				nullableContext = Nullability.Oblivious;
			}

			var constraintHandleCollection = gp.GetConstraints();
			List<IType> result = new List<IType>(constraintHandleCollection.Count + 1);
			bool hasNonInterfaceConstraint = false;
			foreach (var constraintHandle in constraintHandleCollection) {
				var constraint = metadata.GetGenericParameterConstraint(constraintHandle);
				var ty = module.ResolveType(constraint.Type, new GenericContext(Owner), constraint.GetCustomAttributes(), nullableContext);
				result.Add(ty);
				hasNonInterfaceConstraint |= (ty.Kind != TypeKind.Interface);
			}
			if (this.HasValueTypeConstraint) {
				result.Add(Compilation.FindType(KnownTypeCode.ValueType));
			} else if (!hasNonInterfaceConstraint) {
				result.Add(Compilation.FindType(KnownTypeCode.Object));
			}
			return result;
		}

		public override int GetHashCode()
		{
			return 0x51fc5b83 ^ module.PEFile.GetHashCode() ^ handle.GetHashCode();
		}

		public override bool Equals(IType other)
		{
			return other is MetadataTypeParameter tp && handle == tp.handle && module.PEFile == tp.module.PEFile;
		}

		public override string ToString()
		{
			return $"{MetadataTokens.GetToken(handle):X8} Index={Index} Owner={Owner}";
		}
	}
}