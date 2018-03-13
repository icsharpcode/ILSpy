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
		public ReducedExtensionMethod(IMethod baseMethod)
		{
			this.ReducedFrom = baseMethod;
		}

		public override bool Equals(object obj)
		{
			var other = obj as ReducedExtensionMethod;
			if (other == null)
				return false;
			return ReducedFrom.Equals(other.ReducedFrom);
		}
		
		public override int GetHashCode()
		{
			unchecked {
				return ReducedFrom.GetHashCode() + 1;
			}
		}

		public override string ToString()
		{
			return string.Format("[ReducedExtensionMethod: ReducedFrom={0}]", ReducedFrom);
		}

		#region IMember implementation

		[Serializable]
		public sealed class ReducedExtensionMethodMemberReference : IMemberReference
		{
			readonly IMethod baseMethod;

			public ReducedExtensionMethodMemberReference (IMethod baseMethod)
			{
				this.baseMethod = baseMethod;
			}

			public IMember Resolve(ITypeResolveContext context)
			{
				return new ReducedExtensionMethod ((IMethod)baseMethod.ToReference ().Resolve (context));
			}
			
			ISymbol ISymbolReference.Resolve(ITypeResolveContext context)
			{
				return Resolve(context);
			}

			public ITypeReference DeclaringTypeReference => baseMethod.ToReference ().DeclaringTypeReference;
		}
		
		public IMemberReference ToReference()
		{
			return new ReducedExtensionMethodMemberReference (ReducedFrom);
		}
		
		ISymbolReference ISymbol.ToReference()
		{
			return ToReference();
		}

		public IMember MemberDefinition => ReducedFrom.MemberDefinition;

		public IUnresolvedMember UnresolvedMember => ReducedFrom.UnresolvedMember;

		public IType ReturnType => ReducedFrom.ReturnType;

		public IReadOnlyList<IMember> ImplementedInterfaceMembers => ReducedFrom.ImplementedInterfaceMembers;

		public bool IsExplicitInterfaceImplementation => ReducedFrom.IsExplicitInterfaceImplementation;

		public bool IsVirtual => ReducedFrom.IsVirtual;

		public bool IsOverride => ReducedFrom.IsOverride;

		public bool IsOverridable => ReducedFrom.IsOverridable;

		public TypeParameterSubstitution Substitution => ReducedFrom.Substitution;

		public IMethod Specialize(TypeParameterSubstitution substitution)
		{
			return new ReducedExtensionMethod((IMethod)ReducedFrom.Specialize(substitution));
		}
		
		IMember IMember.Specialize(TypeParameterSubstitution substitution)
		{
			return Specialize(substitution);
		}

		#endregion

		#region IMethod implementation

		public IReadOnlyList<IUnresolvedMethod> Parts => ReducedFrom.Parts;

		public IReadOnlyList<IAttribute> ReturnTypeAttributes => ReducedFrom.ReturnTypeAttributes;

		public IReadOnlyList<ITypeParameter> TypeParameters => ReducedFrom.TypeParameters;

		public bool IsExtensionMethod => true;

		public bool IsConstructor => ReducedFrom.IsConstructor;

		public bool IsDestructor => ReducedFrom.IsDestructor;

		public bool IsOperator => ReducedFrom.IsOperator;

		public bool IsPartial => ReducedFrom.IsPartial;

		public bool IsAsync => ReducedFrom.IsAsync;

		public bool HasBody => ReducedFrom.HasBody;

		public bool IsAccessor => ReducedFrom.IsAccessor;

		public IMember AccessorOwner => ReducedFrom.AccessorOwner;

		public IMethod ReducedFrom { get; }

		public IReadOnlyList<IType> TypeArguments => ReducedFrom.TypeArguments;

		#endregion

		#region IParameterizedMember implementation
		List<IParameter> parameters;
		public IReadOnlyList<IParameter> Parameters {
			get {
				if (parameters == null)
					parameters = new List<IParameter> (ReducedFrom.Parameters.Skip (1));
				return parameters;
			}
		}

		#endregion

		#region IEntity implementation

		public SymbolKind SymbolKind => ReducedFrom.SymbolKind;

		public ITypeDefinition DeclaringTypeDefinition => ReducedFrom.DeclaringTypeDefinition;

		public IType DeclaringType => ReducedFrom.DeclaringType;

		public IAssembly ParentAssembly => ReducedFrom.ParentAssembly;

		public IReadOnlyList<IAttribute> Attributes => ReducedFrom.Attributes;

		public bool IsStatic => false;

		public bool IsAbstract => ReducedFrom.IsAbstract;

		public bool IsSealed => ReducedFrom.IsSealed;

		public bool IsShadowing => ReducedFrom.IsShadowing;

		public bool IsSynthetic => ReducedFrom.IsSynthetic;

		#endregion

		#region IHasAccessibility implementation

		public Accessibility Accessibility => ReducedFrom.Accessibility;

		public bool IsPrivate => ReducedFrom.IsPrivate;

		public bool IsPublic => ReducedFrom.IsPublic;

		public bool IsProtected => ReducedFrom.IsProtected;

		public bool IsInternal => ReducedFrom.IsInternal;

		public bool IsProtectedOrInternal => ReducedFrom.IsProtectedOrInternal;

		public bool IsProtectedAndInternal => ReducedFrom.IsProtectedAndInternal;

		#endregion

		#region INamedElement implementation

		public string FullName => ReducedFrom.FullName;

		public string Name => ReducedFrom.Name;

		public string ReflectionName => ReducedFrom.ReflectionName;

		public string Namespace => ReducedFrom.Namespace;

		#endregion

		#region ICompilationProvider implementation

		public ICompilation Compilation => ReducedFrom.Compilation;

		#endregion
	}
}

