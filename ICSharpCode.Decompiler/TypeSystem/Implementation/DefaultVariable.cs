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

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation of <see cref="IVariable"/>.
	/// </summary>
	public sealed class DefaultVariable : IVariable
	{
		public DefaultVariable(IType type, string name)
		{
			this.Type = type ?? throw new ArgumentNullException("type");
			this.Name = name ?? throw new ArgumentNullException("name");
		}
		
		public DefaultVariable(IType type, string name,
		                       bool isConst = false, object constantValue = null)
			: this(type, name)
		{
			this.IsConst = isConst;
			this.ConstantValue = constantValue;
		}
		
		public string Name { get; }

		public IType Type { get; }

		public bool IsConst { get; }

		public object ConstantValue { get; }

		public SymbolKind SymbolKind => SymbolKind.Variable;

		public ISymbolReference ToReference()
		{
			return new VariableReference(Type.ToTypeReference(), Name, IsConst, ConstantValue);
		}
	}
	
	public sealed class VariableReference : ISymbolReference
	{
		readonly ITypeReference variableTypeReference;
		readonly string name;
		readonly bool isConst;
		readonly object constantValue;
		
		public VariableReference(ITypeReference variableTypeReference, string name, bool isConst, object constantValue)
		{
			this.variableTypeReference = variableTypeReference ?? throw new ArgumentNullException("variableTypeReference");
			this.name = name ?? throw new ArgumentNullException("name");
			this.isConst = isConst;
			this.constantValue = constantValue;
		}
		
		public ISymbol Resolve(ITypeResolveContext context)
		{
			return new DefaultVariable(variableTypeReference.Resolve(context), name, isConst, constantValue);
		}
	}
}
