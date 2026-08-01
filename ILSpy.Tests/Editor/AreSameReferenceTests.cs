// Copyright (c) 2026 Siegfried Pammer
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
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

[TestFixture]
public class AreSameReferenceTests
{
	static DecompilerTypeSystem typeSystem = null!;

	[OneTimeSetUp]
	public void LoadTypeSystem()
	{
		var file = new PEFile(typeof(AreSameReferenceTests).Assembly.Location);
		var resolver = new UniversalAssemblyResolver(file.FileName, throwOnError: false, file.DetectTargetFrameworkId());
		typeSystem = new DecompilerTypeSystem(file, resolver);
	}

	[Test]
	public void GeneratedMembersWithoutMetadataTokensAreNotConflated()
	{
		// Generated members have a nil MetadataToken; two distinct ones from the same
		// module must not be treated as the same reference by the token comparison.
		var a = new NilTokenMember(typeSystem.MainModule);
		var b = new NilTokenMember(typeSystem.MainModule);

		Assert.That(DecompilerTextView.AreSameReference(a, b), Is.False);
		Assert.That(DecompilerTextView.AreSameReference(a, a), Is.True);
	}

	[Test]
	public void MembersWithRealTokensStillCompareByDefinition()
	{
		var type = typeSystem.MainModule.Compilation.FindType(new FullTypeName(typeof(AreSameReferenceTests).FullName!)).GetDefinition()!;
		var method = type.GetMethods(m => m.Name == nameof(GeneratedMembersWithoutMetadataTokensAreNotConflated)).Single();
		var other = type.GetMethods(m => m.Name == nameof(MembersWithRealTokensStillCompareByDefinition)).Single();

		Assert.That(DecompilerTextView.AreSameReference(method, method.MemberDefinition), Is.True);
		Assert.That(DecompilerTextView.AreSameReference(method, other), Is.False);
	}

	// A minimal stand-in for a type-system-generated member: nil token, real module.
	sealed class NilTokenMember(IModule module) : IMember
	{
		public EntityHandle MetadataToken => default;
		public IMember MemberDefinition => this;
		public IModule ParentModule => module;
		public SymbolKind SymbolKind => SymbolKind.Method;
		public string Name => "Generated";
		public string FullName => Name;
		public string Namespace => string.Empty;
		public string ReflectionName => Name;
		public ICompilation Compilation => module.Compilation;
		public bool Equals(IMember? obj, TypeVisitor typeNormalization) => ReferenceEquals(this, obj);

		public IType ReturnType => throw new NotImplementedException();
		public IEnumerable<IMember> ExplicitlyImplementedInterfaceMembers => throw new NotImplementedException();
		public bool IsExplicitInterfaceImplementation => false;
		public bool IsVirtual => false;
		public bool IsOverride => false;
		public bool IsOverridable => false;
		public TypeParameterSubstitution Substitution => TypeParameterSubstitution.Identity;
		public IMember Specialize(TypeParameterSubstitution substitution) => throw new NotImplementedException();
		public ITypeDefinition? DeclaringTypeDefinition => null;
		public IType DeclaringType => null!;
		public Accessibility Accessibility => Accessibility.Public;
		public bool IsStatic => false;
		public bool IsAbstract => false;
		public bool IsSealed => false;
		public IEnumerable<IAttribute> GetAttributes() => throw new NotImplementedException();
		public bool HasAttribute(KnownAttribute attribute) => false;
		public IAttribute? GetAttribute(KnownAttribute attribute) => null;
	}
}
