// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;

using System.Composition;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AI;
using ICSharpCode.ILSpyX.AI;

namespace ICSharpCode.ILSpy.Options
{
	[ExportOptionPage(Order = 40)]
	public sealed class AISettingsViewModel : ObservableObject, IOptionPage, IDisposable
	{
		readonly IAIProviderFactory providerFactory;
		readonly SecureKeyStorage keyStorage;
		AISelectionService? selectionService;
		AISettingsModel settings = null!;
		AIProfile? selectedProfile;
		AIProfile? draft;
		bool draftIsNew;
		string apiKeyInput = string.Empty;
		string statusMessage = string.Empty;
		bool isTestingConnection;
		CancellationTokenSource? testCancellation;
		readonly AsyncCommand addProfileCommand;
		readonly AsyncCommand duplicateProfileCommand;
		readonly AsyncCommand deleteProfileCommand;
		readonly AsyncCommand saveCommand;
		readonly AsyncCommand cancelCommand;
		readonly AsyncCommand moveProfileUpCommand;
		readonly AsyncCommand moveProfileDownCommand;
		readonly AsyncCommand addModelCommand;
		readonly AsyncCommand renameModelCommand;
		readonly AsyncCommand deleteModelCommand;
		readonly AsyncCommand moveModelUpCommand;
		readonly AsyncCommand moveModelDownCommand;

		[ImportingConstructor]
		public AISettingsViewModel(IAIProviderFactory providerFactory, SecureKeyStorage keyStorage)
		{
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
			this.keyStorage = keyStorage ?? throw new ArgumentNullException(nameof(keyStorage));
			Providers = AIProviderCatalog.All.Select(provider => provider.Id).ToArray();
			ProviderDescriptors = AIProviderCatalog.All;
			addProfileCommand = new AsyncCommand(AddProfileAsync, () => Settings is not null);
			duplicateProfileCommand = new AsyncCommand(DuplicateProfileAsync, () => AIProfileDraft is not null);
			deleteProfileCommand = new AsyncCommand(DeleteProfileAsync, () => SelectedProfile is not null && Profiles.Count > 1);
			saveCommand = new AsyncCommand(SaveDraftAsync, () => AIProfileDraft is not null);
			cancelCommand = new AsyncCommand(CancelDraftAsync, () => AIProfileDraft is not null);
			moveProfileUpCommand = new AsyncCommand(() => MoveProfileAsync(-1), () => SelectedProfile is not null);
			moveProfileDownCommand = new AsyncCommand(() => MoveProfileAsync(1), () => SelectedProfile is not null);
			addModelCommand = new AsyncCommand(AddModelAsync, () => AIProfileDraft is not null);
			renameModelCommand = new AsyncCommand(RenameModelAsync, () => AIProfileDraft is not null && !string.IsNullOrWhiteSpace(SelectedModel));
			deleteModelCommand = new AsyncCommand(DeleteModelAsync, () => AIProfileDraft is not null && AIProfileDraft.Models.Count > 1);
			moveModelUpCommand = new AsyncCommand(() => MoveModelAsync(-1), () => AIProfileDraft is not null && !string.IsNullOrWhiteSpace(SelectedModel));
			moveModelDownCommand = new AsyncCommand(() => MoveModelAsync(1), () => AIProfileDraft is not null && !string.IsNullOrWhiteSpace(SelectedModel));
			SaveKeyCommand = new AsyncCommand(SaveKeyAsync, () => Settings is not null);
			ClearKeyCommand = new AsyncCommand(ClearKeyAsync, () => Settings is not null);
			TestConnectionCommand = new AsyncCommand(TestConnectionAsync, () => CanTestConnection);
			CancelTestConnectionCommand = new AsyncCommand(CancelTestConnectionAsync, () => IsTestingConnection);
		}

		public string Title => "AI Assistant";

