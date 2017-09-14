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
			foreach (IType varArg in varArgTypes) {
				paramList.Add(new DefaultParameter(varArg, name: string.Empty, owner: this));
			}
			this.parameters = paramList.ToArray();
		}
		
		public int RegularParameterCount {
			get { return baseMethod.Parameters.Count - 1; }
		}
		
		public IList<IParameter> Parameters {
			get { return parameters; }
		}
		
		public override bool Equals(object obj)
		{
			VarArgInstanceMethod other = obj as VarArgInstanceMethod;
			return other != null && baseMethod.Equals(other.baseMethod);
		}
		
		public override int GetHashCode()
		{
			return baseMethod.GetHashCode();
		}
		
		public override string ToString()
		{
			StringBuilder b = new StringBuilder("[");
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
			for (int i = 0; i < this.Parameters.Count; i++) {
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

		public IList<IUnresolvedMethod> Parts {
			get { return baseMethod.Parts; }
		}

		public IList<IAttribute> ReturnTypeAttributes {
			get { return baseMethod.ReturnTypeAttributes; }
		}

		public IList<ITypeParameter> TypeParameters {
			get { return baseMethod.TypeParameters; }
		}

		public bool IsParameterized {
			get { return baseMethod.IsParameterized; }
		}

		public IList<IType> TypeArguments {
			get { return baseMethod.TypeArguments; }
		}

		public bool IsExtensionMethod {
			get { return baseMethod.IsExtensionMethod; }
		}

		public bool IsConstructor {
			get { return baseMethod.IsConstructor; }
		}

		public bool IsDestructor {
			get { return baseMethod.IsDestructor; }
		}

		public bool IsOperator {
			get { return baseMethod.IsOperator; }
		}

		public bool IsPartial {
			get { return baseMethod.IsPartial; }
		}

		public bool IsAsync {
			get { return baseMethod.IsAsync; }
		}

		public bool HasBody {
			get { return baseMethod.HasBody; }
		}

		public bool IsAccessor {
			get { return baseMethod.IsAccessor; }
		}

		public IMember AccessorOwner {
			get { return baseMethod.AccessorOwner; }
		}

		public IMethod ReducedFrom {
			get { return baseMethod.ReducedFrom; }
		}

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

		public IMember MemberDefinition {
			get { return baseMethod.MemberDefinition; }
		}

		public IUnresolvedMember UnresolvedMember {
			get { return baseMethod.UnresolvedMember; }
		}

		public IType ReturnType {
			get { return baseMethod.ReturnType; }
		}

		public IList<IMember> ImplementedInterfaceMembers {
			get { return baseMethod.ImplementedInterfaceMembers; }
		}

		public bool IsExplicitInterfaceImplementation {
			get { return baseMethod.IsExplicitInterfaceImplementation; }
		}

		public bool IsVirtual {
			get { return baseMethod.IsVirtual; }
		}

		public bool IsOverride {
			get { return baseMethod.IsOverride; }
		}

		public bool IsOverridable {
			get { return baseMethod.IsOverridable; }
		}

		public TypeParameterSubstitution Substitution {
			get { return baseMethod.Substitution; }
		}

		#endregion

		#region ISymbol implementation

		ISymbolReference ISymbol.ToReference()
		{
			return ToReference();
		}

		public SymbolKind SymbolKind {
			get { return baseMethod.SymbolKind; }
		}

		public string Name {
			get { return baseMethod.Name; }
		}

		#endregion

		#region IEntity implementation
		
		public DomRegion Region {
			get { return baseMethod.Region; }
		}

		public DomRegion BodyRegion {
			get { return baseMethod.BodyRegion; }
		}

		public ITypeDefinition DeclaringTypeDefinition {
			get { return baseMethod.DeclaringTypeDefinition; }
		}

		public IType DeclaringType {
			get { return baseMethod.DeclaringType; }
		}

		public IAssembly ParentAssembly {
			get { return baseMethod.ParentAssembly; }
		}

		public IList<IAttribute> Attributes {
			get { return baseMethod.Attributes; }
		}

		public bool IsStatic {
			get { return baseMethod.IsStatic; }
		}

		public bool IsAbstract {
			get { return baseMethod.IsAbstract; }
		}

		public bool IsSealed {
			get { return baseMethod.IsSealed; }
		}

		public bool IsShadowing {
			get { return baseMethod.IsShadowing; }
		}

		public bool IsSynthetic {
			get { return baseMethod.IsSynthetic; }
		}

		#endregion

		#region IHasAccessibility implementation

		public Accessibility Accessibility {
			get { return baseMethod.Accessibility; }
		}

		public bool IsPrivate {
			get { return baseMethod.IsPrivate; }
		}

		public bool IsPublic {
			get { return baseMethod.IsPublic; }
		}

		public bool IsProtected {
			get { return baseMethod.IsProtected; }
		}

		public bool IsInternal {
			get { return baseMethod.IsInternal; }
		}

		public bool IsProtectedOrInternal {
			get { return baseMethod.IsProtectedOrInternal; }
		}

		public bool IsProtectedAndInternal {
			get { return baseMethod.IsProtectedAndInternal; }
		}

		#endregion

		#region INamedElement implementation

		public string FullName {
			get { return baseMethod.FullName; }
		}

		public string ReflectionName {
			get { return baseMethod.ReflectionName; }
		}

		public string Namespace {
			get { return baseMethod.Namespace; }
		}

		#endregion

		#region ICompilationProvider implementation

		public ICompilation Compilation {
			get { return baseMethod.Compilation; }
		}

		#endregion
	}
}
