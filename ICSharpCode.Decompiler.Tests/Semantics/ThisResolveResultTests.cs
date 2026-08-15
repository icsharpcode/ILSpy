// Copyright (c) 2026 Christoph Wille
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
using System.Linq;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.Tests.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Semantics
{
	// Sample hierarchy decompiled by ThisResolveResultTests: every way of naming the current
	// instance (unqualified member, explicit 'this', 'base', a struct's by-ref 'this') has to
	// end up annotated with the same 'this' parameter of the method being decompiled.
	internal class ThisReferenceBase
	{
		public virtual int Get() => 0;
	}

	internal class ThisReferenceSample : ThisReferenceBase
	{
		public int value;

		public ThisReferenceSample(int value)
		{
			// The parameter shadows the field, so this access keeps its qualifier.
			this.value = value;
		}

		public int Unqualified() => value;

		public override int Get() => base.Get();
	}

	internal struct ThisReferenceStructSample
	{
		public int value;

		public int Unqualified() => value;
	}

	[TestFixture]
	public class ThisResolveResultTests
	{
		// All samples live in this test assembly, so a single decompiler (and the PE file
		// shared with the other fixtures) serves every test here. CSharpDecompiler is not
		// safe for concurrent decompilations, so this fixture must stay non-parallelizable;
		// marking it [Parallelizable] requires a decompiler per test.
		static readonly Lazy<CSharpDecompiler> decompiler = new Lazy<CSharpDecompiler>(
			delegate {
				var module = TypeSystemLoaderTests.TestAssembly;
				var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
				return new CSharpDecompiler(module, resolver, new DecompilerSettings());
			});

		static SyntaxTree Decompile(System.Type type)
		{
			return decompiler.Value.DecompileType(new FullTypeName(type.FullName));
		}

		static ILVariable AssertIsThisParameter(ResolveResult rr, string what)
		{
			Assert.That(rr, Is.InstanceOf<ThisResolveResult>(), what);
			var variable = (rr as ILVariableResolveResult)?.Variable;
			Assert.That(variable, Is.Not.Null, what);
			Assert.That(variable.IsThis(), Is.True, what);
			return variable;
		}

		static ResolveResult ResolveResultOf(SyntaxTree tree, string methodName, System.Func<MethodDeclaration, Expression> select)
		{
			var method = tree.Descendants.OfType<MethodDeclaration>().Single(m => m.Name == methodName);
			return select(method).GetResolveResult();
		}

		[Test]
		public void ExplicitThisIsTheThisParameter()
		{
			var tree = Decompile(typeof(ThisReferenceSample));
			var ctor = tree.Descendants.OfType<ConstructorDeclaration>().Single();
			var thisRef = ctor.Descendants.OfType<ThisReferenceExpression>().Single();
			var variable = AssertIsThisParameter(thisRef.GetResolveResult(), "this");
			Assert.That(variable.Type.FullName, Is.EqualTo(typeof(ThisReferenceSample).FullName));
		}

		[Test]
		public void UnqualifiedFieldAccessTargetsTheThisParameter()
		{
			var tree = Decompile(typeof(ThisReferenceSample));
			var rr = ResolveResultOf(tree, "Unqualified", m => m.Descendants.OfType<IdentifierExpression>().Single(id => id.Identifier == "value"));
			var mrr = rr as MemberResolveResult;
			Assert.That(mrr, Is.Not.Null, "unqualified field access");
			AssertIsThisParameter(mrr.TargetResult, "target of unqualified field access");
		}

		[Test]
		public void BaseReferenceIsTheThisParameterWithTheBaseType()
		{
			var tree = Decompile(typeof(ThisReferenceSample));
			var rr = ResolveResultOf(tree, "Get", m => m.Descendants.OfType<BaseReferenceExpression>().Single());
			var variable = AssertIsThisParameter(rr, "base");
			Assert.That(rr.Type.FullName, Is.EqualTo(typeof(ThisReferenceBase).FullName));
			Assert.That(variable.Type.FullName, Is.EqualTo(typeof(ThisReferenceSample).FullName));
			Assert.That(((ThisResolveResult)rr).CausesNonVirtualInvocation, Is.True);
		}

		[Test]
		public void StructUnqualifiedFieldAccessTargetsTheByRefThisParameter()
		{
			var tree = Decompile(typeof(ThisReferenceStructSample));
			var rr = ResolveResultOf(tree, "Unqualified", m => m.Descendants.OfType<IdentifierExpression>().Single(id => id.Identifier == "value"));
			var mrr = rr as MemberResolveResult;
			Assert.That(mrr, Is.Not.Null, "unqualified field access");
			var variable = AssertIsThisParameter(mrr.TargetResult, "target of unqualified field access");
			Assert.That(variable.Type, Is.InstanceOf<ByReferenceType>());
			Assert.That(mrr.TargetResult.Type.Kind, Is.Not.EqualTo(TypeKind.ByReference), "the reference is spelled as the struct itself");
		}
	}
}
