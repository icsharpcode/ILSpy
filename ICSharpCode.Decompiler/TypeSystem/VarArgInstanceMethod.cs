// Copyright (c) 2016 Daniel Grunwald
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
using System.Linq;
using System.Text;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Used when calling a vararg method. Stores the actual parameter types being passed.
	/// </summary>
	public class VarArgInstanceMethod : IMethod
	{
		readonly IMethod baseMethod;
		readonly IParameter[] parameters;

		public VarArgInstanceMethod(IMethod baseMethod, IEnumerable<IType> varArgTypes)
		{
			this.baseMethod = baseMethod;
			var paramList = new List<IParameter>(baseMethod.Parameters);
			Debug.Assert(paramList.Last().Type.Kind == TypeKind.ArgList);
			paramList.RemoveAt(paramList.Count - 1);
			foreach (var varArg in varArgTypes) {
				paramList.Add(new DefaultParameter(varArg, name: string.Empty, owner: this));
			}
			this.parameters = paramList.ToArray();
		}
		
		public int RegularParameterCount => baseMethod.Parameters.Count - 1;

		public IReadOnlyList<IParameter> Parameters => parameters;

		public override bool Equals(object obj)
		{
			var other = obj as VarArgInstanceMethod;
			return other != null && baseMethod.Equals(other.baseMethod);
		}
		
		public override int GetHashCode()
		{
			return baseMethod.GetHashCode();
		}
		
		public override string ToString()
		{
			var b = new StringBuilder("[");
			b.Append(this.SymbolKind);
			if (this.DeclaringType != null) {
				b.Append(this.DeclaringType.ReflectionName);
				b.Append('.');
			}
			b.Append(this.Name);
			if (this.TypeParameters.Count > 0) {
				b.Append("``");
				b.Append(this.TypeParameters.Count);
			}
			b.Append('(');
			for (var i = 0; i < this.Parameters.Count; i++) {
				if (i > 0)
					b.Append(", ");
				if (i == this.RegularParameterCount)
					b.Append("..., ");
				b.Append(this.Parameters[i].Type.ReflectionName);
			}
			if (this.Parameters.Count == this.RegularParameterCount) {
				b.Append(", ...");
			}
			b.Append("):");
			b.Append(this.ReturnType.ReflectionName);
			b.Append(']');
			return b.ToString();
		}
		
		#region IMethod implementation
		public IMethod Specialize(TypeParameterSubstitution substitution)
		{
			return new VarArgInstanceMethod(
				baseMethod.Specialize(substitution),
				parameters.Skip(baseMethod.Parameters.Count - 1).Select(p => p.Type.AcceptVisitor(substitution)).ToList());
		}

		public IReadOnlyList<IUnresolvedMethod> Parts => baseMethod.Parts;

		public IReadOnlyList<IAttribute> ReturnTypeAttributes => baseMethod.ReturnTypeAttributes;

		public IReadOnlyList<ITypeParameter> TypeParameters => baseMethod.TypeParameters;

		public IReadOnlyList<IType> TypeArguments => baseMethod.TypeArguments;

		public bool IsExtensionMethod => baseMethod.IsExtensionMethod;

		public bool IsConstructor => baseMethod.IsConstructor;

		public bool IsDestructor => baseMethod.IsDestructor;

		public bool IsOperator => baseMethod.IsOperator;

		public bool IsPartial => baseMethod.IsPartial;

		public bool IsAsync => baseMethod.IsAsync;

		public bool HasBody => baseMethod.HasBody;

		public bool IsAccessor => baseMethod.IsAccessor;

		public IMember AccessorOwner => baseMethod.AccessorOwner;

		public IMethod ReducedFrom => baseMethod.ReducedFrom;

		#endregion

		#region IMember implementation
		
		public IMemberReference ToReference()
		{
			throw new NotImplementedException();
		}

		IMember IMember.Specialize(TypeParameterSubstitution substitution)
		{
			return Specialize(substitution);
		}

		public IMember MemberDefinition => baseMethod.MemberDefinition;

		public IUnresolvedMember UnresolvedMember => baseMethod.UnresolvedMember;

		public IType ReturnType => baseMethod.ReturnType;

		public IReadOnlyList<IMember> ImplementedInterfaceMembers => baseMethod.ImplementedInterfaceMembers;

		public bool IsExplicitInterfaceImplementation => baseMethod.IsExplicitInterfaceImplementation;

		public bool IsVirtual => baseMethod.IsVirtual;

		public bool IsOverride => baseMethod.IsOverride;

		public bool IsOverridable => baseMethod.IsOverridable;

		public TypeParameterSubstitution Substitution => baseMethod.Substitution;

		#endregion

		#region ISymbol implementation

		ISymbolReference ISymbol.ToReference()
		{
			return ToReference();
		}

		public SymbolKind SymbolKind => baseMethod.SymbolKind;

		public string Name => baseMethod.Name;

		#endregion

		#region IEntity implementation
		
		public ITypeDefinition DeclaringTypeDefinition => baseMethod.DeclaringTypeDefinition;

		public IType DeclaringType => baseMethod.DeclaringType;

		public IAssembly ParentAssembly => baseMethod.ParentAssembly;

		public IReadOnlyList<IAttribute> Attributes => baseMethod.Attributes;

		public bool IsStatic => baseMethod.IsStatic;

		public bool IsAbstract => baseMethod.IsAbstract;

		public bool IsSealed => baseMethod.IsSealed;

		public bool IsShadowing => baseMethod.IsShadowing;

		public bool IsSynthetic => baseMethod.IsSynthetic;

		#endregion

		#region IHasAccessibility implementation

		public Accessibility Accessibility => baseMethod.Accessibility;

		public bool IsPrivate => baseMethod.IsPrivate;

		public bool IsPublic => baseMethod.IsPublic;

		public bool IsProtected => baseMethod.IsProtected;

		public bool IsInternal => baseMethod.IsInternal;

		public bool IsProtectedOrInternal => baseMethod.IsProtectedOrInternal;

		public bool IsProtectedAndInternal => baseMethod.IsProtectedAndInternal;

		#endregion

		#region INamedElement implementation

		public string FullName => baseMethod.FullName;

		public string ReflectionName => baseMethod.ReflectionName;

		public string Namespace => baseMethod.Namespace;

		#endregion

		#region ICompilationProvider implementation

		public ICompilation Compilation => baseMethod.Compilation;

		#endregion
	}
}
