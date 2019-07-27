// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.Text;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation of <see cref="IParameter"/>.
	/// </summary>
	public sealed class DefaultParameter : IParameter
	{
		readonly IType type;
		readonly string name;
		readonly IReadOnlyList<IAttribute> attributes;
		readonly ReferenceKind referenceKind;
		readonly bool isParams, isOptional;
		readonly object defaultValue;
		readonly IParameterizedMember owner;
		
		public DefaultParameter(IType type, string name)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (name == null)
				throw new ArgumentNullException("name");
			this.type = type;
			this.name = name;
			this.attributes = EmptyList<IAttribute>.Instance;
		}
		
		public DefaultParameter(IType type, string name, IParameterizedMember owner = null, IReadOnlyList<IAttribute> attributes = null,
		                        ReferenceKind referenceKind = ReferenceKind.None, bool isParams = false, bool isOptional = false, object defaultValue = null)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (name == null)
				throw new ArgumentNullException("name");
			this.type = type;
			this.name = name;
			this.owner = owner;
			this.attributes = attributes ?? EmptyList<IAttribute>.Instance;
			this.referenceKind = referenceKind;
			this.isParams = isParams;
			this.isOptional = isOptional;
			this.defaultValue = defaultValue;
		}
		
		SymbolKind ISymbol.SymbolKind {
			get { return SymbolKind.Parameter; }
		}
		
		public IParameterizedMember Owner {
			get { return owner; }
		}
		
		public IEnumerable<IAttribute> GetAttributes() => attributes;

		public ReferenceKind ReferenceKind => referenceKind;
		public bool IsRef => referenceKind == ReferenceKind.Ref;
		public bool IsOut => referenceKind == ReferenceKind.Out;
		public bool IsIn => referenceKind == ReferenceKind.In;

		public bool IsParams => isParams;

		public bool IsOptional => isOptional;

		public string Name {
			get { return name; }
		}
		
		public IType Type {
			get { return type; }
		}
		
		bool IVariable.IsConst {
			get { return false; }
		}
		
		public bool HasConstantValueInSignature {
			get { return IsOptional; }
		}
		
		public object GetConstantValue(bool throwOnInvalidMetadata)
		{
			return defaultValue;
		}
		
		public override string ToString()
		{
			return ToString(this);
		}
		
		public static string ToString(IParameter parameter)
		{
			StringBuilder b = new StringBuilder();
			if (parameter.IsRef)
				b.Append("ref ");
			if (parameter.IsOut)
				b.Append("out ");
			if (parameter.IsIn)
				b.Append("in ");
			if (parameter.IsParams)
				b.Append("params ");
			b.Append(parameter.Name);
			b.Append(':');
			b.Append(parameter.Type.ReflectionName);
			if (parameter.IsOptional && parameter.HasConstantValueInSignature) {
				b.Append(" = ");
				object val = parameter.GetConstantValue(throwOnInvalidMetadata: false);
				if (val != null)
					b.Append(val.ToString());
				else
					b.Append("null");
			}
			return b.ToString();
		}
	}
}
