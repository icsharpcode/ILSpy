// Copyright (c) 2018 Daniel Grunwald
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
using System.Reflection;
using System.Reflection.Metadata;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Base class for fake members.
	/// </summary>
	abstract class FakeMember : IMember
	{
		readonly ICompilation compilation;

		protected FakeMember(ICompilation compilation)
		{
			this.compilation = compilation ?? throw new ArgumentNullException(nameof(compilation));
		}

		IMember IMember.MemberDefinition => this;

		public IType ReturnType { get; set; } = SpecialType.UnknownType;

		IEnumerable<IMember> IMember.ExplicitlyImplementedInterfaceMembers => EmptyList<IMember>.Instance;

		bool IMember.IsExplicitInterfaceImplementation => false;

		bool IMember.IsVirtual => false;
		bool IMember.IsOverride => false;
		bool IMember.IsOverridable => false;

		TypeParameterSubstitution IMember.Substitution => TypeParameterSubstitution.Identity;
		EntityHandle IEntity.MetadataToken => default;

		public string Name { get; set; }

		ITypeDefinition IEntity.DeclaringTypeDefinition => DeclaringType?.GetDefinition();

		public IType DeclaringType { get; set; }

		IModule IEntity.ParentModule => DeclaringType?.GetDefinition()?.ParentModule;

		IEnumerable<IAttribute> IEntity.GetAttributes() => EmptyList<IAttribute>.Instance;

		public Accessibility Accessibility { get; set; } = Accessibility.Public;

		public bool IsStatic { get; set; }
		bool IEntity.IsAbstract => false;
		bool IEntity.IsSealed => false;

		public abstract SymbolKind SymbolKind { get; }

		ICompilation ICompilationProvider.Compilation => compilation;

		string INamedElement.FullName {
			get {
				if (DeclaringType != null)
					return DeclaringType.FullName + "." + Name;
				else
					return Name;
			}
		}

		string INamedElement.ReflectionName {
			get {
				if (DeclaringType != null)
					return DeclaringType.ReflectionName + "." + Name;
				else
					return Name;
			}
		}

		string INamedElement.Namespace => DeclaringType?.Namespace;

		bool IMember.Equals(IMember obj, TypeVisitor typeNormalization)
		{
			return Equals(obj);
		}

		public abstract IMember Specialize(TypeParameterSubstitution substitution);
	}

	class FakeField : FakeMember, IField
	{
		public FakeField(ICompilation compilation) : base(compilation)
		{
		}

		bool IField.IsReadOnly => false;
		bool IField.IsVolatile => false;

		bool IVariable.IsConst => false;
		object IVariable.GetConstantValue(bool throwOnInvalidMetadata) => null;
		IType IVariable.Type => ReturnType;

		public override SymbolKind SymbolKind => SymbolKind.Field;

		public override IMember Specialize(TypeParameterSubstitution substitution)
		{
			return SpecializedField.Create(this, substitution);
		}
	}

	class FakeMethod : FakeMember, IMethod
	{
		readonly SymbolKind symbolKind;

		public FakeMethod(ICompilation compilation, SymbolKind symbolKind) : base(compilation)
		{
			this.symbolKind = symbolKind;
		}

		public override SymbolKind SymbolKind => symbolKind;
		
		IEnumerable<IAttribute> IMethod.GetReturnTypeAttributes() => EmptyList<IAttribute>.Instance;
		bool IMethod.ReturnTypeIsRefReadOnly => false;
		bool IMethod.ThisIsRefReadOnly => false;
		bool IMethod.IsInitOnly => false;

		public IReadOnlyList<ITypeParameter> TypeParameters { get; set; } = EmptyList<ITypeParameter>.Instance;

		IReadOnlyList<IType> IMethod.TypeArguments => TypeParameters;

		bool IMethod.IsExtensionMethod => false;
		bool IMethod.IsLocalFunction => false;
		bool IMethod.IsConstructor => symbolKind == SymbolKind.Constructor;
		bool IMethod.IsDestructor => symbolKind == SymbolKind.Destructor;
		bool IMethod.IsOperator => symbolKind == SymbolKind.Operator;

		bool IMethod.HasBody => false;
		bool IMethod.IsAccessor => false;
		IMember IMethod.AccessorOwner => null;
		MethodSemanticsAttributes IMethod.AccessorKind => 0;

		IMethod IMethod.ReducedFrom => null;

		public IReadOnlyList<IParameter> Parameters { get; set; } = EmptyList<IParameter>.Instance;
		
		public override IMember Specialize(TypeParameterSubstitution substitution)
		{
			return SpecializedMethod.Create(this, substitution);
		}

		IMethod IMethod.Specialize(TypeParameterSubstitution substitution)
		{
			return SpecializedMethod.Create(this, substitution);
		}

		internal static IMethod CreateDummyConstructor(ICompilation compilation, IType declaringType, Accessibility accessibility = Accessibility.Public)
		{
			return new FakeMethod(compilation, SymbolKind.Constructor) {
				DeclaringType = declaringType,
				Name = ".ctor",
				ReturnType = compilation.FindType(KnownTypeCode.Void),
				Accessibility = accessibility,
			};
		}
	}
}
