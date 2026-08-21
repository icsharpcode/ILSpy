// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AI;
using ICSharpCode.ILSpyX.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
	public class RenameSuggesterTests
	{
		[TestCase(null, false)]
		[TestCase("", false)]
		[TestCase("a", true)]
		[TestCase("b1", true)]
		[TestCase("method_47", true)]
		[TestCase("class", false)]
		[TestCase("ProcessPayment", false)]
		public void IsLikelyObfuscated_RecognizesShortAndGeneratedNames(string? name, bool expected)
		{
			RenameSuggester.IsLikelyObfuscated(name).Should().Be(expected);
		}

		[Test]
		public void ParseSuggestions_StripsFencesAndDeduplicatesNames()
		{
			string response = "```json\n[\n  {\"name\":\"ProcessPayment\",\"confidence\":0.91,\"reasoning\":\"clear\"},\n  {\"name\":\"ProcessPayment\",\"confidence\":0.5,\"reasoning\":\"duplicate\"},\n  {\"name\":\"class\",\"confidence\":0.1,\"reasoning\":\"keyword\"}\n]\n```";

			var suggestions = RenameSuggester.ParseSuggestions(response);

			suggestions.Should().HaveCount(1);
			suggestions[0].Name.Should().Be("ProcessPayment");
			suggestions[0].Confidence.Should().Be(0.91);
		}

		[Test]
		public void ParseSuggestions_RejectsInvalidJson()
		{
			var action = () => RenameSuggester.ParseSuggestions("not json");

			action.Should().Throw<RenameSuggestionParseException>();
		}

		[TestCase(0.59, 59)]
		[TestCase(0.60, 60)]
		[TestCase(1.0, 100)]
		[TestCase(59, 59)]
		public void ConfidencePercent_NormalizesProviderValues(double confidence, int expected)
		{
			new RenameSuggestion("GoodName", confidence, "reason").ConfidencePercent.Should().Be(expected);
		}

		[Test]
		public async Task SuggestAsync_WithNamingHint_IncludesHintInPrompt()
		{
			CSharpDecompiler decompiler = CreateDecompiler();
			IMethod method = GetSampleMethod(decompiler);
			var provider = new FakeProvider("[{\"name\":\"ParseHeader\",\"confidence\":0.9,\"reasoning\":\"reads the header\"}]");
			var suggester = new RenameSuggester(Snapshot(), new FakeFactory(provider));

			IReadOnlyList<RenameSuggestion> suggestions = await suggester.SuggestAsync(method, decompiler, additionalContext: null, namingHint: "prefer a Header prefix");

			suggestions.Should().ContainSingle();
			suggestions[0].Name.Should().Be("ParseHeader");
			provider.LastRequest!.Messages.Should().ContainSingle();
			provider.LastRequest.Messages[0].Content.Should().Contain("Naming hint from the user: prefer a Header prefix");
		}

		[Test]
		public void SuggestAsync_WithNamingHint_StillRejectsNonObfuscatedNames()
		{
			CSharpDecompiler decompiler = CreateDecompiler();
			ITypeDefinition type = decompiler.TypeSystem.MainModule.GetTypeDefinition(new FullTypeName(typeof(RenameSuggesterTests).FullName!));
			var suggester = new RenameSuggester(Snapshot(), new FakeFactory(new FakeProvider("[]")));

			Func<Task> action = () => suggester.SuggestAsync(type, decompiler, additionalContext: null, namingHint: "anything");

			action.Should().ThrowAsync<ArgumentException>();
		}

		static AISelectionSnapshot Snapshot() => new() { ProfileId = "p1", ProfileName = "Test", ProviderType = "openai", Endpoint = "https://example.test", Model = "test", ApiKey = "key", CredentialId = "cred" };

		static CSharpDecompiler CreateDecompiler()
		{
			var module = new PEFile(typeof(RenameSuggesterTests).Assembly.Location);
			var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
			return new CSharpDecompiler(module, resolver, new DecompilerSettings());
		}

		static IMethod GetSampleMethod(CSharpDecompiler decompiler)
		{
			ITypeDefinition type = decompiler.TypeSystem.MainModule.GetTypeDefinition(new FullTypeName(typeof(RenameSuggestSample).FullName!));
			return type.Methods.Single(method => method.Name == nameof(RenameSuggestSample.method_5));
		}

		sealed class FakeFactory : IAIProviderFactory
		{
			readonly ILLMProvider provider;
			public FakeFactory(ILLMProvider provider) => this.provider = provider;
			public Task<ILLMProvider> CreateAsync(AISelectionSnapshot snapshot, CancellationToken cancellationToken = default)
				=> Task.FromResult(provider);
		}

		sealed class FakeProvider : ILLMProvider
		{
			readonly string response;
			public FakeProvider(string response) => this.response = response;
			public LLMRequest? LastRequest { get; private set; }

			public async IAsyncEnumerable<string> CompleteAsync(LLMRequest request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
			{
				LastRequest = request;
				yield return response;
				await Task.CompletedTask;
			}

			public Task<bool> TestConnectionAsync(CancellationToken cancellationToken) => Task.FromResult(true);
		}
	}

	sealed class RenameSuggestSample
	{
		public void method_5()
		{
		}
	}
}
