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
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Analyzers;
using ICSharpCode.ILSpyX.Analyzers.Builtin;

using ICSharpCode.ILSpy.Languages;

using NSubstitute;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers.Library;

[TestFixture]
public class MemberOverridesAnalyzerTests
{
	static readonly SymbolKind[] ValidSymbolKinds = { SymbolKind.Event, SymbolKind.Indexer, SymbolKind.Method, SymbolKind.Property };
	static readonly SymbolKind[] InvalidSymbolKinds =
		Enum.GetValues(typeof(SymbolKind)).Cast<SymbolKind>().Except(ValidSymbolKinds).ToArray();

	ICompilation testAssembly = null!;

	[OneTimeSetUp]
	public void Setup()
	{
		var fileName = GetType().Assembly.Location;
		using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
		var module = new PEFile(fileName, stream, PEStreamOptions.PrefetchEntireImage, MetadataReaderOptions.None);
		testAssembly = new SimpleCompilation(module.WithOptions(TypeSystemOptions.Default), MinimalCorlib.Instance);
	}

	[Test]
	public void VerifyDoesNotShowForNoSymbol()
	{
		var analyzer = new MemberOverridesAnalyzer();
		var shouldShow = analyzer.Show(symbol: null!);
		Assert.That(!shouldShow, "The analyzer will be unexpectedly shown for no symbol");
	}

	[Test]
	[TestCaseSource(nameof(InvalidSymbolKinds))]
	public void VerifyDoesNotShowForNonMembers(SymbolKind symbolKind)
	{
		var symbolMock = Substitute.For<ISymbol>();
		symbolMock.SymbolKind.Returns(symbolKind);
		var analyzer = new MemberOverridesAnalyzer();
		var shouldShow = analyzer.Show(symbolMock);
		Assert.That(!shouldShow, $"The analyzer will be unexpectedly shown for symbol '{symbolKind}'");
	}

	[Test]
	[TestCaseSource(nameof(ValidSymbolKinds))]
	public void VerifyDoesNotShowForNonOverrideMembers(SymbolKind symbolKind)
	{
		var memberMock = SetupMemberMock(symbolKind, isOverride: false);
		var analyzer = new MemberOverridesAnalyzer();
		var shouldShow = analyzer.Show(memberMock);
		Assert.That(!shouldShow, $"The analyzer will be unexpectedly shown for non-override symbol '{symbolKind}'");
	}

	[Test]
	[TestCaseSource(nameof(ValidSymbolKinds))]
	public void VerifyShowsForOverrideMembers(SymbolKind symbolKind)
	{
		var memberMock = SetupMemberMock(symbolKind, isOverride: true);
		var analyzer = new MemberOverridesAnalyzer();
		var shouldShow = analyzer.Show(memberMock);
		Assert.That(shouldShow, $"The analyzer will not be shown for override symbol '{symbolKind}'");
	}

	[Test]
	public void VerifyReturnsAllOverriddenBaseMembers()
	{
		var symbol = SetupMethodForAnalysis(typeof(LeafClass), nameof(LeafClass.TestMethod));
		var analyzer = new MemberOverridesAnalyzer();

		var results = analyzer.Analyze(symbol, CreateContext()).OfType<IMethod>().ToList();

		// The analyzer walks the override chain nearest-base-first; the order is what
		// the analyzer panel displays, so assert it.
		Assert.That(results.Select(r => r.DeclaringTypeDefinition!.Name),
			Is.EqualTo(new[] { nameof(MiddleClass), nameof(GrandBaseClass) }));
	}

	[Test]
	public void VerifyDoesNotReturnInterfaceMembers()
	{
		// Interface members are the "Implements" analysis; "Overrides" only walks base classes.
		var symbol = SetupMethodForAnalysis(typeof(LeafClass), nameof(LeafClass.TestMethod));
		var analyzer = new MemberOverridesAnalyzer();

		var results = analyzer.Analyze(symbol, CreateContext()).OfType<IMethod>().ToList();

		Assert.That(results.Select(r => r.DeclaringTypeDefinition!.Kind),
			Has.All.Not.EqualTo(TypeKind.Interface));
	}

	[Test]
	public void VerifyReturnsOverriddenProperty()
	{
		var typeDefinition = testAssembly.FindType(typeof(LeafClass)).GetDefinition();
		var symbol = typeDefinition!.Properties.First(p => p.Name == nameof(LeafClass.TestProperty));
		var analyzer = new MemberOverridesAnalyzer();

		var results = analyzer.Analyze(symbol, CreateContext()).OfType<IProperty>().ToList();

		Assert.That(results.Select(r => r.DeclaringTypeDefinition!.Name),
			Is.EquivalentTo(new[] { nameof(GrandBaseClass) }));
	}

	[Test]
	public void VerifyDoesNotReturnShadowedNonVirtualMembers()
	{
		// ShadowBase.ShadowedMethod is unrelated to the override chain that starts
		// at ShadowMiddle's 'new virtual' declaration; it must not appear.
		var symbol = SetupMethodForAnalysis(typeof(ShadowLeaf), nameof(ShadowLeaf.ShadowedMethod));
		var analyzer = new MemberOverridesAnalyzer();

		var results = analyzer.Analyze(symbol, CreateContext()).OfType<IMethod>().ToList();

		Assert.That(results.Select(r => r.DeclaringTypeDefinition!.Name),
			Is.EquivalentTo(new[] { nameof(ShadowMiddle) }));
	}

	static AnalyzerContext CreateContext()
	{
		return new AnalyzerContext {
			AssemblyList = new AssemblyList(),
			Language = new CSharpLanguage(),
		};
	}

	ISymbol SetupMethodForAnalysis(Type type, string methodName)
	{
		var typeDefinition = testAssembly.FindType(type).GetDefinition();
		return typeDefinition!.Methods.First(m => m.Name == methodName);
	}

	static IMember SetupMemberMock(SymbolKind symbolKind, bool isOverride)
	{
		var memberMock = Substitute.For<IMember>();
		memberMock.SymbolKind.Returns(symbolKind);
		memberMock.IsOverride.Returns(isOverride);
		return memberMock;
	}

	interface ITestInterface
	{
		void TestMethod();
	}

	class GrandBaseClass
	{
		public virtual void TestMethod() => throw new NotImplementedException();
		public virtual int TestProperty => throw new NotImplementedException();
	}

	class MiddleClass : GrandBaseClass
	{
		public override void TestMethod() => throw new NotImplementedException();
	}

	class LeafClass : MiddleClass, ITestInterface
	{
		public override void TestMethod() => throw new NotImplementedException();
		public override int TestProperty => throw new NotImplementedException();
	}

	class ShadowBase
	{
		public void ShadowedMethod() => throw new NotImplementedException();
	}

	class ShadowMiddle : ShadowBase
	{
		public new virtual void ShadowedMethod() => throw new NotImplementedException();
	}

	class ShadowLeaf : ShadowMiddle
	{
		public override void ShadowedMethod() => throw new NotImplementedException();
	}
}
