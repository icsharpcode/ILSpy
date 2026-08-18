// Copyright (c) 2026 Dr. Masroor Ehsan

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
			context.StringLiterals.Should().Contain("phase-two");
			context.Callers.Should().BeEmpty();
			context.Callees.Should().BeEmpty();
			context.IL.Should().BeNull();
			context.ApproximateTokenCount.Should().Be(TokenCounter.CountTokens(context.ToMarkdown(), true));
		}

		[Test]
		public void Build_ExtractsOptionalILLiteralsAndCallGraph()
		{
			using var module = OpenTestModule();
			CSharpDecompiler decompiler = CreateDecompiler(module);
			ITypeDefinition type = GetSampleType(decompiler);
			IMethod method = type.Methods.Single(m => m.Name == nameof(ContextSample.Method));

			var settings = new AISettings { SendIL = true, SendCallGraph = true, MaxContextTokens = 128000 };
			DecompilationContext context = new ContextBuilder(settings).Build(method, decompiler);

			context.IL.Should().Contain(".maxstack");
			context.Callees.Should().Contain(name => name.Contains(nameof(GC.KeepAlive), StringComparison.Ordinal));
			context.Callers.Should().Contain(name => name.Contains(nameof(ContextSample.Caller), StringComparison.Ordinal));

			IMethod literalMethod = type.Methods.Single(m => m.Name == nameof(ContextSample.Literal));
			DecompilationContext literalContext = new ContextBuilder(settings).Build(literalMethod, decompiler);
			literalContext.StringLiterals.Should().Contain("phase-two");
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
		public void TryFitCode_AccountsForMarkdownOverheadAndMarksTruncation()
		{
			string firstLine = new('a', 60);
			var context = new DecompilationContext {
				FullyQualifiedName = "Example.Type.Method",
				AssemblyName = "Example",
				Attributes = Enumerable.Range(1, 100).Select(i => "Example.Attribute" + i).ToArray(),
				DecompiledCSharp = firstLine + "\n" + new string('b', 200)
			};
			int budget = TokenCounter.CountTokens(
				(context with { DecompiledCSharp = firstLine + "..." }).ToMarkdown(),
				isCode: true);

			bool success = ContextBuilder.TryFitCode(context, budget, out DecompilationContext fitted);

			success.Should().BeTrue();
			fitted.DecompiledCSharp.Should().Be(firstLine + "...");
			fitted.ApproximateTokenCount.Should().BeLessThanOrEqualTo(budget);
		}

		[Test]
		public void FindStatementBoundary_IgnoresStringsAndComments()
		{
			const string code = "string value = \"; }\"; // } ;\nreturn value;";

			int cutoff = code.IndexOf('\n');
			ContextBuilder.FindStatementBoundary(code, cutoff).Should().Be(code.IndexOf("; //", StringComparison.Ordinal) + 1);
		}

		[Test]
		public void FindStatementBoundary_HandlesVerbatimAndRawStrings()
		{
			const string code = "var verbatim = @\"; }\"; var raw = \"\"\"; }\"\"\";";

			int expected = code.IndexOf("; var", StringComparison.Ordinal) + 1;
			ContextBuilder.FindStatementBoundary(code, code.Length).Should().Be(code.LastIndexOf(';') + 1);
			ContextBuilder.FindStatementBoundary(code, expected + 2).Should().Be(expected);
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
		public void ToMarkdown_UsesFenceLongerThanEmbeddedBackticks()
		{
			var context = new DecompilationContext {
				DecompiledCSharp = "string marker = \"```\";"
			};

			string markdown = context.ToMarkdown();

			markdown.Should().Contain("````csharp\nstring marker = \"```\";\n````");
		}

		[Test]
		public void ToMarkdown_EscapesControlCharactersInStringLiterals()
		{
			var context = new DecompilationContext {
				DecompiledCSharp = "class Example {}",
				StringLiterals = new[] { "first line\n- injected item" }
			};

			string markdown = context.ToMarkdown();

			markdown.Should().Contain("- \"first line\\n- injected item\"");
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

		public void Caller()
		{
			Method(1);
		}

		public string Literal() => "phase-two";

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
			return targetMethod?.Name switch {
				"get_ParentModule" => module,
				"get_MetadataToken" => handle,
				_ => throw new NotSupportedException(targetMethod?.Name)
			};
		}
	}

}
