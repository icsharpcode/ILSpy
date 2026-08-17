// Copyright (c) 2026 Masroor
using System;
using System.Composition;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Input.Platform;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	[Export]
	[ExportToolPane(ContentId = PaneContentId, Alignment = ToolPaneAlignment.Bottom, Order = 2, IsVisibleByDefault = false)]
	[Shared]
	public sealed partial class AIOutputPaneModel : ToolPaneModel, IDisposable
	{
		public const string PaneContentId = "AIOutput";

		readonly AISettings settings;
		readonly AIExplanationService explanationService;
		CancellationTokenSource? cancellation;

		[ObservableProperty]
		string targetName = string.Empty;
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(CanCopy))]
		string response = string.Empty;
		[ObservableProperty]
		string errorMessage = string.Empty;
		[ObservableProperty]
		string statusMessage = "Ready";
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(CanCopy))]
		bool isBusy;
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(CanCopy))]
		bool isComplete;

		[ImportingConstructor]
		public AIOutputPaneModel(SettingsService settingsService, IAIProviderFactory providerFactory)
		{
			settings = settingsService?.AISettings ?? throw new ArgumentNullException(nameof(settingsService));
			explanationService = new AIExplanationService(settings, providerFactory ?? throw new ArgumentNullException(nameof(providerFactory)));
			Id = PaneContentId;
			Title = "AI Output";
		}

		public bool CanCopy => !IsBusy && IsComplete && Response.Length != 0;

		public Task StartAsync(IEntity entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			MetadataFile module = entity.ParentModule?.MetadataFile
				?? throw new InvalidOperationException("The selected symbol has no decompilable module.");
			var decompiler = new CSharpDecompiler(module, module.GetAssemblyResolver(true), new ICSharpCode.Decompiler.DecompilerSettings());
			return StartAsync(entity.FullName, token => explanationService.ExplainStreamingAsync(entity, decompiler, token));
		}

		public async Task StartAsync(string name, Func<CancellationToken, IAsyncEnumerable<string>> streamFactory)
		{
			ArgumentNullException.ThrowIfNull(streamFactory);
			cancellation?.Cancel();
			var requestCancellation = new CancellationTokenSource();
			cancellation = requestCancellation;
			TargetName = name ?? string.Empty;
			Response = string.Empty;
			IsComplete = false;
			ErrorMessage = string.Empty;
			StatusMessage = "Generating…";
			IsBusy = true;
			try
			{
				await Task.Run(() => ConsumeAsync(streamFactory, requestCancellation), requestCancellation.Token).ConfigureAwait(false);
			}
			catch (OperationCanceledException)
			{
				await Dispatcher.UIThread.InvokeAsync(() => {
					if (ReferenceEquals(cancellation, requestCancellation))
					{
						IsComplete = false;
						StatusMessage = "Canceled";
					}
				});
			}
			catch (AIRequestException exception)
			{
				await Dispatcher.UIThread.InvokeAsync(() => {
					if (ReferenceEquals(cancellation, requestCancellation))
					{
						IsComplete = false;
						ErrorMessage = exception.Message;
						StatusMessage = "Request failed";
					}
				});
			}
			catch (AIConfigurationException exception)
			{
				await Dispatcher.UIThread.InvokeAsync(() => {
					if (ReferenceEquals(cancellation, requestCancellation))
					{
						IsComplete = false;
						ErrorMessage = exception.Message;
						StatusMessage = "Configuration required";
					}
				});
			}
			catch (Exception)
			{
				await Dispatcher.UIThread.InvokeAsync(() => {
					if (ReferenceEquals(cancellation, requestCancellation))
					{
						IsComplete = false;
						ErrorMessage = "The AI request failed. Check provider settings and try again.";
						StatusMessage = "Request failed";
					}
				});
			}
			finally
			{
				await Dispatcher.UIThread.InvokeAsync(() => {
					if (ReferenceEquals(cancellation, requestCancellation))
					{
						IsBusy = false;
						cancellation = null;
					}
				});
				requestCancellation.Dispose();
			}
		}

		async Task ConsumeAsync(Func<CancellationToken, IAsyncEnumerable<string>> streamFactory, CancellationTokenSource requestCancellation)
		{
			var response = new StringBuilder();
			await foreach (string chunk in streamFactory(requestCancellation.Token).ConfigureAwait(false))
			{
				if (string.IsNullOrEmpty(chunk))
					continue;
				response.Append(chunk);
				await Dispatcher.UIThread.InvokeAsync(() => {
					if (ReferenceEquals(cancellation, requestCancellation))
						Response = response.ToString();
				});
			}
			await Dispatcher.UIThread.InvokeAsync(() => {
				if (ReferenceEquals(cancellation, requestCancellation))
				{
					IsComplete = response.Length != 0;
					StatusMessage = response.Length == 0 ? "The provider returned an empty response." : "Complete";
				}
			});
		}

		[RelayCommand]
		void Cancel() => cancellation?.Cancel();

		[RelayCommand]
		void Clear()
		{
			cancellation?.Cancel();
			cancellation = null;
			IsBusy = false;
			Response = string.Empty;
			IsComplete = false;
			ErrorMessage = string.Empty;
			TargetName = string.Empty;
			StatusMessage = "Ready";
		}

		public async Task<bool> CopyToClipboardAsync(IClipboard? clipboard)
		{
			if (!CanCopy || clipboard is null)
				return false;
			await clipboard.SetTextAsync(Response);
			StatusMessage = "Copied to clipboard";
			return true;
		}

		public void Dispose()
		{
			cancellation?.Cancel();
			cancellation?.Dispose();
		}
	}
}
