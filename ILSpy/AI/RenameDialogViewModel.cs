// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Avalonia.Threading;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AI.Decompiler;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	public sealed class RenameDialogViewModel : INotifyPropertyChanged, IDisposable
	{
		readonly IEntity entity;
		readonly RenameSuggester suggester;
		readonly RenameAnnotationManager annotations;
		CancellationTokenSource? cancellation;
		RenameSuggestion? selectedSuggestion;
		string statusMessage = "Ready";
		string errorMessage = string.Empty;
		string rawResponse = string.Empty;
		bool isBusy;

		public RenameDialogViewModel(IEntity entity, AISelectionSnapshot snapshot, IAIProviderFactory providerFactory)
		{
			this.entity = entity ?? throw new ArgumentNullException(nameof(entity));
			suggester = new RenameSuggester(snapshot ?? throw new ArgumentNullException(nameof(snapshot)), providerFactory ?? throw new ArgumentNullException(nameof(providerFactory)));
			annotations = RenameAnnotationManager.ForAssembly(entity.ParentModule?.MetadataFile?.FileName ?? throw new ArgumentException("The selected symbol has no assembly file.", nameof(entity)));
			annotations.Load();
			CancelCommand = new RelayCommand(Cancel, () => IsBusy);
			ApplyCommand = new RelayCommand(Apply, () => CanApply);
		}

		public string TargetName => entity.FullName;
		public ObservableCollection<RenameSuggestion> Suggestions { get; } = new();
		public RenameSuggestion? SelectedSuggestion {
			get => selectedSuggestion;
			set { if (selectedSuggestion != value) { selectedSuggestion = value; OnChanged(nameof(SelectedSuggestion)); OnChanged(nameof(CanApply)); ((RelayCommand)ApplyCommand).RaiseCanExecuteChanged(); } }
		}
		public string StatusMessage { get => statusMessage; private set { if (statusMessage != value) { statusMessage = value; OnChanged(nameof(StatusMessage)); } } }
		public string ErrorMessage { get => errorMessage; private set { if (errorMessage != value) { errorMessage = value; OnChanged(nameof(ErrorMessage)); } } }
		public string RawResponse { get => rawResponse; private set { if (rawResponse != value) { rawResponse = value; OnChanged(nameof(RawResponse)); OnChanged(nameof(HasRawResponse)); } } }
		public bool HasRawResponse => !string.IsNullOrEmpty(RawResponse);
		public bool IsBusy { get => isBusy; private set { if (isBusy != value) { isBusy = value; OnChanged(nameof(IsBusy)); ((RelayCommand)CancelCommand).RaiseCanExecuteChanged(); } } }
		public bool CanApply => !IsBusy && SelectedSuggestion is not null;
		public ICommand CancelCommand { get; }
		public ICommand ApplyCommand { get; }
		public event PropertyChangedEventHandler? PropertyChanged;

		public async Task StartAsync(CancellationToken cancellationToken = default)
		{
			if (IsBusy)
				return;
			using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			cancellation = linked;
			IsBusy = true;
			Suggestions.Clear();
			SelectedSuggestion = null;
			ErrorMessage = string.Empty;
			RawResponse = string.Empty;
			StatusMessage = "Suggesting names…";
			try
			{
				IReadOnlyList<RenameSuggestion> suggestions = await Task.Run(
					() => suggester.SuggestAsync(entity, CreateDecompiler(entity), linked.Token), linked.Token).ConfigureAwait(false);
				await Dispatcher.UIThread.InvokeAsync(() => {
					foreach (RenameSuggestion suggestion in suggestions)
						Suggestions.Add(suggestion);
					SelectedSuggestion = Suggestions.Count == 0 ? null : Suggestions[0];
					StatusMessage = Suggestions.Count == 0 ? "No suggestions returned." : "Select a name to apply.";
				});
			}
			catch (OperationCanceledException)
			{
				await Dispatcher.UIThread.InvokeAsync(() => StatusMessage = "Canceled");
			}
			catch (RenameSuggestionParseException exception)
			{
				await Dispatcher.UIThread.InvokeAsync(() => { ErrorMessage = exception.Message; RawResponse = exception.RawText; StatusMessage = "Invalid provider response"; });
			}
			catch (AIConfigurationException exception)
			{
				await Dispatcher.UIThread.InvokeAsync(() => { ErrorMessage = exception.Message; StatusMessage = "Configuration required"; });
			}
			catch (AIRequestException exception)
			{
				await Dispatcher.UIThread.InvokeAsync(() => { ErrorMessage = exception.Message; StatusMessage = "Request failed"; });
			}
			catch (Exception)
			{
				await Dispatcher.UIThread.InvokeAsync(() => { ErrorMessage = "The AI request failed. Check provider settings and try again."; StatusMessage = "Request failed"; });
			}
			finally
			{
				cancellation = null;
				await Dispatcher.UIThread.InvokeAsync(() => IsBusy = false);
			}
		}

		public void Cancel() => cancellation?.Cancel();

		void Apply()
		{
			if (SelectedSuggestion is not { } suggestion)
				return;
			annotations.SetRename(entity, suggestion.Name);
			annotations.Save();
			StatusMessage = $"Applied {suggestion.Name}. Reopen or refresh the decompiled view to see it.";
		}

		static CSharpDecompiler CreateDecompiler(IEntity entity)
		{
			MetadataFile module = entity.ParentModule?.MetadataFile ?? throw new InvalidOperationException("The selected symbol has no decompilable module.");
			return new CSharpDecompiler(module, module.GetAssemblyResolver(true), new ICSharpCode.Decompiler.DecompilerSettings());
		}

		void OnChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		public void Dispose() => cancellation?.Cancel();

		sealed class RelayCommand : ICommand
		{
			readonly Action execute;
			readonly Func<bool> canExecute;
			public RelayCommand(Action execute, Func<bool> canExecute) { this.execute = execute; this.canExecute = canExecute; }
			public event EventHandler? CanExecuteChanged;
			public bool CanExecute(object? parameter) => canExecute();
			public void Execute(object? parameter) => execute();
			public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
