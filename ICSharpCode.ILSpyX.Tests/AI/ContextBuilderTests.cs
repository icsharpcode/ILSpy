// Copyright (c) 2026 Masroor
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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using AwesomeAssertions;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
	public class ContextBuilderTests
	{
		[Test]
		public void Build_ExtractsCodeAndMetadata()
		{
			using var module = OpenTestModule();
			CSharpDecompiler decompiler = CreateDecompiler(module);
			ITypeDefinition type = GetSampleType(decompiler);

			DecompilationContext context = new ContextBuilder(new AISettings()).Build(type, decompiler);

			context.DecompiledCSharp.Should().Contain(nameof(ContextSample.Method));
			context.FullyQualifiedName.Should().Be(type.FullName);
			context.AssemblyName.Should().Be(type.ParentModule!.AssemblyName);
			context.TargetFramework.Should().Be(module.Metadata.DetectTargetFrameworkId());
			context.Attributes.Should().Contain(typeof(SerializableAttribute).FullName);
			context.ImplementedInterfaces.Should().Contain(typeof(IDisposable).FullName);
			context.StringLiterals.Should().BeEmpty();
			context.Callers.Should().BeEmpty();
			context.Callees.Should().BeEmpty();
			context.IL.Should().BeNull();
			context.ApproximateTokenCount.Should().Be(TokenCounter.CountTokens(context.ToMarkdown(), true));
		}

		[Test]
		public void Build_RequiresTheDecompilerMainModuleInstance()
		{
			using var firstModule = OpenTestModule();
			using var secondModule = OpenTestModule();
			CSharpDecompiler firstDecompiler = CreateDecompiler(firstModule);
			CSharpDecompiler secondDecompiler = CreateDecompiler(secondModule);
			ITypeDefinition firstType = GetSampleType(firstDecompiler);
			var builder = new ContextBuilder(new AISettings());

			Action action = () => builder.Build(firstType, secondDecompiler);

			action.Should().Throw<ArgumentException>();
		}

		[Test]
		public void Build_RejectsUnsupportedMetadataHandleBeforeDecompilation()
		{
			using var module = OpenTestModule();
			CSharpDecompiler decompiler = CreateDecompiler(module);
			IEntity entity = EntityProxy.Create(
				decompiler.TypeSystem.MainModule,
				MetadataTokens.ParameterHandle(1));
			var builder = new ContextBuilder(new AISettings());

			Action action = () => builder.Build(entity, decompiler);

			action.Should().Throw<ArgumentException>().Which.Message.Should().Contain("Parameter");
		}

		[Test]
		public void Build_TinyBudgetMatchesRenderedMarkdownTokenCount()
		{
			using var module = OpenTestModule();
			CSharpDecompiler decompiler = CreateDecompiler(module);
			var settings = new AISettings { MaxContextTokens = 1 };

			DecompilationContext context = new ContextBuilder(settings).Build(GetSampleType(decompiler), decompiler);

			context.ApproximateTokenCount.Should().Be(TokenCounter.CountTokens(context.ToMarkdown(), true));
			context.ApproximateTokenCount.Should().BeLessThanOrEqualTo(settings.MaxContextTokens);
		}

		[Test]
		public void GetUnicodeSafePrefixLength_DoesNotSplitSurrogatePair()
		{
			const string text = "A\ud83d\ude00B";

			ContextBuilder.GetUnicodeSafePrefixLength(text, 2).Should().Be(1);
			ContextBuilder.GetUnicodeSafePrefixLength(text, 3).Should().Be(3);
		}

		[Test]
		public void ToMarkdown_LimitsStringLiteralsToTwenty()
		{
			var context = new DecompilationContext {
				DecompiledCSharp = "class Example {}",
				StringLiterals = Enumerable.Range(1, 21).Select(i => "literal-" + i).ToArray()
			};

			string markdown = context.ToMarkdown();

			markdown.Should().Contain("literal-20");
			markdown.Should().NotContain("literal-21");
			markdown.Should().Contain("... and 1 more");
		}

		[Test]
		public void ToMarkdown_ProducesStructuredOutput()
		{
			var context = new DecompilationContext {
				FullyQualifiedName = "Example.Type.Method",
				AssemblyName = "Example",
				TargetFramework = ".NETCoreApp,Version=v10.0",
				DecompiledCSharp = "public void Method() { }",
				Attributes = new[] { "System.ObsoleteAttribute" },
				ImplementedInterfaces = new[] { "System.IDisposable" }
			};

			string markdown = context.ToMarkdown();

			markdown.Should().Contain("# Example.Type.Method");
			markdown.Should().Contain("**Assembly:** Example");
			markdown.Should().Contain("```csharp");
			markdown.Should().Contain("public void Method() { }");
			markdown.Should().Contain("System.ObsoleteAttribute");
			markdown.Should().Contain("System.IDisposable");
		}

		static PEFile OpenTestModule()
		{
			return new PEFile(typeof(ContextBuilderTests).Assembly.Location);
		}

		static CSharpDecompiler CreateDecompiler(PEFile module)
		{
			var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
			return new CSharpDecompiler(module, resolver, new DecompilerSettings());
		}

		static ITypeDefinition GetSampleType(CSharpDecompiler decompiler)
		{
			return decompiler.TypeSystem.MainModule.GetTypeDefinition(new FullTypeName(typeof(ContextSample).FullName!));
		}
	}

	[Serializable]
	internal sealed class ContextSample : IDisposable
	{
		public void Method(int value)
		{
			GC.KeepAlive(value);
		}

		public void Dispose()
		{
		}
	}

	class EntityProxy : DispatchProxy
	{
		IModule? module;
		EntityHandle handle;

		public static IEntity Create(IModule module, EntityHandle handle)
		{
			IEntity entity = Create<IEntity, EntityProxy>();
			var proxy = (EntityProxy)(object)entity;
			proxy.module = module;
			proxy.handle = handle;
			return entity;
		}

		protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
		{
			return targetMethod?.Name switch
			{
				"get_ParentModule" => module,
				"get_MetadataToken" => handle,
				_ => throw new NotSupportedException(targetMethod?.Name)
			};
		}
	}

}
