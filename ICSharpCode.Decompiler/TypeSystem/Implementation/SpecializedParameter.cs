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

using System.Collections.Generic;
using System.Diagnostics;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	sealed class SpecializedParameter : IParameter
	{
		readonly IParameter baseParameter;
		readonly IType newType;
		readonly IParameterizedMember newOwner;

		public SpecializedParameter(IParameter baseParameter, IType newType, IParameterizedMember newOwner)
		{
			Debug.Assert(baseParameter != null && newType != null);
			this.baseParameter = baseParameter;
			this.newType = newType;
			this.newOwner = newOwner;
		}

		IEnumerable<IAttribute> IParameter.GetAttributes() => baseParameter.GetAttributes();
		ReferenceKind IParameter.ReferenceKind => baseParameter.ReferenceKind;
		bool IParameter.IsRef => baseParameter.IsRef;
		bool IParameter.IsOut => baseParameter.IsOut;
		bool IParameter.IsIn => baseParameter.IsIn;
		bool IParameter.IsParams => baseParameter.IsParams;
		bool IParameter.IsOptional => baseParameter.IsOptional;
		bool IParameter.HasConstantValueInSignature => baseParameter.HasConstantValueInSignature;
		IParameterizedMember IParameter.Owner => newOwner;
		string IVariable.Name => baseParameter.Name;
		string ISymbol.Name => baseParameter.Name;
		IType IVariable.Type => newType;
		bool IVariable.IsConst => baseParameter.IsConst;
		object IVariable.GetConstantValue(bool throwOnInvalidMetadata) => baseParameter.GetConstantValue(throwOnInvalidMetadata);
		SymbolKind ISymbol.SymbolKind => SymbolKind.Parameter;

		public override string ToString()
		{
			return DefaultParameter.ToString(this);
		}
	}
}
