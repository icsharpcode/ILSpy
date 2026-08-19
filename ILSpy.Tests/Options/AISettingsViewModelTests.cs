// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class AISettingsViewModelTests
{
	[Test]
	public async Task AddThenCancel_LeavesSavedStateSelectionAndSecureStorageUntouched()
	{
		var backend = new RecordingKeyBackend();
		var viewModel = CreateViewModel(backend, out AISettings settings);
		string savedXml = Serialize(settings);
		string activeProfileId = settings.ActiveProfileId;

		await InvokeAsync(viewModel, "AddProfileAsync");

		viewModel.AIProfileDraft.Should().NotBeNull();
		viewModel.AIProfileDraft!.Id.Should().NotBe(activeProfileId);
		settings.Profiles.Should().ContainSingle();
		settings.ActiveProfileId.Should().Be(activeProfileId);

		await InvokeAsync(viewModel, "CancelDraftAsync");

		viewModel.AIProfileDraft.Id.Should().Be(activeProfileId);
		Serialize(settings).Should().Be(savedXml);
		backend.Writes.Should().Be(0);
		backend.Deletes.Should().Be(0);
	}

	[Test]
	public async Task Duplicate_CopiesNonSecretFieldsWithFreshIdentityAndNoCredential()
	{
		var viewModel = CreateViewModel(new RecordingKeyBackend(), out AISettings settings);
		AIProfile saved = settings.ActiveProfile;
		saved.Name = "Work";
		saved.BaseUrl = "https://proxy.example.test";
		saved.Models.Add("gpt-4o-mini");
		saved.LastSelectedModel = "gpt-4o-mini";
		saved.HasStoredKey = true;
		viewModel.SelectedProfile = saved;

		await InvokeAsync(viewModel, "DuplicateProfileAsync");

		AIProfile duplicate = viewModel.AIProfileDraft!;
		duplicate.Id.Should().NotBe(saved.Id);
		duplicate.Name.Should().Be("Work Copy");
		duplicate.BaseUrl.Should().Be(saved.BaseUrl);
		duplicate.Models.Should().Equal(saved.Models);
		duplicate.LastSelectedModel.Should().Be(saved.LastSelectedModel);
		duplicate.HasStoredKey.Should().BeFalse();
		settings.Profiles.Should().ContainSingle();
	}

	[Test]
	public async Task Cancel_DiscardsDraftEditsAndTransientKeyInput()
	{
		var viewModel = CreateViewModel(new RecordingKeyBackend(), out AISettings settings);
		AIProfile saved = settings.ActiveProfile.Clone();

		viewModel.AIProfileDraft!.Name = "Unsaved";
		viewModel.AIProfileDraft.BaseUrl = "https://draft.example.test";
		viewModel.AIProfileDraft.Models[0] = "draft-model";
		viewModel.ApiKeyInput = "draft-secret";

		await InvokeAsync(viewModel, "CancelDraftAsync");

		viewModel.AIProfileDraft!.Name.Should().Be(saved.Name);
		viewModel.AIProfileDraft.BaseUrl.Should().Be(saved.BaseUrl);
		viewModel.AIProfileDraft.Models.Should().Equal(saved.Models);
		viewModel.ApiKeyInput.Should().BeEmpty();
		settings.ActiveProfile.Name.Should().Be(saved.Name);
	}

	[Test]
	public async Task RenameModel_PreservesOrderAndUpdatesRememberedSelection()
	{
		var viewModel = CreateViewModel(new RecordingKeyBackend(), out _);
		viewModel.AIProfileDraft!.Models.Add("gpt-4o-mini");
		viewModel.SelectedModel = "gpt-4o";
		viewModel.ModelNameInput = "  gpt-4.1  ";

		await InvokeAsync(viewModel, "RenameModelAsync");

		viewModel.AIProfileDraft.Models.Should().Equal("gpt-4.1", "gpt-4o-mini");
		viewModel.AIProfileDraft.LastSelectedModel.Should().Be("gpt-4.1");
		viewModel.SelectedModel.Should().Be("gpt-4.1");
	}

	[Test]
	public async Task Save_RejectsInvalidEndpointWithoutMutatingSavedProfile()
	{
		var viewModel = CreateViewModel(new RecordingKeyBackend(), out AISettings settings);
		string originalEndpoint = settings.ActiveProfile.BaseUrl;
		viewModel.AIProfileDraft!.BaseUrl = "api.example.test";

		await InvokeAsync(viewModel, "SaveDraftAsync");

		settings.ActiveProfile.BaseUrl.Should().Be(originalEndpoint);
		viewModel.StatusMessage.Should().Contain("endpoint");
	}

	[Test]
	public async Task SaveKeyFailure_PreservesMetadataAndDoesNotExposeKey()
	{
		var backend = new RecordingKeyBackend { FailOnSave = true };
		var viewModel = CreateViewModel(backend, out AISettings settings);
		string savedName = settings.ActiveProfile.Name;
		viewModel.AIProfileDraft!.Name = "Changed";
		viewModel.ApiKeyInput = "super-secret-test-key";

		await InvokeAsync(viewModel, "SaveKeyAsync");

		settings.ActiveProfile.Name.Should().Be(savedName);
		settings.ActiveProfile.HasStoredKey.Should().BeFalse();
		viewModel.AIProfileDraft.Name.Should().Be("Changed");
		viewModel.StatusMessage.Should().NotContain("super-secret-test-key");
		Serialize(settings).Should().NotContain("super-secret-test-key");
		backend.LastIdentifier.Should().Be(viewModel.AIProfileDraft.CredentialId);
	}

	[Test]
	public async Task TestConnection_UsesDraftSnapshotWithoutPersistingIt()
	{
		var backend = new RecordingKeyBackend();
		var factory = new RecordingProviderFactory();
		var viewModel = CreateViewModel(backend, out AISettings settings, factory);
		settings.PrivacyConsentAccepted = true;
		string savedXml = Serialize(settings);
		string activeProfileId = settings.ActiveProfileId;
		viewModel.AIProfileDraft!.Name = "Diagnostic draft";
		viewModel.AIProfileDraft.BaseUrl = "https://diagnostic.example.test";
		viewModel.AIProfileDraft.Models[0] = "diagnostic-model";
		viewModel.AIProfileDraft.LastSelectedModel = "diagnostic-model";
		viewModel.SelectedModel = "diagnostic-model";
		viewModel.ApiKeyInput = "diagnostic-key";

		await InvokeAsync(viewModel, "TestConnectionAsync");

		factory.Snapshot.Should().NotBeNull();
		factory.Snapshot!.ProfileId.Should().Be(activeProfileId);
		factory.Snapshot.ProfileName.Should().Be("Diagnostic draft");
		factory.Snapshot.Endpoint.Should().Be("https://diagnostic.example.test");
		factory.Snapshot.Model.Should().Be("diagnostic-model");
		factory.Snapshot.ApiKey.Should().Be("diagnostic-key");
		viewModel.StatusMessage.Should().Be("Connection succeeded for Diagnostic draft / diagnostic-model.");
		settings.ActiveProfileId.Should().Be(activeProfileId);
		Serialize(settings).Should().Be(savedXml);
	}

	static AISettingsViewModel CreateViewModel(
		RecordingKeyBackend backend,
		out AISettings settings,
		IAIProviderFactory? providerFactory = null)
	{
		settings = new AISettings();
		var viewModel = new AISettingsViewModel(
			providerFactory ?? new RecordingProviderFactory(),
			new SecureKeyStorage(backend));
		typeof(AISettingsViewModel).GetProperty(nameof(AISettingsViewModel.Settings))!
			.SetValue(viewModel, settings);
		typeof(AISettingsViewModel).GetProperty(nameof(AISettingsViewModel.AIProfileDraft))!
			.SetValue(viewModel, settings.ActiveProfile.Clone());
		return viewModel;
	}

	static async Task InvokeAsync(AISettingsViewModel viewModel, string methodName)
	{
		MethodInfo method = typeof(AISettingsViewModel).GetMethod(
			methodName, BindingFlags.Instance | BindingFlags.NonPublic)
			?? throw new InvalidOperationException($"Method '{methodName}' was not found.");
		await (Task)method.Invoke(viewModel, null)!;
	}

	static string Serialize(AISettings settings)
		=> settings.SaveToXml().ToString(SaveOptions.DisableFormatting);

	sealed class RecordingKeyBackend : ISecureKeyStorageBackend
	{
		readonly Dictionary<string, string> keys = new(StringComparer.Ordinal);

		public bool FailOnSave { get; init; }
		public int Writes { get; private set; }
		public int Deletes { get; private set; }
		public string? LastIdentifier { get; private set; }

		public Task SaveAsync(string provider, string key, CancellationToken cancellationToken)
		{
			LastIdentifier = provider;
			Writes++;
			if (FailOnSave)
				throw new SecureKeyStorageUnavailableException("test save failure");
			keys[provider] = key;
			return Task.CompletedTask;
		}

		public Task<SecureKeyStorageBackendReadResult> LoadAsync(string provider, CancellationToken cancellationToken)
		{
			LastIdentifier = provider;
			return Task.FromResult(keys.TryGetValue(provider, out string? key)
				? SecureKeyStorageBackendReadResult.Found(key)
				: SecureKeyStorageBackendReadResult.NotFound);
		}

		public Task DeleteAsync(string provider, CancellationToken cancellationToken)
		{
			LastIdentifier = provider;
			Deletes++;
			keys.Remove(provider);
			return Task.CompletedTask;
		}
	}

	sealed class RecordingProviderFactory : IAIProviderFactory
	{
		public AISelectionSnapshot? Snapshot { get; private set; }

		public Task<ILLMProvider> CreateAsync(AISelectionSnapshot snapshot, CancellationToken cancellationToken = default)
		{
			Snapshot = snapshot;
			return Task.FromResult<ILLMProvider>(new SuccessfulProvider());
		}
	}

	sealed class SuccessfulProvider : ILLMProvider
	{
		public async IAsyncEnumerable<string> CompleteAsync(
			LLMRequest request,
			[EnumeratorCancellation] CancellationToken cancellationToken)
		{
			await Task.CompletedTask;
			yield break;
		}

		public Task<bool> TestConnectionAsync(CancellationToken cancellationToken)
			=> Task.FromResult(true);
	}
}
