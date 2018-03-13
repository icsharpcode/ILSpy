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
		public DefaultParameter(IType type, string name)
		{
			this.Type = type ?? throw new ArgumentNullException("type");
			this.Name = name ?? throw new ArgumentNullException("name");
		}
		
		public DefaultParameter(IType type, string name, IParameterizedMember owner = null, IReadOnlyList<IAttribute> attributes = null,
		                        bool isRef = false, bool isOut = false, bool isParams = false, bool isOptional = false, object defaultValue = null)
		{
			this.Type = type ?? throw new ArgumentNullException("type");
			this.Name = name ?? throw new ArgumentNullException("name");
			this.Owner = owner;
			this.Attributes = attributes;
			this.IsRef = isRef;
			this.IsOut = isOut;
			this.IsParams = isParams;
			this.IsOptional = isOptional;
			this.ConstantValue = defaultValue;
		}
		
		SymbolKind ISymbol.SymbolKind => SymbolKind.Parameter;

		public IParameterizedMember Owner { get; }

		public IReadOnlyList<IAttribute> Attributes { get; }

		public bool IsRef { get; }

		public bool IsOut { get; }

		public bool IsParams { get; }

		public bool IsOptional { get; }

		public string Name { get; }

		public IType Type { get; }

		bool IVariable.IsConst => false;

		public object ConstantValue { get; }

		public override string ToString()
		{
			return ToString(this);
		}
		
		public static string ToString(IParameter parameter)
		{
			var b = new StringBuilder();
			if (parameter.IsRef)
				b.Append("ref ");
			if (parameter.IsOut)
				b.Append("out ");
			if (parameter.IsParams)
				b.Append("params ");
			b.Append(parameter.Name);
			b.Append(':');
			b.Append(parameter.Type.ReflectionName);
			if (parameter.IsOptional) {
				b.Append(" = ");
				if (parameter.ConstantValue != null)
					b.Append(parameter.ConstantValue.ToString());
				else
					b.Append("null");
			}
			return b.ToString();
		}

		public ISymbolReference ToReference()
		{
			if (Owner == null)
				return new ParameterReference(Type.ToTypeReference(), Name, IsRef, IsOut, IsParams, IsOptional, ConstantValue);
			return new OwnedParameterReference(Owner.ToReference(), Owner.Parameters.IndexOf(this));
		}
	}
	
	sealed class OwnedParameterReference : ISymbolReference
	{
		readonly IMemberReference memberReference;
		readonly int index;
		
		public OwnedParameterReference(IMemberReference member, int index)
		{
			this.memberReference = member ?? throw new ArgumentNullException("member");
			this.index = index;
		}
		
		public ISymbol Resolve(ITypeResolveContext context)
		{
			var member = memberReference.Resolve(context) as IParameterizedMember;
			if (member != null && index >= 0 && index < member.Parameters.Count)
				return member.Parameters[index];
			else
				return null;
		}
	}
	
	public sealed class ParameterReference : ISymbolReference
	{
		readonly ITypeReference type;
		readonly string name;
		readonly bool isRef, isOut, isParams, isOptional;
		readonly object defaultValue;
		
		public ParameterReference(ITypeReference type, string name, bool isRef, bool isOut, bool isParams, bool isOptional, object defaultValue)
		{
			this.type = type ?? throw new ArgumentNullException("type");
			this.name = name ?? throw new ArgumentNullException("name");
			this.isRef = isRef;
			this.isOut = isOut;
			this.isParams = isParams;
			this.isOptional = isOptional;
			this.defaultValue = defaultValue;
		}

		public ISymbol Resolve(ITypeResolveContext context)
		{
			return new DefaultParameter(type.Resolve(context), name, isRef: isRef, isOut: isOut, isParams: isParams, isOptional: isOptional, defaultValue: defaultValue);
		}
	}
}
