// Copyright (c) 2026 Masroor
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

using Avalonia.Threading;

using System.Composition;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.Options
{
	[ExportOptionPage(Order = 40)]
	public sealed class AISettingsViewModel : ObservableObject, IOptionPage, IDisposable
	{
		readonly IAIProviderFactory providerFactory;
		readonly SecureKeyStorage keyStorage;
		AISettings settings = null!;
		string apiKeyInput = string.Empty;
		string statusMessage = string.Empty;
		bool isTestingConnection;
		CancellationTokenSource? testCancellation;

		[ImportingConstructor]
		public AISettingsViewModel(IAIProviderFactory providerFactory, SecureKeyStorage keyStorage)
		{
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
			this.keyStorage = keyStorage ?? throw new ArgumentNullException(nameof(keyStorage));
			Providers = new[] { "openai", "ollama", "custom" };
			SaveKeyCommand = new AsyncCommand(SaveKeyAsync, () => Settings is not null);
			ClearKeyCommand = new AsyncCommand(ClearKeyAsync, () => Settings is not null);
			TestConnectionCommand = new AsyncCommand(TestConnectionAsync, () => CanTestConnection);
			CancelTestConnectionCommand = new AsyncCommand(CancelTestConnectionAsync, () => IsTestingConnection);
		}

		public string Title => "AI Assistant";

		public IReadOnlyList<string> Providers { get; }

		public AISettings Settings
		{
			get => settings;
			private set
			{
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

		public string ApiKeyInput
		{
			get => apiKeyInput;
			set
			{
				if (!SetProperty(ref apiKeyInput, value ?? string.Empty))
					return;
				if (Settings is not null)
					Settings.ApiKey = apiKeyInput;
			}
		}

		public string StatusMessage
		{
			get => statusMessage;
			private set => SetProperty(ref statusMessage, value);
		}

		public bool IsTestingConnection
		{
			get => isTestingConnection;
			private set
			{
				if (!SetProperty(ref isTestingConnection, value))
					return;
				OnPropertyChanged(nameof(CanTestConnection));
				TestConnectionCommand.RaiseCanExecuteChanged();
				CancelTestConnectionCommand.RaiseCanExecuteChanged();
			}
		}

		public bool HasConfiguredKey => Settings is not null
			&& (!string.IsNullOrWhiteSpace(Settings.ApiKey)
				|| !string.IsNullOrWhiteSpace(Settings.ApiKeyPlaceholder));

		public bool CanTestConnection
		{
			get
			{
				if (Settings is null || IsTestingConnection || !Settings.PrivacyConsentAccepted)
					return false;
				if (!AISettings.IsSupportedProvider(Settings.Provider)
					|| string.IsNullOrWhiteSpace(Settings.BaseUrl)
					|| string.IsNullOrWhiteSpace(Settings.Model))
					return false;
				return Settings.Provider == "ollama" || HasConfiguredKey;
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
			ApiKeyInput = string.Empty;
			StatusMessage = string.Empty;
			_ = LoadStoredKeyAsync(Settings, CancellationToken.None);
		}

		public void LoadDefaults()
		{
			Settings.LoadFromXml(null!);
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
				SecureKeyLookupResult result = await keyStorage.TryLoadKeyAsync(target.Provider, cancellationToken);
				if (result.Status == SecureKeyLookupStatus.Found && result.Value is { } key)
				{
					await Dispatcher.UIThread.InvokeAsync(() => {
						if (ReferenceEquals(Settings, target))
						{
							target.ApiKey = key;
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
			try
			{
				await keyStorage.SaveKeyAsync(Settings.Provider, ApiKeyInput, CancellationToken.None);
				await Dispatcher.UIThread.InvokeAsync(() => {
					Settings.ApiKeyPlaceholder = "configured";
					StatusMessage = "API key saved in secure storage.";
					OnPropertyChanged(nameof(HasConfiguredKey));
					OnPropertyChanged(nameof(CanTestConnection));
					TestConnectionCommand.RaiseCanExecuteChanged();
				});
			}
			catch (Exception)
			{
				StatusMessage = "Unable to save the API key. Secure storage is required.";
			}
		}

		async Task ClearKeyAsync()
		{
			try
			{
				await keyStorage.DeleteKeyAsync(Settings.Provider, CancellationToken.None);
				await Dispatcher.UIThread.InvokeAsync(() => {
					Settings.ApiKey = string.Empty;
					Settings.ApiKeyPlaceholder = string.Empty;
					ApiKeyInput = string.Empty;
					StatusMessage = "API key removed.";
					OnPropertyChanged(nameof(HasConfiguredKey));
					OnPropertyChanged(nameof(CanTestConnection));
					TestConnectionCommand.RaiseCanExecuteChanged();
				});
			}
			catch (Exception)
			{
				StatusMessage = "Unable to remove the API key from secure storage.";
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
				bool success = await new AIExplanationService(Settings, providerFactory)
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
				try { await execute(); }
				finally { running = false; RaiseCanExecuteChanged(); }
			}
			public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
