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
		readonly MetadataAssembly assembly;
		readonly GenericParameterHandle handle;

		readonly GenericParameterAttributes attr;

		// lazy-loaded:
		IReadOnlyList<IAttribute> customAttributes;
		IReadOnlyList<IType> constraints;

		public static ITypeParameter[] Create(MetadataAssembly assembly, IEntity owner, GenericParameterHandleCollection handles)
		{
			if (handles.Count == 0)
				return Empty<ITypeParameter>.Array;
			var tps = new ITypeParameter[handles.Count];
			int i = 0;
			foreach (var handle in handles) {
				tps[i] = Create(assembly, owner, i, handle);
				i++;
			}
			return tps;
		}

		public static MetadataTypeParameter Create(MetadataAssembly assembly, IEntity owner, int index, GenericParameterHandle handle)
		{
			var metadata = assembly.metadata;
			var gp = metadata.GetGenericParameter(handle);
			Debug.Assert(gp.Index == index);
			return new MetadataTypeParameter(assembly, owner, index, assembly.GetString(gp.Name), handle, gp.Attributes);
		}

		private MetadataTypeParameter(MetadataAssembly assembly, IEntity owner, int index, string name,
			GenericParameterHandle handle, GenericParameterAttributes attr)
			: base(owner, index, name, GetVariance(attr))
		{
			this.assembly = assembly;
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

		public override IReadOnlyList<IAttribute> Attributes {
			get {
				var attr = LazyInit.VolatileRead(ref this.customAttributes);
				if (attr != null)
					return attr;
				return LazyInit.GetOrSet(ref this.customAttributes, DecodeAttributes());
			}
		}

		IAttribute[] DecodeAttributes()
		{
			var metadata = assembly.metadata;
			var gp = metadata.GetGenericParameter(handle);

			var attributes = gp.GetCustomAttributes();
			var b = new AttributeListBuilder(assembly, attributes.Count);
			b.Add(attributes);
			return b.Build();
		}

		public override bool HasDefaultConstructorConstraint => (attr & GenericParameterAttributes.DefaultConstructorConstraint) != 0;
		public override bool HasReferenceTypeConstraint => (attr & GenericParameterAttributes.ReferenceTypeConstraint) != 0;
		public override bool HasValueTypeConstraint => (attr & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0;

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
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return $"{MetadataTokens.GetToken(handle):X8} Index={Index} Owner={Owner}";
		}
	}
}