		public IReadOnlyList<string> Providers { get; }
		public IReadOnlyList<AIProviderDescriptor> ProviderDescriptors { get; }
		public IReadOnlyList<AIProfile> Profiles => Settings is null ? Array.Empty<AIProfile>() : Settings.Profiles;
		public AIProfile? SelectedProfile {
			get => selectedProfile;
			set => SelectProfile(value);
		}
		public ICommand AddProfileCommand => addProfileCommand;
		public ICommand DuplicateProfileCommand => duplicateProfileCommand;
		public ICommand DeleteProfileCommand => deleteProfileCommand;
		public ICommand SaveCommand => saveCommand;
		public ICommand CancelCommand => cancelCommand;
		public ICommand MoveProfileUpCommand => moveProfileUpCommand;
		public ICommand MoveProfileDownCommand => moveProfileDownCommand;
		public ICommand AddModelCommand => addModelCommand;
		public ICommand RenameModelCommand => renameModelCommand;
		public ICommand DeleteModelCommand => deleteModelCommand;
		public ICommand MoveModelUpCommand => moveModelUpCommand;
		public ICommand MoveModelDownCommand => moveModelDownCommand;
		string modelNameInput = string.Empty;
		string selectedModel = string.Empty;
		public string ModelNameInput { get => modelNameInput; set => SetProperty(ref modelNameInput, value ?? string.Empty); }
		public string SelectedModel {
			get => selectedModel;
			set {
				if (!SetProperty(ref selectedModel, value ?? string.Empty) || AIProfileDraft is null)
					return;
				if (AIProfileDraft.Models.Contains(SelectedModel, StringComparer.OrdinalIgnoreCase))
					AIProfileDraft.LastSelectedModel = SelectedModel;
				InvalidateConnectionTest();
			}
		}
		public AIProviderDescriptor? SelectedProviderDescriptor {
			get => AIProviderCatalog.TryGet(AIProfileDraft?.ProviderType, out AIProviderDescriptor? descriptor) ? descriptor : null;
			set {
				if (value is null || AIProfileDraft is null || string.Equals(AIProfileDraft.ProviderType, value.Id, StringComparison.OrdinalIgnoreCase))
					return;
				AIProviderDescriptor previous = AIProviderCatalog.TryGet(AIProfileDraft.ProviderType, out AIProviderDescriptor? old) ? old : value;
				bool useDefaultEndpoint = string.IsNullOrWhiteSpace(AIProfileDraft.BaseUrl) || string.Equals(AIProfileDraft.BaseUrl, previous.DefaultBaseUrl, StringComparison.OrdinalIgnoreCase);
				bool useDefaultModel = AIProfileDraft.Models.Count == 0 || (AIProfileDraft.Models.Count == 1 && string.Equals(AIProfileDraft.Models[0], previous.DefaultModel, StringComparison.OrdinalIgnoreCase));
				AIProfileDraft.ProviderType = value.Id;
				if (useDefaultEndpoint)
					AIProfileDraft.BaseUrl = value.DefaultBaseUrl;
				if (useDefaultModel)
				{
					AIProfileDraft.Models.Clear();
					AIProfileDraft.Models.Add(value.DefaultModel);
					AIProfileDraft.LastSelectedModel = value.DefaultModel;
				}
				NotifyDraftChanged();
			}
		}
		public string ProviderKeyRequirement => SelectedProviderDescriptor?.KeyRequirement switch {
			AIProviderKeyRequirement.Required => "API key required",
			AIProviderKeyRequirement.Optional => "API key optional",
			AIProviderKeyRequirement.None => "No API key required",
			_ => "Provider unavailable"
		};

