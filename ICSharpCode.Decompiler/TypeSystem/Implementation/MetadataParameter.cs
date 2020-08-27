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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	sealed class MetadataParameter : IParameter
	{
		readonly MetadataModule module;
		readonly ParameterHandle handle;
		readonly ParameterAttributes attributes;

		public IType Type { get; }
		public IParameterizedMember Owner { get; }

		// lazy-loaded:
		string name;
		// these can't be bool? as bool? is not thread-safe from torn reads
		byte constantValueInSignatureState;
		byte decimalConstantState;

		internal MetadataParameter(MetadataModule module, IParameterizedMember owner, IType type, ParameterHandle handle)
		{
			this.module = module;
			this.Owner = owner;
			this.Type = type;
			this.handle = handle;

			var param = module.metadata.GetParameter(handle);
			this.attributes = param.Attributes;
			if (!IsOptional)
				decimalConstantState = ThreeState.False; // only optional parameters can be constants
		}

		public EntityHandle MetadataToken => handle;

		#region Attributes
		public IEnumerable<IAttribute> GetAttributes()
		{
			var b = new AttributeListBuilder(module);
			var metadata = module.metadata;
			var parameter = metadata.GetParameter(handle);

			if (IsOptional && !HasConstantValueInSignature)
				b.Add(KnownAttribute.Optional);

			if (!IsOut && !IsIn)
			{
				if ((attributes & ParameterAttributes.In) == ParameterAttributes.In)
					b.Add(KnownAttribute.In);
				if ((attributes & ParameterAttributes.Out) == ParameterAttributes.Out)
					b.Add(KnownAttribute.Out);
			}
			b.Add(parameter.GetCustomAttributes(), SymbolKind.Parameter);
			b.AddMarshalInfo(parameter.GetMarshallingDescriptor());

			return b.Build();
		}
		#endregion

		const ParameterAttributes inOut = ParameterAttributes.In | ParameterAttributes.Out;

		public ReferenceKind ReferenceKind => DetectRefKind();
		public bool IsRef => DetectRefKind() == ReferenceKind.Ref;
		public bool IsOut => Type.Kind == TypeKind.ByReference && (attributes & inOut) == ParameterAttributes.Out;
		public bool IsIn => DetectRefKind() == ReferenceKind.In;

		public bool IsOptional => (attributes & ParameterAttributes.Optional) != 0;

		ReferenceKind DetectRefKind()
		{
			if (Type.Kind != TypeKind.ByReference)
				return ReferenceKind.None;
			if ((attributes & inOut) == ParameterAttributes.Out)
				return ReferenceKind.Out;
			if ((module.TypeSystemOptions & TypeSystemOptions.ReadOnlyStructsAndParameters) != 0)
			{
				var metadata = module.metadata;
				var parameterDef = metadata.GetParameter(handle);
				if (parameterDef.GetCustomAttributes().HasKnownAttribute(metadata, KnownAttribute.IsReadOnly))
					return ReferenceKind.In;
			}
			return ReferenceKind.Ref;
		}

		public bool IsParams {
			get {
				if (Type.Kind != TypeKind.Array)
					return false;
				var metadata = module.metadata;
				var parameterDef = metadata.GetParameter(handle);
				return parameterDef.GetCustomAttributes().HasKnownAttribute(metadata, KnownAttribute.ParamArray);
			}
		}

		public string Name {
			get {
				string name = LazyInit.VolatileRead(ref this.name);
				if (name != null)
					return name;
				var metadata = module.metadata;
				var parameterDef = metadata.GetParameter(handle);
				return LazyInit.GetOrSet(ref this.name, metadata.GetString(parameterDef.Name));
			}
		}

		bool IVariable.IsConst => false;

		public object GetConstantValue(bool throwOnInvalidMetadata)
		{
			try
			{
				var metadata = module.metadata;
				var parameterDef = metadata.GetParameter(handle);
				if (IsDecimalConstant)
					return DecimalConstantHelper.GetDecimalConstantValue(module, parameterDef.GetCustomAttributes());

				var constantHandle = parameterDef.GetDefaultValue();
				if (constantHandle.IsNil)
					return null;

				var constant = metadata.GetConstant(constantHandle);
				var blobReader = metadata.GetBlobReader(constant.Value);
				try
				{
					return blobReader.ReadConstant(constant.TypeCode);
				}
				catch (ArgumentOutOfRangeException)
				{
					throw new BadImageFormatException($"Constant with invalid typecode: {constant.TypeCode}");
				}
			}
			catch (BadImageFormatException) when (!throwOnInvalidMetadata)
			{
				return null;
			}
		}

		public bool HasConstantValueInSignature {
			get {
				if (constantValueInSignatureState == ThreeState.Unknown)
				{
					if (IsDecimalConstant)
					{
						constantValueInSignatureState = ThreeState.From(DecimalConstantHelper.AllowsDecimalConstants(module));
					}
					else
					{
						constantValueInSignatureState = ThreeState.From(!module.metadata.GetParameter(handle).GetDefaultValue().IsNil);
					}
				}
				return constantValueInSignatureState == ThreeState.True;
			}
		}

		bool IsDecimalConstant {
			get {
				if (decimalConstantState == ThreeState.Unknown)
				{
					var parameterDef = module.metadata.GetParameter(handle);
					decimalConstantState = ThreeState.From(DecimalConstantHelper.IsDecimalConstant(module, parameterDef.GetCustomAttributes()));
				}
				return decimalConstantState == ThreeState.True;
			}
		}

		SymbolKind ISymbol.SymbolKind => SymbolKind.Parameter;

		public override string ToString()
		{
			return $"{MetadataTokens.GetToken(handle):X8} {DefaultParameter.ToString(this)}";
		}
	}
}
