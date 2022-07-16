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
using System.Reflection;
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
			foreach (IType varArg in varArgTypes)
			{
				paramList.Add(new DefaultParameter(varArg, name: string.Empty, owner: this));
			}
			this.parameters = paramList.ToArray();
		}

		public IMethod BaseMethod => baseMethod;

		public int RegularParameterCount {
			get { return baseMethod.Parameters.Count - 1; }
		}

		public IReadOnlyList<IParameter> Parameters {
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

		public bool Equals(IMember obj, TypeVisitor typeNormalization)
		{
			VarArgInstanceMethod other = obj as VarArgInstanceMethod;
			return other != null && baseMethod.Equals(other.baseMethod, typeNormalization);
		}

		public override string ToString()
		{
			StringBuilder b = new StringBuilder("[");
			b.Append(this.SymbolKind);
			if (this.DeclaringType != null)
			{
				b.Append(this.DeclaringType.ReflectionName);
				b.Append('.');
			}
			b.Append(this.Name);
			if (this.TypeParameters.Count > 0)
			{
				b.Append("``");
				b.Append(this.TypeParameters.Count);
			}
			b.Append('(');
			for (int i = 0; i < this.Parameters.Count; i++)
			{
				if (i > 0)
					b.Append(", ");
				if (i == this.RegularParameterCount)
					b.Append("..., ");
				b.Append(this.Parameters[i].Type.ReflectionName);
			}
			if (this.Parameters.Count == this.RegularParameterCount)
			{
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

		IEnumerable<IAttribute> IEntity.GetAttributes() => baseMethod.GetAttributes();
		bool IEntity.HasAttribute(KnownAttribute attribute) => baseMethod.HasAttribute(attribute);
		IAttribute IEntity.GetAttribute(KnownAttribute attribute) => baseMethod.GetAttribute(attribute);

		IEnumerable<IAttribute> IMethod.GetReturnTypeAttributes() => baseMethod.GetReturnTypeAttributes();
		bool IMethod.ReturnTypeIsRefReadOnly => baseMethod.ReturnTypeIsRefReadOnly;
		bool IMethod.ThisIsRefReadOnly => baseMethod.ThisIsRefReadOnly;
		bool IMethod.IsInitOnly => baseMethod.IsInitOnly;

		public IReadOnlyList<ITypeParameter> TypeParameters {
			get { return baseMethod.TypeParameters; }
		}

		public IReadOnlyList<IType> TypeArguments {
			get { return baseMethod.TypeArguments; }
		}

		public System.Reflection.Metadata.EntityHandle MetadataToken => baseMethod.MetadataToken;

		public bool IsExtensionMethod {
			get { return baseMethod.IsExtensionMethod; }
		}

		bool IMethod.IsLocalFunction {
			get { return baseMethod.IsLocalFunction; }
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

		public bool HasBody {
			get { return baseMethod.HasBody; }
		}

		public bool IsAccessor => baseMethod.IsAccessor;
		public IMember AccessorOwner => baseMethod.AccessorOwner;
		public MethodSemanticsAttributes AccessorKind => baseMethod.AccessorKind;

		public IMethod ReducedFrom {
			get { return baseMethod.ReducedFrom; }
		}

		#endregion

		#region IMember implementation

		IMember IMember.Specialize(TypeParameterSubstitution substitution)
		{
			return Specialize(substitution);
		}

		public IMember MemberDefinition {
			get { return baseMethod.MemberDefinition; }
		}

		public IType ReturnType {
			get { return baseMethod.ReturnType; }
		}

		public IEnumerable<IMember> ExplicitlyImplementedInterfaceMembers {
			get { return baseMethod.ExplicitlyImplementedInterfaceMembers; }
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

		public SymbolKind SymbolKind {
			get { return baseMethod.SymbolKind; }
		}

		public string Name {
			get { return baseMethod.Name; }
		}

		#endregion

		#region IEntity implementation

		public ITypeDefinition DeclaringTypeDefinition {
			get { return baseMethod.DeclaringTypeDefinition; }
		}

		public IType DeclaringType {
			get { return baseMethod.DeclaringType; }
		}

		public IModule ParentModule {
			get { return baseMethod.ParentModule; }
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

		#endregion

		#region IHasAccessibility implementation

		public Accessibility Accessibility {
			get { return baseMethod.Accessibility; }
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
