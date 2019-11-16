// Copyright (c) 2019 Siegfried Pammer
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
using System.Linq;
using System.Reflection;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// A local function has zero or more compiler-generated parameters added at the end.
	/// </summary>
	class LocalFunctionMethod : IMethod
	{
		readonly IMethod baseMethod;

		public LocalFunctionMethod(IMethod baseMethod, int numberOfCompilerGeneratedParameters)
		{
			this.baseMethod = baseMethod;
			this.NumberOfCompilerGeneratedParameters = numberOfCompilerGeneratedParameters;
		}


		public bool Equals(IMember obj, TypeVisitor typeNormalization)
		{
			if (!(obj is LocalFunctionMethod other))
				return false;
			return baseMethod.Equals(other.baseMethod, typeNormalization)
				&& NumberOfCompilerGeneratedParameters == other.NumberOfCompilerGeneratedParameters
				&& NumberOfCompilerGeneratedGenerics == other.NumberOfCompilerGeneratedGenerics;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is LocalFunctionMethod other))
				return false;
			return baseMethod.Equals(other.baseMethod)
				&& NumberOfCompilerGeneratedParameters == other.NumberOfCompilerGeneratedParameters
				&& NumberOfCompilerGeneratedGenerics == other.NumberOfCompilerGeneratedGenerics;
		}
		
		public override int GetHashCode()
		{
			unchecked {
				return baseMethod.GetHashCode() + NumberOfCompilerGeneratedParameters + 1;
			}
		}

		public override string ToString()
		{
			return string.Format("[LocalFunctionMethod: ReducedFrom={0}, NumberOfGeneratedParameters={1}, NumberOfCompilerGeneratedGenerics={2}]", ReducedFrom, NumberOfCompilerGeneratedParameters, NumberOfCompilerGeneratedGenerics);
		}

		internal int NumberOfCompilerGeneratedParameters { get; }

		internal int NumberOfCompilerGeneratedGenerics { get; set; }

		internal bool IsStaticLocalFunction => NumberOfCompilerGeneratedParameters == 0 && (baseMethod.IsStatic || (baseMethod.DeclaringTypeDefinition.IsCompilerGenerated() && !baseMethod.DeclaringType.GetFields(f => !f.IsStatic).Any()));

		public IMember MemberDefinition => this;

		public IType ReturnType => baseMethod.ReturnType;
		IEnumerable<IMember> IMember.ExplicitlyImplementedInterfaceMembers => baseMethod.ExplicitlyImplementedInterfaceMembers;
		bool IMember.IsExplicitInterfaceImplementation => baseMethod.IsExplicitInterfaceImplementation;
		public bool IsVirtual => baseMethod.IsVirtual;
		public bool IsOverride => baseMethod.IsOverride;
		public bool IsOverridable => baseMethod.IsOverridable;
		public TypeParameterSubstitution Substitution => baseMethod.Substitution;

		public IMethod Specialize(TypeParameterSubstitution substitution)
		{
			return SpecializedMethod.Create(this, substitution);
		}
		
		IMember IMember.Specialize(TypeParameterSubstitution substitution)
		{
			return Specialize(substitution);
		}

		public IReadOnlyList<ITypeParameter> TypeParameters => baseMethod.TypeParameters;
		public bool IsExtensionMethod => baseMethod.IsExtensionMethod;
		public bool IsLocalFunction => true;
		public bool IsConstructor => baseMethod.IsConstructor;
		public bool IsDestructor => baseMethod.IsDestructor;
		public bool IsOperator => baseMethod.IsOperator;
		public bool HasBody => baseMethod.HasBody;
		public bool IsAccessor => baseMethod.IsAccessor;
		public IMember AccessorOwner => baseMethod.AccessorOwner;
		public MethodSemanticsAttributes AccessorKind => baseMethod.AccessorKind;
		public IMethod ReducedFrom => baseMethod;
		public IReadOnlyList<IType> TypeArguments => baseMethod.TypeArguments;

		List<IParameter> parameters;
		public IReadOnlyList<IParameter> Parameters {
			get {
				if (parameters == null)
					parameters = new List<IParameter>(baseMethod.Parameters.SkipLast(NumberOfCompilerGeneratedParameters));
				return parameters;
			}
		}

		public System.Reflection.Metadata.EntityHandle MetadataToken => baseMethod.MetadataToken;
		public SymbolKind SymbolKind => baseMethod.SymbolKind;
		public ITypeDefinition DeclaringTypeDefinition => baseMethod.DeclaringTypeDefinition;
		public IType DeclaringType => baseMethod.DeclaringType;
		public IModule ParentModule => baseMethod.ParentModule;
		IEnumerable<IAttribute> IEntity.GetAttributes() => baseMethod.GetAttributes();
		IEnumerable<IAttribute> IMethod.GetReturnTypeAttributes() => baseMethod.GetReturnTypeAttributes();
		bool IMethod.ReturnTypeIsRefReadOnly => baseMethod.ReturnTypeIsRefReadOnly;
		bool IMethod.ThisIsRefReadOnly => baseMethod.ThisIsRefReadOnly;
		/// <summary>
		/// We consider local functions as always static, because they do not have a "this parameter".
		/// Even local functions in instance methods capture this.
		/// </summary>
		public bool IsStatic => true;
		public bool IsAbstract => baseMethod.IsAbstract;
		public bool IsSealed => baseMethod.IsSealed;

		public Accessibility Accessibility => baseMethod.Accessibility;

		public string FullName => baseMethod.FullName;
		public string Name => baseMethod.Name;
		public string ReflectionName => baseMethod.ReflectionName;
		public string Namespace => baseMethod.Namespace;

		public ICompilation Compilation => baseMethod.Compilation;
	}
}

