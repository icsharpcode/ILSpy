// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Avalonia.Input.Platform;
using Avalonia.Threading;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	public sealed class ExplainDialogViewModel : IDisposable
	{
		readonly IEntity entity;
		readonly AIExplanationService explanationService;
		readonly Func<IEntity, CSharpDecompiler> decompilerFactory;
		CancellationTokenSource? cancellation;
		string response = string.Empty;
		string errorMessage = string.Empty;
		string statusMessage = "Ready";
		bool isBusy;
		bool isCanceled;

		public ExplainDialogViewModel(
			IEntity entity,
			AISettings settings,
			IAIProviderFactory providerFactory,
			Func<IEntity, CSharpDecompiler>? decompilerFactory = null)
		{
			this.entity = entity ?? throw new ArgumentNullException(nameof(entity));
			this.explanationService = new AIExplanationService(settings ?? throw new ArgumentNullException(nameof(settings)), providerFactory ?? throw new ArgumentNullException(nameof(providerFactory)));
			this.decompilerFactory = decompilerFactory ?? CreateDecompiler;
			CancelCommand = new RelayCommand(Cancel, () => IsBusy);
		}

		public string TargetName => entity.FullName;
		public string Response {
			get => response;
			private set { if (response != value) { response = value; PropertyChanged?.Invoke(this, new(nameof(Response))); PropertyChanged?.Invoke(this, new(nameof(CanCopy))); } }
		}
		public string ErrorMessage {
			get => errorMessage;
			private set { if (errorMessage != value) { errorMessage = value; PropertyChanged?.Invoke(this, new(nameof(ErrorMessage))); } }
		}
		public string StatusMessage {
			get => statusMessage;
			private set { if (statusMessage != value) { statusMessage = value; PropertyChanged?.Invoke(this, new(nameof(StatusMessage))); } }
		}
		public bool IsBusy {
			get => isBusy;
			private set { if (isBusy != value) { isBusy = value; PropertyChanged?.Invoke(this, new(nameof(IsBusy))); ((RelayCommand)CancelCommand).RaiseCanExecuteChanged(); } }
		}
		public bool CanCopy => !IsBusy && !string.IsNullOrEmpty(Response);
		public bool WasCanceled => isCanceled;
		public ICommand CancelCommand { get; }
		public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

		public async Task StartAsync(CancellationToken cancellationToken = default)
		{
			if (IsBusy)
				return;
			using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			cancellation = linked;
			IsBusy = true;
			isCanceled = false;
			Response = string.Empty;
			ErrorMessage = string.Empty;
			StatusMessage = "Explaining…";
			try
			{
				string result = await Task.Run(
					async () => await explanationService.ExplainAsync(entity, decompilerFactory(entity), linked.Token).ConfigureAwait(false),
					linked.Token).ConfigureAwait(false);
				await Dispatcher.UIThread.InvokeAsync(() => {
					Response = result;
					StatusMessage = string.IsNullOrEmpty(result) ? "The provider returned an empty response." : "Complete";
				});
			}
			catch (OperationCanceledException)
			{
				isCanceled = true;
				await Dispatcher.UIThread.InvokeAsync(() => {
					ErrorMessage = string.Empty;
					StatusMessage = "Canceled";
				});
			}
			catch (AIRequestException exception)
			{
				await Dispatcher.UIThread.InvokeAsync(() => {
					ErrorMessage = exception.Message;
					StatusMessage = "Request failed";
				});
			}
			catch (AIConfigurationException exception)
			{
				await Dispatcher.UIThread.InvokeAsync(() => {
					ErrorMessage = exception.Message;
					StatusMessage = "Configuration required";
				});
			}
			catch (Exception)
			{
				await Dispatcher.UIThread.InvokeAsync(() => {
					ErrorMessage = "The AI request failed. Check provider settings and try again.";
					StatusMessage = "Request failed";
				});
			}
			finally
			{
				cancellation = null;
				await Dispatcher.UIThread.InvokeAsync(() => IsBusy = false);
			}
		}

		public void Cancel()
		{
			if (IsBusy)
				cancellation?.Cancel();
		}

		public async Task<bool> CopyToClipboardAsync(IClipboard? clipboard)
		{
			if (!CanCopy || clipboard is null)
				return false;
			await clipboard.SetTextAsync(Response);
			StatusMessage = "Copied to clipboard";
			return true;
		}

		public void Dispose() => cancellation?.Cancel();

		static CSharpDecompiler CreateDecompiler(IEntity entity)
		{
			MetadataFile module = entity.ParentModule?.MetadataFile
				?? throw new InvalidOperationException("The selected symbol has no decompilable module.");
			return new CSharpDecompiler(module, module.GetAssemblyResolver(true), new ICSharpCode.Decompiler.DecompilerSettings());
		}

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
