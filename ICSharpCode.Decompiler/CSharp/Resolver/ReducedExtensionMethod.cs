//
// ReducedExtensionMethod.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2013 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Resolver
{
	/// <summary>
	/// An invocated extension method hides the extension parameter in its parameter list.
	/// It's used to hide the internals of extension method invocation in certain situation to simulate the
	/// syntactic way of writing extension methods on semantic level.
	/// </summary>
	public class ReducedExtensionMethod : IMethod
	{
		readonly IMethod baseMethod;

		public ReducedExtensionMethod(IMethod baseMethod)
		{
			this.baseMethod = baseMethod;
		}

		public bool Equals(IMember obj, TypeVisitor typeNormalization)
		{
			var other = obj as ReducedExtensionMethod;
			if (other == null)
				return false;
			return baseMethod.Equals(other.baseMethod, typeNormalization);
		}

		public override bool Equals(object obj)
		{
			var other = obj as ReducedExtensionMethod;
			if (other == null)
				return false;
			return baseMethod.Equals(other.baseMethod);
		}
		
		public override int GetHashCode()
		{
			unchecked {
				return baseMethod.GetHashCode() + 1;
			}
		}

		public override string ToString()
		{
			return string.Format("[ReducedExtensionMethod: ReducedFrom={0}]", ReducedFrom);
		}

		#region IMember implementation
		public IMember MemberDefinition {
			get {
				return baseMethod.MemberDefinition;
			}
		}
		
		public IType ReturnType {
			get {
				return baseMethod.ReturnType;
			}
		}

		public IEnumerable<IMember> ExplicitlyImplementedInterfaceMembers {
			get {
				return baseMethod.ExplicitlyImplementedInterfaceMembers;
			}
		}

		public bool IsExplicitInterfaceImplementation {
			get {
				return baseMethod.IsExplicitInterfaceImplementation;
			}
		}

		public bool IsVirtual {
			get {
				return baseMethod.IsVirtual;
			}
		}

		public bool IsOverride {
			get {
				return baseMethod.IsOverride;
			}
		}

		public bool IsOverridable {
			get {
				return baseMethod.IsOverridable;
			}
		}

		public TypeParameterSubstitution Substitution {
			get {
				return baseMethod.Substitution;
			}
		}

		public IMethod Specialize(TypeParameterSubstitution substitution)
		{
			return new ReducedExtensionMethod((IMethod)baseMethod.Specialize(substitution));
		}
		
		IMember IMember.Specialize(TypeParameterSubstitution substitution)
		{
			return Specialize(substitution);
		}

		#endregion

		#region IMethod implementation

		public IReadOnlyList<ITypeParameter> TypeParameters {
			get {
				return baseMethod.TypeParameters;
			}
		}

		public bool IsExtensionMethod {
			get {
				return true;
			}
		}

		public bool IsConstructor {
			get {
				return baseMethod.IsConstructor;
			}
		}

		public bool IsDestructor {
			get {
				return baseMethod.IsDestructor;
			}
		}

		public bool IsOperator {
			get {
				return baseMethod.IsOperator;
			}
		}
		
		public bool HasBody {
			get {
				return baseMethod.HasBody;
			}
		}

		public bool IsAccessor {
			get {
				return baseMethod.IsAccessor;
			}
		}

		public IMember AccessorOwner {
			get {
				return baseMethod.AccessorOwner;
			}
		}

		public IMethod ReducedFrom {
			get {
				return baseMethod;
			}
		}

		public IReadOnlyList<IType> TypeArguments {
			get {
				return baseMethod.TypeArguments;
			}
		}
		#endregion

		#region IParameterizedMember implementation
		List<IParameter> parameters;
		public IReadOnlyList<IParameter> Parameters {
			get {
				if (parameters == null)
					parameters = new List<IParameter> (baseMethod.Parameters.Skip (1));
				return parameters;
			}
		}

		#endregion

		#region IEntity implementation

		public System.Reflection.Metadata.EntityHandle MetadataToken => baseMethod.MetadataToken;

		public SymbolKind SymbolKind {
			get {
				return baseMethod.SymbolKind;
			}
		}
		
		public ITypeDefinition DeclaringTypeDefinition {
			get {
				return baseMethod.DeclaringTypeDefinition;
			}
		}

		public IType DeclaringType {
			get {
				return baseMethod.DeclaringType;
			}
		}

		public IModule ParentModule {
			get {
				return baseMethod.ParentModule;
			}
		}

		IEnumerable<IAttribute> IEntity.GetAttributes() => baseMethod.GetAttributes();
		IEnumerable<IAttribute> IMethod.GetReturnTypeAttributes() => baseMethod.GetReturnTypeAttributes();

		public bool IsStatic {
			get {
				return false;
			}
		}

		public bool IsAbstract {
			get {
				return baseMethod.IsAbstract;
			}
		}

		public bool IsSealed {
			get {
				return baseMethod.IsSealed;
			}
		}
		
		#endregion

		#region IHasAccessibility implementation

		public Accessibility Accessibility {
			get {
				return baseMethod.Accessibility;
			}
		}
		
		#endregion

		#region INamedElement implementation

		public string FullName {
			get {
				return baseMethod.FullName;
			}
		}

		public string Name {
			get {
				return baseMethod.Name;
			}
		}

		public string ReflectionName {
			get {
				return baseMethod.ReflectionName;
			}
		}

		public string Namespace {
			get {
				return baseMethod.Namespace;
			}
		}

		#endregion

		#region ICompilationProvider implementation

		public ICompilation Compilation {
			get {
				return baseMethod.Compilation;
			}
		}

		#endregion
	}
}

