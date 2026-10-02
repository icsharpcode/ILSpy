// Copyright (c) 2026 Piero Viano
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
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Analyzers;
using ICSharpCode.ILSpyX.Analyzers.Builtin;

using ICSharpCode.ILSpy.Languages;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Dependencies;

[TestFixture]
public class ModuleAnalyzersTests
{
	AssemblyList list = null!;
	LoadedAssembly library = null!;
	LoadedAssembly consumer = null!;
	string consumerName = null!;

	[OneTimeSetUp]
	public async Task Setup()
	{
		var (libraryPath, consumerPath, _, name) = DependencyFixtures.EmitPair("Analyze");
		consumerName = name;
		list = new AssemblyList();
		library = list.OpenAssembly(libraryPath);
		consumer = list.OpenAssembly(consumerPath);
		await library.GetLoadResultAsync();
		await consumer.GetLoadResultAsync();
		// An assembly that has nothing to do with the library must not contribute results.
		await list.OpenAssembly(FixtureAssembly.Emit(DependencyFixtures.UniqueName("Bystander"))).GetLoadResultAsync();
	}

	AnalyzerContext CreateContext(CancellationToken ct = default)
		=> new() { AssemblyList = list, Language = new CSharpLanguage(), CancellationToken = ct };

	IModule LibraryModule()
		=> new DecompilerTypeSystem(library.GetMetadataFileOrNull()!, library.GetAssemblyResolver()).MainModule;

	[Test]
	public void Module_Analyzers_Show_Only_For_Assemblies()
	{
		var module = LibraryModule();
		var type = module.TopLevelTypeDefinitions.First(t => t.Name == "Api");

		new ModuleDependentCodeAnalyzer().Show(module).Should().BeTrue();
		new ModuleDependentCodeAnalyzer().Show(type).Should().BeFalse();
		new ModuleDependentCodeAnalyzer().Show(null).Should().BeFalse();
		new ModuleReferencedByAnalyzer().Show(module).Should().BeTrue();
		new ModuleReferencedByAnalyzer().Show(type).Should().BeFalse();
	}

	[Test]
	public void Dependent_Code_Lists_Body_Signature_And_Base_Type_Users_Only()
	{
		var results = new ModuleDependentCodeAnalyzer().Analyze(LibraryModule(), CreateContext()).ToList();

		var names = results.OfType<IEntity>().Select(e => e.FullName).ToList();
		names.Should().Contain($"{consumerName}.{DependencyFixtures.BodyUserType}.{DependencyFixtures.BodyUserMethod}");
		names.Should().Contain($"{consumerName}.{DependencyFixtures.SignatureUserType}.{DependencyFixtures.SignatureUserMethod}");
		names.Should().Contain($"{consumerName}.{DependencyFixtures.DerivedType}");
		names.Should().NotContain(n => n.Contains(DependencyFixtures.IndependentType, StringComparison.Ordinal));
		results.OfType<IEntity>().Should().OnlyContain(e => e.ParentModule!.MetadataFile == consumer.GetMetadataFileOrNull(),
			"only assemblies that reference the library contribute");
		names.Should().OnlyHaveUniqueItems();
	}

	[Test]
	public void Dependent_Code_Honours_Cancellation()
	{
		using var cts = new CancellationTokenSource();
		cts.Cancel();

		var act = () => new ModuleDependentCodeAnalyzer().Analyze(LibraryModule(), CreateContext(cts.Token)).ToList();

		act.Should().Throw<OperationCanceledException>();
	}

	[Test]
	public void Referenced_By_Lists_The_Consumer_Module()
	{
		var results = new ModuleReferencedByAnalyzer().Analyze(LibraryModule(), CreateContext()).ToList();

		results.Should().ContainSingle().Which.Should().BeAssignableTo<IModule>()
			.Which.MetadataFile.Should().BeSameAs(consumer.GetMetadataFileOrNull());
	}
}
