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

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	sealed class MetadataParameter : IParameter
	{
		readonly MetadataAssembly assembly;
		readonly ParameterHandle handle;
		readonly ParameterAttributes attributes;

		public IType Type { get; }
		public IParameterizedMember Owner { get; }

		internal MetadataParameter(MetadataAssembly assembly, IParameterizedMember owner, IType type, ParameterHandle handle)
		{
			this.assembly = assembly;
			this.Owner = owner;
			this.Type = type;
			this.handle = handle;

			var param = assembly.metadata.GetParameter(handle);
			this.attributes = param.Attributes;
		}

		public EntityHandle MetadataToken => handle;

		public IReadOnlyList<IAttribute> Attributes => throw new NotImplementedException();

		const ParameterAttributes inOut = ParameterAttributes.In | ParameterAttributes.Out;
		public bool IsRef => Type.Kind == TypeKind.ByReference && (attributes & inOut) != ParameterAttributes.Out;
		public bool IsOut => Type.Kind == TypeKind.ByReference && (attributes & inOut) == ParameterAttributes.Out;

		public bool IsParams => throw new NotImplementedException();

		public bool IsOptional => (attributes & ParameterAttributes.HasDefault) != 0;

		public string Name => throw new NotImplementedException();

		bool IVariable.IsConst => false;

		public object ConstantValue => throw new NotImplementedException();

		SymbolKind ISymbol.SymbolKind => SymbolKind.Parameter;

		public override string ToString()
		{
			return $"{MetadataTokens.GetToken(handle):X8} {DefaultParameter.ToString(this)}";
		}
	}
}
