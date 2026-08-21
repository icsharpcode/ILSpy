// Copyright (c) 2026 Dr. Masroor Ehsan

using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AI.Decompiler;
using ICSharpCode.ILSpy.AI;
using ICSharpCode.ILSpyX.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
	public class BatchRenameSuggesterTests
	{
		[Test]
		public void OrderMembers_VisitsLocalDependenciesBeforeCallers()
		{
			using var module = new PEFile(typeof(BatchRenameSample).Assembly.Location);
			var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
			var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
			ITypeDefinition type = decompiler.TypeSystem.MainModule.GetTypeDefinition(new FullTypeName(typeof(BatchRenameSample).FullName!));

			var methods = BatchRenameSuggester.OrderMembers(type)
				.OfType<IMethod>()
				.Where(method => method.Name is nameof(BatchRenameSample.Entry) or nameof(BatchRenameSample.Middle) or nameof(BatchRenameSample.Leaf))
				.Select(method => method.Name)
				.ToArray();

			methods.Should().ContainInOrder(nameof(BatchRenameSample.Leaf), nameof(BatchRenameSample.Middle), nameof(BatchRenameSample.Entry));
		}

		[Test]
		public async Task SuggestAsync_ReportsStructuredInitialAndFinalProgress()
		{
			using var module = new PEFile(typeof(BatchRenameSample).Assembly.Location);
			var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
			var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
			ITypeDefinition type = decompiler.TypeSystem.MainModule.GetTypeDefinition(new FullTypeName(typeof(BatchRenameSample).FullName!));
			var progress = new List<BatchRenameProgress>();
			var suggester = new BatchRenameSuggester(Snapshot(), new FakeFactory());

			IReadOnlyList<BatchRenameItem> items = await suggester.SuggestAsync(type, decompiler, new Progress<BatchRenameProgress>(progress.Add));

			items.Should().NotBeEmpty();
			progress[0].Completed.Should().Be(0);
			progress[0].Total.Should().Be(items.Count);
			progress[^1].Completed.Should().Be(progress[^1].Total);
			progress[^1].SkippedOrErrorCount.Should().Be(0);
		}

		static AISelectionSnapshot Snapshot() => new() { ProfileId = "p1", ProfileName = "Test", ProviderType = "openai", Endpoint = "https://example.test", Model = "test", ApiKey = "key", CredentialId = "cred" };

		sealed class FakeFactory : IAIProviderFactory
		{
			public Task<ILLMProvider> CreateAsync(AISelectionSnapshot snapshot, CancellationToken cancellationToken = default)
				=> Task.FromResult<ILLMProvider>(new FakeProvider());
		}

		sealed class FakeProvider : ILLMProvider
		{
			public async IAsyncEnumerable<string> CompleteAsync(LLMRequest request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
			{
				yield return "[{\"name\":\"RenamedMember\",\"confidence\":0.9,\"reasoning\":\"clear\"}]";
				await Task.CompletedTask;
			}

			public Task<bool> TestConnectionAsync(CancellationToken cancellationToken) => Task.FromResult(true);
		}
	}

	sealed class BatchRenameSample
	{
		public void method_1()
		{
		}

		public void Entry() => Middle();

		public void Middle() => Leaf();

		public void Leaf()
		{
		}
	}
}
