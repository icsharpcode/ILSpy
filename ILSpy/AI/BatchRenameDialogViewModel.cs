// Copyright (c) 2026 Masroor

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
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Annotations;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	public sealed class BatchRenameItemViewModel : INotifyPropertyChanged
	{
		readonly BatchRenameItem item;
		readonly Action? changed;
		bool isSelected;
		RenameSuggestion? selectedSuggestion;

		public BatchRenameItemViewModel(BatchRenameItem item, Action? changed = null)
		{
			this.item = item ?? throw new ArgumentNullException(nameof(item));
			this.changed = changed;
			isSelected = item.HasSuggestions;
			selectedSuggestion = item.Suggestions.FirstOrDefault();
		}

		public IEntity Entity => item.Entity;
		public string OldName => item.OldName;
		public string NewName => SelectedSuggestion?.Name ?? string.Empty;
		public IReadOnlyList<RenameSuggestion> Suggestions => item.Suggestions;
		public string Error => item.Error ?? string.Empty;
		public bool HasError => !string.IsNullOrEmpty(Error);
		public bool IsSelected
		{
			get => isSelected;
			set
			{
				if (isSelected == value)
					return;
				isSelected = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
				changed?.Invoke();
			}
		}
		public RenameSuggestion? SelectedSuggestion
		{
			get => selectedSuggestion;
			set
			{
				if (selectedSuggestion == value)
					return;
				selectedSuggestion = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSuggestion)));
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NewName)));
				changed?.Invoke();
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;
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

		public BatchRenameDialogViewModel(ITypeDefinition type, AISettings settings, IAIProviderFactory providerFactory)
		{
			this.type = type ?? throw new ArgumentNullException(nameof(type));
			suggester = new BatchRenameSuggester(settings ?? throw new ArgumentNullException(nameof(settings)), providerFactory ?? throw new ArgumentNullException(nameof(providerFactory)));
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
			ErrorMessage = string.Empty;
			StatusMessage = "Preparing batch rename";
			try
			{
				IProgress<string> progress = new Progress<string>(name => StatusMessage = "Analyzing " + name);
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
