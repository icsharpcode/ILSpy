// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

using Avalonia.Threading;

using System.Composition;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.Options
{
	[ExportOptionPage(Order = 40)]
	public sealed class AISettingsViewModel : ObservableObject, IOptionPage, IDisposable
	{
		readonly IAIProviderFactory providerFactory;
		readonly SecureKeyStorage keyStorage;
		AISelectionService? selectionService;
		AISettings settings = null!;
		AIProfile? draft;
		string apiKeyInput = string.Empty;
		string statusMessage = string.Empty;
		bool isTestingConnection;
		CancellationTokenSource? testCancellation;

		[ImportingConstructor]
		public AISettingsViewModel(IAIProviderFactory providerFactory, SecureKeyStorage keyStorage)
		{
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
			this.keyStorage = keyStorage ?? throw new ArgumentNullException(nameof(keyStorage));
			Providers = AIProviderCatalog.All.Select(provider => provider.Id).ToArray();
			SaveKeyCommand = new AsyncCommand(SaveKeyAsync, () => Settings is not null);
			ClearKeyCommand = new AsyncCommand(ClearKeyAsync, () => Settings is not null);
			TestConnectionCommand = new AsyncCommand(TestConnectionAsync, () => CanTestConnection);
			CancelTestConnectionCommand = new AsyncCommand(CancelTestConnectionAsync, () => IsTestingConnection);
		}

		public string Title => "AI Assistant";

		public IReadOnlyList<string> Providers { get; }
		public IReadOnlyList<AIProfile> Profiles => Settings is null ? Array.Empty<AIProfile>() : Settings.Profiles;
		public AIProfile? SelectedProfile {
			get => AIProfileDraft;
			set { if (value is not null) { AIProfileDraft = value.Clone(); OnPropertyChanged(); } }
		}
		public ICommand AddProfileCommand => new AsyncCommand(AddProfileAsync, () => Settings is not null);
		public ICommand DuplicateProfileCommand => new AsyncCommand(DuplicateProfileAsync, () => AIProfileDraft is not null);
		public ICommand DeleteProfileCommand => new AsyncCommand(DeleteProfileAsync, () => AIProfileDraft is not null && Profiles.Count > 1);
		public ICommand SaveCommand => new AsyncCommand(SaveDraftAsync, () => AIProfileDraft is not null);
		public ICommand CancelCommand => new AsyncCommand(CancelDraftAsync, () => AIProfileDraft is not null);
		public ICommand MoveProfileUpCommand => new AsyncCommand(() => MoveProfileAsync(-1), () => AIProfileDraft is not null);
		public ICommand MoveProfileDownCommand => new AsyncCommand(() => MoveProfileAsync(1), () => AIProfileDraft is not null);
		public ICommand AddModelCommand => new AsyncCommand(AddModelAsync, () => AIProfileDraft is not null);
		public ICommand DeleteModelCommand => new AsyncCommand(DeleteModelAsync, () => AIProfileDraft is not null && AIProfileDraft.Models.Count > 1);
		public ICommand MoveModelUpCommand => new AsyncCommand(() => MoveModelAsync(-1), () => AIProfileDraft is not null);
		public ICommand MoveModelDownCommand => new AsyncCommand(() => MoveModelAsync(1), () => AIProfileDraft is not null);
		public string ModelNameInput { get; set; } = string.Empty;
		public string SelectedModel { get; set; } = string.Empty;

		public AISettings Settings {
			get => settings;
			private set {
				if (ReferenceEquals(settings, value))
					return;
				if (settings is not null)
					settings.PropertyChanged -= SettingsPropertyChanged;
				settings = value;
				settings.PropertyChanged += SettingsPropertyChanged;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasConfiguredKey));
				OnPropertyChanged(nameof(CanTestConnection));
				TestConnectionCommand.RaiseCanExecuteChanged();
				CancelTestConnectionCommand.RaiseCanExecuteChanged();
			}
		}

		/// <summary>Unsaved non-secret profile draft used by the editor.</summary>
		public AIProfile? AIProfileDraft {
			get => draft;
			private set => SetProperty(ref draft, value);
		}

		public string ApiKeyInput {
			get => apiKeyInput;
			set {
				if (!SetProperty(ref apiKeyInput, value ?? string.Empty))
					return;
				// Secret input remains transient. It is written only by an explicit save operation.
			}
		}

		public string StatusMessage {
			get => statusMessage;
			private set => SetProperty(ref statusMessage, value);
		}

		public bool IsTestingConnection {
			get => isTestingConnection;
			private set {
				if (!SetProperty(ref isTestingConnection, value))
					return;
				OnPropertyChanged(nameof(CanTestConnection));
				TestConnectionCommand.RaiseCanExecuteChanged();
				CancelTestConnectionCommand.RaiseCanExecuteChanged();
			}
		}

		public bool HasConfiguredKey => AIProfileDraft?.HasStoredKey == true
			|| !string.IsNullOrWhiteSpace(ApiKeyInput);

		public bool CanTestConnection {
			get {
				if (Settings is null || IsTestingConnection || !Settings.PrivacyConsentAccepted)
					return false;
				AIProfile profile = AIProfileDraft ?? Settings.ActiveProfile;
				if (!AISettings.IsSupportedProvider(profile.ProviderType)
					|| string.IsNullOrWhiteSpace(profile.BaseUrl)
					|| string.IsNullOrWhiteSpace(profile.ResolveModel()))
					return false;
				return AIProviderCatalog.Get(profile.ProviderType).KeyRequirement == AIProviderKeyRequirement.None || HasConfiguredKey;
			}
		}

		public ICommand SaveKeyCommand { get; }
		public ICommand ClearKeyCommand { get; }
		public AsyncCommand TestConnectionCommand { get; }
		public AsyncCommand CancelTestConnectionCommand { get; }

		public void Load(SettingsService service)
		{
			ArgumentNullException.ThrowIfNull(service);
			Settings = service.AISettings;
			selectionService = AppComposition.TryGetExport<AISelectionService>();
			AIProfileDraft = Settings.ActiveProfile.Clone();
			ApiKeyInput = string.Empty;
			StatusMessage = string.Empty;
			_ = LoadStoredKeyAsync(Settings, CancellationToken.None);
		}

		async Task AddProfileAsync()
		{
			AIProfile profile = AIProfile.Create(AIProviderCatalog.Get("openai"));
			profile.Name = MakeUniqueName("New profile");
			AIProfileDraft = profile;
			await Task.CompletedTask;
		}

		async Task DuplicateProfileAsync()
		{
			if (AIProfileDraft is null)
				return;
			AIProfileDraft = AIProfileDraft.Duplicate();
			AIProfileDraft.Name = MakeUniqueName(AIProfileDraft.Name + " Copy");
			await Task.CompletedTask;
		}

		async Task DeleteProfileAsync()
		{
			if (AIProfileDraft is null || Settings.Profiles.Count <= 1)
				return;
			AIProfile? existing = Settings.Profiles.FirstOrDefault(p => p.Id == AIProfileDraft.Id);
			if (existing is not null && selectionService is not null)
				await selectionService.DeleteProfileAsync(existing.Id);
			AIProfileDraft = Settings.ActiveProfile.Clone();
			OnPropertyChanged(nameof(Profiles));
		}

		async Task SaveDraftAsync()
		{
			if (AIProfileDraft is null)
				return;
			await CommitDraftAsync(AIProfileDraft.Clone(), null, false);
		}

		async Task CancelDraftAsync() { AIProfileDraft = Settings.ActiveProfile.Clone(); await Task.CompletedTask; }

		async Task MoveProfileAsync(int delta)
		{
			if (AIProfileDraft is not null && selectionService is not null)
				await selectionService.MoveProfileAsync(AIProfileDraft.Id, delta);
			OnPropertyChanged(nameof(Profiles));
		}

		async Task AddModelAsync()
		{
			if (AIProfileDraft is null)
				return;
			string model = ModelNameInput.Trim();
			if (model.Length == 0)
			{ StatusMessage = "Enter a model name."; return; }
			if (AIProfileDraft.Models.Any(m => string.Equals(m, model, StringComparison.OrdinalIgnoreCase)))
			{ StatusMessage = "That model is already listed."; return; }
			AIProfileDraft.Models.Add(model);
			if (string.IsNullOrWhiteSpace(AIProfileDraft.LastSelectedModel))
				AIProfileDraft.LastSelectedModel = model;
			ModelNameInput = string.Empty;
			OnPropertyChanged(nameof(AIProfileDraft));
			await Task.CompletedTask;
		}

		async Task DeleteModelAsync()
		{
			if (AIProfileDraft is null || AIProfileDraft.Models.Count <= 1)
				return;
			int index = AIProfileDraft.Models.FindIndex(m => string.Equals(m, SelectedModel, StringComparison.OrdinalIgnoreCase));
			if (index < 0)
				return;
			AIProfileDraft.Models.RemoveAt(index);
			if (!AIProfileDraft.Models.Contains(AIProfileDraft.LastSelectedModel, StringComparer.OrdinalIgnoreCase))
				AIProfileDraft.LastSelectedModel = AIProfileDraft.Models[0];
			SelectedModel = AIProfileDraft.LastSelectedModel;
			OnPropertyChanged(nameof(AIProfileDraft));
			await Task.CompletedTask;
		}

		async Task MoveModelAsync(int delta)
		{
			if (AIProfileDraft is null)
				return;
			int index = AIProfileDraft.Models.FindIndex(m => string.Equals(m, SelectedModel, StringComparison.OrdinalIgnoreCase));
			int target = index + Math.Sign(delta);
			if (index < 0 || target < 0 || target >= AIProfileDraft.Models.Count)
				return;
			(AIProfileDraft.Models[index], AIProfileDraft.Models[target]) = (AIProfileDraft.Models[target], AIProfileDraft.Models[index]);
			OnPropertyChanged(nameof(AIProfileDraft));
			await Task.CompletedTask;
		}

		string MakeUniqueName(string proposed)
		{
			string name = proposed.Trim();
			int suffix = 2;
			while (Profiles.Any(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)))
				name = proposed.Trim() + " " + suffix++;
			return name;
		}

		public void LoadDefaults()
		{
			Settings.LoadFromXml(null!);
			AIProfileDraft = Settings.ActiveProfile.Clone();
			ApiKeyInput = string.Empty;
			StatusMessage = string.Empty;
			OnPropertyChanged(nameof(HasConfiguredKey));
			OnPropertyChanged(nameof(CanTestConnection));
			TestConnectionCommand.RaiseCanExecuteChanged();
		}

		async Task LoadStoredKeyAsync(AISettings target, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(target.ApiKeyPlaceholder))
				return;
			try
			{
				SecureKeyLookupResult result = await keyStorage.TryLoadKeyAsync(target.ActiveProfile.CredentialId, cancellationToken);
				if (result.Status == SecureKeyLookupStatus.Found)
				{
					await Dispatcher.UIThread.InvokeAsync(() => {
						if (ReferenceEquals(Settings, target))
						{
							target.ActiveProfile.HasStoredKey = true;
							OnPropertyChanged(nameof(HasConfiguredKey));
							OnPropertyChanged(nameof(CanTestConnection));
							TestConnectionCommand.RaiseCanExecuteChanged();
						}
					});
				}
				else if (result.Status == SecureKeyLookupStatus.Unavailable)
					await Dispatcher.UIThread.InvokeAsync(() => StatusMessage = "Secure API-key storage is unavailable.");
			}
			catch (Exception)
			{
				await Dispatcher.UIThread.InvokeAsync(() => StatusMessage = "Unable to load the configured API key from secure storage.");
			}
		}

		async Task SaveKeyAsync()
		{
			if (string.IsNullOrWhiteSpace(ApiKeyInput))
			{
				await ClearKeyAsync();
				return;
			}
			if (AIProfileDraft is not null)
				await CommitDraftAsync(AIProfileDraft.Clone(), ApiKeyInput, true);
		}

		async Task ClearKeyAsync()
		{
			if (AIProfileDraft is not null)
				await CommitDraftAsync(AIProfileDraft.Clone(), null, false, removeKey: true);
		}

		async Task CommitDraftAsync(AIProfile draftProfile, string? replacementKey, bool replaceKey, bool removeKey = false)
		{
			draftProfile.Normalize();
			IReadOnlyList<string> errors = draftProfile.Validate();
			if (errors.Count != 0)
			{
				StatusMessage = string.Join(" ", errors);
				return;
			}

			AIProfile? previous = Settings.Profiles.FirstOrDefault(p => p.Id == draftProfile.Id);
			SecureKeyLookupResult oldKey = SecureKeyLookupResult.NotFound;
			if (replaceKey || removeKey)
			{
				if (previous?.HasStoredKey == true)
					oldKey = await keyStorage.TryLoadKeyAsync(draftProfile.CredentialId, CancellationToken.None);
				if (oldKey.Status == SecureKeyLookupStatus.Unavailable)
				{
					StatusMessage = "Secure API-key storage is unavailable.";
					return;
				}
			}

			try
			{
				if (replaceKey)
				{
					await keyStorage.SaveKeyAsync(draftProfile.CredentialId, replacementKey!, CancellationToken.None);
					draftProfile.HasStoredKey = true;
				}
				else if (removeKey)
				{
					await keyStorage.DeleteKeyAsync(draftProfile.CredentialId, CancellationToken.None);
					draftProfile.HasStoredKey = false;
				}

				if (selectionService is not null)
					await selectionService.SaveProfileAsync(draftProfile.Clone());
				else
				{
					if (previous is null)
						Settings.Profiles.Add(draftProfile.Clone());
					else
						Settings.Profiles[Settings.Profiles.IndexOf(previous)] = draftProfile.Clone();
					Settings.NotifyProfilesChanged();
				}

				AIProfileDraft = draftProfile.Clone();
				ApiKeyInput = string.Empty;
				StatusMessage = replaceKey ? "API key saved in secure storage." : removeKey ? "API key removed." : "Profile saved.";
				OnPropertyChanged(nameof(Profiles));
				OnPropertyChanged(nameof(HasConfiguredKey));
				OnPropertyChanged(nameof(CanTestConnection));
				TestConnectionCommand.RaiseCanExecuteChanged();
			}
			catch (Exception)
			{
				try
				{
					if (oldKey.Status == SecureKeyLookupStatus.Found)
						await keyStorage.SaveKeyAsync(draftProfile.CredentialId, oldKey.Value!, CancellationToken.None);
					else if (replaceKey || removeKey)
						await keyStorage.DeleteKeyAsync(draftProfile.CredentialId, CancellationToken.None);
				}
				catch (Exception) { }
				StatusMessage = "Unable to save the profile or API key. Previous saved state was retained where possible.";
			}
		}

		async Task CancelTestConnectionAsync()
		{
			testCancellation?.Cancel();
			await Task.CompletedTask;
		}

		async Task TestConnectionAsync()
		{
			if (!CanTestConnection)
				return;
			using var cancellation = new CancellationTokenSource();
			testCancellation = cancellation;
			IsTestingConnection = true;
			StatusMessage = "Testing connection…";
			try
			{
				AIProfile profile = AIProfileDraft?.Clone() ?? Settings.ActiveProfile.Clone();
				profile.Normalize();
				string? apiKey = string.IsNullOrWhiteSpace(ApiKeyInput) ? null : ApiKeyInput;
				if (apiKey is null && profile.HasStoredKey)
				{
					SecureKeyLookupResult lookup = await keyStorage.TryLoadKeyAsync(profile.CredentialId, cancellation.Token);
					if (lookup.Status == SecureKeyLookupStatus.Unavailable)
						throw new AIConfigurationException("Secure API-key storage is unavailable.");
					if (lookup.Status == SecureKeyLookupStatus.Found)
						apiKey = lookup.Value;
				}
				var snapshot = new AISelectionSnapshot {
					ProfileId = profile.Id,
					ProfileName = profile.Name,
					ProviderType = profile.ProviderType,
					Endpoint = profile.BaseUrl,
					Model = profile.ResolveModel(),
					ApiKey = apiKey,
					CredentialId = profile.CredentialId,
					MaxContextTokens = Settings.MaxContextTokens,
					StreamResponses = Settings.StreamResponses,
					SendIL = Settings.SendIL,
					SendCallGraph = Settings.SendCallGraph
				};
				bool success = await new AIExplanationService(snapshot, providerFactory)
					.TestConnectionAsync(cancellation.Token);
				StatusMessage = success ? "Connection succeeded." : "The provider returned no response.";
			}
			catch (OperationCanceledException)
			{
				StatusMessage = "Connection test canceled.";
			}
			catch (AIRequestException exception)
			{
				StatusMessage = exception.Message;
			}
			catch (AIConfigurationException exception)
			{
				StatusMessage = exception.Message;
			}
			finally
			{
				testCancellation = null;
				IsTestingConnection = false;
			}
		}

		void SettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(AISettings.Provider) or nameof(AISettings.ApiKey)
				or nameof(AISettings.ApiKeyPlaceholder) or nameof(AISettings.BaseUrl)
				or nameof(AISettings.Model) or nameof(AISettings.PrivacyConsentAccepted))
			{
				OnPropertyChanged(nameof(HasConfiguredKey));
				OnPropertyChanged(nameof(CanTestConnection));
				TestConnectionCommand.RaiseCanExecuteChanged();
			}
		}

		public void Dispose()
		{
			testCancellation?.Cancel();
			if (settings is not null)
				settings.PropertyChanged -= SettingsPropertyChanged;
		}

		public sealed class AsyncCommand : ICommand
		{
			readonly Func<Task> execute;
			readonly Func<bool> canExecute;
			bool running;

			public AsyncCommand(Func<Task> execute, Func<bool> canExecute)
			{
				this.execute = execute;
				this.canExecute = canExecute;
			}

			public event EventHandler? CanExecuteChanged;
			public bool CanExecute(object? parameter) => !running && canExecute();
			public async void Execute(object? parameter)
			{
				if (!CanExecute(parameter))
					return;
				running = true;
				RaiseCanExecuteChanged();
				try
				{ await execute(); }
				finally { running = false; RaiseCanExecuteChanged(); }
			}
			public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
