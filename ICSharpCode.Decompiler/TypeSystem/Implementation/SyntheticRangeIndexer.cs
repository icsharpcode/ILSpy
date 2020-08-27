// Copyright (c) 2020 Daniel Grunwald
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Synthetic method representing a compiler-generated indexer
	/// with the signature 'get_Item(System.Index)' or 'get_Item(System.Range)'.
	/// Can also be a setter.
	/// Used for the "Implicit Index support"/"Implicit Range support" for the C# 8 ranges feature.
	/// </summary>
	class SyntheticRangeIndexAccessor : IMethod
	{
		/// <summary>
		/// The underlying method: `get_Item(int)`, `set_Item(int, T)` or `Slice(int, int)`.
		/// </summary>
		readonly IMethod underlyingMethod;
		readonly IType indexOrRangeType;
		readonly IReadOnlyList<IParameter> parameters;
		readonly bool slicing;

		public SyntheticRangeIndexAccessor(IMethod underlyingMethod, IType indexOrRangeType, bool slicing)
		{
			Debug.Assert(underlyingMethod != null);
			Debug.Assert(indexOrRangeType != null);
			this.underlyingMethod = underlyingMethod;
			this.indexOrRangeType = indexOrRangeType;
			this.slicing = slicing;
			var parameters = new List<IParameter>();
			parameters.Add(new DefaultParameter(indexOrRangeType, ""));
			if (slicing)
			{
				Debug.Assert(underlyingMethod.Parameters.Count == 2);
			}
			else
			{
				parameters.AddRange(underlyingMethod.Parameters.Skip(1));
			}
			this.parameters = parameters;
		}

		public bool IsSlicing => slicing;

		bool IMethod.ReturnTypeIsRefReadOnly => underlyingMethod.ReturnTypeIsRefReadOnly;
		bool IMethod.ThisIsRefReadOnly => underlyingMethod.ThisIsRefReadOnly;
		bool IMethod.IsInitOnly => underlyingMethod.IsInitOnly;

		IReadOnlyList<ITypeParameter> IMethod.TypeParameters => EmptyList<ITypeParameter>.Instance;
		IReadOnlyList<IType> IMethod.TypeArguments => EmptyList<IType>.Instance;

		bool IMethod.IsExtensionMethod => false;
		bool IMethod.IsLocalFunction => false;
		bool IMethod.IsConstructor => false;
		bool IMethod.IsDestructor => false;
		bool IMethod.IsOperator => false;
		bool IMethod.HasBody => underlyingMethod.HasBody;
		bool IMethod.IsAccessor => underlyingMethod.IsAccessor;
		IMember IMethod.AccessorOwner => underlyingMethod.AccessorOwner;
		MethodSemanticsAttributes IMethod.AccessorKind => underlyingMethod.AccessorKind;
		IMethod IMethod.ReducedFrom => underlyingMethod.ReducedFrom;
		IReadOnlyList<IParameter> IParameterizedMember.Parameters => parameters;
		IMember IMember.MemberDefinition => underlyingMethod.MemberDefinition;
		IType IMember.ReturnType => underlyingMethod.ReturnType;
		IEnumerable<IMember> IMember.ExplicitlyImplementedInterfaceMembers => EmptyList<IMember>.Instance;
		bool IMember.IsExplicitInterfaceImplementation => false;
		bool IMember.IsVirtual => underlyingMethod.IsVirtual;
		bool IMember.IsOverride => underlyingMethod.IsOverride;
		bool IMember.IsOverridable => underlyingMethod.IsOverridable;
		TypeParameterSubstitution IMember.Substitution => underlyingMethod.Substitution;
		EntityHandle IEntity.MetadataToken => underlyingMethod.MetadataToken;
		public string Name => underlyingMethod.Name;
		IType IEntity.DeclaringType => underlyingMethod.DeclaringType;
		ITypeDefinition IEntity.DeclaringTypeDefinition => underlyingMethod.DeclaringTypeDefinition;
		IModule IEntity.ParentModule => underlyingMethod.ParentModule;
		Accessibility IEntity.Accessibility => underlyingMethod.Accessibility;
		bool IEntity.IsStatic => underlyingMethod.IsStatic;
		bool IEntity.IsAbstract => underlyingMethod.IsAbstract;
		bool IEntity.IsSealed => underlyingMethod.IsSealed;
		SymbolKind ISymbol.SymbolKind => SymbolKind.Method;
		ICompilation ICompilationProvider.Compilation => underlyingMethod.Compilation;
		string INamedElement.FullName => underlyingMethod.FullName;
		string INamedElement.ReflectionName => underlyingMethod.ReflectionName;
		string INamedElement.Namespace => underlyingMethod.Namespace;

		public override bool Equals(object obj)
		{
			return obj is SyntheticRangeIndexAccessor g
				&& this.underlyingMethod.Equals(g.underlyingMethod)
				&& this.indexOrRangeType.Equals(g.indexOrRangeType)
				&& this.slicing == g.slicing;
		}

		public override int GetHashCode()
		{
			return underlyingMethod.GetHashCode() ^ indexOrRangeType.GetHashCode();
		}

		bool IMember.Equals(IMember obj, TypeVisitor typeNormalization)
		{
			return obj is SyntheticRangeIndexAccessor g
				&& this.underlyingMethod.Equals(g.underlyingMethod, typeNormalization)
				&& this.indexOrRangeType.AcceptVisitor(typeNormalization).Equals(g.indexOrRangeType.AcceptVisitor(typeNormalization));
		}

		IEnumerable<IAttribute> IEntity.GetAttributes() => underlyingMethod.GetAttributes();

		IEnumerable<IAttribute> IMethod.GetReturnTypeAttributes() => underlyingMethod.GetReturnTypeAttributes();

		IMethod IMethod.Specialize(TypeParameterSubstitution substitution)
		{
			return new SyntheticRangeIndexAccessor(underlyingMethod.Specialize(substitution), indexOrRangeType, slicing);
		}

		IMember IMember.Specialize(TypeParameterSubstitution substitution)
		{
			return new SyntheticRangeIndexAccessor(underlyingMethod.Specialize(substitution), indexOrRangeType, slicing);
		}
	}
}
