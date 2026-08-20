// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Avalonia.Threading;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Annotations;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	public sealed class BatchRenameItemViewModel : INotifyPropertyChanged
	{
		public const double BatchRenameAutoSelectConfidence = 0.60;
		readonly BatchRenameItem item;
		readonly Action? changed;
		bool isSelected;
		RenameSuggestion? selectedSuggestion;

		public BatchRenameItemViewModel(BatchRenameItem item, Action? changed = null)
		{
			this.item = item ?? throw new ArgumentNullException(nameof(item));
			this.changed = changed;
			isSelected = ShouldAutoSelect(item.Suggestions.FirstOrDefault());
			selectedSuggestion = item.Suggestions.FirstOrDefault();
		}

		public IEntity Entity => item.Entity;
		public string OldName => item.OldName;
		public string NewName => SelectedSuggestion?.Name ?? string.Empty;
		public int SelectedSuggestionConfidencePercent => SelectedSuggestion?.ConfidencePercent ?? 0;
		public string SelectedSuggestionReasoning => SelectedSuggestion?.Reasoning ?? string.Empty;
		public IReadOnlyList<RenameSuggestion> Suggestions => item.Suggestions;
		public string Error => item.Error ?? string.Empty;
		public bool HasError => !string.IsNullOrEmpty(Error);
		public bool IsSelected {
			get => isSelected;
			set {
				if (isSelected == value)
					return;
				isSelected = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
				changed?.Invoke();
			}
		}
		public RenameSuggestion? SelectedSuggestion {
			get => selectedSuggestion;
			set {
				if (selectedSuggestion == value)
					return;
				selectedSuggestion = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSuggestion)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewName)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSuggestionConfidencePercent)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSuggestionReasoning)));
				changed?.Invoke();
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		public static bool ShouldAutoSelect(RenameSuggestion? suggestion)
			=> suggestion is not null && suggestion.Confidence >= BatchRenameAutoSelectConfidence;
	}

	public sealed class BatchRenameDialogViewModel : INotifyPropertyChanged, IDisposable
	{
		readonly ITypeDefinition type;
		readonly BatchRenameSuggester suggester;
		readonly RenameAnnotationManager annotations;
		CancellationTokenSource? cancellation;
		string statusMessage = "Ready";
		string errorMessage = string.Empty;
		bool isBusy;
		int progressValue;
		int progressMaximum;
		string progressCurrentMember = string.Empty;
		int progressSkippedOrErrorCount;

		public BatchRenameDialogViewModel(ITypeDefinition type, AISelectionSnapshot snapshot, IAIProviderFactory providerFactory)
		{
			this.type = type ?? throw new ArgumentNullException(nameof(type));
			suggester = new BatchRenameSuggester(snapshot ?? throw new ArgumentNullException(nameof(snapshot)), providerFactory ?? throw new ArgumentNullException(nameof(providerFactory)));
			annotations = new RenameAnnotationManager(type.ParentModule?.MetadataFile?.FileName ?? throw new ArgumentException("The selected type has no assembly file.", nameof(type)));
			annotations.Load();
			CancelCommand = new RelayCommand(Cancel, () => IsBusy);
			ApplyCommand = new RelayCommand(Apply, () => CanApply);
		}

		public string TargetName => type.FullName;
		public ObservableCollection<BatchRenameItemViewModel> Items { get; } = new();
		public string StatusMessage { get => statusMessage; private set { if (statusMessage != value) { statusMessage = value; OnChanged(nameof(StatusMessage)); } } }
		public string ErrorMessage { get => errorMessage; private set { if (errorMessage != value) { errorMessage = value; OnChanged(nameof(ErrorMessage)); } } }
		public bool IsBusy { get => isBusy; private set { if (isBusy != value) { isBusy = value; OnChanged(nameof(IsBusy)); ((RelayCommand)CancelCommand).RaiseCanExecuteChanged(); ((RelayCommand)ApplyCommand).RaiseCanExecuteChanged(); } } }
		public bool CanApply => !IsBusy && Items.Any(item => item.IsSelected && item.SelectedSuggestion is not null);
		public int ProgressValue { get => progressValue; private set { if (progressValue != value) { progressValue = value; OnChanged(nameof(ProgressValue)); OnChanged(nameof(ProgressPercent)); } } }
		public int ProgressMaximum { get => progressMaximum; private set { if (progressMaximum != value) { progressMaximum = value; OnChanged(nameof(ProgressMaximum)); OnChanged(nameof(ProgressPercent)); } } }
		public double ProgressPercent => ProgressMaximum == 0 ? 0 : (double)ProgressValue / ProgressMaximum * 100;
		public string ProgressCurrentMember { get => progressCurrentMember; private set { if (progressCurrentMember != value) { progressCurrentMember = value; OnChanged(nameof(ProgressCurrentMember)); } } }
		public int ProgressSkippedOrErrorCount { get => progressSkippedOrErrorCount; private set { if (progressSkippedOrErrorCount != value) { progressSkippedOrErrorCount = value; OnChanged(nameof(ProgressSkippedOrErrorCount)); } } }
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
			Items.Clear();
			ProgressValue = 0;
			ProgressMaximum = 0;
			ProgressCurrentMember = string.Empty;
			ProgressSkippedOrErrorCount = 0;
			ErrorMessage = string.Empty;
			StatusMessage = "Preparing batch rename";
			try
			{
				IProgress<BatchRenameProgress> progress = new Progress<BatchRenameProgress>(ReportProgress);
				IReadOnlyList<BatchRenameItem> suggestions = await Task.Run(
					() => suggester.SuggestAsync(type, CreateDecompiler(type), progress, linked.Token), linked.Token).ConfigureAwait(false);
				await Dispatcher.UIThread.InvokeAsync(() => {
					foreach (BatchRenameItem item in suggestions)
						Items.Add(new BatchRenameItemViewModel(item, RaiseCanApplyChanged));
					StatusMessage = Items.Count == 0 ? "No obfuscated members found." : "Review proposed renames.";
					OnChanged(nameof(CanApply));
				});
			}
			catch (OperationCanceledException)
			{
				await Dispatcher.UIThread.InvokeAsync(() => StatusMessage = "Canceled");
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
				await Dispatcher.UIThread.InvokeAsync(() => { ErrorMessage = "The batch AI request failed. Check provider settings and try again."; StatusMessage = "Request failed"; });
			}
			finally
			{
				cancellation = null;
				await Dispatcher.UIThread.InvokeAsync(() => IsBusy = false);
			}
		}

		public void Cancel() => cancellation?.Cancel();

		void ReportProgress(BatchRenameProgress progress)
		{
			void Update()
			{
				ProgressValue = progress.Completed;
				ProgressMaximum = progress.Total;
				ProgressCurrentMember = progress.CurrentMember ?? string.Empty;
				ProgressSkippedOrErrorCount = progress.SkippedOrErrorCount;
				if (progress.CurrentMember is not null && progress.Completed < progress.Total)
					StatusMessage = $"Analyzing {progress.CurrentMember}";
			}
			if (Dispatcher.UIThread.CheckAccess())
				Update();
			else
				Dispatcher.UIThread.Post(Update);
		}

		void Apply()
		{
			foreach (BatchRenameItemViewModel item in Items.Where(item => item.IsSelected && item.SelectedSuggestion is not null))
				annotations.SetRename(item.Entity, item.SelectedSuggestion!.Name);
			annotations.Save();
			StatusMessage = "Applied selected renames. Reopen or refresh the decompiled view to see them.";
		}

		static CSharpDecompiler CreateDecompiler(ITypeDefinition type)
		{
			MetadataFile module = type.ParentModule?.MetadataFile ?? throw new InvalidOperationException("The selected type has no decompilable module.");
			return new CSharpDecompiler(module, module.GetAssemblyResolver(true), new DecompilerSettings());
		}

		void OnChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		void RaiseCanApplyChanged()
		{
			OnChanged(nameof(CanApply));
			((RelayCommand)ApplyCommand).RaiseCanExecuteChanged();
		}
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