		public AISettingsModel Settings {
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

		public string DraftName {
			get => AIProfileDraft?.Name ?? string.Empty;
			set {
				if (AIProfileDraft is null || string.Equals(AIProfileDraft.Name, value, StringComparison.Ordinal))
					return;
				AIProfileDraft.Name = value ?? string.Empty;
				NotifyDraftChanged();
			}
		}

		public string DraftBaseUrl {
			get => AIProfileDraft?.BaseUrl ?? string.Empty;
			set {
				if (AIProfileDraft is null || string.Equals(AIProfileDraft.BaseUrl, value, StringComparison.Ordinal))
					return;
				AIProfileDraft.BaseUrl = value ?? string.Empty;
				NotifyDraftChanged();
			}
		}

		public string ApiKeyInput {
			get => apiKeyInput;
			set {
				if (!SetProperty(ref apiKeyInput, value ?? string.Empty))
					return;
				InvalidateConnectionTest();
				OnPropertyChanged(nameof(HasConfiguredKey));
				OnPropertyChanged(nameof(CanTestConnection));
				TestConnectionCommand.RaiseCanExecuteChanged();
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
				if (!AISettingsModel.IsSupportedProvider(profile.ProviderType)
					|| string.IsNullOrWhiteSpace(profile.BaseUrl)
					|| string.IsNullOrWhiteSpace(profile.ResolveModel()))
					return false;
				return AIProviderCatalog.Get(profile.ProviderType).KeyRequirement != AIProviderKeyRequirement.Required || HasConfiguredKey;
			}
		}

		public ICommand SaveKeyCommand { get; }
		public ICommand ClearKeyCommand { get; }
		public ICommand ActivateProfileCommand { get; private set; } = null!;
		public AsyncCommand TestConnectionCommand { get; }
		public AsyncCommand CancelTestConnectionCommand { get; }

		public void Load(SettingsService service)
		{
			ArgumentNullException.ThrowIfNull(service);
			Settings = service.AISettings;
			selectionService = AppComposition.TryGetExport<AISelectionService>();
			ActivateProfileCommand = new AsyncCommand(ActivateProfileAsync, () => SelectedProfile is not null && selectionService is not null);
			OnPropertyChanged(nameof(ActivateProfileCommand));
			selectedProfile = Settings.ActiveProfile;
			AIProfileDraft = Settings.ActiveProfile.Clone();
			draftIsNew = false;
			SelectedModel = AIProfileDraft.ResolveModel();
			ApiKeyInput = string.Empty;
			StatusMessage = string.Empty;
			_ = LoadStoredKeyAsync(Settings, CancellationToken.None);
		}

		async Task AddProfileAsync()
		{
			AIProfile profile = AIProfile.Create(AIProviderCatalog.Get("openai"));
			profile.Name = MakeUniqueName("New profile");
			selectedProfile = null;
			draftIsNew = true;
			AIProfileDraft = profile;
			SelectedModel = profile.ResolveModel();
			ClearTransientEditorState();
			OnPropertyChanged(nameof(SelectedProfile));
			NotifyDraftChanged();
			await Task.CompletedTask;
		}

		void SelectProfile(AIProfile? profile)
		{
			if (profile is null || ReferenceEquals(selectedProfile, profile))
				return;
			testCancellation?.Cancel();
			selectedProfile = profile;
			draftIsNew = false;
			AIProfileDraft = profile.Clone();
			SelectedModel = AIProfileDraft.ResolveModel();
			ClearTransientEditorState();
			OnPropertyChanged(nameof(SelectedProfile));
			NotifyDraftChanged();
		}

		async Task ActivateProfileAsync()
		{
			if (SelectedProfile is null || selectionService is null)
				return;
			try
			{
				await selectionService.ApplySelectionAsync(SelectedProfile.Id, SelectedProfile.ResolveModel());
				StatusMessage = $"'{SelectedProfile.Name}' is now the active AI profile.";
			}
			catch (Exception)
			{
				StatusMessage = "Unable to activate the selected AI profile.";
			}
		}

		void ClearTransientEditorState()
		{
			ApiKeyInput = string.Empty;
			ModelNameInput = string.Empty;
			StatusMessage = string.Empty;
		}

		void NotifyDraftChanged()
		{
			InvalidateConnectionTest();
			OnPropertyChanged(nameof(AIProfileDraft));
			OnPropertyChanged(nameof(DraftName));
			OnPropertyChanged(nameof(DraftBaseUrl));
			OnPropertyChanged(nameof(SelectedProviderDescriptor));
			OnPropertyChanged(nameof(ProviderKeyRequirement));
			OnPropertyChanged(nameof(HasConfiguredKey));
			OnPropertyChanged(nameof(CanTestConnection));
			RaiseEditorCommandStates();
		}

		void InvalidateConnectionTest()
		{
			CancellationTokenSource? cancellation = testCancellation;
			testCancellation = null;
			cancellation?.Cancel();
			if (IsTestingConnection)
				IsTestingConnection = false;
		}

		void RaiseEditorCommandStates()
		{
			foreach (AsyncCommand command in new[] {
				addProfileCommand, duplicateProfileCommand, deleteProfileCommand, saveCommand, cancelCommand,
				moveProfileUpCommand, moveProfileDownCommand, addModelCommand, renameModelCommand,
				deleteModelCommand, moveModelUpCommand, moveModelDownCommand, TestConnectionCommand,
				ActivateProfileCommand as AsyncCommand
			}.OfType<AsyncCommand>())
				command.RaiseCanExecuteChanged();
		}

		static Task<bool> ConfirmDeleteProfileAsync(AIProfile profile)
		{
			return ShowConfirmationAsync(
				"Delete AI profile",
				$"Delete '{profile.Name}' and its stored API key? Existing conversations remain readable. If this is the active profile, ILSpy selects the next profile.",
				"Delete");
		}

		Task<bool> ConfirmRemoveKeyAsync()
		{
			return ShowConfirmationAsync(
				"Remove API key",
				$"Remove the stored API key for '{AIProfileDraft?.Name}'? Providers that require a key will stop working until a replacement is saved.",
				"Remove key");
		}

		static async Task<bool> ShowConfirmationAsync(string title, string message, string confirmLabel)
		{
			Window? owner = UiContext.MainWindow;
			if (owner is null)
				return false;
			bool confirmed = false;
			var confirm = new Button { Content = confirmLabel, MinWidth = 90 };
			var cancel = new Button { Content = Resources.Cancel, MinWidth = 90 };
			var window = new Window {
				Title = title,
				SizeToContent = SizeToContent.WidthAndHeight,
				CanResize = false,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				ShowInTaskbar = false,
				Content = new StackPanel {
					Margin = new global::Avalonia.Thickness(16),
					MaxWidth = 480,
					Spacing = 16,
					Children = {
						new TextBlock { Text = message, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
						new StackPanel {
							Orientation = Orientation.Horizontal,
							HorizontalAlignment = HorizontalAlignment.Right,
							Spacing = 8,
							Children = { confirm, cancel }
						}
					}
				}
			};
			confirm.Click += (_, _) => { confirmed = true; window.Close(); };
			cancel.Click += (_, _) => window.Close();
			await window.ShowDialog(owner);
			return confirmed;
		}

		async Task DuplicateProfileAsync()
		{
			if (AIProfileDraft is null)
				return;
			AIProfileDraft = AIProfileDraft.Duplicate();
			AIProfileDraft.Name = MakeUniqueName(AIProfileDraft.Name + " Copy");
			selectedProfile = null;
			draftIsNew = true;
			ClearTransientEditorState();
			OnPropertyChanged(nameof(SelectedProfile));
			NotifyDraftChanged();
			await Task.CompletedTask;
		}

		async Task DeleteProfileAsync()
		{
			if (SelectedProfile is null || Settings.Profiles.Count <= 1)
				return;
			if (!await ConfirmDeleteProfileAsync(SelectedProfile))
				return;
			if (selectionService is null)
			{
				StatusMessage = "AI profile persistence is unavailable.";
				return;
			}
			try
			{
				await selectionService.DeleteProfileAsync(SelectedProfile.Id);
			}
			catch (Exception)
			{
				StatusMessage = "Unable to delete the profile or its stored API key. No profile metadata was removed.";
				return;
			}
			selectedProfile = Settings.ActiveProfile;
			draftIsNew = false;
			AIProfileDraft = selectedProfile.Clone();
			ClearTransientEditorState();
			SelectedModel = AIProfileDraft.ResolveModel();
			OnPropertyChanged(nameof(SelectedProfile));
			OnPropertyChanged(nameof(Profiles));
			NotifyDraftChanged();
		}

		async Task SaveDraftAsync()
		{
			if (AIProfileDraft is null)
				return;
			string? replacementKey = string.IsNullOrWhiteSpace(ApiKeyInput) ? null : ApiKeyInput;
			await CommitDraftAsync(AIProfileDraft.Clone(), replacementKey, replacementKey is not null);
		}

		async Task CancelDraftAsync()
		{
			selectedProfile ??= Settings.ActiveProfile;
			draftIsNew = false;
			AIProfileDraft = selectedProfile.Clone();
			SelectedModel = AIProfileDraft.ResolveModel();
			ClearTransientEditorState();
			OnPropertyChanged(nameof(SelectedProfile));
			NotifyDraftChanged();
			await Task.CompletedTask;
		}

		async Task MoveProfileAsync(int delta)
		{
			if (!draftIsNew && SelectedProfile is not null && selectionService is not null)
			{
				try
				{
					await selectionService.MoveProfileAsync(SelectedProfile.Id, delta);
				}
				catch (Exception)
				{
					StatusMessage = "Unable to reorder AI profiles.";
				}
			}
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
			NotifyDraftChanged();
			await Task.CompletedTask;
		}

		async Task RenameModelAsync()
		{
			if (AIProfileDraft is null)
				return;
			string replacement = ModelNameInput.Trim();
			if (replacement.Length == 0)
			{ StatusMessage = "Enter a model name."; return; }
			int index = AIProfileDraft.Models.FindIndex(model => string.Equals(model, SelectedModel, StringComparison.OrdinalIgnoreCase));
			if (index < 0)
				return;
			if (AIProfileDraft.Models.Where((model, i) => i != index).Any(model => string.Equals(model, replacement, StringComparison.OrdinalIgnoreCase)))
			{ StatusMessage = "That model is already listed."; return; }
			AIProfileDraft.Models[index] = replacement;
			AIProfileDraft.LastSelectedModel = replacement;
			SelectedModel = replacement;
			ModelNameInput = string.Empty;
			NotifyDraftChanged();
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
			NotifyDraftChanged();
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
			NotifyDraftChanged();
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
			selectedProfile = Settings.ActiveProfile;
			draftIsNew = false;
			AIProfileDraft = Settings.ActiveProfile.Clone();
			SelectedModel = AIProfileDraft.ResolveModel();
			ClearTransientEditorState();
			OnPropertyChanged(nameof(SelectedProfile));
			ApiKeyInput = string.Empty;
			StatusMessage = string.Empty;
			OnPropertyChanged(nameof(HasConfiguredKey));
			OnPropertyChanged(nameof(CanTestConnection));
			TestConnectionCommand.RaiseCanExecuteChanged();
		}

		async Task LoadStoredKeyAsync(AISettingsModel target, CancellationToken cancellationToken)
		{
			if (!target.ActiveProfile.HasStoredKey && string.IsNullOrWhiteSpace(target.ApiKeyPlaceholder))
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
				StatusMessage = "Enter a replacement API key.";
				return;
			}
			if (AIProfileDraft is not null)
				await CommitDraftAsync(AIProfileDraft.Clone(), ApiKeyInput, true);
		}

		async Task ClearKeyAsync()
		{
			if (AIProfileDraft is not null && await ConfirmRemoveKeyAsync())
				await CommitDraftAsync(AIProfileDraft.Clone(), null, false, removeKey: true);
		}

		async Task CommitDraftAsync(AIProfile draftProfile, string? replacementKey, bool replaceKey, bool removeKey = false)
		{
			if (selectionService is null)
			{
				ApiKeyInput = string.Empty;
				StatusMessage = "AI profile persistence is unavailable.";
				return;
			}
			draftProfile.Normalize();
			IReadOnlyList<string> errors = draftProfile.Validate();
			if (errors.Count != 0)
			{
				ApiKeyInput = string.Empty;
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
					ApiKeyInput = string.Empty;
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

				await selectionService.SaveProfileAsync(draftProfile.Clone());

				selectedProfile = Settings.Profiles.FirstOrDefault(profile => profile.Id == draftProfile.Id);
				draftIsNew = false;
				AIProfileDraft = draftProfile.Clone();
				SelectedModel = AIProfileDraft.ResolveModel();
				ApiKeyInput = string.Empty;
				StatusMessage = replaceKey ? "API key saved in secure storage." : removeKey ? "API key removed." : "Profile saved.";
				OnPropertyChanged(nameof(Profiles));
				OnPropertyChanged(nameof(HasConfiguredKey));
				OnPropertyChanged(nameof(CanTestConnection));
				TestConnectionCommand.RaiseCanExecuteChanged();
				OnPropertyChanged(nameof(SelectedProfile));
				NotifyDraftChanged();
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
				ApiKeyInput = string.Empty;
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
				IReadOnlyList<string> errors = profile.Validate();
				if (errors.Count != 0)
				{
					StatusMessage = string.Join(" ", errors);
					return;
				}
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
				string target = $"{profile.Name} / {profile.ResolveModel()}";
				if (ReferenceEquals(testCancellation, cancellation))
					StatusMessage = success ? $"Connection succeeded for {target}." : $"{target} returned no response.";
			}
			catch (OperationCanceledException)
			{
				if (ReferenceEquals(testCancellation, cancellation))
					StatusMessage = "Connection test canceled.";
			}
			catch (AIRequestException exception)
			{
				if (ReferenceEquals(testCancellation, cancellation))
					StatusMessage = exception.Message;
			}
			catch (AIConfigurationException exception)
			{
				if (ReferenceEquals(testCancellation, cancellation))
					StatusMessage = exception.Message;
			}
			finally
			{
				if (ReferenceEquals(testCancellation, cancellation))
				{
					testCancellation = null;
					IsTestingConnection = false;
				}
			}
		}

		void SettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(AISettingsModel.Provider) or nameof(AISettingsModel.ApiKey)
				or nameof(AISettingsModel.ApiKeyPlaceholder) or nameof(AISettingsModel.BaseUrl)
				or nameof(AISettingsModel.Model) or nameof(AISettingsModel.PrivacyConsentAccepted))
			{
				OnPropertyChanged(nameof(HasConfiguredKey));
				OnPropertyChanged(nameof(CanTestConnection));
				TestConnectionCommand.RaiseCanExecuteChanged();
			}
		}

		public void Dispose()
		{
			InvalidateConnectionTest();
			ApiKeyInput = string.Empty;
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
